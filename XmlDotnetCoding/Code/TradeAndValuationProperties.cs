using PortlandTradeReporterLib.Common;
using PortlandTradeReporterLib.Validations;
using PortlandTradeReporterLib.Maps;
using PortlandTradeReporterLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using PortlandAzureDataAccess.Trading;
using PortlandAzureDataAccess.CustomerData;

// C:\Portland\Fuel Trading Company\Portland IT - IT\Projects\Trade Reporting\ESMA Field Coding Log.xlsx
// C:\Portland\Fuel Trading Company\Portland IT - IT\Projects\Trade Reporting\Useful Report Validations.xlsm

namespace PortlandTradeReporterLib.Models
{
    public class TradeAndValuationProperties(TradingContext db, CustomerDataContext custDb)
    {
        private readonly TradingContext _db = db;
        private readonly CustomerDataContext _custDb = custDb;
        public TradeAndValuationProperties? previousReport; // nothing needed doing with this one
        public TradeLEI reportsubmittingentityID;         // From 1.2
        public TradeLEI? entityresponsibleforreporting;    // From 1.3
        public TradeLEI counterparty1;                    // From 1.4
        public TradeLEI counterparty2;                    // From 1.9
        public TradeLEI? brokerId;                         // From 1.15
        public TradeLEI? clearingMember;                   // From 1.16
        public TradeLEI? centralcounterparty;              // From 2.33
        public TradeLEI? ptrrServiceProvider;              // From 2.40
        public TradeLEI? otherpaymentpayer;               // From 2.77
        public TradeLEI? otherpaymentreceiver;               // From 2.78

        private bool _counterparty2identifiertype;

        private DateTime _reportingTimeStamp;   // done
        private DateTime _executionTimeStamp; // done
        private TradeActionType _actionType; // done
        private EventType? _eventType;
        private string _uti;
        private string? _reporttrackingnumber;
        private string? _priorUTIforonetooneandonetomanyrelationsbetweentransactions;
        private string? _subsequentpositionUTI;
        private DateTime? _expirationDate;
        private DateTime _eventDate;
        private DateTime? _earlyTerminationDate;
        //private DateTime _terminationDate;
        private DateTime _effectiveDate;
        private DateTime? _valuationtimestamp;
        private DateTime? _confirmationtimestamp;
        private DateTime? _finalcontractualsettlementdate;
        private DateTime? _unadjustedeffectivedateoftheprice;
        private DateTime? _unadjustedenddateoftheprice;
        private DateTime? _effectivedateofthenotionalamountofleg1;
        private DateTime? _enddateofthenotionalamountofleg1;
        private DateTime? _effectivedateofthenotionalquantityofleg1;
        private DateTime? _enddateofthenotionalquantityofleg1;
        private DateTime? _effectivedateofthenotionalamountofleg2;
        private DateTime? _enddateofthenotionalamountofleg2;
        private DateTime? _effectivedateofthenotionalquantityofleg2;
        private DateTime? _enddateofthenotionalquantityofleg2;
        private DateTime? _deliveryintervalstarttime;
        private DateTime? _deliveryintervalendtime;
        private DateTime? _deliverystartdate;
        private DateTime? _deliveryenddate;
        private DateTime? _effectivedateofthestrikeprice;
        private DateTime? _enddateofthestrikeprice;
        private DateTime? _optionpremiumpaymentdate;
        private DateTime? _maturitydateoftheunderlying;
        private ContractType _contractType;
        private string _venueOfExecution;
        private BuyerOrSeller? _direction;
        private MakeOrTake? _directionofleg1;
        private MakeOrTake? _directionofleg2;
        private char _cleared;
        private bool? _directlylinkedtocommercialactivityortreasuryfinancing;

        private TradeReportLevel _level;
        private AssetClass _assetClass;
        private string? _ptrrid;
        private bool? _ptrr;
        private PTRRTechnique? _typeofPTRRtechnique;
        private string _packageidentifier;
        private string? _isin;
        private string? _uniqueproductidentifierUPI;
        private string _productclassification;
        private bool _derivativebasedoncryptoassets = false;
        private UnderlyingIDType? _underlyingidentificationtype;
        private string? _underlyingidentification;
        private IndicatorOfUnderlyingIndex? _indicatoroftheunderlyingindex;
        private string? _nameoftheunderlyingindex;
        private string? _custombasketcode;
        private string? _identifierofthebasketsconstituents;
        private string? _settlementcurrency1;
        private string? _settlementcurrency2;
        private DeliveryType _deliverytype;
        private OtherPaymentType? _otherPaymentType;
        private decimal? _valuationamount;
        private string? _valuationCurrency;
        private ValuationMethod? _valuationmethod;
        private decimal? _delta;
        private bool _collateralportfolioindicator;
        private string? _collateralportfoliocode;
        private Confirmed? _confirmed;
        private ClearingObligation? _clearingobligation;
        private DateTime? _clearingtimestamp;
        private MasterAgreementType _masterAgreementtype;
        private string? _othermasteragreementtype;
        private DateTime? _masterAgreementversion;
        private bool? _intragroup;
        private ReportPrice? _price;
        private ReportPrice? _priceineffectbetweentheunadjustedeffectiveandenddate;
        private ReportPrice? _packagetransactionprice;
        private ReportPrice? _packagetransactionspread;
        private ReportPrice? _strikePrice;
        private ReportPrice? _strikepriceineffectonassociatedeffectivedate;
        private decimal _notionalamountofleg1;
        private string _notionalcurrency1;
        private string? _pricecurrency;
        private string? _packagetransactionalpricecurrency;
        private decimal? _notionalamountineffectonassociatedeffectivedateofleg1;
        private decimal? _totalnotionalquantityofleg1;
        private decimal? _notionalquantityineffectonassociatedeffectivedateofleg1;
        private decimal? _notionalamountineffectonassociatedeffectivedateofleg2;
        private decimal? _totalnotionalquantityofleg2;
        private decimal? _notionalquantityineffectonassociatedeffectivedateofleg2;
        private string? _packagetransactionspreadcurrency;
        private string? _baseproduct;
        private string? _subproduct;
        private string? _furthersubproduct;
        private string? _deliverypointorzone;
        private string? _interconnectionPoint;
        private QuantityUnit? _quantityunit;
        private decimal? _price_timeintervalquantity;
        private string? _currencyoftheprice_timeintervalquantity;
        private string? _strikepricecurrency_currencypair;
        private decimal? _optionpremiumamount;
        private string? _optionpremiumcurrency;
        private decimal? _exchangerate1;
        private decimal? _forwardexchangerate;
        private string? _exchangeratebasis;
        private decimal? _deliverycapacity;
        private DateTime? _otherpaymentdate;
        private decimal? _notionalamountofleg2;
        private string? _notionalcurrency2;
        private string? _otherpaymentcurrency;
        private string? _loadtype;
        private decimal? _otherpaymentamount;
        private OptionStyle _optionstyle;
        private OptionType _optiontype;
        private DaysOfTheWeek? _daysoftheweek;
        private Duration? _duration;


