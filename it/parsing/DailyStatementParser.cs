using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GetStoneXOTCDailyValuesConsole
{
    internal class DailyStatementParser(string DailyStatementAsCSVString)
    {
        string _csvString = DailyStatementAsCSVString;
        private bool _isParsed = false;
        public bool IsParsed { get { return _isParsed; } }

        List<StoneXTradeData> _tradeData = [];
        StoneXAccountData _accountData = new();

        public List<StoneXTradeData> TradeData { get { return _tradeData; } }
        public StoneXAccountData AccountData { get { return _accountData; } }

		public void Parse()
{
    if (_isParsed) return;

    var lines = _csvString.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    // Find all "Daily Statement" sections and their dates
    var dailyStatementSections = FindDailyStatementSections(lines);

    // Parse all "Cash Settlements" sections
    var cashSettlementSections = FindCashSettlementSections(lines, dailyStatementSections);
    foreach (var section in cashSettlementSections)
    {
        ParseCashSettlementSection(lines, section.StartIndex, section.Date);
    }

    // Parse all "Open Positions and Market Values" sections (first and last)
    var tradeSections = FindTradeSections(lines, dailyStatementSections);
    foreach (var section in tradeSections)
    {
        ParseTradeSection(lines, section.StartIndex, section.Date);
    }

    // Deduplicate trades based on TradeId + StartDate + EndDate
    DeduplicateTrades();

    // Parse the last "Account Information" section
    var lastAccountSection = FindLastAccountSection(lines, dailyStatementSections);
    if (lastAccountSection.HasValue)
    {
        ParseAccountSection(lines, lastAccountSection.Value.StartIndex, lastAccountSection.Value.Date);
    }

    _isParsed = true;
}

private List<(int StartIndex, DateOnly Date)> FindDailyStatementSections(string[] lines)
{
    var sections = new List<(int StartIndex, DateOnly Date)>();

    for (int i = 0; i < lines.Length - 1; i++)
    {
        var line = lines[i].Trim();
        if (line == "Daily Statement")
        {
            // Next line should contain the date
            if (i + 1 < lines.Length)
            {
                var dateLine = lines[i + 1].Trim();
                if (TryParseDate(dateLine, out var date))
                {
                    sections.Add((i, date));
                }
            }
        }
    }

    return sections;
}

private List<(int StartIndex, DateOnly Date)> FindTradeSections(string[] lines, List<(int StartIndex, DateOnly Date)> dailyStatementSections)
{
    var sections = new List<(int StartIndex, DateOnly Date)>();

    for (int i = 0; i < lines.Length - 1; i++)
    {
        var line = lines[i].Trim();
        if (line == "Open Positions and Market Values")
        {
            // Find the associated date from the nearest Daily Statement section before this line
            var date = FindDateForSection(i, dailyStatementSections);
            sections.Add((i, date));
        }
    }

    // Return first and last sections only
    if (sections.Count == 0) return sections;
    if (sections.Count == 1) return sections;

    return new List<(int, DateOnly)> { sections[0], sections[sections.Count - 1] };
}

private (int StartIndex, DateOnly Date)? FindLastAccountSection(string[] lines, List<(int StartIndex, DateOnly Date)> dailyStatementSections)
{
    for (int i = lines.Length - 1; i >= 0; i--)
    {
        var line = lines[i].Trim();
        if (line == "Account Information")
        {
            // Find the associated date from the nearest Daily Statement section before this line
            var date = FindDateForSection(i, dailyStatementSections);
            return (i, date);
        }
    }
    return null;
}

private DateOnly FindDateForSection(int sectionIndex, List<(int StartIndex, DateOnly Date)> dailyStatementSections)
{
    // Find the most recent Daily Statement section before this section
    DateOnly date = DateOnly.MinValue;

    foreach (var section in dailyStatementSections)
    {
        if (section.StartIndex < sectionIndex)
        {
            date = section.Date;
        }
        else
        {
            break;
        }
    }

    return date;
}

private List<(int StartIndex, DateOnly Date)> FindCashSettlementSections(string[] lines, List<(int StartIndex, DateOnly Date)> dailyStatementSections)
{
    var sections = new List<(int StartIndex, DateOnly Date)>();

    for (int i = 0; i < lines.Length - 1; i++)
    {
        var line = lines[i].Trim();
        if (line == "Cash Settlements")
        {
            // Find the associated date from the nearest Daily Statement section before this line
            var date = FindDateForSection(i, dailyStatementSections);
            sections.Add((i, date));
        }
    }

    return sections;
}

private void ParseCashSettlementSection(string[] lines, int startIndex, DateOnly publishedDate)
{
    int i = startIndex;

    // Skip until we find the header row for Commodity Cash Settlements
    while (i < lines.Length && !lines[i].Contains("Trade Date,Trade Id,Long,Short,Type"))
    {
        i++;
    }

    if (i >= lines.Length) return;
    i++; // Move past header

    // Parse cash settlements until we hit the next major section or a total line
    while (i < lines.Length)
    {
        var line = lines[i].Trim();

        // Stop if we hit a major section
        if (line == "Open Positions and Market Values" ||
            line == "Account Information" ||
            line == "Disclaimers")
        {
            break;
        }

        // Skip page breaks, disclaimers, and repeated headers but continue parsing
        if (string.IsNullOrWhiteSpace(line) ||
            line.StartsWith("Page ") ||
            line == "Disclaimers" ||
            line.Contains("StoneX Financial Ltd is a subsidiary") ||
            line == "Daily Statement" ||
            line == "Activity" ||
            line == "Cash Settlements" ||
            line == "Commodity Cash Settlements" ||
            line.Contains("StoneX Financial Ltd,") ||
            line.Contains("1st Floor Moor House") ||
            line.Contains("Telephone +44") ||
            line.Contains("Registered in England") ||
            line.Contains("Authorised and regulated") ||
            line.Contains("Trade Date,Trade Id,Long,Short,Type"))
        {
            i++;
            continue;
        }

        // Check if this is a total line (starts with $)
        if (line.StartsWith("$"))
        {
            // This is the total, we can stop parsing this section
            break;
        }

        // Check if this line starts with a date pattern (cash settlement data row)
        var parts = line.Split(',');
        if (parts.Length > 1 && TryParseDate(parts[0], out var tradeDate))
        {
            // This is a main cash settlement data row
            var trade = ParseCashSettlementDataRow(lines, ref i, tradeDate, publishedDate);
            if (trade != null)
            {
                _tradeData.Add(trade);
            }
        }
        else
        {
            i++;
        }
    }
}

private StoneXTradeData? ParseCashSettlementDataRow(string[] lines, ref int index, DateOnly tradeDate, DateOnly publishedDate)
{
    var line = lines[index].Trim();
    var parts = line.Split(',');

    try
    {
        // Parse the main data row for cash settlement
        // Expected format: Trade Date, Trade Id, Quantity, Type, Description, Trade Price, Settlement Price, Cash Amount, Expiry Date, Applied On

        int partIndex = 1; // Skip trade date (already parsed)
        var tradeId = int.Parse(parts[partIndex++]);

        // Quantity (could be Long or Short, we'll determine from confirmation line)
        var quantity = decimal.Parse(parts[partIndex++]);

        // Type (should be "CASH")
        var type = parts[partIndex++];

        // Description/Contract Description
        var contractDescription = parts[partIndex++];

        // Trade Price
        var tradePrice = ParseCurrency(parts[partIndex++]);

        // Settlement Price (maps to MarketPrice)
        var settlementPrice = decimal.Parse(parts[partIndex++].Replace("$", "").Replace(",", ""));

        // Cash Amount (maps to MarketValue)
        var cashAmount = ParseCurrency(parts[partIndex++]);

        // Expiry Date (maps to EndDate)
        var expiryDate = ParseDate(parts[partIndex++]);

        // Applied On date (we can use this as the trade's effective date if needed)
        var appliedOn = ParseDate(parts[partIndex++]);

        // Check the next line for quantity confirmation
        index++;
        bool isLong = true; // Default to Long
        decimal confirmedQuantity = quantity;

        if (index < lines.Length)
        {
            var confirmLine = lines[index].Trim();
            var confirmParts = confirmLine.Split(',');

            // The confirmation line should have quantity and cash amount
            if (confirmParts.Length >= 2)
            {
                if (decimal.TryParse(confirmParts[0], out var qtyFromConfirm))
                {
                    confirmedQuantity = qtyFromConfirm;
                }

                // Determine if Long or Short based on cash amount sign
                // If cash amount is negative and we're receiving (buying), it's typically a short position settling
                // If cash amount is positive and we're paying out, it's typically a long position settling
                // However, the sign convention may vary, so we'll use the quantity as-is
                // and assume positive quantity means Long
                isLong = confirmedQuantity > 0;
            }
        }

        var trade = new StoneXTradeData
        {
            TradeId = tradeId,
            TradeDate = tradeDate,
            PublishedDate = publishedDate,
            GlobalId = null, // Cash settlements don't have GlobalId
            Long = isLong ? Math.Abs(confirmedQuantity) : null,
            Short = isLong ? null : Math.Abs(confirmedQuantity),
            ContractDescription = contractDescription,
            StartDate = appliedOn, // Using Applied On as start date
            EndDate = expiryDate,
            TradePrice = tradePrice,
            MarketPrice = settlementPrice, // Settlement Price maps to MarketPrice
            NativeMv = null, // Not provided in cash settlements
            MarketValue = cashAmount // Cash Amount maps to MarketValue
        };

        index++; // Move past the confirmation line
        return trade;
    }
    catch (Exception ex)
    {
        // Log or handle parsing errors
        Console.WriteLine($"Error parsing cash settlement at line {index}: {ex.Message}");
        index++;
        return null;
    }
}

private void ParseTradeSection(string[] lines, int startIndex, DateOnly publishedDate)
{
    int i = startIndex;

    // Skip until we find the header row
    while (i < lines.Length && !lines[i].Contains("Trade Date,Trade Id,Global Id"))
    {
        i++;
    }

    if (i >= lines.Length) return;
    i++; // Move past header

    // Parse trades until we hit the next major section
    while (i < lines.Length)
    {
        var line = lines[i].Trim();

        // Stop only if we hit a truly different section (not page breaks)
        if (line == "Account Information" ||
            line == "Activity" ||
            line == "New Trades")
        {
            break;
        }

        // Skip page breaks, disclaimers, and repeated headers but continue parsing
        if (string.IsNullOrWhiteSpace(line) ||
            line.StartsWith("Page ") ||
            line == "Disclaimers" ||
            line.Contains("StoneX Financial Ltd is a subsidiary") ||
            line == "Daily Statement" ||
            line.Contains("StoneX Financial Ltd,") ||
            line.Contains("1st Floor Moor House") ||
            line.Contains("Telephone +44") ||
            line.Contains("Registered in England") ||
            line.Contains("Authorised and regulated") ||
            line.Contains("Trade Date,Trade Id,Global Id") ||
            line == "Commodity Open Positions")
        {
            i++;
            continue;
        }

        // Check if this line starts with a date pattern (trade data row)
        var parts = line.Split(',');
        if (parts.Length > 1 && TryParseDate(parts[0], out var tradeDate))
        {
            // This is a main trade data row
            var trade = ParseTradeDataRow(lines, ref i, tradeDate, publishedDate);
            if (trade != null)
            {
                _tradeData.Add(trade);
            }
        }
        else
        {
            i++;
        }
    }
}

private StoneXTradeData? ParseTradeDataRow(string[] lines, ref int index, DateOnly tradeDate, DateOnly publishedDate)
{
    var line = lines[index].Trim();
    var parts = line.Split(',');

    try
    {
        // Check if contract description is on previous line(s)
        var contractDescription = "";
        int descStartIndex = index - 1;

        // Look back for contract description lines (lines that don't start with dates)
        while (descStartIndex >= 0)
        {
            var prevLine = lines[descStartIndex].Trim();
            var prevParts = prevLine.Split(',');

            // Stop if we hit a line that starts with a date or is a header/section marker
            if (TryParseDate(prevParts[0], out _) ||
                prevLine.Contains("Trade Date,Trade Id") ||
                prevLine == "Commodity Open Positions" ||
                prevLine.Contains("Long Avg") ||
                prevLine.Contains("Short Avg") ||
                prevLine.Split(",").Length == 3 ||
                prevLine == "")
            {
                break;
            }

            contractDescription = prevLine + " " + contractDescription;
            descStartIndex--;
        }

        // Parse the main data row
        // partIndex starts at 1 to skip the trade date (parts[0]) which was already parsed
        int partIndex = 1;
        var tradeId = int.Parse(parts[partIndex++]);
        var globalId = parts[partIndex++] == "0" ? (int?)null : int.Parse(parts[partIndex - 1]);

        // Next field is the quantity (Long or Short, we'll determine from next rows)
        var quantity = decimal.Parse(parts[partIndex++]);

        // Check if contract description is inline (rest of fields until we hit dates)
        int inlineDescStart = partIndex;
        while (partIndex < parts.Length && !TryParseDate(parts[partIndex], out _))
        {
            contractDescription += (string.IsNullOrWhiteSpace(contractDescription) ? "" : " ") + parts[partIndex++];
        }

        // Check for contract description on next line(s) after the data row
        int nextLineIndex = index + 1;
        while (nextLineIndex < lines.Length)
        {
            var nextLine = lines[nextLineIndex].Trim();
            var nextParts = nextLine.Split(',');

            // Stop if we hit "Long Avg" or "Short Avg"
            if (nextLine.StartsWith("Long Avg") || nextLine.StartsWith("Short Avg"))
            {
                break;
            }

            // Stop if it's a data line
            if (TryParseDate(nextParts[0], out _))
            {
                break;
            }

            // If it's a short line with no comma, it's likely part of contract description
            if (nextParts.Length <= 2 && !string.IsNullOrWhiteSpace(nextLine))
            {
                contractDescription += " " + nextLine;
                nextLineIndex++;
            }
            else
            {
                break;
            }
        }

        contractDescription = contractDescription.Trim();

        // Parse dates and prices
        var startDate = ParseDate(parts[partIndex++]);
        var endDate = ParseDate(parts[partIndex++]);
        var tradePrice = ParseCurrency(parts[partIndex++]);
        var marketPrice = decimal.Parse(parts[partIndex++]);
        var nativeMv = ParseCurrency(parts[partIndex++]);
        var marketValue = ParseCurrency(parts[partIndex++]);

        // Now look for "Long Avg" or "Short Avg" in the next row
        index++;
        bool isLong = false;
        while (index < lines.Length)
        {
            var avgLine = lines[index].Trim();
            if (avgLine.StartsWith("Long Avg"))
            {
                isLong = true;
                break;
            }
            else if (avgLine.StartsWith("Short Avg"))
            {
                isLong = false;
                break;
            }
            index++;
        }

        // Move to next line to get the actual quantity from row 26-style line
        index++;
        if (index < lines.Length)
        {
            var qtyLine = lines[index].Trim();
            var qtyParts = qtyLine.Split(',');
            if (qtyParts.Length > 0)
            {
                if (decimal.TryParse(qtyParts[0], out var actualQty))
                {
                    quantity = actualQty;
                }
            }
        }

        var trade = new StoneXTradeData
        {
            TradeId = tradeId,
            TradeDate = tradeDate,
            PublishedDate = publishedDate,
            GlobalId = globalId,
            Long = isLong ? quantity : null,
            Short = isLong ? null : quantity,
            ContractDescription = contractDescription,
            StartDate = startDate,
            EndDate = endDate,
            TradePrice = tradePrice,
            MarketPrice = marketPrice,
            NativeMv = nativeMv,
            MarketValue = marketValue
        };

        index++; // Move past the quantity line
        return trade;
    }
    catch (Exception ex)
    {
        // Log or handle parsing errors
        Console.WriteLine($"Error parsing trade at line {index}: {ex.Message}");
        index++;
        return null;
    }
}

private void ParseAccountSection(string[] lines, int startIndex, DateOnly publishedDate)
{
    _accountData.PublishedDate = publishedDate;
    _accountData.EffectiveDate = publishedDate;

    int i = startIndex + 1; // Skip "Account Information" line

    while (i < lines.Length)
    {
        var line = lines[i].Trim();

        // Stop at section boundaries
        if (string.IsNullOrWhiteSpace(line) ||
            line.StartsWith("Page ") ||
            line == "Disclaimers" ||
            line.Contains("StoneX Financial Ltd is a subsidiary"))
        {
            break;
        }

        var parts = line.Split(',');
        if (parts.Length >= 2)
        {
            var label = parts[0].Trim();
            var value = parts[1].Trim();

            switch (label)
            {
                case "Currency":
                    _accountData.Currency = value;
                    break;
                case "Cash Beginning Balance":
                    _accountData.CashBeginningBalance = ParseCurrency(value);
                    break;
                case "Commission":
                    _accountData.Commission = ParseCurrency(value);
                    break;
                case "Net Profit/Loss":
                    _accountData.NetProfitLoss = ParseCurrency(value);
                    break;
                case "Other Cash Movements":
                    _accountData.OtherCashMovements = ParseCurrency(value);
                    break;
                case "Cash Ending Balance":
                    _accountData.CashEndingBalance = ParseCurrency(value);
                    break;
                case "Market Value of Open":
                    _accountData.MarketValueOfOpen = ParseCurrency(value);
                    // Next line is "Positions" continuation - skip it
                    i++;
                    break;
                case "Market Value of Deferred":
                    _accountData.MarketValueOfDeferred = ParseCurrency(value);
                    // Next line is "Premiums" continuation - skip it
                    i++;
                    break;
                case "Unsettled Premiums":
                    _accountData.UnsettledPremiums = ParseCurrency(value);
                    break;
                case "Net Liquidating Value":
                    _accountData.NetLiquidatingValue = ParseCurrency(value);
                    break;
                case "Variation Margin":
                    _accountData.VariationMargin = ParseCurrency(value);
                    break;
                case "Variation Margin Threshold":
                    _accountData.VariationMarginThreshold = ParseCurrency(value);
                    break;
                case "Variation Margin Excess/Deficit":
                    _accountData.VariationMarginXsDfct = ParseCurrency(value);
                    break;
                case "Initial Margin":
                    _accountData.InitialMargin = ParseCurrency(value);
                    break;
                case "Initial Margin Threshold":
                    _accountData.InitialMarginThreshold = ParseCurrency(value);
                    break;
                case "Initial Margin Excess/Deficit":
                    _accountData.InitialMarginXsDfct = ParseCurrency(value);
                    break;
                case "Withdrawable Funds":
                    _accountData.WithdrawableFunds = ParseCurrency(value);
                    break;
                case "Minimum Transfer Amount":
                    _accountData.MinimumTransferAmount = ParseCurrency(value);
                    break;
                case "Rounding":
                    _accountData.Rounding = ParseCurrency(value);
                    break;
                case "Funds Due":
                    _accountData.FundsDue = ParseCurrency(value);
                    break;
            }
        }

        i++;
    }
}

private void DeduplicateTrades()
{
    // Group by TradeId + StartDate + EndDate and keep the last occurrence
    var deduplicatedTrades = _tradeData
        .GroupBy(t => new { t.TradeId, t.StartDate, t.EndDate })
        .Select(g => g.Last())
        .ToList();

    _tradeData = deduplicatedTrades;
}

private decimal ParseCurrency(string value)
{
    // Remove $, commas, and handle parentheses for negative values
    value = value.Trim();
    bool isNegative = value.StartsWith("(") && value.EndsWith(")");

    value = value.Replace("$", "")
                .Replace(",", "")
                .Replace("(", "")
                .Replace(")", "");

    if (decimal.TryParse(value, out var result))
    {
        return isNegative ? -result : result;
    }

    return 0;
}

private DateOnly ParseDate(string value)
{
    value = value.Trim();

    // Try multiple date formats
    string[] formats = { "dd-MMM-yyyy", "d-MMM-yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };

    if (DateOnly.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
    {
        return date;
    }

    // Fallback to default parsing
    if (DateOnly.TryParse(value, out date))
    {
        return date;
    }

    return DateOnly.MinValue;
}

private bool    TryParseDate(string value, out DateOnly date)
{
    value = value.Trim();

    // Try multiple date formats
    string[] formats = { "dd-MMM-yyyy", "d-MMM-yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };

    if (DateOnly.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
    {
        return true;
    }

    // Fallback to default parsing
    return DateOnly.TryParse(value, out date);
}
        
    }
}
