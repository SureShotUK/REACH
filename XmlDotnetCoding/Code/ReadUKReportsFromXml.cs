using System;
using System.Collections.Generic;
using System.Linq;
using PortlandTradeReporterLib.Models;
using PortlandTradeReporterLib.Xml.UK.Sending.DerivativesTradeReport;
using PortlandAzureDataAccess.Trading;
using PortlandAzureDataAccess.CustomerData;

namespace XmlDotnetCoding
{
    /// <summary>
    /// Reverse-engineers XML trade reports back into TradeAndValuationProperties objects
    /// </summary>
    public class ReadUKReportsFromXml
    {
        private readonly TradingContext _db;
        private readonly CustomerDataContext _custDb;
        private readonly TradeFileReader _fileReader;

        public ReadUKReportsFromXml(TradingContext db, CustomerDataContext custDb)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _custDb = custDb ?? throw new ArgumentNullException(nameof(custDb));
            _fileReader = new TradeFileReader();
        }

        /// <summary>
        /// Reads a trade report XML file and converts it to a list of TradeAndValuationProperties
        /// </summary>
        /// <param name="filePath">Path to the XML file</param>
        /// <returns>List of TradeAndValuationProperties extracted from the report</returns>
        public List<TradeAndValuationProperties> ReadTradeReportsFromFile(string filePath)
        {
            var tradeReport = _fileReader.ReadTradeFile(filePath);
            return ConvertToTradeAndValuationProperties(tradeReport);
        }

        /// <summary>
        /// Converts a DerivativesTradeReportV03 to a list of TradeAndValuationProperties
        /// </summary>
        /// <param name="tradeReport">The trade report from XML</param>
        /// <returns>List of TradeAndValuationProperties</returns>
        public List<TradeAndValuationProperties> ConvertToTradeAndValuationProperties(DerivativesTradeReportV03 tradeReport)
        {
            if (tradeReport == null)
            {
                throw new ArgumentNullException(nameof(tradeReport), "Trade report cannot be null");
            }

            var results = new List<TradeAndValuationProperties>();
            var reportItems = _fileReader.GetTradeReportItems(tradeReport);

            foreach (var reportItem in reportItems)
            {
                var tradeProps = ConvertReportItemToTradeProps(reportItem);
                if (tradeProps != null)
                {
                    results.Add(tradeProps);
                }
            }

            return results;
        }

        /// <summary>
        /// Converts a single TradeReport32Choice__1 to TradeAndValuationProperties
        /// </summary>
        /// <param name="reportItem">The report item to convert</param>
        /// <returns>TradeAndValuationProperties object</returns>
        private TradeAndValuationProperties ConvertReportItemToTradeProps(TradeReport32Choice__1 reportItem)
        {
            if (reportItem?.Item == null)
            {
                return null;
            }

            var itemType = reportItem.Item.GetType().Name;

            return itemType switch
            {
                "TradeData42__1" => ConvertNewTrade((TradeData42__1)reportItem.Item),
                "TradeData42__2" => ConvertModifyTrade((TradeData42__2)reportItem.Item),
                "TradeData42__4" => ConvertTerminationTrade((TradeData42__4)reportItem.Item),
                "TradeData42__7" => ConvertErrorTrade((TradeData42__7)reportItem.Item),
                _ => null
            };
        }

        #region Convert New Trade (NEWT)
        private TradeAndValuationProperties ConvertNewTrade(TradeData42__1 tradeData)
        {
            var props = new TradeAndValuationProperties(_db, _custDb)
            {
                Actiontype = "NEWT",
                Level = GetLevelFromModificationLevel(tradeData.Lvl)
            };

            // Extract counterparty data
            ExtractCounterpartyData(tradeData.CtrPtySpcfcData, props);

            // Extract common trade data
            if (tradeData.CmonTradData != null)
            {
                ExtractContractData(tradeData.CmonTradData.CtrctData, props);
                ExtractTransactionData(tradeData.CmonTradData.TxData, props);
            }

            return props;
        }
        #endregion