        #region properties

        public DateTime Reportingtimestamp { get { return _reportingTimeStamp; } set => _reportingTimeStamp = Dates.XmlDate(value); }
        public string ReportsubmittingentityID { get { return reportsubmittingentityID.lei; } set => SetReportSubmittingEntityID(value).GetAwaiter().GetResult(); }
        public string? Entityresponsibleforreporting { get { return entityresponsibleforreporting?.lei; } set => SetEntityResponsibleForReporting(value).GetAwaiter().GetResult(); }
        public string Counterparty1Reportingcounterparty { get { return counterparty1.lei; } set => SetCounterParty1(value).GetAwaiter().GetResult(); }
        public char Natureofthecounterparty1 { get { return EnumHelpers.LEINatureOfCounterpartyToChar(counterparty1.classification); } }  // Assigned when the _counterparty1 is set
        public string? Corporatesectorofthecounterparty1 { get { return counterparty1?.corporateSector; } } // Assigned when the _counterparty1 is set
        public bool? Clearingthresholdofcounterparty1 { get => GetAboveClearingThreshold(counterparty1.classification); }
        public bool Counterparty2identifiertype { get { return _counterparty2identifiertype; } } // Assigned when the _counterparty2 is set
        public string Counterparty2 { get { return counterparty2.lei; } set => SetCounterparty2(value).GetAwaiter().GetResult(); }
        public string? Countryofthecounterparty2 { get { return counterparty2.country; } }  // Assigned when the _counterparty2 is set
        public char? Natureofthecounterparty2 { get { return EnumHelpers.LEINatureOfCounterpartyToChar(counterparty2.classification); } } // Assigned when the _counterparty2 is set
        public string? Corporatesectorofthecounterparty2 { get { return counterparty2.corporateSector; } } // Assigned when the _counterparty2 is set
        public bool? Clearingthresholdofcounterparty2 { get => GetAboveClearingThreshold(counterparty2.classification); }
        public bool Reportingobligationofthecounterparty2 { get => CheckReportingObligationForCP2(); }
        public string? BrokerID { get { return brokerId?.lei; } set => SetBrokerId(value).GetAwaiter().GetResult(); }
        public string? Clearingmember { get { return clearingMember?.lei; } set => SetClearingMember(value).GetAwaiter().GetResult(); }
        public string? Direction { get { return EnumHelpers.BuyerOrSellerToString(_direction); } set { _direction = EnumHelpers.BuyerOrSellerFromString(value); } }
        public string? Directionofleg1 { get { return EnumHelpers.MakeOrTakeToString(_directionofleg1); } set { _directionofleg1 = EnumHelpers.MakeOrTakeFromString(value); } }
        public string? Directionofleg2 { get { return EnumHelpers.MakeOrTakeToString(_directionofleg2); } set { _directionofleg2 = EnumHelpers.MakeOrTakeFromString(value); } }
        public bool? Directlylinkedtocommercialactivityortreasuryfinancing { get { return _directlylinkedtocommercialactivityortreasuryfinancing; } set { _directlylinkedtocommercialactivityortreasuryfinancing = value; } }
        public string UTI { get { return _uti; } set => SetUTI(value); }
        public string? Reporttrackingnumber { get { return _reporttrackingnumber; } set => SetReporttrackingnumber(value); }
        public string PriorUTIforonetooneandonetomanyrelationsbetweentransactions { get { return _priorUTIforonetooneandonetomanyrelationsbetweentransactions; } set => SetPriorUTIFOTMRBT(value); }
        public string SubsequentpositionUTI { get { return _subsequentpositionUTI; } set => SetSubsequentpositionUTI(value); }
        public string PTRRID { get { return _ptrrid; } set => SetPTRRID(value); }
        public string Packageidentifier { get { return _packageidentifier; } set => SetPackageidentifier(value); }
        public string? ISIN { get { return _isin; } set => SetISIN(value); }
        public string? UniqueproductidentifierUPI { get { return _uniqueproductidentifierUPI; } set => SetUniqueproductidentifierUPI(value?.ToUpper()); }
        public string Productclassification { get { return _productclassification; } set => SetProductclassification(value.ToUpper()); }
        public string ContractType { get => EnumHelpers.ContractTypeToString(_contractType); set => SetContractType(value); }
        public string Assetclass { get { return EnumHelpers.AssetClassToString(_assetClass); } set => SetAssetClass(value); }
        public bool Derivativebasedoncryptoassets { get { return _derivativebasedoncryptoassets; } set { _derivativebasedoncryptoassets = value; } }
        public char? Underlyingidentificationtype { get => EnumHelpers.UnderlyingIDTypeToChar(_underlyingidentificationtype); set { _underlyingidentificationtype = EnumHelpers.UnderlyingIDTypeFromChar(value); } }
        public string? Underlyingidentification { get { return _underlyingidentification; } set => SetUnderlyingidentification(value); }
        public string? Indicatoroftheunderlyingindex { get => EnumHelpers.IndicatorOfUnderlyingIndexToString(_indicatoroftheunderlyingindex); set { _indicatoroftheunderlyingindex = EnumHelpers.IndicatorOfUnderlyingIndexFromString(value); } }
        public string? Nameoftheunderlyingindex { get { return _nameoftheunderlyingindex; } set => SetNameoftheunderlyingindex(value); }
        public string? Custombasketcode { get { return _custombasketcode; } set => SetCustombasketcode(value); }
        public string? Identifierofthebasketsconstituents { get { return _identifierofthebasketsconstituents; } set => SetIdentifierofthebasketsconstituents(value); }
        public string? Settlementcurrency1 { get { return _settlementcurrency1; } set => SetSettlementcurrency1(value); }
        public string? Settlementcurrency2 { get { return _settlementcurrency2; } set => SetSettlementcurrency2(value); }
        public decimal? Valuationamount { get { return _valuationamount; } set => SetValuationAmount(value); }
        public string? Valuationcurrency { get { return _valuationCurrency; } set => SetValuationCurrency(value); }
        public DateTime? Valuationtimestamp { get { return _valuationtimestamp; } set { _valuationtimestamp = Dates.XmlDate(value); } }
        public string? Valuationmethod { get => EnumHelpers.ValuationMethodToString(_valuationmethod); set => SetValuationMethod(value); }
        public decimal? Delta { get { return _delta; } set => SetDelta(value); }
        public bool Collateralportfolioindicator { get { return _collateralportfolioindicator; } set { _collateralportfolioindicator = value; } }
        public string? Collateralportfoliocode { get { return _collateralportfoliocode; } set => SetCollateralPortfolioCode(value); }
        public DateTime? Confirmationtimestamp { get { return _confirmationtimestamp; } set { _confirmationtimestamp = Dates.XmlDate(value); } }
        public string? Confirmed { get { return EnumHelpers.ConfirmedToString(_confirmed); } set => SetConfirmed(value); }
        public string? Clearingobligation { get { return EnumHelpers.ClearingObligationToString(_clearingobligation); } set => SetClearingObligation(value); }
        public char Cleared { get { return _cleared; } set => SetCleared(value); }
        public DateTime? Clearingtimestamp { get { return _clearingtimestamp; } set { _clearingtimestamp = Dates.XmlDate(value); } }
        public string? Centralcounterparty { get { return centralcounterparty?.lei; } set => SetCentralCounterparty(value).GetAwaiter().GetResult(); }

