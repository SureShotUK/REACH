namespace CompaniesHouseSearch.Models;

public class LeadRecord
{
    public string CompanyNumber { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string RegisteredAddress { get; set; } = "";
    public string SICCodes { get; set; } = "";
    public string CompanyType { get; set; } = "";
    public string IncorporatedDate { get; set; } = "";
    public string Directors { get; set; } = "";
    public string ProductMatch { get; set; } = "";
    public int MatchScore { get; set; }
    public string CompaniesHouseURL { get; set; } = "";
}