        #region Convert Modify Trade (MODI)
        private TradeAndValuationProperties ConvertModifyTrade(TradeData42__2 tradeData)
        {
            var props = new TradeAndValuationProperties(_db, _custDb)
            {
                Actiontype = "MODI",
                Level = GetLevelFromModificationLevel(tradeData.Lvl)
            };

            // Extract counterparty data
            ExtractCounterpartyData(tradeData.CtrPtySpcfcData, props);

            // Extract common trade data
            if (tradeData.CmonTradData != null)
            {
                ExtractContractData(tradeData.CmonTradData.CtrctData, props);
                ExtractTransactionData(tradeData.CmonTradData.TxData, props);
            }

            return props;
        }
        #endregion

        #region Convert Termination Trade (TERM)
        private TradeAndValuationProperties ConvertTerminationTrade(TradeData42__4 tradeData)
        {
            var props = new TradeAndValuationProperties(_db, _custDb)
            {
                Actiontype = "TERM",
                Level = GetLevelFromModificationLevel(tradeData.Lvl)
            };

            // Extract counterparty data (limited for termination)
            ExtractCounterpartyDataForTermOrError(tradeData.CtrPtySpcfcData, props);

            // Extract transaction data for termination
            if (tradeData.CmonTradData?.TxData != null)
            {
                ExtractTransactionDataForTermination(tradeData.CmonTradData.TxData, props);
            }

            return props;
        }
        #endregion

        #region Convert Error Trade (EROR)
        private TradeAndValuationProperties ConvertErrorTrade(TradeData42__7 tradeData)
        {
            var props = new TradeAndValuationProperties(_db, _custDb)
            {
                Actiontype = "EROR",
                Level = GetLevelFromModificationLevel(tradeData.Lvl)
            };

            // Extract counterparty data (limited for error)
            ExtractCounterpartyDataForTermOrError(tradeData.CtrPtySpcfcData, props);

            // Extract transaction data for error
            if (tradeData.CmonTradData?.TxData != null)
            {
                ExtractTransactionDataForError(tradeData.CmonTradData.TxData, props);
            }

            return props;
        }
        #endregion

        #region Extract Counterparty Data
        private void ExtractCounterpartyData(CounterpartySpecificData36__1 ctrPtyData, TradeAndValuationProperties props)
        {
            if (ctrPtyData == null) return;

            // Reporting timestamp
            props.Reportingtimestamp = ctrPtyData.RptgTmStmp;

            if (ctrPtyData.CtrPty == null) return;

            // Submitting agent
            if (ctrPtyData.CtrPty.SubmitgAgt?.Item is string submitAgent)
            {
                props.ReportsubmittingentityID = submitAgent;
            }

            // Entity responsible for reporting
            if (ctrPtyData.CtrPty.NttyRspnsblForRpt?.Item is string entityResp)
            {
                props.Entityresponsibleforreporting = entityResp;
            }

            // Reporting counterparty (Counterparty 1)
            if (ctrPtyData.CtrPty.RptgCtrPty != null)
            {
                ExtractReportingCounterparty(ctrPtyData.CtrPty.RptgCtrPty, props);
            }

            // Other counterparty (Counterparty 2)
            if (ctrPtyData.CtrPty.OthrCtrPty != null)
            {
                ExtractOtherCounterparty(ctrPtyData.CtrPty.OthrCtrPty, props);
            }

            // Broker
            if (ctrPtyData.CtrPty.Brkr?.Item is string brokerLei)
            {
                props.BrokerID = brokerLei;
            }

            // Clearing member
            if (ctrPtyData.CtrPty.ClrMmb?.Item is LegalPersonIdentification1__1 clearingMember)
            {
                if (clearingMember.Id?.Item is string clrMemberLei)
                {
                    props.Clearingmember = clrMemberLei;
                }
            }
        }

        private void ExtractCounterpartyData(CounterpartySpecificData36__2 ctrPtyData, TradeAndValuationProperties props)
        {
            if (ctrPtyData == null) return;

            props.Reportingtimestamp = ctrPtyData.RptgTmStmp;

            if (ctrPtyData.CtrPty == null) return;

            if (ctrPtyData.CtrPty.SubmitgAgt?.Item is string submitAgent)
            {
                props.ReportsubmittingentityID = submitAgent;
            }

            if (ctrPtyData.CtrPty.NttyRspnsblForRpt?.Item is string entityResp)
            {
                props.Entityresponsibleforreporting = entityResp;
            }

            if (ctrPtyData.CtrPty.RptgCtrPty != null)
            {
                ExtractReportingCounterparty(ctrPtyData.CtrPty.RptgCtrPty, props);
            }

            if (ctrPtyData.CtrPty.OthrCtrPty != null)
            {
                ExtractOtherCounterparty(ctrPtyData.CtrPty.OthrCtrPty, props);
            }

            if (ctrPtyData.CtrPty.Brkr?.Item is string brokerLei)
            {
                props.BrokerID = brokerLei;
            }

            if (ctrPtyData.CtrPty.ClrMmb?.Item is LegalPersonIdentification1__1 clearingMember)
            {
                if (clearingMember.Id?.Item is string clrMemberLei)
                {
                    props.Clearingmember = clrMemberLei;
                }
            }
        }

