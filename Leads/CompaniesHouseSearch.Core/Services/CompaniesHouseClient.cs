using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CompaniesHouseSearch.Helpers;
using CompaniesHouseSearch.Models;

namespace CompaniesHouseSearch.Services;

public class CompaniesHouseClient
{
    private readonly HttpClient _http;
    private readonly HttpClient _documentHttp;
    private readonly RateLimiter _rateLimiter = new();
    private const int PageSize = 20;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CompaniesHouseClient(string apiKey, string baseUrl)
    {
        var authHeader = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:")));

        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _http.DefaultRequestHeaders.Authorization = authHeader;

        _documentHttp = new HttpClient
        {
            BaseAddress = new Uri("https://document-api.company-information.service.gov.uk")
        };
        _documentHttp.DefaultRequestHeaders.Authorization = authHeader;
    }

    // ── Batch search (used by console app) ──────────────────────────────────

    public async Task<List<CompanyItem>> SearchAsync(string[] sicCodes, string? region, int maxResults)
    {
        var results = new List<CompanyItem>();
        int startIndex = 0;

        Console.WriteLine($"Searching Companies House for SIC codes: {string.Join(", ", sicCodes)}...");

        while (results.Count < maxResults)
        {
            await _rateLimiter.WaitAsync();

            var url = BuildSearchUrl(sicCodes, region, startIndex);
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"API error {(int)response.StatusCode}: {response.ReasonPhrase}");
                break;
            }

            var page = await DeserializeAsync<CompanySearchResponse>(response);

            if (page?.Items == null || page.Items.Count == 0) break;

            results.AddRange(page.Items);

            Console.WriteLine($"  Fetched {results.Count} of {Math.Min(maxResults, page.Hits)} companies...");

            if (results.Count >= page.Hits || results.Count >= maxResults) break;

            startIndex += PageSize;
        }

        return results.Take(maxResults).ToList();
    }

    // Kept for console app backwards compatibility
    public async Task<List<string>> GetDirectorsAsync(string companyNumber)
    {
        var officers = await GetOfficersAsync(companyNumber);
        return officers
            .Where(o => o.IsActive && o.OfficerRole.Equals("director", StringComparison.OrdinalIgnoreCase))
            .Select(o => ToTitleCase(o.Name))
            .ToList();
    }

    // ── Single-company deep-dive methods (used by web app) ──────────────────

    public async Task<CompanyProfile?> GetCompanyProfileAsync(string companyNumber)
    {
        await _rateLimiter.WaitAsync();
        var response = await _http.GetAsync($"/company/{companyNumber}");
        if (!response.IsSuccessStatusCode) return null;
        return await DeserializeAsync<CompanyProfile>(response);
    }

    public async Task<List<OfficerItem>> GetOfficersAsync(string companyNumber, int resignedWithinMonths = 18)
    {
        await _rateLimiter.WaitAsync();

        // Request all officers including resigned (no filter on API side)
        var response = await _http.GetAsync($"/company/{companyNumber}/officers?items_per_page=50");
        if (!response.IsSuccessStatusCode) return [];

        var result = await DeserializeAsync<OfficersResponse>(response);
        if (result?.Items == null) return [];

        return result.Items
            .Where(o => o.IsActive || o.ResignedWithinMonths(resignedWithinMonths))
            .ToList();
    }

    public async Task<List<PscItem>> GetPscAsync(string companyNumber)
    {
        await _rateLimiter.WaitAsync();
        var response = await _http.GetAsync($"/company/{companyNumber}/persons-with-significant-control");
        if (!response.IsSuccessStatusCode) return [];

        var result = await DeserializeAsync<PscResponse>(response);
        return result?.Items.Where(p => p.IsActive).ToList() ?? [];
    }

    public async Task<InsolvencyInfo?> GetInsolvencyAsync(string companyNumber)
    {
        await _rateLimiter.WaitAsync();
        var response = await _http.GetAsync($"/company/{companyNumber}/insolvency");

        // 404 is normal — means no insolvency history
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        if (!response.IsSuccessStatusCode) return null;

        return await DeserializeAsync<InsolvencyInfo>(response);
    }

    public async Task<FilingHistoryResponse?> GetFilingHistoryAsync(string companyNumber, string category = "accounts")
    {
        await _rateLimiter.WaitAsync();
        var url = $"/company/{companyNumber}/filing-history?category={category}&items_per_page=5";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        return await DeserializeAsync<FilingHistoryResponse>(response);
    }

    public async Task<byte[]?> GetDocumentBytesAsync(string documentMetadataUrl)
    {
        await _rateLimiter.WaitAsync();

        // Append /content and request PDF format
        var contentUrl = documentMetadataUrl.TrimEnd('/') + "/content";

        using var request = new HttpRequestMessage(HttpMethod.Get, contentUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));

        var response = await _documentHttp.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadAsByteArrayAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string BuildSearchUrl(string[] sicCodes, string? region, int startIndex)
    {
        var query = new StringBuilder("/advanced-search/companies?company_status=active");
        query.Append($"&sic_codes={Uri.EscapeDataString(string.Join(",", sicCodes))}");
        query.Append($"&items_per_page={PageSize}");
        query.Append($"&start_index={startIndex}");

        if (!string.IsNullOrWhiteSpace(region))
            query.Append($"&location={Uri.EscapeDataString(region)}");

        return query.ToString();
    }

    private static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private static string ToTitleCase(string name) =>
        System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
}
