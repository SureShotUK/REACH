using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class OfficersResponse
{
    [JsonPropertyName("items")]
    public List<OfficerItem> Items { get; set; } = [];

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}

public class OfficerItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("officer_role")]
    public string OfficerRole { get; set; } = "";

    [JsonPropertyName("appointed_on")]
    public string? AppointedOn { get; set; }

    [JsonPropertyName("resigned_on")]
    public string? ResignedOn { get; set; }

    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    [JsonPropertyName("occupation")]
    public string? Occupation { get; set; }

    [JsonPropertyName("country_of_residence")]
    public string? CountryOfResidence { get; set; }

    public bool IsActive => string.IsNullOrEmpty(ResignedOn);

    public bool ResignedWithinMonths(int months)
    {
        if (string.IsNullOrEmpty(ResignedOn)) return false;
        if (!DateTime.TryParse(ResignedOn, out var resigned)) return false;
        return resigned >= DateTime.Today.AddMonths(-months);
    }
}