        private void ExtractCounterpartyDataForTermOrError(CounterpartySpecificData36__3 ctrPtyData, TradeAndValuationProperties props)
        {
            if (ctrPtyData == null) return;

            props.Reportingtimestamp = ctrPtyData.RptgTmStmp;

            if (ctrPtyData.CtrPty == null) return;

            if (ctrPtyData.CtrPty.SubmitgAgt?.Item is string submitAgent)
            {
                props.ReportsubmittingentityID = submitAgent;
            }

            if (ctrPtyData.CtrPty.NttyRspnsblForRpt?.Item is string entityResp)
            {
                props.Entityresponsibleforreporting = entityResp;
            }

            if (ctrPtyData.CtrPty.RptgCtrPty != null)
            {
                ExtractReportingCounterpartyLimited(ctrPtyData.CtrPty.RptgCtrPty, props);
            }

            if (ctrPtyData.CtrPty.OthrCtrPty != null)
            {
                ExtractOtherCounterpartyLimited(ctrPtyData.CtrPty.OthrCtrPty, props);
            }
        }

        private void ExtractReportingCounterparty(Counterparty45__1 rptgCtrPty, TradeAndValuationProperties props)
        {
            if (rptgCtrPty?.Id?.Item is LegalPersonIdentification1__1 legalPerson)
            {
                if (legalPerson.Id?.Item is string lei)
                {
                    props.Counterparty1Reportingcounterparty = lei;
                }
            }

            // Direction
            if (rptgCtrPty?.DrctnOrSd?.Item != null)
            {
                if (rptgCtrPty.DrctnOrSd.Item is OptionParty1Code direction)
                {
                    props.Direction = direction.ToString();
                }
                else if (rptgCtrPty.DrctnOrSd.Item is Direction2__1 direction2)
                {
                    props.Directionofleg1 = direction2.DrctnOfTheFrstLeg.ToString();
                    props.Directionofleg2 = direction2.DrctnOfTheScndLeg.ToString();
                }
            }

            // Nature and sector data is skipped as it comes from database
        }

        private void ExtractReportingCounterpartyLimited(Counterparty45__2 rptgCtrPty, TradeAndValuationProperties props)
        {
            if (rptgCtrPty?.Id?.Item is LegalPersonIdentification1__1 legalPerson)
            {
                if (legalPerson.Id?.Item is string lei)
                {
                    props.Counterparty1Reportingcounterparty = lei;
                }
            }
        }

        private void ExtractOtherCounterparty(Counterparty46__1 othrCtrPty, TradeAndValuationProperties props)
        {
            if (othrCtrPty?.IdTp?.Item is LegalPersonIdentification1__1 legalPerson)
            {
                if (legalPerson.Id?.Item is string lei)
                {
                    props.Counterparty2 = lei;
                }
            }
        }

        private void ExtractOtherCounterpartyLimited(Counterparty46__2 othrCtrPty, TradeAndValuationProperties props)
        {
            if (othrCtrPty?.IdTp?.Item is LegalPersonIdentification1__1 legalPerson)
            {
                if (legalPerson.Id?.Item is string lei)
                {
                    props.Counterparty2 = lei;
                }
            }
        }
        #endregion

