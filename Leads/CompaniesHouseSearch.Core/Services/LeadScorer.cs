using CompaniesHouseSearch.Models;

namespace CompaniesHouseSearch.Services;

public class LeadScorer
{
    private static readonly Dictionary<string, string[]> SicProductMap = new()
    {
        ["49410"] = ["Fuel Cards", "Bunker Networks"],
        ["49310"] = ["Fuel Cards", "Hedging"],
        ["49390"] = ["Fuel Cards", "Hedging"],
        ["01110"] = ["Fuel Cards", "Bulk Delivery"],
        ["01500"] = ["Fuel Cards", "Bulk Delivery"],
        ["01610"] = ["Fuel Cards", "Bulk Delivery"],
        ["01620"] = ["Fuel Cards", "Bulk Delivery"],
        ["46710"] = ["Fuel Cards"],
        ["47300"] = ["Fuel Cards"],
        ["43120"] = ["Fuel Cards", "Bulk Delivery"],
        ["77320"] = ["Fuel Cards", "Bulk Delivery"],
    };

    private static readonly HashSet<string> PremiumProducts = ["Bunker Networks", "Hedging", "Bulk Delivery"];

    public (string ProductMatch, int Score) Score(CompanyItem company)
    {
        var matchedProducts = new HashSet<string>();

        foreach (var sic in company.SicCodes)
            if (SicProductMap.TryGetValue(sic, out var products))
                foreach (var p in products)
                    matchedProducts.Add(p);

        if (matchedProducts.Count == 0) return ("No match", 0);

        int score = 5;

        if (matchedProducts.Any(p => PremiumProducts.Contains(p))) score += 2;

        if (DateTime.TryParse(company.DateOfCreation, out var incorporated))
        {
            var ageYears = (DateTime.Today - incorporated).TotalDays / 365.25;
            if (ageYears >= 10) score += 2;
            else if (ageYears >= 5) score += 1;
        }

        if (company.CompanyStatus.Equals("active", StringComparison.OrdinalIgnoreCase)) score += 1;

        return (string.Join(", ", matchedProducts.Order()), Math.Min(score, 10));
    }

    public (string ProductMatch, int Score) ScoreFromSicCodes(List<string> sicCodes, string dateOfCreation, string status)
    {
        var dummy = new CompanyItem
        {
            SicCodes = sicCodes,
            DateOfCreation = dateOfCreation,
            CompanyStatus = status
        };
        return Score(dummy);
    }
}