        public string MasterAgreementtype { get { return EnumHelpers.MasterAgreementTypeToString(_masterAgreementtype); } set => SetMasterAgreementType(value); }
        public string? Othermasteragreementtype { get { return _othermasteragreementtype; } set => SetOtherMasterAgreementType(value); }
        public DateTime? MasterAgreementversion { get { return _masterAgreementversion; } set { _masterAgreementversion = Dates.XmlDate(value); } }
        public bool? Intragroup { get { return _intragroup; } set { _intragroup = value; } }
        public bool? PTRR { get { return _ptrr; } set { _ptrr = value; } }
        public string? TypeofPTRRtechnique { get { return EnumHelpers.PTRRTechniqueToString(_typeofPTRRtechnique); } set { _typeofPTRRtechnique = EnumHelpers.PTRRTechniqueFromString(value); } }
        public string? PTRRserviceprovider { get { return ptrrServiceProvider?.lei; } set { SetPTRRServiceProvider(value).GetAwaiter().GetResult(); } }
        public string Venueofexecution { get { return _venueOfExecution; } set => SetVenueOfExecution(value.ToUpper()); }
        public DateTime Executiontimestamp { get { return _executionTimeStamp; } set { _executionTimeStamp = Dates.XmlDate(value); } }
        public DateTime Effectivedate { get { return _effectiveDate; } set { _effectiveDate = Dates.XmlDate(value); } }

        public DateTime? Expirationdate { get { return _expirationDate; } set { _expirationDate = Dates.XmlDate(value); } }
        public DateTime? Earlyterminationdate { get { return _earlyTerminationDate; } set { _earlyTerminationDate = Dates.XmlDate(value); } }
        public DateTime? Finalcontractualsettlementdate { get { return _finalcontractualsettlementdate; } set { _finalcontractualsettlementdate = Dates.XmlDate(value); } }
        public string Deliverytype { get => EnumHelpers.DeliveryTypeToString(_deliverytype); set => SetDeliverytype(value); }
        public ReportPrice? Price { get { return _price; } set { _price = value; } }
        public string? Pricecurrency { get { return _pricecurrency; } set => SetPriceCurrency(value); }
        public DateTime? Unadjustedeffectivedateoftheprice { get { return _unadjustedeffectivedateoftheprice; } set { _unadjustedeffectivedateoftheprice = Dates.XmlDate(value); } }
        public DateTime? Unadjustedenddateoftheprice { get { return _unadjustedenddateoftheprice; } set { _unadjustedenddateoftheprice = Dates.XmlDate(value); } }
        public ReportPrice? Priceineffectbetweentheunadjustedeffectiveandenddate { get { return _priceineffectbetweentheunadjustedeffectiveandenddate; } set { _priceineffectbetweentheunadjustedeffectiveandenddate = value; } }
        public ReportPrice? Packagetransactionprice { get { return _packagetransactionprice; } set { _packagetransactionprice = value; } }
        public string? Packagetransactionpricecurrency { get { return _packagetransactionalpricecurrency; } set => SetPackageTransactionPriceCurrency(value); }
        public decimal Notionalamountofleg1 { get { return _notionalamountofleg1; } set => SetNotionalAmountofLeg1(value); }
        public string Notionalcurrency1 { get { return _notionalcurrency1; } set => SetNotionalCurrency1(value); }
        public DateTime? Effectivedateofthenotionalamountofleg1 { get { return _effectivedateofthenotionalamountofleg1; } set { _effectivedateofthenotionalamountofleg1 = Dates.XmlDate(value); } }
        public DateTime? Enddateofthenotionalamountofleg1 { get { return _enddateofthenotionalamountofleg1; } set { _enddateofthenotionalamountofleg1 = Dates.XmlDate(value); } }
        public decimal? Notionalamountineffectonassociatedeffectivedateofleg1 { get { return _notionalamountineffectonassociatedeffectivedateofleg1; } set => Setnotionalamountineffectonassociatedeffectivedateofleg1(value); }
        public decimal? Totalnotionalquantityofleg1 { get { return _totalnotionalquantityofleg1; } set => SetTotalNotionalQuantityOfLeg1(value); }
        public DateTime? Effectivedateofthenotionalquantityofleg1 { get { return _effectivedateofthenotionalquantityofleg1; } set { _effectivedateofthenotionalquantityofleg1 = Dates.XmlDate(value); } }
        public DateTime? Enddateofthenotionalquantityofleg1 { get { return _enddateofthenotionalquantityofleg1; } set { _enddateofthenotionalquantityofleg1 = Dates.XmlDate(value); } }
        public decimal? Notionalquantityineffectonassociatedeffectivedateofleg1 { get { return _notionalquantityineffectonassociatedeffectivedateofleg1; } set => SetNotionalQuantityInEffectOnAssociatedEffectiveDateOfLeg1(value); }
        public decimal? Notionalamountofleg2 { get { return _notionalamountofleg2; } set => SetNotionalAmountofLeg2(value); }
        public string? Notionalcurrency2 { get { return _notionalcurrency2; } set => SetNotionalCurrency2(value); }
        public DateTime? Effectivedateofthenotionalamountofleg2 { get { return _effectivedateofthenotionalamountofleg2; } set { _effectivedateofthenotionalamountofleg2 = Dates.XmlDate(value); } }
        public DateTime? Enddateofthenotionalamountofleg2 { get { return _enddateofthenotionalamountofleg2; } set { _enddateofthenotionalamountofleg2 = Dates.XmlDate(value); } } 
        public decimal? Notionalamountineffectonassociatedeffectivedateofleg2 { get { return _notionalamountineffectonassociatedeffectivedateofleg2; } set => Setnotionalamountineffectonassociatedeffectivedateofleg2(value); }
        public decimal? Totalnotionalquantityofleg2 { get { return _totalnotionalquantityofleg2; } set => SetTotalNotionalQuantityOfLeg2(value); }
        public DateTime? Effectivedateofthenotionalquantityofleg2 { get { return _effectivedateofthenotionalquantityofleg2; } set { _effectivedateofthenotionalquantityofleg2 = Dates.XmlDate(value); } }
        public DateTime? Enddateofthenotionalquantityofleg2 { get { return _enddateofthenotionalquantityofleg2; } set { _enddateofthenotionalquantityofleg2 = Dates.XmlDate(value); } }
        public decimal? Notionalquantityineffectonassociatedeffectivedateofleg2 { get { return _notionalquantityineffectonassociatedeffectivedateofleg2; } set => SetNotionalQuantityInEffectOnAssociatedEffectiveDateOfLeg2(value); }
        public string? Otherpaymenttype { get => EnumHelpers.OtherPaymentTypeToString(_otherPaymentType); set { _otherPaymentType = EnumHelpers.OtherPaymentTypeFromString(value); } }
        public decimal? Otherpaymentamount { get { return _otherpaymentamount; } set => SetOtherPaymentAmount(value); }
        public string? Otherpaymentcurrency { get { return _otherpaymentcurrency; } set => SetOtherPaymentCurrency(value); }
        public DateTime? Otherpaymentdate { get { return _otherpaymentdate; } set { _otherpaymentdate = Dates.XmlDate(value); } }
        public string? Otherpaymentpayer { get { return otherpaymentpayer?.lei; } set => SetOtherPaymentPayer(value).GetAwaiter().GetResult(); }
        public string? Otherpaymentreceiver { get { return otherpaymentreceiver?.lei; } set => SetOtherPaymentReceiver(value).GetAwaiter().GetResult(); }
        public decimal? Fixedrateofleg1orcoupon { get; set; }
        public string? Fixedrateorcoupondaycountconventionleg1 { get; set; }
        public string? Fixedrateorcouponpaymentfrequencyperiodleg1 { get; set; }
        public int? Fixedrateorcouponpaymentfrequencyperiodmultiplierleg1 { get; set; }
        public string? Identifierofthefloatingrateofleg1 { get; set; }
        public string? Indicatorofthefloatingrateofleg1 { get; set; }
        public string? Nameofthefloatingrateofleg1 { get; set; }
        public string? Floatingratedaycountconventionofleg1 { get; set; }
        public string? Floatingratepaymentfrequencyperiodofleg1 { get; set; }
        public int? Floatingratepaymentfrequencyperiodmultiplierofleg1 { get; set; }
        public string? Floatingratereferenceperiodofleg1timeperiod { get; set; }
        public int? Floatingratereferenceperiodofleg1multiplier { get; set; }
        public string? Floatingrateresetfrequencyperiodofleg1 { get; set; }
        public int? Floatingrateresetfrequencymultiplierofleg1 { get; set; }
        public decimal? Spreadofleg1 { get; set; }
        public string? Spreadcurrencyofleg1 { get; set; }
        public decimal? Fixedrateofleg2 { get; set; }
        public string? Fixedratedaycountconventionleg2 { get; set; }
        public string Fixedratepaymentfrequencyperiodleg2 { get; set; }
        public int? Fixedratepaymentfrequencyperiodmultiplierleg2 { get; set; }
        public string? Identifierofthefloatingrateofleg2 { get; set; }
        public string? Indicatorofthefloatingrateofleg2 { get; set; }
        public string? Nameofthefloatingrateofleg2 { get; set; }
        public string? Floatingratedaycountconventionofleg2 { get; set; }
        public string? Floatingratepaymentfrequencyperiodofleg2 { get; set; }
        public int? Floatingratepaymentfrequencyperiodmultiplierofleg2 { get; set; }
        public string? Floatingratereferenceperiodofleg2timeperiod { get; set; }
        public int? Floatingratereferenceperiodofleg2multiplier { get; set; }
        public string? Floatingrateresetfrequencyperiodofleg2 { get; set; }
        public int? Floatingrateresetfrequencymultiplierofleg2 { get; set; }
        public decimal? Spreadofleg2 { get; set; }
        public string? Spreadcurrencyofleg2 { get; set; }
        public ReportPrice? Packagetransactionspread { get { return _packagetransactionspread; } set { _packagetransactionspread = value; } }
        public string? Packagetransactionspreadcurrency { get { return _packagetransactionspreadcurrency; } set => SetPackageTransactionSpreadCurrency(value); }
        #endregion
        public decimal? Exchangerate1 { get { return _exchangerate1; } set => SetExchangeRate1(value); }
        public decimal? Forwardexchangerate { get { return _forwardexchangerate; }  set => SetForwardExchangeRate(value);  }
        public string? Exchangeratebasis { get { return _exchangeratebasis; }  set => SetExchangeRateBasis(value); }
        public FullProductEnum? Products { set => SetProducts(value); }
        public string? Baseproduct { get { return _baseproduct; } set { _baseproduct = value; } }
        public string? Subproduct { get { return _subproduct; } set { _subproduct = value; } }
        public string? Furthersubproduct { get{ return _furthersubproduct; } set { _furthersubproduct = value; } }
        public string? Deliverypointorzone { get { return _deliverypointorzone; } set => SetDeliveryPointOrZone(value); }
        public string? InterconnectionPoint { get { return _interconnectionPoint; } set => SetInterconnectionPoint(value); }
        public string? Loadtype { get { return _loadtype; } set => EnumHelpers.LoadTypeFromString(value); }