        #region Extract Contract Data
        private void ExtractContractData(ContractType14__1 ctrctData, TradeAndValuationProperties props)
        {
            if (ctrctData == null) return;

            // Contract type
            props.ContractType = ctrctData.CtrctTp.ToString();

            // Asset class
            props.Assetclass = ctrctData.AsstClss.ToString();

            // Product classification
            if (!string.IsNullOrWhiteSpace(ctrctData.PdctClssfctn))
            {
                props.Productclassification = ctrctData.PdctClssfctn;
            }

            // Derivative based on crypto assets
            props.Derivativebasedoncryptoassets = ctrctData.DerivBasedOnCrptAsst;

            // Product ID
            if (ctrctData.PdctId != null)
            {
                if (!string.IsNullOrWhiteSpace(ctrctData.PdctId.ISIN))
                {
                    props.ISIN = ctrctData.PdctId.ISIN;
                }

                if (ctrctData.PdctId.UnqPdctIdr?.Item is string upi)
                {
                    props.UniqueproductidentifierUPI = upi;
                }
            }

            // Settlement currency
            if (ctrctData.SttlmCcy?.Ccy != null)
            {
                props.Settlementcurrency1 = ctrctData.SttlmCcy.Ccy;
            }
        }
        #endregion

        #region Extract Transaction Data (New/Modify)
        private void ExtractTransactionData(TradeTransaction49__1 txData, TradeAndValuationProperties props)
        {
            if (txData == null) return;

            // UTI
            if (txData.TxId?.Item is string uti)
            {
                props.UTI = uti;
            }

            // Venue of execution
            if (!string.IsNullOrWhiteSpace(txData.PltfmIdr))
            {
                props.Venueofexecution = txData.PltfmIdr;
            }

            // Execution timestamp
            props.Executiontimestamp = txData.ExctnTmStmp;

            // Effective date
            props.Effectivedate = txData.FctvDt;

            // Expiration date
            if (txData.XprtnDtSpecified)
            {
                props.Expirationdate = txData.XprtnDt;
            }

            // Settlement date
            if (txData.SttlmDtSpecified)
            {
                props.Finalcontractualsettlementdate = txData.SttlmDt;
            }

            // Delivery type
            props.Deliverytype = txData.DlvryTp.ToString();

            // Post-trade risk reduction flag
            if (txData.PstTradRskRdctnFlgSpecified)
            {
                props.PTRR = txData.PstTradRskRdctnFlg;
            }

            // Collateral portfolio
            ExtractCollateralPortfolio(txData.CollPrtflCd, props);

            // Trade confirmation
            ExtractTradeConfirmation(txData.TradConf, props);

            // Trade clearing
            ExtractTradeClearing(txData.TradClr, props);

            // Master agreement
            ExtractMasterAgreement(txData.MstrAgrmt, props);

            // Price
            if (txData.TxPric != null)
            {
                ExtractPrice(txData.TxPric, props);
            }

            // Notional amount
            if (txData.NtnlAmt != null)
            {
                ExtractNotionalAmount(txData.NtnlAmt, props);
            }

            // Notional quantity
            if (txData.NtnlQty != null)
            {
                ExtractNotionalQuantity(txData.NtnlQty, props);
            }

            // Commodity
            if (txData.Cmmdty != null)
            {
                ExtractCommodity(txData.Cmmdty, props);
            }

            // Option
            if (txData.Optn != null)
            {
                ExtractOption(txData.Optn, props);
            }

            // Derivative event
            if (txData.DerivEvt != null)
            {
                ExtractDerivativeEvent(txData.DerivEvt, props);
            }
        }

