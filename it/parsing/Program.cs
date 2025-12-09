using System;
using System.IO;
using System.Linq;

namespace GetStoneXOTCDailyValuesConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== StoneX Daily Statement Parser Demo ===\n");

            try
            {
                // Read the CSV file
                string csvPath = "Example.csv";

                if (!File.Exists(csvPath))
                {
                    Console.WriteLine($"Error: Could not find {csvPath}");
                    Console.WriteLine("Make sure Example.csv is in the same directory as the executable.");
                    return;
                }

                string csvContent = File.ReadAllText(csvPath);
                Console.WriteLine($"Loaded CSV file: {csvPath}\n");

                // Create and run the parser
                var parser = new DailyStatementParser(csvContent);
                parser.Parse();

                if (!parser.IsParsed)
                {
                    Console.WriteLine("Error: Parser failed to parse the document.");
                    return;
                }

                Console.WriteLine("Parsing completed successfully!\n");

                // Display Trade Data
                Console.WriteLine("=== TRADE DATA ===");
                Console.WriteLine($"Total trades found: {parser.TradeData.Count}\n");

                foreach (var trade in parser.TradeData)
                {
                    Console.WriteLine($"Trade ID: {trade.TradeId}");
                    Console.WriteLine($"  Published Date: {trade.PublishedDate}");
                    Console.WriteLine($"  Trade Date: {trade.TradeDate}");
                    Console.WriteLine($"  Global ID: {trade.GlobalId?.ToString() ?? "N/A"}");
                    Console.WriteLine($"  Position: {(trade.Long.HasValue ? $"Long {trade.Long.Value}" : $"Short {trade.Short.Value}")}");
                    Console.WriteLine($"  Contract: {trade.ContractDescription}");
                    Console.WriteLine($"  Period: {trade.StartDate} to {trade.EndDate}");
                    Console.WriteLine($"  Trade Price: ${trade.TradePrice:N2}");
                    Console.WriteLine($"  Market Price: ${trade.MarketPrice:N6}");
                    Console.WriteLine($"  Market Value: ${trade.MarketValue:N2}");
                    Console.WriteLine($"  Native MV: ${trade.NativeMv:N2}");
                    Console.WriteLine();
                }

                // Display Account Data
                Console.WriteLine("\n=== ACCOUNT DATA ===");
                var account = parser.AccountData;
                Console.WriteLine($"Published Date: {account.PublishedDate}");
                Console.WriteLine($"Effective Date: {account.EffectiveDate}");
                Console.WriteLine($"Currency: {account.Currency}");
                Console.WriteLine();

                Console.WriteLine("Cash Balances:");
                Console.WriteLine($"  Beginning Balance: ${account.CashBeginningBalance:N2}");
                Console.WriteLine($"  Commission: ${account.Commission:N2}");
                Console.WriteLine($"  Net Profit/Loss: ${account.NetProfitLoss:N2}");
                Console.WriteLine($"  Other Cash Movements: ${account.OtherCashMovements:N2}");
                Console.WriteLine($"  Ending Balance: ${account.CashEndingBalance:N2}");
                Console.WriteLine();

                Console.WriteLine("Market Values:");
                Console.WriteLine($"  Market Value of Open Positions: ${account.MarketValueOfOpen:N2}");
                Console.WriteLine($"  Market Value of Deferred Premiums: ${account.MarketValueOfDeferred:N2}");
                Console.WriteLine($"  Unsettled Premiums: ${account.UnsettledPremiums:N2}");
                Console.WriteLine($"  Net Liquidating Value: ${account.NetLiquidatingValue:N2}");
                Console.WriteLine();

                Console.WriteLine("Margin Information:");
                Console.WriteLine($"  Variation Margin: ${account.VariationMargin:N2}");
                Console.WriteLine($"  Variation Margin Threshold: ${account.VariationMarginThreshold:N2}");
                Console.WriteLine($"  Variation Margin Excess/Deficit: ${account.VariationMarginXsDfct:N2}");
                Console.WriteLine($"  Initial Margin: ${account.InitialMargin:N2}");
                Console.WriteLine($"  Initial Margin Threshold: ${account.InitialMarginThreshold:N2}");
                Console.WriteLine($"  Initial Margin Excess/Deficit: ${account.InitialMarginXsDfct:N2}");
                Console.WriteLine();

                Console.WriteLine("Funds:");
                Console.WriteLine($"  Withdrawable Funds: ${account.WithdrawableFunds:N2}");
                Console.WriteLine($"  Minimum Transfer Amount: ${account.MinimumTransferAmount:N2}");
                Console.WriteLine($"  Rounding: ${account.Rounding:N2}");
                Console.WriteLine($"  Funds Due: ${account.FundsDue:N2}");
                Console.WriteLine();

                // Summary
                Console.WriteLine("\n=== SUMMARY ===");
                Console.WriteLine($"Date: {account.PublishedDate}");
                Console.WriteLine($"Total Open Trades: {parser.TradeData.Count}");
                Console.WriteLine($"Total Market Value: ${parser.TradeData.Sum(t => t.MarketValue ?? 0):N2}");
                Console.WriteLine($"Net Liquidating Value: ${account.NetLiquidatingValue:N2}");
                Console.WriteLine($"Initial Margin Required: ${account.InitialMargin:N2}");

                Console.WriteLine("\n=== END OF REPORT ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
