using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class CompanySearchResponse
{
    [JsonPropertyName("hits")]
    public int Hits { get; set; }

    [JsonPropertyName("items")]
    public List<CompanyItem> Items { get; set; } = [];
}