        #region Repeatable non Optional Fields
        public DateTime? Deliveryintervalstarttime { get { return _deliveryintervalstarttime; } set { _deliveryintervalstarttime = Dates.XmlDate(value); } }
        public DateTime? Deliveryintervalendtime { get { return _deliveryintervalendtime; } set { _deliveryintervalendtime = Dates.XmlDate(value); } }
        public DateTime? Deliverystartdate { get { return _deliverystartdate; } set { _deliverystartdate = Dates.XmlDate(value); } }
        public DateTime? Deliveryenddate { get { return _deliveryenddate; } set { _deliveryenddate = Dates.XmlDate(value); } }

        public string? Duration { get => EnumHelpers.DurationToString(_duration); set => EnumHelpers.DurationFromString(value); }
        public string? Daysoftheweek { get => EnumHelpers.DaysOfTheWeekToString(_daysoftheweek); set => EnumHelpers.DaysOfTheWeekFromString(value); }
        public decimal? Deliverycapacity { get { return _deliverycapacity; } set => SetDeliveryCapacity(value);  }
        public string? QuantityUnit { get { return EnumHelpers.QuantityUnitToString(_quantityunit); } set => SetQuantityUnit(value); }
        public decimal? Price_timeintervalquantity { get { return _price_timeintervalquantity; } set => SetPriceTimeIntervalQuantity(value); }
        public string? Currencyoftheprice_timeintervalquantity { get { return _currencyoftheprice_timeintervalquantity; } set => SetCurrencyOfThePriceTimeIntervalQuantity(value); }
        #endregion Repeatable non Optional Fields
        public string? Optiontype { get => EnumHelpers.OptionTypeToString(_optiontype); set => EnumHelpers.OptionTypeFromString(value); }
        public string? Optionstyle { get => EnumHelpers.OptionStyleToString(_optionstyle); set => EnumHelpers.OptionStyleFromString(value); }
        public decimal? Strikeprice { get { return _strikePrice?.Price; }  }
        public DateTime? Effectivedateofthestrikeprice { get { return _effectivedateofthestrikeprice; } set { _effectivedateofthestrikeprice = Dates.XmlDate(value); } }
        public DateTime? Enddateofthestrikeprice { get { return _enddateofthestrikeprice; } set { _enddateofthestrikeprice = Dates.XmlDate(value); } }
        public decimal? Strikepriceineffectonassociatedeffectivedate { get { return _strikepriceineffectonassociatedeffectivedate?.Price; } }
        public string? Strikepricecurrency_currencypair { get { return _strikepricecurrency_currencypair; } set => SetStrikePriceCurrencyCurrencyPair(value); }
        public decimal? Optionpremiumamount { get { return _optionpremiumamount; } set => SetOptionPremiumAmount(value); }
        public string Optionpremiumcurrency { get { return _optionpremiumcurrency; } set => SetOptionPremiumCurrency(value); }
        public DateTime? Optionpremiumpaymentdate { get { return _optionpremiumpaymentdate; } set { _optionpremiumpaymentdate = Dates.XmlDate(value); } }
        public DateTime? Maturitydateoftheunderlying { get { return _maturitydateoftheunderlying; } set { _maturitydateoftheunderlying = Dates.XmlDate(value); } }
        public string Seniority { get; set; }
        public string Referenceentity { get; set; }
        public int Series { get; set; }
        public int Version { get; set; }
        public decimal Indexfactor { get; set; }
        public bool Tranche { get; set; }
        public decimal CDSindexattachmentpoint { get; set; }
        public decimal CDSindexdetachmentpoint { get; set; }
        public string Actiontype { get => EnumHelpers.ActionTypeToString(_actionType); set { _actionType = EnumHelpers.ActionTypeFromString(value); } }
        public string? Eventtype { get { return EnumHelpers.EventTypeToString(_eventType); } set { _eventType = EnumHelpers.EventTypeFromString(value); } }
        public DateTime Eventdate { get { return _eventDate; } set { _eventDate = Dates.XmlDate(value); } }
        public string Level { get { return EnumHelpers.TradeReportLevelToString(_level); } set { _level = EnumHelpers.TradeReportLevelFromString(value); } }



