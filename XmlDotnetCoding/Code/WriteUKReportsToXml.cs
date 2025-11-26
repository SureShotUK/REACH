using PortlandExcelLib;
using PortlandTradeReporterLib.Enums;
using PortlandTradeReporterLib.Models;
using PortlandTradeReporterLib.Xml.UK.Sending.DerivativesTradeReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortlandTradeReporterLib
{
    internal class WriteUKReportsToXml
    {
        private readonly List<TradeAndValuationProperties> _reports;
        private readonly string _tradeRef;
        public WriteUKReportsToXml(List<TradeAndValuationProperties> Reports, string TradeRef)
        {
            _reports = Reports;
            _tradeRef = TradeRef;
        }

        internal void WriteReportsToUKXml(string reportFolder = "")
        {
            if (_reports.Count <= 0) return;

            TradeData57Choice__1 tradData = new();
            // tradData.Items should be the Rpt which is of type TradeReport32Choice__1
            List<TradeReport32Choice__1> reportList = new();

            foreach (var report in _reports)
            {
                reportList.Add(MakeNewTradDataRpt(report));
            }
            if(string.IsNullOrWhiteSpace(reportFolder)) reportFolder = TradeAndValConstants.UKTradeReportPath;
            if (_reports[0].counterparty2.country == "DE" || _reports[0].counterparty1.country == "DE")
            {
                reportFolder = TradeAndValConstants.EUTradeReportPath;
            }

            UKMessageCreator xmlWriter = new(reportList, _tradeRef, reportFolder);
            xmlWriter.CreateXmlReport();
        }

        /// <summary>
        /// This is to report a New Trade
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        private static TradeReport32Choice__1 MakeNewTradDataRpt(TradeAndValuationProperties report)
        {
            string actionType = report.Actiontype;
            switch(actionType)
            {
                case "NEWT":
                    return WriteReportForNewTrade(report);
                case "MODI":
                    return WriteReportForModifyTrade(report);
                case "TERM":
                    return WriteReportForEarlyTermination(report);
                case "EROR":
                    return WriteReportForErrorReport(report);
                default:
                    throw new ArgumentException($"Invalid action type: {actionType}");
            }
        }

        private static TradeReport32Choice__1 WriteReportForNewTrade(TradeAndValuationProperties report)
        {
            ModificationLevel1Code level = UKEnumHelper.ReturnModificationLevel1CodeFromString(report.Level);

            TradeReport32Choice__1 rpt = new TradeReport32Choice__1()
            {
                Item = new TradeData42__1()
                {
                    CmonTradData = NewTradeCmonTradDataToXml(report),
                    CtrPtySpcfcData = NewTradeCtrPtySpcfcDataToXml(report),
                    Lvl = level
                }
            };
            return rpt;
        }

        private static TradeReport32Choice__1 WriteReportForModifyTrade(TradeAndValuationProperties report)
        {
            ModificationLevel1Code level = UKEnumHelper.ReturnModificationLevel1CodeFromString(report.Level);

            TradeReport32Choice__1 rpt = new TradeReport32Choice__1()
            {
                Item = new TradeData42__2()
                {
                    CmonTradData = ModifyTradeCmonTradDataToXml(report),
                    CtrPtySpcfcData = ModifyTradeCtrPtySpcfcDataToXml(report),
                    Lvl = level
                }
            };
            return rpt;
        }

        private static TradeReport32Choice__1 WriteReportForEarlyTermination(TradeAndValuationProperties report)
        {
            ModificationLevel1Code level = UKEnumHelper.ReturnModificationLevel1CodeFromString(report.Level);

            TradeReport32Choice__1 rpt = new TradeReport32Choice__1()
            {
                Item = new TradeData42__4()
                {
                    CmonTradData = EarlyTermTradeCmonTradDataToXml(report),
                    CtrPtySpcfcData = EarlyTermOrErrorTradeCtrPtySpcfcDataToXml(report),
                    Lvl = level
                }
            };
            return rpt;
        }

        private static TradeReport32Choice__1 WriteReportForErrorReport(TradeAndValuationProperties report)
        {
            ModificationLevel1Code level = UKEnumHelper.ReturnModificationLevel1CodeFromString(report.Level);

            TradeReport32Choice__1 rpt = new()
            {
                Item = new TradeData42__7()
                {
                    CmonTradData = ErrorTradeCmonTradDataToXml(report),
                    CtrPtySpcfcData = EarlyTermOrErrorTradeCtrPtySpcfcDataToXml(report),
                    Lvl = level
                }
            };
            return rpt;
        }

        #region CtrPtySpcfcData  - CounterpartySpecificData36__1   TODO
        private static CounterpartySpecificData36__1 NewTradeCtrPtySpcfcDataToXml(TradeAndValuationProperties report)
        {
            CounterpartySpecificData36__1 ctrPrySpcfcData = new CounterpartySpecificData36__1()
            {
                RptgTmStmp = report.Reportingtimestamp,
                CtrPty = GetTradeCounterpartyReport20__1(report)
            };
            return ctrPrySpcfcData;
        }

        private static CounterpartySpecificData36__2 ModifyTradeCtrPtySpcfcDataToXml(TradeAndValuationProperties report)
        {
            CounterpartySpecificData36__2 ctrPrySpcfcData = new CounterpartySpecificData36__2()
            {
                RptgTmStmp = report.Reportingtimestamp,
                CtrPty = GetTradeCounterpartyReport20__1(report)
            };
            return ctrPrySpcfcData;
        }

        private static CounterpartySpecificData36__3 EarlyTermOrErrorTradeCtrPtySpcfcDataToXml(TradeAndValuationProperties report)
        {
            CounterpartySpecificData36__3 ctrPrySpcfcData = new ()
            {
                RptgTmStmp = report.Reportingtimestamp,
                CtrPty = GetTradeCounterpartyReport20__2(report)
            };
            return ctrPrySpcfcData;
        }

        //private static CounterpartySpecificData36__3 ErrorTradeCtrPtySpcfcDataToXml(TradeAndValuationProperties report)
        //{
        //    CounterpartySpecificData36__3 ctrPrySpcfcData = new CounterpartySpecificData36__3()
        //    {
        //        RptgTmStmp = report.Reportingtimestamp,
        //        CtrPty = GetTradeCounterpartyReport20__2(report)
        //    };
        //    return ctrPrySpcfcData;
        //}
        #endregion

        #region New Trade Report Elements - Common Trade Data  - CommonTradeDataReport69__1
        private static CommonTradeDataReport69__1 NewTradeCmonTradDataToXml(TradeAndValuationProperties report)
        {
            CommonTradeDataReport69__1 CmonTradData = new CommonTradeDataReport69__1()
            {
                CtrctData = NewTradeContractData(report),
                TxData = NewTradeTxData(report),
            };
            return CmonTradData;
        }

        private static CommonTradeDataReport69__2 ModifyTradeCmonTradDataToXml(TradeAndValuationProperties report)
        {
            CommonTradeDataReport69__2 CmonTradData = new CommonTradeDataReport69__2()
            {
                CtrctData = NewTradeContractData(report),
                TxData = ModifyTradeTxData(report),
            };
            return CmonTradData;
        }

        private static CommonTradeDataReport69__4 EarlyTermTradeCmonTradDataToXml(TradeAndValuationProperties report)
        {
            CommonTradeDataReport69__4 CmonTradData = new ()
            {
                //CtrctData = EarlyTermContractData(report), // TODO Finish this off for Early Termination
                TxData = EarlyTermTxData(report),
            };
            return CmonTradData;
        }

        private static CommonTradeDataReport69__6 ErrorTradeCmonTradDataToXml(TradeAndValuationProperties report)
        {
            CommonTradeDataReport69__6 CmonTradData = new()
            {
                //CtrctData = EarlyTermContractData(report), // TODO Finish this off for Early Termination
                TxData = ErrorTxData(report),
            };
            return CmonTradData;
        }

        #endregion New Trade Report Elements

        #region Common Trade Contract Data - CtrctData ContractType14__1



        #endregion  Common Trade Contract Data - ContractType14__1

        #region TxData - TradeTransaction49__  1 or 2
        internal static TradeTransaction49__1 NewTradeTxData(TradeAndValuationProperties report)
        {
            TradeTransaction49__1 txData = new TradeTransaction49__1();

            // TxId
            if (!string.IsNullOrWhiteSpace(report.UTI))          // UTI
            {
                UniqueTransactionIdentifier2Choice__1 utixml = new() { Item = report.UTI };
                txData.TxId = utixml;
            }
            txData.CollPrtflCd = GetCollateralPortfolioCode5Choice__1(report); // CollPrtflCd
            if (!string.IsNullOrWhiteSpace(report.Confirmed))       // TradConf
            {
                txData.TradConf = GetTradeConfirmation1Choice(report);
            }
            txData.TradClr = GetTradeClearing11__1(report); // TradClr
            txData.MstrAgrmt = GetMasterAgreement8__1(report);
            // PstTradRskRdctnFlg
            if (report.PTRR.HasValue)
            {
                txData.PstTradRskRdctnFlg = report.PTRR.Value;
                txData.PstTradRskRdctnFlgSpecified = true;
            }
            txData.PltfmIdr = report.Venueofexecution;      // Pltfmldr
            txData.ExctnTmStmp = report.Executiontimestamp;     // ExctnTmStmp
            txData.FctvDt = report.Effectivedate;       // FctvDt
            if (report.Expirationdate.HasValue)     // XprtnDt
            {
                txData.XprtnDt = report.Expirationdate.Value;
                txData.XprtnDtSpecified = true;
            }
            if (report.Finalcontractualsettlementdate.HasValue)     // SttlmtDt
            {
                txData.SttlmDt = report.Finalcontractualsettlementdate.Value;
                txData.SttlmDtSpecified = true;
            }
            txData.DlvryTp = UKEnumHelper.ReturnPhysicalTransferType4CodeFromString(report.Deliverytype);       // DlvryTp
            if (report.Price is not null)           // TxPric
            {
                txData.TxPric = GetPriceData2__1(report);
            }
            txData.NtnlAmt = GetNotionalAmountLegs5__1(report);     // NtnlAmt
            if (report.Totalnotionalquantityofleg1.HasValue)        // NtnlQty
            {
                txData.NtnlQty = GetNotionalQuantityLegs5__1(report);
            }
            if (!string.IsNullOrWhiteSpace(report.Baseproduct))     // Cmmdty
            {
                txData.Cmmdty = GetCommdtyProducts(report);
            }
            
            if (!string.IsNullOrWhiteSpace(report.Optiontype) && !string.IsNullOrWhiteSpace(report.Optionstyle) &&
                report.Optionpremiumpaymentdate.HasValue && !report.Maturitydateoftheunderlying.HasValue)
            {                                                                                                       // Optn
                txData.Optn = GetOptionOrSwaption10__1(report);
            }

            txData.DerivEvt = new()
            {
                TmStmp = new DateAndDateTime2Choice__1()
                {
                    Item = report.Eventdate
                }
            };

            if (!string.IsNullOrWhiteSpace(report.Eventtype))
            {
                txData.DerivEvt.Tp = UKEnumHelper.ReturnDerivativeEventType3Code__1FromString(report.Eventtype);
            }

            return txData;
        }

        internal static TradeTransaction49__2 ModifyTradeTxData(TradeAndValuationProperties report)
        {
            TradeTransaction49__2 txData = new TradeTransaction49__2();

            // TxId
            if (!string.IsNullOrWhiteSpace(report.UTI))          // UTI
            {
                UniqueTransactionIdentifier2Choice__2 utixml = new() 
                { 
                    Item = new GenericIdentification175__3() { Id = report.UTI }
                };
                txData.TxId = utixml;
            }
            txData.CollPrtflCd = GetCollateralPortfolioCode5Choice__1(report); // CollPrtflCd
            if (!string.IsNullOrWhiteSpace(report.Confirmed))       // TradConf
            {
                txData.TradConf = GetTradeConfirmation1Choice(report);
            }
            txData.TradClr = GetTradeClearing11__1(report); // TradClr
            txData.MstrAgrmt = GetMasterAgreement8__1(report);
            // PstTradRskRdctnFlg
            if (report.PTRR.HasValue)
            {
                txData.PstTradRskRdctnFlg = report.PTRR.Value;
                txData.PstTradRskRdctnFlgSpecified = true;
            }
            txData.PltfmIdr = report.Venueofexecution;      // Pltfmldr
            txData.ExctnTmStmp = report.Executiontimestamp;     // ExctnTmStmp
            txData.FctvDt = report.Effectivedate;       // FctvDt
            if (report.Expirationdate.HasValue)     // XprtnDt
            {
                txData.XprtnDt = report.Expirationdate.Value;
                txData.XprtnDtSpecified = true;
            }
            if (report.Finalcontractualsettlementdate.HasValue)     // SttlmtDt
            {
                txData.SttlmDt = report.Finalcontractualsettlementdate.Value;
                txData.SttlmDtSpecified = true;
            }
            txData.DlvryTp = UKEnumHelper.ReturnPhysicalTransferType4CodeFromString(report.Deliverytype);       // DlvryTp
            if (report.Price is not null)           // TxPric
            {
                txData.TxPric = GetPriceData2__1(report);
            }
            txData.NtnlAmt = GetNotionalAmountLegs5__1(report);     // NtnlAmt
            if (report.Totalnotionalquantityofleg1.HasValue)        // NtnlQty
            {
                txData.NtnlQty = GetNotionalQuantityLegs5__1(report);
            }
            if (!string.IsNullOrWhiteSpace(report.Baseproduct))     // Cmmdty
            {
                txData.Cmmdty = GetCommdtyProducts(report);
            }

            if (!string.IsNullOrWhiteSpace(report.Optiontype) && !string.IsNullOrWhiteSpace(report.Optionstyle) &&
                report.Optionpremiumpaymentdate.HasValue && !report.Maturitydateoftheunderlying.HasValue)
            {                                                                                                       // Optn
                txData.Optn = GetOptionOrSwaption10__1(report);
            }

            txData.DerivEvt = new()
            {
                TmStmp = new DateAndDateTime2Choice__1()
                {
                    Item = report.Eventdate
                }
            };

            if (!string.IsNullOrWhiteSpace(report.Eventtype))
            {
                txData.DerivEvt.Tp = UKEnumHelper.ReturnDerivativeEventType3Code__1FromString(report.Eventtype);
                txData.DerivEvt.TpSpecified = true;
            }

            if (!string.IsNullOrWhiteSpace(report.PriorUTIforonetooneandonetomanyrelationsbetweentransactions))
            {
                UniqueTransactionIdentifier3Choice__1 utiIdentifier = new()
                {
                    // Item = report.PriorUTIforonetooneandonetomanyrelationsbetweentransactions
                    Item = new GenericIdentification175__3()
                    {
                        Id = report.PriorUTIforonetooneandonetomanyrelationsbetweentransactions
                    }
                };

                //GenericIdentification175__3 ptryId = new()
                //{
                //    Id = report.PriorUTIforonetooneandonetomanyrelationsbetweentransactions
                //};

                txData.PrrTxId = utiIdentifier;
            }

            return txData;
        }

        internal static TradeTransaction49__4 EarlyTermTxData(TradeAndValuationProperties report)
        {
            TradeTransaction49__4 txData = new();

            // TxId
            if (!string.IsNullOrWhiteSpace(report.UTI))          // UTI
            {
                UniqueTransactionIdentifier2Choice__2 utixml = new()
                {
                    Item = new GenericIdentification175__3() { Id = report.UTI }
                };
                txData.TxId = utixml;
            }
            //txData.CollPrtflCd = GetCollateralPortfolioCode5Choice__1(report); // CollPrtflCd
            
            
            // PstTradRskRdctnFlg
            if (report.PTRR.HasValue && report.PTRR.Value)
            {
                txData.PstTradRskRdctnFlg = report.PTRR.Value;
                txData.PstTradRskRdctnFlgSpecified = true;
            }
           
            txData.EarlyTermntnDt = report.Earlyterminationdate ?? DateTime.Today;     // EarlyterminationDate
           


            txData.DerivEvt = new()
            {
                TmStmp = new DateAndDateTime2Choice__1()
                {
                    Item = report.Eventdate
                }
            };

            if (!string.IsNullOrWhiteSpace(report.Eventtype))
            {
                txData.DerivEvt.Tp = UKEnumHelper.ReturnDerivativeEventType3Code__1FromString(report.Eventtype);
            }


            return txData;
        }

        // ErrorTxData
        internal static TradeTransaction49__6 ErrorTxData(TradeAndValuationProperties report)
        {
            TradeTransaction49__6 txData = new();

            // TxId
            if (!string.IsNullOrWhiteSpace(report.UTI))          // UTI
            {
                UniqueTransactionIdentifier2Choice__2 utixml = new()
                {
                    Item = new GenericIdentification175__3() { Id = report.UTI }
                };
                txData.TxId = utixml;
            }
            //txData.CollPrtflCd = GetCollateralPortfolioCode5Choice__1(report); // CollPrtflCd


            // PstTradRskRdctnFlg
            if (report.PTRR.HasValue && report.PTRR.Value)
            {
                txData.PstTradRskRdctnFlg = report.PTRR.Value;
                txData.PstTradRskRdctnFlgSpecified = true;
            }

            txData.DerivEvt = new DerivativeEvent6__5()
            {
                TmStmp = new DateAndDateTime2Choice__1()
                {
                    Item = report.Eventdate
                }
            };

            return txData;
        }

        #endregion

        #region CtrPtySpcfcData Elements Common To Both New and Modify reports

        private static TradeCounterpartyReport20__1 GetTradeCounterpartyReport20__1(TradeAndValuationProperties report)
        {
            TradeCounterpartyReport20__1 CtrPty = new()
            {
                SubmitgAgt = new OrganisationIdentification15Choice__1()
                {
                    Item = report.ReportsubmittingentityID
                },
                NttyRspnsblForRpt = new OrganisationIdentification15Choice__1()
                {
                    Item = report.Entityresponsibleforreporting
                },
                RptgCtrPty = new Counterparty45__1()
                {
                    // This could be either FTC or the client depending on whether this is our report or the report we are filing on behalf of the customer
                    Id = new PartyIdentification248Choice__1()
                    {
                        Item = new LegalPersonIdentification1__1()
                        {
                            Id = new OrganisationIdentification15Choice__1()
                            {
                                Item = report.Counterparty1Reportingcounterparty
                            }
                        }
                    },
                    Ntr = new CounterpartyTradeNature15Choice__1()
                    {
                        // This is filled in in the next section
                    }
                }
            };
            // RptgCtrPty
            if (!string.IsNullOrWhiteSpace(report.Corporatesectorofthecounterparty1) && report.Clearingthresholdofcounterparty1.HasValue)
            {
                if (report.Natureofthecounterparty1 == 'F')
                {
                    CtrPty.RptgCtrPty.Ntr = new CounterpartyTradeNature15Choice__1()
                    {
                        Item = new FinancialInstitutionSector1__1()
                        {
                            Sctr =
                            [
                                new FinancialPartyClassification2Choice__1(){ Item = UKEnumHelper.ReturnFinancialPartySectorType3Code__1FromString(report.Corporatesectorofthecounterparty1) }
                            ],
                            ClrThrshld = report.Clearingthresholdofcounterparty1.Value
                        },
                        ItemElementName = ItemChoiceType.FI
                    };
                }
                else if (report.Natureofthecounterparty1 == 'N' && report.Directlylinkedtocommercialactivityortreasuryfinancing.HasValue)
                {
                    CtrPty.RptgCtrPty.Ntr = new CounterpartyTradeNature15Choice__1()
                    {
                        Item = new NonFinancialInstitutionSector10__1()
                        {
                            Sctr =
                            [
                                new GenericIdentification175__1(){ Id = report.Corporatesectorofthecounterparty1 }
                            ],
                            ClrThrshld = report.Clearingthresholdofcounterparty1.Value,
                            DrctlyLkdActvty = report.Directlylinkedtocommercialactivityortreasuryfinancing.Value,
                            DrctlyLkdActvtySpecified = true
                        },
                        ItemElementName = ItemChoiceType.NFI
                    };
                }
                else if (report.Natureofthecounterparty1 == 'C')
                {
                    CtrPty.RptgCtrPty.Ntr.Item = NoReasonCode.NORE;
                }
                else if (report.Natureofthecounterparty1 == 'O')
                {
                    CtrPty.RptgCtrPty.Ntr.Item = NoReasonCode.NORE;
                }
            }
            if (!string.IsNullOrWhiteSpace(report.Direction))
            {
                OptionParty1Code direction = OptionParty1Code.SLLR;
                if (report.Direction == "BYER")
                {
                    direction = OptionParty1Code.BYER;
                }
                CtrPty.RptgCtrPty.DrctnOrSd = new Direction4Choice__1()
                {
                    Item = direction
                };
            }
            else if (!string.IsNullOrWhiteSpace(report.Directionofleg1) && !string.IsNullOrWhiteSpace(report.Directionofleg2))
            {
                CtrPty.RptgCtrPty.DrctnOrSd = new Direction4Choice__1()
                {
                    Item = new Direction2__1()
                    {
                        DrctnOfTheFrstLeg = UKEnumHelper.ReturnOptionParty3CodeFromString(report.Directionofleg1),
                        DrctnOfTheScndLeg = UKEnumHelper.ReturnOptionParty3CodeFromString(report.Directionofleg2)
                    }
                };
            }


            // OthrCtrPty
            if (!string.IsNullOrWhiteSpace(report.counterparty2.lei) && report.Counterparty2identifiertype)
            {
                CtrPty.OthrCtrPty = new Counterparty46__1()
                {
                    IdTp = new PartyIdentification248Choice__2()
                    {
                        Item = new LegalPersonIdentification1__1()
                        {
                            Id = new OrganisationIdentification15Choice__1()
                            {
                                Item = report.Counterparty2
                            }//,
                            //Ctry = report.Countryofthecounterparty2
                        }
                    },
                };

                if (!string.IsNullOrWhiteSpace(report.Corporatesectorofthecounterparty2) && report.Clearingthresholdofcounterparty2.HasValue)
                {
                    if (report.Natureofthecounterparty2 == 'F')
                    {
                        CtrPty.OthrCtrPty.Ntr = new CounterpartyTradeNature15Choice__2()
                        {
                            Item = new FinancialInstitutionSector1__1()
                            {
                                Sctr =
                                [
                                    new FinancialPartyClassification2Choice__1(){ Item = UKEnumHelper.ReturnFinancialPartySectorType3Code__1FromString(report.Corporatesectorofthecounterparty2) }
                                ],
                                ClrThrshld = report.Clearingthresholdofcounterparty2.Value
                            },
                            ItemElementName = ItemChoiceType1.FI
                        };
                    }
                    else if (report.Natureofthecounterparty2 == 'N')
                    {
                        CtrPty.OthrCtrPty.Ntr = new CounterpartyTradeNature15Choice__2()
                        {
                            Item = new NonFinancialInstitutionSector10__2()
                            {
                                Sctr =
                                [
                                    new GenericIdentification175__1(){ Id = report.Corporatesectorofthecounterparty2 }
                                ],
                                ClrThrshld = report.Clearingthresholdofcounterparty2.Value
                            },
                            ItemElementName = ItemChoiceType1.NFI
                        };
                    }
                    else if (report.Natureofthecounterparty2 == 'C')
                    {
                        CtrPty.OthrCtrPty.Ntr.Item = NoReasonCode.NORE;
                    }
                    else if (report.Natureofthecounterparty2 == 'O')
                    {
                        CtrPty.OthrCtrPty.Ntr.Item = NoReasonCode.NORE;
                    }
                }
                CtrPty.OthrCtrPty.RptgOblgtn = report.Reportingobligationofthecounterparty2;
            }

            // Brkr
            if (!string.IsNullOrWhiteSpace(report.BrokerID))
            {
                CtrPty.Brkr = new OrganisationIdentification15Choice__1()
                {
                    Item = report.BrokerID
                };
            }
            // ClrMmb
            if (!string.IsNullOrWhiteSpace(report.Clearingmember))
            {
                CtrPty.ClrMmb = new PartyIdentification248Choice__1()
                {
                    Item = new LegalPersonIdentification1__1()
                    {
                        Id = new OrganisationIdentification15Choice__1()
                        {
                            Item = report.Clearingmember
                        }
                    }
                };
            }
            // NttyRspnsblForRpt
            if (!string.IsNullOrWhiteSpace(report.Entityresponsibleforreporting))
            {
                CtrPty.NttyRspnsblForRpt = new OrganisationIdentification15Choice__1()
                {
                    Item = report.Entityresponsibleforreporting
                };
            }

            // SubmitgAgt
            if (!string.IsNullOrWhiteSpace(report.ReportsubmittingentityID))
            {
                CtrPty.SubmitgAgt = new OrganisationIdentification15Choice__1()
                {
                    Item = report.ReportsubmittingentityID
                };
            }

            return CtrPty;
        }

        private static TradeCounterpartyReport20__2 GetTradeCounterpartyReport20__2(TradeAndValuationProperties report)
        {
            TradeCounterpartyReport20__2 CtrPty = new()
            {
                SubmitgAgt = new OrganisationIdentification15Choice__1()
                {
                    Item = report.ReportsubmittingentityID
                },
                NttyRspnsblForRpt = new OrganisationIdentification15Choice__1()
                {
                    Item = report.Entityresponsibleforreporting
                },
                RptgCtrPty = new Counterparty45__2()
                {
                    // This could be either FTC or the client depending on whether this is our report or the report we are filing on behalf of the customer
                    Id = new PartyIdentification248Choice__1()
                    {
                        Item = new LegalPersonIdentification1__1()
                        {
                            Id = new OrganisationIdentification15Choice__1()
                            {
                                Item = report.Counterparty1Reportingcounterparty
                            }
                        }
                    }
                }
            };
           
            // OthrCtrPty
            if (!string.IsNullOrWhiteSpace(report.counterparty2.lei) && report.Counterparty2identifiertype)
            {
                CtrPty.OthrCtrPty = new Counterparty46__2()
                {
                    IdTp = new PartyIdentification248Choice__2()
                    {
                        Item = new LegalPersonIdentification1__1()
                        {
                            Id = new OrganisationIdentification15Choice__1()
                            {
                                Item = report.Counterparty2
                            }//,
                            //Ctry = report.Countryofthecounterparty2
                        }
                    },
                };
                
            }

            return CtrPty;
        }

        #endregion
        #region CmonTradData Elements Common to both New and Modify Reports

        internal static ContractType14__1 NewTradeContractData(TradeAndValuationProperties report)
        {
            // CtrctTp, AsstClss, PdctClssfctn, DerivBasedOnCrptAsst
            ContractType14__1 ctrctData = new ContractType14__1()
            {
                AsstClss = UKEnumHelper.ReturnProductType4Code__1FromString(report.Assetclass),
                CtrctTp = UKEnumHelper.ReturnFinancialInstrumentContractType2CodeFromString(report.ContractType),
                DerivBasedOnCrptAsst = report.Derivativebasedoncryptoassets,
                PdctClssfctn = report.Productclassification
            };

            // PdctId
            SecurityIdentification46__1 pdctId = new();
            if (!string.IsNullOrWhiteSpace(report.ISIN))
            {
                pdctId.ISIN = report.ISIN;
                ctrctData.PdctId = pdctId;
            }
            if (!string.IsNullOrWhiteSpace(report.UniqueproductidentifierUPI))
            {
                UniqueProductIdentifier2Choice__1 unqPdctIdr = new UniqueProductIdentifier2Choice__1() { Item = report.UniqueproductidentifierUPI };
                pdctId.UnqPdctIdr = unqPdctIdr;
                ctrctData.PdctId = pdctId;
            }
            // SttlmCcy
            if (!string.IsNullOrWhiteSpace(report.Settlementcurrency1)) { ctrctData.SttlmCcy = new CurrencyExchange23__1() { Ccy = report.Settlementcurrency1 }; }

            return ctrctData;
        }
        #endregion
        #region TxData Elements Common to New and Modify Reports

        private static CollateralPortfolioCode5Choice__1 GetCollateralPortfolioCode5Choice__1(TradeAndValuationProperties report)
        {
            CollateralPortfolioCode5Choice__1 collPrtflCd = new CollateralPortfolioCode5Choice__1();
            if (!report.Collateralportfolioindicator)
            {
                collPrtflCd = new CollateralPortfolioCode5Choice__1()
                {
                    Item = new PortfolioCode3Choice()
                    {
                        Item = NotApplicable1Code.NOAP
                    }
                };
            }
            else
            {
                collPrtflCd.Item = new PortfolioCode3Choice() { Item = report.Collateralportfoliocode };
            }
            return collPrtflCd;
        }

        private static TradeConfirmation1Choice GetTradeConfirmation1Choice(TradeAndValuationProperties report)
        {
            TradeConfirmation1Choice tradConf = new TradeConfirmation1Choice();
            if (report.Confirmed == "NCNF") // Need the non confirmed return
            {
                TradeNonConfirmation1 nonConfd = new() { Tp = TradeConfirmationType2Code.NCNF };
                tradConf.Item = nonConfd;
            }
            else    // The default for our trades will be "YCNF"
            {
                TradeConfirmation2 confd = new();
                confd.Tp = UKEnumHelper.ReturnTradeConfirmationType1CodeFromString(report.Confirmed);
                if (report.Confirmationtimestamp.HasValue)
                {
                    confd.TmStmp = report.Confirmationtimestamp.Value;
                }
                tradConf.Item = confd;
            }
            return tradConf;
        }

        private static TradeClearing11__1 GetTradeClearing11__1(TradeAndValuationProperties report)
        {
            // TradClr
            TradeClearing11__1 tradClr = new();
            Cleared23Choice__1 clrSts = new Cleared23Choice__1();
            if (report.Cleared == 'N')
            {
                clrSts.Item = new ClearingExceptionOrExemption3Choice__1() { Item = NoReasonCode.NORE };
            }
            else
            {
                ClearingPartyAndTime22__1 dtls = new ClearingPartyAndTime22__1()
                {
                    CCP = new OrganisationIdentification15Choice__1() { Item = report.Centralcounterparty },
                    ClrDtTm = report.Clearingtimestamp.Value,
                    ClrDtTmSpecified = true
                };
                ClearingPartyAndTime21Choice__1 clrd = new() { Item = dtls };

                clrSts.Item = clrd;
            }
            tradClr.ClrSts = clrSts;

            if (!string.IsNullOrWhiteSpace(report.Clearingobligation))
            {
                tradClr.ClrOblgtn = UKEnumHelper.ReturnClearingObligationType1CodeFromString(report.Clearingobligation);
                tradClr.ClrOblgtnSpecified = true;
            }
            if (report.Intragroup.HasValue)
            {
                tradClr.IntraGrp = report.Intragroup.Value;
                tradClr.IntraGrpSpecified = true;
            }
            return tradClr;
        }

        private static MasterAgreement8__1 GetMasterAgreement8__1(TradeAndValuationProperties report)
        {
            // MstrAgrmt
            MasterAgreement8__1 mstrAgrmt = new();
            AgreementType2Choice__1 tp = new AgreementType2Choice__1()
            {
                Item = UKEnumHelper.ReturnMasterAgreementType2CodeFromString(report.MasterAgreementtype)
            };
            mstrAgrmt.Tp = tp;
            if (!string.IsNullOrWhiteSpace(report.Othermasteragreementtype)) mstrAgrmt.OthrMstrAgrmtDtls = report.Othermasteragreementtype;
            if (report.MasterAgreementversion.HasValue) mstrAgrmt.Vrsn = report.MasterAgreementversion.Value.ToString("yyyy");
            return mstrAgrmt;
        }

        private static PriceData2__1 GetPriceData2__1(TradeAndValuationProperties report)
        {
            ReportPrice price = report.Price;
            SecuritiesTransactionPrice17Choice__1 pric = new();
            if (price.PriceType == PriceType.MONETARY)
            {
                ActiveOrHistoricCurrencyAnd13DecimalAmount__1 amt = new()
                {
                    Ccy = report.Pricecurrency,
                    Value = Math.Round(price.Price, 13)
                };

                AmountAndDirection106__1 mntryVal = new()
                {
                    Amt = amt,
                    //Sgn = price.Sign == '+',
                    //SgnSpecified = true
                };
                pric.Item = mntryVal;
            }
            else if (price.PriceType == PriceType.PERCENTAGE)
            {
                pric.Item = price.Price;
            }

            PriceData2__1 txPric = new()
            {
                Pric = pric,
            };
            return txPric;
        }
        private static NotionalAmountLegs5__1 GetNotionalAmountLegs5__1(TradeAndValuationProperties report)
        {
            NotionalAmountLegs5__1 ntnlAmt = new();
            // First Leg
            NotionalAmount5__1 frstLeg = new()
            {
                Amt = new AmountAndDirection106__2()
                {
                    Amt = new ActiveOrHistoricCurrencyAnd5DecimalAmount__1()
                    {
                        Value = Math.Round(report.Notionalamountofleg1, 5),
                        Ccy = report.Notionalcurrency1
                    },
                    //Sgn = report.Notionalamountofleg1 >= 0,
                    //SgnSpecified = true
                }
            };

            // THIS SECTION IS REPEATABLE BUT HAS NOT BEEN CODED FOR REPEATING VALUES YET TODO
            if (report.Enddateofthenotionalamountofleg1.HasValue || report.Notionalamountineffectonassociatedeffectivedateofleg1.HasValue)
            {
                Schedule11__1 schdlPrd = new();
                if (report.Notionalamountineffectonassociatedeffectivedateofleg1.HasValue && report.Effectivedateofthenotionalamountofleg1.HasValue)
                {
                    schdlPrd.UadjstdFctvDt = report.Effectivedateofthenotionalamountofleg1.Value;
                    schdlPrd.Amt = new AmountAndDirection106__2()
                    {
                        Amt = new ActiveOrHistoricCurrencyAnd5DecimalAmount__1()
                        {
                            Value = Math.Round(report.Notionalamountineffectonassociatedeffectivedateofleg1.Value, 5),
                            Ccy = report.Notionalcurrency1
                        }
                    };
                }
                if (report.Enddateofthenotionalamountofleg1.HasValue)
                {
                    schdlPrd.UadjstdEndDt = report.Enddateofthenotionalamountofleg1.Value;
                }
                frstLeg.SchdlPrd = [schdlPrd];
            }
            ntnlAmt.FrstLeg = frstLeg;

            // Second Leg
            if (report.Notionalamountofleg2.HasValue)
            {
                NotionalAmount6__1 scndLeg = new()
                {
                    Amt = new AmountAndDirection106__2()
                    {
                        Amt = new ActiveOrHistoricCurrencyAnd5DecimalAmount__1()
                        {
                            Value = Math.Round(report.Notionalamountofleg2.Value, 5),
                            Ccy = report.Notionalcurrency2
                        },
                        //Sgn = report.Notionalamountofleg2 >= 0,
                        //SgnSpecified = true
                    }
                };

                // THIS SECTION IS REPEATABLE BUT HAS NOT BEEN CODED FOR REPEATING VALUES YET TODO
                if (report.Enddateofthenotionalamountofleg2.HasValue || report.Notionalamountineffectonassociatedeffectivedateofleg2.HasValue)
                {
                    Schedule11__1 schdlPrd = new();
                    if (report.Notionalamountineffectonassociatedeffectivedateofleg2.HasValue && report.Effectivedateofthenotionalamountofleg2.HasValue)
                    {
                        schdlPrd.UadjstdFctvDt = report.Effectivedateofthenotionalamountofleg2.Value;
                        schdlPrd.Amt = new AmountAndDirection106__2()
                        {
                            Amt = new ActiveOrHistoricCurrencyAnd5DecimalAmount__1()
                            {
                                Value = Math.Round(report.Notionalamountineffectonassociatedeffectivedateofleg2.Value, 5),
                                Ccy = report.Notionalcurrency2
                            }
                        };
                    }
                    if (report.Enddateofthenotionalamountofleg2.HasValue)
                    {
                        schdlPrd.UadjstdEndDt = report.Enddateofthenotionalamountofleg2.Value;
                    }
                    scndLeg.SchdlPrd = [schdlPrd];
                }
                ntnlAmt.ScndLeg = scndLeg;
            }
            return ntnlAmt;
        }
        private static NotionalQuantityLegs5__1 GetNotionalQuantityLegs5__1(TradeAndValuationProperties report)
        {
            NotionalQuantityLegs5__1 ntnlQty = new()
            {
                FrstLeg = new NotionalQuantity9__1()
                {
                    TtlQty = report.Totalnotionalquantityofleg1.Value,
                    TtlQtySpecified = true
                }
            };
            return ntnlQty;
        }
        private static OptionOrSwaption10__1 GetOptionOrSwaption10__1(TradeAndValuationProperties report)
        {
            OptionOrSwaption10__1 optn = new()
            {
                Tp = UKEnumHelper.ReturnOptionType2CodeFromString(report.Optiontype),
                ExrcStyle = UKEnumHelper.ReturnOptionStyle6CodeFromString(report.Optionstyle),
                PrmPmtDt = report.Optionpremiumpaymentdate.Value,
                //MtrtyDtOfUndrlyg = report.Maturitydateoftheunderlying.Value,
                PrmPmtDtSpecified = true,
                //MtrtyDtOfUndrlygSpecified = true,
                ExrcStyleSpecified = true,
                TpSpecified = true
            };

            if (report.Optionpremiumamount.HasValue)
            {
                optn.PrmAmt = new ActiveOrHistoricCurrencyAnd5DecimalAmount__1()
                {
                    Value = Math.Round(report.Optionpremiumamount.Value, 5),
                    Ccy = report.Optionpremiumcurrency
                };
            }
            if (report.Strikeprice.HasValue && !string.IsNullOrWhiteSpace(report.Strikepricecurrency_currencypair))
            {
                optn.StrkPric = new()
                {
                    Item = new AmountAndDirection106__1()
                    {
                        Amt = new ActiveOrHistoricCurrencyAnd13DecimalAmount__1()
                        {
                            Value = Math.Round(report.Strikeprice.Value, 13),
                            Ccy = report.Strikepricecurrency_currencypair
                        },
                        //Sgn = report.Strikeprice.Value > 0,
                        //SgnSpecified = true
                    }
                };
            }
            return optn;
        }
        #endregion
        private static AssetClassCommodity6Choice__1 GetCommdtyProducts(TradeAndValuationProperties report)
        {
            AssetClassCommodity6Choice__1 cmmdty = new();
            if (report.Subproduct == "OILP" && !string.IsNullOrWhiteSpace(report.Furthersubproduct))
            {
                cmmdty.Item = new AssetClassCommodityEnergy3Choice__1()
                {
                    Item = new EnergyCommodityOil3__1()
                    {
                        BasePdct = AssetClassProductType2Code.NRGY,
                        SubPdct = AssetClassSubProductType8Code.OILP,
                        AddtlSubPdct = UKEnumHelper.ReturnAssetClassDetailedSubProductType32CodeFromString(report.Furthersubproduct)
                    }
                };
            }
            else
            {
                Console.WriteLine("Subproduct was not OILP need to code this in GetCommdtyProducts method in NewTradeController class");
            }
            return cmmdty;
        }
    }
}
