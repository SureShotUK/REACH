using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class PscResponse
{
    [JsonPropertyName("items")]
    public List<PscItem> Items { get; set; } = [];

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}

public class PscItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "";

    [JsonPropertyName("natures_of_control")]
    public List<string> NaturesOfControl { get; set; } = [];

    [JsonPropertyName("notified_on")]
    public string? NotifiedOn { get; set; }

    [JsonPropertyName("ceased_on")]
    public string? CeasedOn { get; set; }

    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    [JsonPropertyName("country_of_residence")]
    public string? CountryOfResidence { get; set; }

    public bool IsActive => string.IsNullOrEmpty(CeasedOn);

    public string NaturesOfControlFormatted => string.Join(", ",
        NaturesOfControl.Select(n => n.Replace("-", " ")));
}
