using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CompaniesHouseSearch.Models;

namespace CompaniesHouseSearch.Services;

public class CsvExporter
{
    public void Export(IEnumerable<LeadRecord> records, string filePath)
    {
        using var writer = new StreamWriter(filePath, append: false, encoding: System.Text.Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        csv.Context.RegisterClassMap<LeadRecordMap>();
        csv.WriteRecords(records);

        Console.WriteLine($"Exported to: {Path.GetFullPath(filePath)}");
    }
}

public class LeadRecordMap : ClassMap<LeadRecord>
{
    public LeadRecordMap()
    {
        Map(m => m.CompanyNumber).Name("Company Number");
        Map(m => m.CompanyName).Name("Company Name");
        Map(m => m.RegisteredAddress).Name("Registered Address");
        Map(m => m.SICCodes).Name("SIC Codes");
        Map(m => m.CompanyType).Name("Company Type");
        Map(m => m.IncorporatedDate).Name("Incorporated Date");
        Map(m => m.Directors).Name("Directors");
        Map(m => m.ProductMatch).Name("Product Match");
        Map(m => m.MatchScore).Name("Match Score");
        Map(m => m.CompaniesHouseURL).Name("Companies House URL");
    }
}
