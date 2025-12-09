using System;
using System.Collections.Generic;
using System.Text;

namespace GetStoneXOTCDailyValuesConsole
{
    internal class StoneXTradeData
    {
        public int TradeId { get; set; }
        public DateOnly TradeDate { get; set; }
        public DateOnly PublishedDate { get; set; }
        public int? GlobalId { get; set; }
        public decimal? Long { get; set; }
        public decimal? Short { get; set; }
        public string? ContractDescription { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TradePrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal? NativeMv { get; set; }
        public decimal? MarketValue { get; set; }
        public string? TriggerBarrier { get; set; }
    }
}