        private void ExtractTransactionData(TradeTransaction49__2 txData, TradeAndValuationProperties props)
        {
            if (txData == null) return;

            // UTI (for modify, it's nested differently)
            if (txData.TxId?.Item is GenericIdentification175__3 utiId)
            {
                props.UTI = utiId.Id;
            }

            // Prior UTI
            if (txData.PrrTxId?.Item is GenericIdentification175__3 priorUti)
            {
                props.PriorUTIforonetooneandonetomanyrelationsbetweentransactions = priorUti.Id;
            }

            // Venue of execution
            if (!string.IsNullOrWhiteSpace(txData.PltfmIdr))
            {
                props.Venueofexecution = txData.PltfmIdr;
            }

            // Execution timestamp
            props.Executiontimestamp = txData.ExctnTmStmp;

            // Effective date
            props.Effectivedate = txData.FctvDt;

            // Expiration date
            if (txData.XprtnDtSpecified)
            {
                props.Expirationdate = txData.XprtnDt;
            }

            // Settlement date
            if (txData.SttlmDtSpecified)
            {
                props.Finalcontractualsettlementdate = txData.SttlmDt;
            }

            // Delivery type
            props.Deliverytype = txData.DlvryTp.ToString();

            // Post-trade risk reduction flag
            if (txData.PstTradRskRdctnFlgSpecified)
            {
                props.PTRR = txData.PstTradRskRdctnFlg;
            }

            // Collateral portfolio
            ExtractCollateralPortfolio(txData.CollPrtflCd, props);

            // Trade confirmation
            ExtractTradeConfirmation(txData.TradConf, props);

            // Trade clearing
            ExtractTradeClearing(txData.TradClr, props);

            // Master agreement
            ExtractMasterAgreement(txData.MstrAgrmt, props);

            // Price
            if (txData.TxPric != null)
            {
                ExtractPrice(txData.TxPric, props);
            }

            // Notional amount
            if (txData.NtnlAmt != null)
            {
                ExtractNotionalAmount(txData.NtnlAmt, props);
            }

            // Notional quantity
            if (txData.NtnlQty != null)
            {
                ExtractNotionalQuantity(txData.NtnlQty, props);
            }

            // Commodity
            if (txData.Cmmdty != null)
            {
                ExtractCommodity(txData.Cmmdty, props);
            }

            // Option
            if (txData.Optn != null)
            {
                ExtractOption(txData.Optn, props);
            }

            // Derivative event
            if (txData.DerivEvt != null)
            {
                ExtractDerivativeEvent(txData.DerivEvt, props);
            }
        }
        #endregion

        #region Extract Transaction Data (Termination)
        private void ExtractTransactionDataForTermination(TradeTransaction49__4 txData, TradeAndValuationProperties props)
        {
            if (txData == null) return;

            // UTI
            if (txData.TxId?.Item is GenericIdentification175__3 utiId)
            {
                props.UTI = utiId.Id;
            }

            // Early termination date
            props.Earlyterminationdate = txData.EarlyTermntnDt;

            // Post-trade risk reduction flag
            if (txData.PstTradRskRdctnFlgSpecified)
            {
                props.PTRR = txData.PstTradRskRdctnFlg;
            }

            // Derivative event
            if (txData.DerivEvt != null)
            {
                ExtractDerivativeEvent(txData.DerivEvt, props);
            }
        }
        #endregion

        #region Extract Transaction Data (Error)
        private void ExtractTransactionDataForError(TradeTransaction49__6 txData, TradeAndValuationProperties props)
        {
            if (txData == null) return;

            // UTI
            if (txData.TxId?.Item is GenericIdentification175__3 utiId)
            {
                props.UTI = utiId.Id;
            }

            // Post-trade risk reduction flag
            if (txData.PstTradRskRdctnFlgSpecified)
            {
                props.PTRR = txData.PstTradRskRdctnFlg;
            }

            // Derivative event
            if (txData.DerivEvt != null)
            {
                ExtractDerivativeEvent(txData.DerivEvt, props);
            }
        }
        #endregion

        #region Helper Methods for Transaction Data Elements

        private void ExtractCollateralPortfolio(CollateralPortfolioCode5Choice__1 collPrtflCd, TradeAndValuationProperties props)
        {
            if (collPrtflCd?.Item is PortfolioCode3Choice portfolio)
            {
                if (portfolio.Item is NotApplicable1Code)
                {
                    props.Collateralportfolioindicator = false;
                }
                else if (portfolio.Item is string portfolioCode)
                {
                    props.Collateralportfolioindicator = true;
                    props.Collateralportfoliocode = portfolioCode;
                }
            }
        }

        private void ExtractTradeConfirmation(TradeConfirmation1Choice tradConf, TradeAndValuationProperties props)
        {
            if (tradConf?.Item is TradeConfirmation2 confirmed)
            {
                props.Confirmed = confirmed.Tp.ToString();
                props.Confirmationtimestamp = confirmed.TmStmp;
            }
            else if (tradConf?.Item is TradeNonConfirmation1 nonConfirmed)
            {
                props.Confirmed = nonConfirmed.Tp.ToString();
            }
        }

