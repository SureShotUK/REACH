using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class CompanyItem
{
    [JsonPropertyName("company_number")]
    public string CompanyNumber { get; set; } = "";

    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = "";

    [JsonPropertyName("registered_office_address")]
    public RegisteredAddress? Address { get; set; }

    [JsonPropertyName("sic_codes")]
    public List<string> SicCodes { get; set; } = [];

    [JsonPropertyName("company_type")]
    public string CompanyType { get; set; } = "";

    [JsonPropertyName("date_of_creation")]
    public string DateOfCreation { get; set; } = "";

    [JsonPropertyName("company_status")]
    public string CompanyStatus { get; set; } = "";
}

public class RegisteredAddress
{
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; set; }

    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; set; }

    [JsonPropertyName("locality")]
    public string? Locality { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    public override string ToString() => string.Join(", ",
        new[] { AddressLine1, AddressLine2, Locality, Region, PostalCode }
        .Where(s => !string.IsNullOrWhiteSpace(s)));
}