        #region Getters

        private static bool? GetAboveClearingThreshold(LEIClassification classification)
        {
            return CheckAboveClearingThreshold(classification);
        }
        private static bool? CheckAboveClearingThreshold(LEIClassification classification) => classification switch
        {
            LEIClassification.NFCMinus => false,
            LEIClassification.NFCPlus => true,
            LEIClassification.FCSmall => false,
            LEIClassification.FCPlus => true,
            _ => null
        };






        private bool? GetDerivativebasedoncryptoassets()
        {
            if (_actionType == TradeActionType.VALU || _actionType == TradeActionType.TERM || _actionType == TradeActionType.EROR)
            {
                return null;
            }
            else
            {
                return _derivativebasedoncryptoassets;
            }
        }

        #endregion
        #region Setters


        private async Task SetReportSubmittingEntityID(string reportSubmittingEntityID)
        {
            DateTime rptDateTime = DateTime.UtcNow;
            if (_reportingTimeStamp != new DateTime()) rptDateTime = _reportingTimeStamp;
            TradeLEI reportSubmittingEntity = await MapLEI.GetTradeLeiFromLEI(reportSubmittingEntityID);
            if (reportSubmittingEntity is not null) reportsubmittingentityID = reportSubmittingEntity;
        }

        private async Task SetCounterParty1(string lei)
        {
            TradeLEI cp1 = await MapLEI.GetTradeLeiFromLEI(lei);
            cp1 = MapLEI.AddTradingLeiToTradeLEI(cp1, DBHelper.GetTradingleiFromDB(lei, _custDb));
            if (cp1 is null) return;
            if (cp1 is not null) { counterparty1 = cp1; }
        }

        private async Task SetCounterparty2(string lei)
        {
            TradeLEI cp2 = await MapLEI.GetTradeLeiFromLEI(lei);
            cp2 = MapLEI.AddTradingLeiToTradeLEI(cp2, DBHelper.GetTradingleiFromDB(lei, _custDb));
            if (cp2 is null) return;

            counterparty2 = cp2;
            _counterparty2identifiertype = true;

        }

        private async Task SetBrokerId(string? lei)
        {
            if (string.IsNullOrWhiteSpace(lei)) { return; }
            TradeLEI broker = await MapLEI.GetTradeLeiFromLEI(lei);
            broker = MapLEI.AddTradingLeiToTradeLEI(broker, DBHelper.GetTradingleiFromDB(lei, _custDb));
            brokerId = broker;

        }

        private async Task SetClearingMember(string? lei)
        {
            if (string.IsNullOrWhiteSpace(lei)) { return; }
            TradeLEI clearingMember = await MapLEI.GetTradeLeiFromLEI(lei);
            clearingMember = MapLEI.AddTradingLeiToTradeLEI(clearingMember, DBHelper.GetTradingleiFromDB(lei, _custDb));
            this.clearingMember = clearingMember;
        }


        private async Task SetCentralCounterparty(string? lei)
        {
            if (string.IsNullOrWhiteSpace(lei)) { return; }
            TradeLEI centralCounterparty = await MapLEI.GetTradeLeiFromLEI(lei);
            centralCounterparty = MapLEI.AddTradingLeiToTradeLEI(centralCounterparty, DBHelper.GetTradingleiFromDB(lei, _custDb));
            centralcounterparty = centralCounterparty;
        }

        private async Task SetOtherPaymentPayer(string? lei)
        {
            if (string.IsNullOrWhiteSpace(lei)) { return; }
            TradeLEI centralCounterparty = await MapLEI.GetTradeLeiFromLEI(lei);
            centralCounterparty = MapLEI.AddTradingLeiToTradeLEI(centralCounterparty, DBHelper.GetTradingleiFromDB(lei, _custDb));
            otherpaymentpayer = centralCounterparty;
        }

        //SetOtherPaymentReceiver
        private async Task SetOtherPaymentReceiver(string? lei)
        {
            if (string.IsNullOrWhiteSpace(lei)) { return; }
            TradeLEI centralCounterparty = await MapLEI.GetTradeLeiFromLEI(lei);
            centralCounterparty = MapLEI.AddTradingLeiToTradeLEI(centralCounterparty, DBHelper.GetTradingleiFromDB(lei, _custDb));
            otherpaymentreceiver = centralCounterparty;
        }

        private async Task SetPTRRServiceProvider(string? lei)
        {
            if (lei is null)
            {
                ptrrServiceProvider = null;
                return;
            }

            TradeLEI ptrrSP = await MapLEI.GetTradeLeiFromLEI(lei);
            //ptrrSP = MapLEI.AddULeiToTradeLEI(ptrrSP, DBHelper.GetUleiFromDB(lei));
            ptrrServiceProvider = ptrrSP;

        }

        private bool CheckReportingObligationForCP2()
        {
            if (counterparty2.classification == LEIClassification.NFCMinus) { return false; }
            return true;
        }