        private void ExtractTradeClearing(TradeClearing11__1 tradClr, TradeAndValuationProperties props)
        {
            if (tradClr == null) return;

            // Clearing obligation
            if (tradClr.ClrOblgtnSpecified)
            {
                props.Clearingobligation = tradClr.ClrOblgtn.ToString();
            }

            // Clearing status
            if (tradClr.ClrSts?.Item is ClearingPartyAndTime21Choice__1 cleared)
            {
                if (cleared.Item is ClearingPartyAndTime22__1 clearingDetails)
                {
                    props.Cleared = 'Y';
                    if (clearingDetails.CCP?.Item is string ccpLei)
                    {
                        props.Centralcounterparty = ccpLei;
                    }
                    if (clearingDetails.ClrDtTmSpecified)
                    {
                        props.Clearingtimestamp = clearingDetails.ClrDtTm;
                    }
                }
            }
            else if (tradClr.ClrSts?.Item is ClearingExceptionOrExemption3Choice__1)
            {
                props.Cleared = 'N';
            }

            // Intra-group
            if (tradClr.IntraGrpSpecified)
            {
                props.Intragroup = tradClr.IntraGrp;
            }
        }

        private void ExtractMasterAgreement(MasterAgreement8__1 mstrAgrmt, TradeAndValuationProperties props)
        {
            if (mstrAgrmt == null) return;

            // Master agreement type
            if (mstrAgrmt.Tp?.Item is MasterAgreementType2Code agreementType)
            {
                props.MasterAgreementtype = agreementType.ToString();
            }

            // Other master agreement type
            if (!string.IsNullOrWhiteSpace(mstrAgrmt.OthrMstrAgrmtDtls))
            {
                props.Othermasteragreementtype = mstrAgrmt.OthrMstrAgrmtDtls;
            }

            // Version
            if (!string.IsNullOrWhiteSpace(mstrAgrmt.Vrsn))
            {
                if (DateTime.TryParse(mstrAgrmt.Vrsn, out var version))
                {
                    props.MasterAgreementversion = version;
                }
            }
        }

        private void ExtractPrice(PriceData2__1 txPric, TradeAndValuationProperties props)
        {
            if (txPric?.Pric?.Item == null) return;

            if (txPric.Pric.Item is AmountAndDirection106__1 monetary)
            {
                var amount = monetary.Amt.Value;
                props.SetPrice(amount, "MONETARY");
                props.Pricecurrency = monetary.Amt.Ccy;
            }
            else if (txPric.Pric.Item is decimal percentage)
            {
                props.SetPrice(percentage, "PERCENTAGE");
            }
        }

        private void ExtractNotionalAmount(NotionalAmountLegs5__1 ntnlAmt, TradeAndValuationProperties props)
        {
            if (ntnlAmt == null) return;

            // First leg
            if (ntnlAmt.FrstLeg?.Amt != null)
            {
                props.Notionalamountofleg1 = ntnlAmt.FrstLeg.Amt.Amt.Value;
                props.Notionalcurrency1 = ntnlAmt.FrstLeg.Amt.Amt.Ccy;

                // Schedule period for leg 1
                if (ntnlAmt.FrstLeg.SchdlPrd != null && ntnlAmt.FrstLeg.SchdlPrd.Length > 0)
                {
                    var schedule = ntnlAmt.FrstLeg.SchdlPrd[0];
                    props.Effectivedateofthenotionalamountofleg1 = schedule.UadjstdFctvDt;
                    props.Enddateofthenotionalamountofleg1 = schedule.UadjstdEndDt;
                    if (schedule.Amt != null)
                    {
                        props.Notionalamountineffectonassociatedeffectivedateofleg1 = schedule.Amt.Amt.Value;
                    }
                }
            }

            // Second leg
            if (ntnlAmt.ScndLeg?.Amt != null)
            {
                props.Notionalamountofleg2 = ntnlAmt.ScndLeg.Amt.Amt.Value;
                props.Notionalcurrency2 = ntnlAmt.ScndLeg.Amt.Amt.Ccy;

                // Schedule period for leg 2
                if (ntnlAmt.ScndLeg.SchdlPrd != null && ntnlAmt.ScndLeg.SchdlPrd.Length > 0)
                {
                    var schedule = ntnlAmt.ScndLeg.SchdlPrd[0];
                    props.Effectivedateofthenotionalamountofleg2 = schedule.UadjstdFctvDt;
                    props.Enddateofthenotionalamountofleg2 = schedule.UadjstdEndDt;
                    if (schedule.Amt != null)
                    {
                        props.Notionalamountineffectonassociatedeffectivedateofleg2 = schedule.Amt.Amt.Value;
                    }
                }
            }
        }

