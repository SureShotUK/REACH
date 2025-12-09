using System;
using System.Collections.Generic;
using System.Text;

namespace GetStoneXOTCDailyValuesConsole
{
    internal class StoneXAccountData
    {
        public DateOnly PublishedDate { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public string Currency { get; set; }
        public decimal CashBeginningBalance { get; set; }
        public decimal Commission { get; set; }
        public decimal NetProfitLoss { get; set; }
        public decimal OtherCashMovements { get; set; }
        public decimal CashEndingBalance { get; set; }
        public decimal MarketValueOfOpen { get; set; }
        public decimal Positions { get; set; }
        public decimal MarketValueOfDeferred { get; set; }
        public decimal Premiums { get; set; }
        public decimal UnsettledPremiums { get; set; }
        public decimal NetLiquidatingValue { get; set; }
        public decimal VariationMargin { get; set; }
        public decimal VariationMarginThreshold { get; set; }
        public decimal VariationMarginXsDfct { get; set; }
        public decimal InitialMargin { get; set; }
        public decimal InitialMarginThreshold { get; set; }
        public decimal InitialMarginXsDfct { get; set; }
        public decimal WithdrawableFunds { get; set; }
        public decimal MinimumTransferAmount { get; set; }
        public decimal Rounding { get; set; }
        public decimal FundsDue { get; set; }

    }
}