        private void SetContractType(string contractType)
        {
            try
            {
                _contractType = EnumHelpers.ContractTypeFromString(contractType);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        private async Task SetEntityResponsibleForReporting(string? lei)
        {
            if (string.IsNullOrWhiteSpace(lei)) { return; }
            TradeLEI reportingEntity = await MapLEI.GetTradeLeiFromLEI(lei);
            if (reportingEntity is not null) reportsubmittingentityID = reportingEntity;
        }

        private void SetVenueOfExecution(string venue)
        {
            if (ValidateTradeReportDataOnEntry.ValidateVenueOfExecution(venue, clearingMember))
            {
                if (clearingMember == null)
                {
                    _venueOfExecution = venue;
                }
                else
                {
                    if (string.IsNullOrEmpty(clearingMember.mic))
                    {
                        _venueOfExecution = venue;
                    }
                    else
                    {
                        _venueOfExecution = clearingMember.mic;
                    }
                }
            }
        }




        private void SetCleared(char cleared)
        {
            if (cleared == 'Y' || cleared == 'y') { _cleared = 'Y'; }
            else if (cleared == 'N' || cleared == 'n') { _cleared = 'N'; }
            ValidateTradeReportDataOnEntry.ValidateCleared(_cleared);
        }




        private void SetAssetClass(string assetClassString)
        {
            try
            {
                AssetClass asset = EnumHelpers.AssetClassFromString(assetClassString);
                _assetClass = asset;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        private void SetUTI(string utiString)
        {
            if (previousReport != null && previousReport.UTI != utiString)
            {
                Alerts.DevMessageRed($"The UTI cannot be changed once it has been reported, the previous report was {previousReport.UTI} and this is trying to enter {utiString}");
            }
            else if (ValidateTradeReportDataOnEntry.ValidateUTI(utiString, _actionType))
            {
                _uti = utiString;
            }
        }

        private void SetReporttrackingnumber(string? reporttrackingnumber)
        {
            if (_level == TradeReportLevel.POSITION || _venueOfExecution == "XXXX" || _venueOfExecution == "XOFF") { return; }
            else
            {
                if (ValidateTradeReportDataOnEntry.ValidateReportTrackingNumber(reporttrackingnumber))
                {
                    _reporttrackingnumber = reporttrackingnumber;
                }
            }
        }

        //  _priorUTIforonetooneandonetomanyrelationsbetweentransactions
        private void SetPriorUTIFOTMRBT(string pUTI)
        {
            if (ValidateTradeReportDataOnEntry.ValidatePriorUTIFOTMRBT(pUTI, _actionType, _eventType))
            {
                _priorUTIforonetooneandonetomanyrelationsbetweentransactions = pUTI;
            }
        }

        private void SetSubsequentpositionUTI(string subs)
        {
            if (_actionType == TradeActionType.MODI || _actionType == TradeActionType.CORR || _actionType == TradeActionType.TERM || _actionType == TradeActionType.REVI || _actionType == TradeActionType.POSC)
            {
                if (ValidateTradeReportDataOnEntry.ValidateSubsequentPositionUTI(subs, _actionType, _eventType))
                {
                    _subsequentpositionUTI = subs;
                }
            }
        }

        private void SetPTRRID(string ptrrid)
        {
            if (_actionType == TradeActionType.NEWT || _actionType == TradeActionType.MODI || _actionType == TradeActionType.CORR || _actionType == TradeActionType.REVI || _actionType == TradeActionType.TERM)
            {
                if (_actionType == TradeActionType.NEWT || _actionType == TradeActionType.MODI || _actionType == TradeActionType.TERM)
                {
                    if (_eventType != EventType.COMP) { return; }
                    else if (_typeofPTRRtechnique == PTRRTechnique.PWAS || _typeofPTRRtechnique == PTRRTechnique.PRBM || _typeofPTRRtechnique == PTRRTechnique.OTHR)
                    {
                        if (ValidateTradeReportDataOnEntry.ValidatePTRRId(ptrrid, ptrrServiceProvider.lei))
                        {
                            _ptrrid = ptrrid;
                        }
                    }

                }
            }
        }




        private void SetPackageidentifier(string pi)
        {

            if (ValidateTradeReportDataOnEntry.ValidatePackageIdentifier(pi))
            {
                _packageidentifier = pi;
            }
        }



        private void SetISIN(string? isin)
        {
            if (ValidateTradeReportDataOnEntry.ValidateISIN(isin))
            {
                _isin = isin;
            }
        }

        private void SetUniqueproductidentifierUPI(string? upi)
        {

            if (ValidateTradeReportDataOnEntry.ValidateUPI(upi))
            {
                _uniqueproductidentifierUPI = upi;
            }
        }



        private void SetUnderlyingidentification(string? uid)
        {
            if (ValidateTradeReportDataOnEntry.ValidateUnderlyingidentification(uid))
            {
                _underlyingidentification = uid;
            }
        }



        private void SetNameoftheunderlyingindex(string? name)
        {
            if (ValidateTradeReportDataOnEntry.ValidateNameoftheunderlyingindex(name))
            {
                _nameoftheunderlyingindex = name;
            }
        }

        private void SetCustombasketcode(string? code)
        {
            if (ValidateTradeReportDataOnEntry.ValidateCustombasketcode(code))
            {
                _custombasketcode = code;
            }
        }

        private void SetProductclassification(string product)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSetProductclassification(product))
            {
                _productclassification = product;
            }
        }

        private void SetIdentifierofthebasketsconstituents(string? idbc)
        {
            if (ValidateTradeReportDataOnEntry.ValidateIdentifierofthebasketsconstituents(idbc))
            {
                _identifierofthebasketsconstituents = idbc;
            }
        }

        private void SetSettlementcurrency1(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "Settlementcurrency1"))
            {
                _settlementcurrency1 = currency;
            }
        }

