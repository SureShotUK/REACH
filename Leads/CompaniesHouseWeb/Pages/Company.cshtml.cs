using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CompaniesHouseSearch.Models;
using CompaniesHouseSearch.Services;

namespace CompaniesHouseWeb.Pages;

public class CompanyModel : PageModel
{
    private readonly CompaniesHouseClient _client;
    private readonly LeadScorer _scorer;

    public CompanyProfile?        Profile            { get; private set; }
    public List<OfficerItem>      ActiveOfficers     { get; private set; } = [];
    public List<OfficerItem>      ResignedOfficers   { get; private set; } = [];
    public List<PscItem>          Persons            { get; private set; } = [];
    public InsolvencyInfo?        Insolvency         { get; private set; }
    public FilingHistoryItem?     LatestAccounts     { get; private set; }
    public string                 ProductMatch       { get; private set; } = "";
    public int                    MatchScore         { get; private set; }
    public bool                   IsInsolvent        => Insolvency?.Cases?.Count > 0;

    public CompanyModel(CompaniesHouseClient client, LeadScorer scorer)
    {
        _client = client;
        _scorer = scorer;
    }

    public async Task<IActionResult> OnGetAsync(string? number)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Length > 8)
            return RedirectToPage("/Index", new { error = "invalid" });

        // Pad to 8 digits (Companies House uses zero-padded numbers)
        number = number.Trim().PadLeft(8, '0');

        // Fire all API calls concurrently (except insolvency which we check after profile)
        var profileTask   = _client.GetCompanyProfileAsync(number);
        var officersTask  = _client.GetOfficersAsync(number, resignedWithinMonths: 18);
        var pscTask       = _client.GetPscAsync(number);
        var historyTask   = _client.GetFilingHistoryAsync(number, "accounts");
        var insolvTask    = _client.GetInsolvencyAsync(number);

        await Task.WhenAll(profileTask, officersTask, pscTask, historyTask, insolvTask);

        Profile = await profileTask;
        if (Profile is null)
            return RedirectToPage("/Index", new { error = "notfound" });

        var allOfficers = await officersTask;
        ActiveOfficers   = allOfficers.Where(o => o.IsActive).ToList();
        ResignedOfficers = allOfficers.Where(o => !o.IsActive).ToList();

        Persons      = await pscTask;
        Insolvency   = await insolvTask;

        var history = await historyTask;
        LatestAccounts = history?.Items.FirstOrDefault();

        var (match, score) = _scorer.ScoreFromSicCodes(
            Profile.SicCodes, Profile.DateOfCreation, Profile.CompanyStatus);
        ProductMatch = match;
        MatchScore   = score;

        return Page();
    }

    // Stream the accounts PDF directly to the browser
    public async Task<IActionResult> OnGetPdfAsync(string documentUrl)
    {
        if (string.IsNullOrWhiteSpace(documentUrl))
            return BadRequest();

        var bytes = await _client.GetDocumentBytesAsync(documentUrl);
        if (bytes is null)
            return NotFound();

        return File(bytes, "application/pdf");
    }
}
