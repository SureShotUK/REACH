using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class CompanyProfile
{
    [JsonPropertyName("company_number")]
    public string CompanyNumber { get; set; } = "";

    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = "";

    [JsonPropertyName("company_status")]
    public string CompanyStatus { get; set; } = "";

    [JsonPropertyName("company_type")]
    public string CompanyType { get; set; } = "";

    [JsonPropertyName("date_of_creation")]
    public string DateOfCreation { get; set; } = "";

    [JsonPropertyName("jurisdiction")]
    public string Jurisdiction { get; set; } = "";

    [JsonPropertyName("registered_office_address")]
    public RegisteredAddress? RegisteredOfficeAddress { get; set; }

    [JsonPropertyName("sic_codes")]
    public List<string> SicCodes { get; set; } = [];

    [JsonPropertyName("accounts")]
    public AccountsInfo? Accounts { get; set; }

    [JsonPropertyName("confirmation_statement")]
    public ConfirmationStatementInfo? ConfirmationStatement { get; set; }

    [JsonPropertyName("has_insolvency_history")]
    public bool HasInsolvencyHistory { get; set; }

    [JsonPropertyName("has_charges")]
    public bool HasCharges { get; set; }
}

public class AccountsInfo
{
    [JsonPropertyName("last_accounts")]
    public LastAccounts? LastAccounts { get; set; }

    [JsonPropertyName("next_accounts")]
    public NextAccounts? NextAccounts { get; set; }

    [JsonPropertyName("accounting_reference_date")]
    public AccountingReferenceDate? AccountingReferenceDate { get; set; }
}

public class LastAccounts
{
    [JsonPropertyName("made_up_to")]
    public string? MadeUpTo { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class NextAccounts
{
    [JsonPropertyName("due_on")]
    public string? DueOn { get; set; }

    [JsonPropertyName("period_end_on")]
    public string? PeriodEndOn { get; set; }

    [JsonPropertyName("overdue")]
    public bool Overdue { get; set; }
}

public class AccountingReferenceDate
{
    [JsonPropertyName("day")]
    public string? Day { get; set; }

    [JsonPropertyName("month")]
    public string? Month { get; set; }
}

public class ConfirmationStatementInfo
{
    [JsonPropertyName("last_made_up_to")]
    public string? LastMadeUpTo { get; set; }

    [JsonPropertyName("next_due")]
    public string? NextDue { get; set; }

    [JsonPropertyName("overdue")]
    public bool Overdue { get; set; }
}