        private void ExtractNotionalQuantity(NotionalQuantityLegs5__1 ntnlQty, TradeAndValuationProperties props)
        {
            if (ntnlQty?.FrstLeg == null) return;

            // First leg total quantity
            if (ntnlQty.FrstLeg.TtlQtySpecified)
            {
                props.Totalnotionalquantityofleg1 = ntnlQty.FrstLeg.TtlQty;
            }

            // Note: Schedule period data (effective dates and quantity in effect) is not available
            // in NotionalQuantity structure - it's only in NotionalAmount structure

            // Second leg
            if (ntnlQty.ScndLeg != null)
            {
                if (ntnlQty.ScndLeg.TtlQtySpecified)
                {
                    props.Totalnotionalquantityofleg2 = ntnlQty.ScndLeg.TtlQty;
                }
            }
        }

        private void ExtractCommodity(AssetClassCommodity6Choice__1 cmmdty, TradeAndValuationProperties props)
        {
            if (cmmdty?.Item is AssetClassCommodityEnergy3Choice__1 energy)
            {
                if (energy.Item is EnergyCommodityOil3__1 oil)
                {
                    props.Baseproduct = oil.BasePdct.ToString();
                    props.Subproduct = oil.SubPdct.ToString();
                    props.Furthersubproduct = oil.AddtlSubPdct.ToString();
                }
            }
            // Add other commodity types as needed
        }

        private void ExtractOption(OptionOrSwaption10__1 optn, TradeAndValuationProperties props)
        {
            if (optn == null) return;

            // Option type
            if (optn.TpSpecified)
            {
                props.Optiontype = optn.Tp.ToString();
            }

            // Option style
            if (optn.ExrcStyleSpecified)
            {
                props.Optionstyle = optn.ExrcStyle.ToString();
            }

            // Premium amount
            if (optn.PrmAmt != null)
            {
                props.Optionpremiumamount = optn.PrmAmt.Value;
                props.Optionpremiumcurrency = optn.PrmAmt.Ccy;
            }

            // Premium payment date
            if (optn.PrmPmtDtSpecified)
            {
                props.Optionpremiumpaymentdate = optn.PrmPmtDt;
            }

            // Strike price
            if (optn.StrkPric?.Item is AmountAndDirection106__1 strikePrice)
            {
                props.SetStrikePrice(strikePrice.Amt.Value, "MONETARY");
                props.Strikepricecurrency_currencypair = strikePrice.Amt.Ccy;
            }

            // Maturity date of underlying
            if (optn.MtrtyDtOfUndrlygSpecified)
            {
                props.Maturitydateoftheunderlying = optn.MtrtyDtOfUndrlyg;
            }
        }

        private void ExtractDerivativeEvent(DerivativeEvent6__1 derivEvt, TradeAndValuationProperties props)
        {
            if (derivEvt == null) return;

            // Event type
            props.Eventtype = derivEvt.Tp.ToString();

            // Event date
            if (derivEvt.TmStmp?.Item is DateTime eventDate)
            {
                props.Eventdate = eventDate;
            }
        }

        private void ExtractDerivativeEvent(DerivativeEvent6__2 derivEvt, TradeAndValuationProperties props)
        {
            if (derivEvt == null) return;

            // Event type
            props.Eventtype = derivEvt.Tp.ToString();

            // Event date
            if (derivEvt.TmStmp?.Item is DateTime eventDate)
            {
                props.Eventdate = eventDate;
            }
        }

        private void ExtractDerivativeEvent(DerivativeEvent6__4 derivEvt, TradeAndValuationProperties props)
        {
            if (derivEvt == null) return;

            // Event type
            props.Eventtype = derivEvt.Tp.ToString();

            // Event date
            if (derivEvt.TmStmp?.Item is DateTime eventDate)
            {
                props.Eventdate = eventDate;
            }
        }

        private void ExtractDerivativeEvent(DerivativeEvent6__5 derivEvt, TradeAndValuationProperties props)
        {
            if (derivEvt == null) return;

            // Event date
            if (derivEvt.TmStmp?.Item is DateTime eventDate)
            {
                props.Eventdate = eventDate;
            }
        }

        #endregion

        #region Utility Methods
        private string GetLevelFromModificationLevel(ModificationLevel1Code level)
        {
            return level.ToString();
        }
        #endregion
    }
}