        private void SetSettlementcurrency2(string currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "Settlementcurrency2"))
            {
                _settlementcurrency2 = currency;
            }
        }

        private void SetValuationAmount(decimal? val)
        {
            if (val.HasValue) { val = Map.DecimalToReportDecimal(val.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (vdt.ValidateDecimal25_5(val, true))
            {
                _valuationamount = val;
            }
            else
            {
                Alerts.DevMessageRed($"Valuation Amount value of {val} is not valid, 25 max characters including max 5 after the decimal place.");
            }
        }

        private void SetValuationCurrency(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "ValuationCurrency"))
            {
                _valuationCurrency = currency;
            }
        }

        private void SetValuationMethod(string? valMethod)
        {
            try
            {
                _valuationmethod = EnumHelpers.ValuationMethodFromString(valMethod);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        private void SetDelta(decimal? val)
        {
            if (ValidateTradeReportDataOnEntry.ValidateDelta(val))
            {
                _delta = val;
            }
        }


        private void SetCollateralPortfolioCode(string? code)
        {
            if (ValidateTradeReportDataOnEntry.ValidateCollateralPortfolioCode(code))
            {
                _collateralportfoliocode = code;
            }
        }


        private void SetConfirmed(string? confirmed)
        {
            try
            {
                Confirmed? conf = EnumHelpers.ConfirmedFromString(confirmed);
                _confirmed = conf;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        private void SetClearingObligation(string? clearingObligation)
        {
            try
            {
                ClearingObligation? clearOb = EnumHelpers.ClearingObligationFromString(clearingObligation);
                _clearingobligation = clearOb;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        private void SetMasterAgreementType(string masterAgreementTypeString)
        {
            try
            {
                MasterAgreementType masterAgreementType = EnumHelpers.MasterAgreementTypeFromString(masterAgreementTypeString);
                _masterAgreementtype = masterAgreementType;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        private void SetOtherMasterAgreementType(string? otherMstrAgreementType)
        {
            if (ValidateTradeReportDataOnEntry.ValidateOtherMasterAgreementType(otherMstrAgreementType))
            {
                _othermasteragreementtype = otherMstrAgreementType;
            }
        }

        private void SetOtherPaymentType(string value)
        {
            OtherPaymentType? otherPaymentType = EnumHelpers.OtherPaymentTypeFromString(value);
            _otherPaymentType = otherPaymentType;
        }

        private void SetDeliverytype(string delType)
        {
            try
            {
                _deliverytype = EnumHelpers.DeliveryTypeFromString(delType);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
        }

        public void SetOtherPaymentCurrency(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "OtherPaymentCurency"))
            {
                _otherpaymentcurrency = currency;
            }
        }
        private void SetNotionalAmountofLeg2(decimal? amount) //2.64
        {
            ValidateDataTypes vdt = new();
            if (amount.HasValue) { amount = Map.DecimalToReportDecimal(amount.Value, 25, 5); }
            if (vdt.ValidateDecimal25_5(amount))
            {
                if (amount.HasValue && amount < 0)
                {
                    Alerts.DevMessageRed($"Notional amount of Leg 2 2.64 should have have a value greater than or equal to 0, {amount} was passed.");
                    return;
                }
                else
                {
                    _notionalamountofleg2 = amount;
                    return;
                }

            }
            Alerts.DevMessageRed($"Notional amount of Leg 2 2.64 should have 25 max numbers with 5 max after decimal point, {amount} was passed.");
        }
        private void SetNotionalCurrency2(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "NotionalCurrency2"))
            {
                _notionalcurrency2 = currency;
            }
        }



        private void SetPriceCurrency(string? pricecurrency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(pricecurrency, "PriceCurrency"))
            {
                _pricecurrency = pricecurrency;
            }
        }
        private void SetPackageTransactionPriceCurrency(string? PackageTransactionPriceCurrency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(PackageTransactionPriceCurrency, "PackageTransactionPriceCurrency"))
            {
                _packagetransactionalpricecurrency = PackageTransactionPriceCurrency;
            }
        }

        private void SetNotionalAmountofLeg1(decimal amount)
        {
            amount = Map.DecimalToReportDecimal(amount, 25, 5); 
            ValidateDataTypes vdt = new();
            if (vdt.ValidateDecimal25_5(amount))
            {
                _notionalamountofleg1 = amount;
                return;
            }
            Alerts.DevMessageRed($"Notional amount of Leg 1 should have 25 max numbers with 5 max after decimal point, {amount} was passed.");
        }

        private void SetNotionalCurrency1(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                Alerts.DevMessageRed("Notional currency 1 is null but it should never be null.");
            }
            else
            {
                if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "NotionalCurrency1"))
                {
                    _notionalcurrency1 = currency;
                }
            }
        }

        private void Setnotionalamountineffectonassociatedeffectivedateofleg1(decimal? notional)
        {
            if (notional.HasValue) { notional = Map.DecimalToReportDecimal(notional.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (notional.HasValue && notional.Value < 0)
            {
                Alerts.DevMessageRed($"Notionalamountineffectonassociatedeffectivedateofleg1 2.59 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
            else if (vdt.ValidateDecimal25_5(notional, true))
            {
                _notionalamountineffectonassociatedeffectivedateofleg1 = notional;
            }
            else
            {
                Alerts.DevMessageRed($"Notionalamountineffectonassociatedeffectivedateofleg1 2.59 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
        }

        private void SetTotalNotionalQuantityOfLeg1(decimal? notional)
        {
            if (notional.HasValue) { notional = Map.DecimalToReportDecimal(notional.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!notional.HasValue || ((notional.HasValue && notional.Value >= 0) && vdt.ValidateDecimal25_5(notional, true)))
            {
                _totalnotionalquantityofleg1 = notional;
            }
            else
            {
                Alerts.DevMessageRed($"TotalNotionalQuantityOfLeg1 2.60 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }

        }


        private void SetNotionalQuantityInEffectOnAssociatedEffectiveDateOfLeg1(decimal? notional)
        {
            if (notional.HasValue) { notional = Map.DecimalToReportDecimal(notional.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!notional.HasValue || ((notional.HasValue && notional.Value >= 0) && vdt.ValidateDecimal25_5(notional, true)))
            {
                _notionalquantityineffectonassociatedeffectivedateofleg1 = notional;
            }
            else
            {
                Alerts.DevMessageRed($"NotionalQuantityInEffectOnAssociatedEffectiveDateOfLeg1 2.63 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
        }


        private void Setnotionalamountineffectonassociatedeffectivedateofleg2(decimal? notional)
        {
            if (notional.HasValue) { notional = Map.DecimalToReportDecimal(notional.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (notional.HasValue && notional.Value < 0)
            {
                Alerts.DevMessageRed($"Notionalamountineffectonassociatedeffectivedateofleg2 2.68 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
            else if (vdt.ValidateDecimal25_5(notional, true))
            {
                _notionalamountineffectonassociatedeffectivedateofleg2 = notional;
            }
            else
            {
                Alerts.DevMessageRed($"Notionalamountineffectonassociatedeffectivedateofleg2 2.68 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
        }

        private void SetTotalNotionalQuantityOfLeg2(decimal? notional)
        {
            if (notional.HasValue) { notional = Map.DecimalToReportDecimal(notional.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!notional.HasValue || ((notional.HasValue && notional.Value >= 0) && vdt.ValidateDecimal25_5(notional, true)))
            {
                _totalnotionalquantityofleg2 = notional;
            }
            else
            {
                Alerts.DevMessageRed($"TotalNotionalQuantityOfLeg2 2.69 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }

        }

        private void SetNotionalQuantityInEffectOnAssociatedEffectiveDateOfLeg2(decimal? notional)
        {
            if (notional.HasValue) { notional = Map.DecimalToReportDecimal(notional.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!notional.HasValue || ((notional.HasValue && notional.Value >= 0) && vdt.ValidateDecimal25_5(notional, true)))
            {
                _notionalquantityineffectonassociatedeffectivedateofleg2 = notional;
            }
            else
            {
                Alerts.DevMessageRed($"NotionalQuantityInEffectOnAssociatedEffectiveDateOfLeg2 2.72 value of {notional} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
        }

        private void SetPackageTransactionSpreadCurrency(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "PackageTransactionSpreadCurrency 2.112"))
            {
                _packagetransactionspreadcurrency = currency;
            }
        }

        private void SetDeliveryPointOrZone(string? delPoint)
        {
            if (ValidateTradeReportDataOnEntry.ValidateDeliveryPointOrZone(delPoint))
            {
                _deliverypointorzone = delPoint;
                return;
            }
        }

        private void SetInterconnectionPoint(string? intPoint)
        {
            if (ValidateTradeReportDataOnEntry.ValidateInterconnectionPoint(intPoint))
            {
                _interconnectionPoint = intPoint;
                return;
            }
        }

        private void SetProducts(FullProductEnum? product)
        {
            if(product is null)
            {
                _baseproduct = null;
                _subproduct = null;
                _furthersubproduct = null;
            }
            Products products = new(product);
            _baseproduct = products.BaseProduct;
            _subproduct = products.SubProduct;
            _furthersubproduct = products.FurtherSubProduct;
        }

        private void SetDeliveryCapacity(decimal? capacity)
        {
            if (capacity.HasValue) { capacity = Map.DecimalToReportDecimal(capacity.Value, 20, 19); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (vdt.ValidateDecimal20_19(capacity, true))
            {
                _deliverycapacity = capacity;
            }
            else
            {
                Alerts.DevMessageRed($"DeliveryCapacity 2.128 value of {capacity} is not valid.");
            }
        }

        private void SetQuantityUnit(string? quantityUnitString)
        {
            try
            {
                QuantityUnit? quantity = EnumHelpers.QuantityUnitFromString(quantityUnitString);
                _quantityunit = quantity;
            }catch(ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessage(ex.Message);
            }
        }

        private void SetPriceTimeIntervalQuantity(decimal? price)
        {
            if (price.HasValue) { price = Map.DecimalToReportDecimal(price.Value, 20, 19); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (vdt.ValidateDecimal20_19(price, true))
            {
                _totalnotionalquantityofleg2 = price;
            }
            else
            {
                Alerts.DevMessageRed($"PriceTimeIntervalQuantity 2.130 value of {price} is not valid.");
            }
        }

        private void SetOtherPaymentAmount(decimal? value)
        {
            if (value.HasValue) { value = Map.DecimalToReportDecimal(value.Value, 25, 5); }
            ValidateDataTypes VD = new();

            if(value.HasValue && value < 0)
            {
                Alerts.DevMessageRed($"Other Payment amount should be above 0, {value} was passed.");
                return;
            }
            else if (VD.ValidateDecimal25_5(value))
            {
                Alerts.DevMessageRed($"Other Payment amount should have 25 max numbers with 5 max after decimal point, {value} was passed.");
                return;
            }
            _otherpaymentamount = value;
            return;
        }

        private void SetCurrencyOfThePriceTimeIntervalQuantity(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "CurrencyOfThePriceTimeIntervalQuantity 2.131"))
            {
                _currencyoftheprice_timeintervalquantity = currency;
            }
        }

        private void SetStrikePriceCurrencyCurrencyPair(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "SetStrikePriceCurrencyCurrencyPair 2.138"))
            {
                _strikepricecurrency_currencypair = currency;
            }
        }

        private void SetOptionPremiumAmount(decimal? premium)
        {
            if (premium.HasValue) { premium = Map.DecimalToReportDecimal(premium.Value, 25, 5); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!premium.HasValue || (premium.Value >= 0 && vdt.ValidateDecimal25_5(premium, true)))
            {
                _optionpremiumamount = premium;
            }
            else
            {
                Alerts.DevMessageRed($"OptionPremiumAmount 2.139 value of {premium} is not valid, 25 max characters including max 5 after the decimal place, greater than 0.");
            }
        }

        private void SetOptionPremiumCurrency(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "OptionPremiumCurrency 2.140"))
            {
                _optionpremiumcurrency = currency;
            }
        }

        private void SetExchangeRate1(decimal? rate)
        {
            if (rate.HasValue) { rate = Map.DecimalToReportDecimal(rate.Value, 18, 13); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!rate.HasValue || (rate.Value >= 0 && vdt.ValidateDecimal18_13(rate, true)))
            {
                _notionalquantityineffectonassociatedeffectivedateofleg2 = rate;
            }
            else
            {
                Alerts.DevMessageRed($"ExchangeRate1 2.113 value of {rate} is not valid, 18 max characters including max 13 after the decimal place, greater than 0.");
            }
        }

        private void SetForwardExchangeRate(decimal? rate)
        {
            if (rate.HasValue) { rate = Map.DecimalToReportDecimal(rate.Value, 18, 13); }
            ValidateDataTypes vdt = new ValidateDataTypes();
            if (!rate.HasValue || (rate.Value >= 0 && vdt.ValidateDecimal18_13(rate, true)))
            {
                _notionalquantityineffectonassociatedeffectivedateofleg2 = rate;
            }
            else
            {
                Alerts.DevMessageRed($"ForwardExchangeRate 2.114 value of {rate} is not valid, 18 max characters including max 13 after the decimal place, greater than 0.");
            }
        }

        private void SetExchangeRateBasis(string? currency)
        {
            if (ValidateTradeReportDataOnEntry.ValidateSettlementcurrency(currency, "ExchangeRateBasis 2.115"))
            {
                _exchangeratebasis = currency;
            }
        }

        #endregion

        #region Public Setters
        public void SetPrice(decimal price, string priceType)
        {
            _price = GenerateReportPrice(price, priceType);
        }

        public void SetPriceInEffectBetweenTheUnadjustedEffectiveAndEndDate(decimal price, string priceType)
        {
            _priceineffectbetweentheunadjustedeffectiveandenddate = GenerateReportPrice(price, priceType);
        }

        public void SetPackageTransactionPrice(decimal price, string priceType)
        {
            _packagetransactionprice = GenerateReportPrice(price, priceType);
        }

        public void SetPackageTransactionSperad(decimal price, string priceType)
        {
            _packagetransactionspread = GenerateReportPrice(price, priceType, true);
        }

        public void SetStrikePrice(decimal? price, string priceType)
        {
            _strikePrice = GenerateReportPrice(price, priceType);
        }

        public void SetStrikePriceInEffectOnAssociatedEffectiveDate(decimal? price, string priceType)
        {
            _strikepriceineffectonassociatedeffectivedate = GenerateReportPrice(price, priceType);
        }

        private ReportPrice? GenerateReportPrice(decimal? price, string priceType, bool IsSpread = false)
        {
            if (price is null) return null;
            char sign = '+';
            if (price < 0) sign = '-';
            try
            {
                PriceType pType = EnumHelpers.PriceTypeFromString(priceType);
                bool numberIsValid;
                if(!IsSpread)
                {
                    numberIsValid = ValidateTradeReportDataOnEntry.ValidatePrice(price.Value, pType);
                }else
                {
                    numberIsValid = ValidateTradeReportDataOnEntry.ValidateSpread(price.Value, pType);
                }
                if (numberIsValid)
                {
                    return new ReportPrice() { Price = price.Value, PriceType = pType, Sign = sign };
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Alerts.DevMessageRed(ex.Message);
            }
            return null;
        }

        private bool TestReportPriceIsValid(decimal? price, PriceType pType)
        {
            return ValidateTradeReportDataOnEntry.ValidatePrice(price, pType);
        }

        private bool TestReportPriceSpreadIsValid(decimal? price, PriceType pType)
        {
            return ValidateTradeReportDataOnEntry.ValidateSpread(price, pType);
        }

        #endregion
    }
}
