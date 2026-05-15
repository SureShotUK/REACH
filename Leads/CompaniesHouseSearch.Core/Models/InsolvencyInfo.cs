using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class InsolvencyInfo
{
    [JsonPropertyName("cases")]
    public List<InsolvencyCase> Cases { get; set; } = [];
}

public class InsolvencyCase
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("dates")]
    public List<InsolvencyDate> Dates { get; set; } = [];

    [JsonPropertyName("practitioners")]
    public List<InsolvencyPractitioner> Practitioners { get; set; } = [];

    [JsonPropertyName("number")]
    public string? Number { get; set; }
}

public class InsolvencyDate
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("date")]
    public string Date { get; set; } = "";
}

public class InsolvencyPractitioner
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("appointed_on")]
    public string? AppointedOn { get; set; }

    [JsonPropertyName("ceased_to_act_on")]
    public string? CeasedToActOn { get; set; }
}
