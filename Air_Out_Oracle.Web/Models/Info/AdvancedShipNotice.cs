using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;

namespace AirOut.Web.Models.Info
{
    public partial class AdvancedShipNotice
    {
        public static AdvancedShipNotice CreateSearchByKey(string receiptkey)
        {
            AdvancedShipNotice asn = new AdvancedShipNotice();
            AdvancedShipNoticeHeader header = new AdvancedShipNoticeHeader();
            header.ReceiptKey = receiptkey;
            asn.AdvancedShipNoticeHeader = header;

            return asn;
        }

        public static AdvancedShipNotice CreateSearchByLineNum(string receiptkey, string linenum)
        {
            AdvancedShipNotice asn = new AdvancedShipNotice();
            AdvancedShipNoticeHeader header = new AdvancedShipNoticeHeader();
            AdvancedShipNoticeDetail detail = new AdvancedShipNoticeDetail();
            detail.ReceiptLineNumber = linenum;
            header.ReceiptKey = receiptkey;
            header.AdvancedShipNoticeDetail.Add(detail);
            asn.AdvancedShipNoticeHeader = header;

            return asn;
        }

        private AdvancedShipNoticeHeader advancedShipNoticeHeaderField;

        public AdvancedShipNotice()
        {
            this.advancedShipNoticeHeaderField = new AdvancedShipNoticeHeader();
        }

        public AdvancedShipNoticeHeader AdvancedShipNoticeHeader
        {
            get
            {
                return this.advancedShipNoticeHeaderField;
            }
            set
            {
                this.advancedShipNoticeHeaderField = value;
            }
        }
    }

    public partial class AdvancedShipNoticeHeader
    {

        private string serialKeyField;

        private string receiptKeyField;

        private string externReceiptKeyField;

        private string receiptGroupField;

        private string storerKeyField;

        private string receiptDateField;

        private string pOKeyField;

        private string carrierKeyField;

        private string carrierNameField;

        private string carrierAddress1Field;

        private string carrierAddress2Field;

        private string carrierCityField;

        private string carrierStateField;

        private string carrierZipField;

        private string carrierReferenceField;

        private string warehouseReferenceField;

        private string originCountryField;

        private string destinationCountryField;

        private string vehicleNumberField;

        private string vehicleDateField;

        private string placeOfLoadingField;

        private string placeOfDischargeField;

        private string placeofDeliveryField;

        private string incoTermsField;

        private string termsNoteField;

        private string containerKeyField;

        private string signatoryField;

        private string placeofIssueField;

        private string openQtyField;

        private string statusField;

        private string notesField;

        private string effectiveDateField;

        private string containerTypeField;

        private string containerQtyField;

        private string typeField;

        private string priorityField;

        private string rMAField;

        private string externalReceiptKey2Field;

        private string transportationModeField;

        private string expectedReceiptDateField;

        private string closedDateField;

        private string sUsr1Field;

        private string sUsr2Field;

        private string sUsr3Field;

        private string sUsr4Field;

        private string sUsr5Field;

        private string allowAutoReceiptField;

        private string carrierCountryField;

        private string carrierPhoneField;

        private string driverNameField;

        private string trailerNumberField;

        private string trailerOwnerField;

        private string arrivalDateTimeField;

        private string adviceNumberField;

        private string adviceDateField;

        private string packingSlipNumberField;

        private string aPPOstringMENTKEYField;

        private string doorField;

        private string tEMPERATUREField;

        private string eARLIESTSHIPDATEField;

        private string rEQUIREDSHIPDATEField;

        private string pROMISEDSHIPDATEField;

        private string pLANNEDSHIPDATEField;

        private string eARLIESTDELIVERYDATEField;

        private string rEQUIREDDELIVERYDATEField;

        private string pROMISEDDELIVERYDATEField;

        private string pLANNEDDELIVERYDATEField;

        private string transportationServiceField;

        private string proNumberField;

        private string freightChargeBaseAmountField;

        private string fREIGHTCHARGEAMOUNTField;

        private string tMFREIGHTCHARGECURRENCYField;

        private string freightCostBaseAmountField;

        private string fREIGHTCOSTAMOUNTField;

        private string tMFREIGHTCOSTCURRENCYField;

        private string carrierRouteStatusField;

        private string addDateField;

        private string editDateField;

        private string cUSTOMTYPEField;

        private string addWhoField;

        private string editWhoField;

        private string routeField;

        private string lOADNUMBERField;

        private string bOXNUMBERField;

        private string cARMODELSField;

        private string cABINETNUMBERField;

        private string pERFORMANCEDATA01Field;

        private string pERFORMANCEDATA02Field;

        private string pERFORMANCEDATA03Field;

        private string pERFORMANCEDATA04Field;

        private string pERFORMANCEDATA05Field;

        private string pERFORMANCEDATA06Field;

        private string pERFORMANCEDATA07Field;

        private string pERFORMANCEDATA08Field;

        private string pERFORMANCEDATA09Field;

        private string pERFORMANCEDATA10Field;

        private string pERFORMANCEDATA11Field;

        private string pERFORMANCEDATA12Field;

        private string pERFORMANCEDATA13Field;

        private string pERFORMANCEDATA14Field;

        private string pERFORMANCEDATA15Field;

        private string pERFORMANCEDATA16Field;

        private string pERFORMANCEDATA17Field;

        private string pERFORMANCEDATA18Field;

        private string pERFORMANCEDATA19Field;

        private string pERFORMANCEDATA20Field;

        private string sUsr6Field;

        private string sUsr7Field;

        private string sUsr8Field;

        private string sUsr9Field;

        private string sUsr10Field;

        private string sUSR11Field;

        private string sUSR12Field;

        private string sUSR13Field;

        private string sUSR14Field;

        private string sUSR15Field;

        private string sUSR16Field;

        private string sUSR17Field;

        private string sUSR18Field;

        private string sUSR19Field;

        private string sUSR20Field;

        private string sUSR21Field;

        private string sUSR22Field;

        private string sUSR23Field;

        private string sUSR24Field;

        private string sUSR25Field;

        private string sUSR26Field;

        private string sUSR27Field;

        private string sUSR28Field;

        private string sUSR29Field;

        private string sUSR30Field;

        private string sUSR31Field;

        private string sUSR32Field;

        private string sUSR33Field;

        private string sUSR34Field;

        private string sUSR35Field;

        private List<AdvancedShipNoticeDetail> advancedShipNoticeDetailField;

        public AdvancedShipNoticeHeader()
        {
            this.advancedShipNoticeDetailField = new List<AdvancedShipNoticeDetail>();
        }

        public string SerialKey
        {
            get
            {
                return this.serialKeyField;
            }
            set
            {
                this.serialKeyField = value;
            }
        }

        public string ReceiptKey
        {
            get
            {
                return this.receiptKeyField;
            }
            set
            {
                this.receiptKeyField = value;
            }
        }

        public string ExternReceiptKey
        {
            get
            {
                return this.externReceiptKeyField;
            }
            set
            {
                this.externReceiptKeyField = value;
            }
        }

        public string ReceiptGroup
        {
            get
            {
                return this.receiptGroupField;
            }
            set
            {
                this.receiptGroupField = value;
            }
        }

        public string StorerKey
        {
            get
            {
                return this.storerKeyField;
            }
            set
            {
                this.storerKeyField = value;
            }
        }

        public string ReceiptDate
        {
            get
            {
                return this.receiptDateField;
            }
            set
            {
                this.receiptDateField = value;
            }
        }

        public string POKey
        {
            get
            {
                return this.pOKeyField;
            }
            set
            {
                this.pOKeyField = value;
            }
        }

        public string CarrierKey
        {
            get
            {
                return this.carrierKeyField;
            }
            set
            {
                this.carrierKeyField = value;
            }
        }

        public string CarrierName
        {
            get
            {
                return this.carrierNameField;
            }
            set
            {
                this.carrierNameField = value;
            }
        }

        public string CarrierAddress1
        {
            get
            {
                return this.carrierAddress1Field;
            }
            set
            {
                this.carrierAddress1Field = value;
            }
        }

        public string CarrierAddress2
        {
            get
            {
                return this.carrierAddress2Field;
            }
            set
            {
                this.carrierAddress2Field = value;
            }
        }

        public string CarrierCity
        {
            get
            {
                return this.carrierCityField;
            }
            set
            {
                this.carrierCityField = value;
            }
        }

        public string CarrierState
        {
            get
            {
                return this.carrierStateField;
            }
            set
            {
                this.carrierStateField = value;
            }
        }

        public string CarrierZip
        {
            get
            {
                return this.carrierZipField;
            }
            set
            {
                this.carrierZipField = value;
            }
        }

        public string CarrierReference
        {
            get
            {
                return this.carrierReferenceField;
            }
            set
            {
                this.carrierReferenceField = value;
            }
        }

        public string WarehouseReference
        {
            get
            {
                return this.warehouseReferenceField;
            }
            set
            {
                this.warehouseReferenceField = value;
            }
        }

        public string OriginCountry
        {
            get
            {
                return this.originCountryField;
            }
            set
            {
                this.originCountryField = value;
            }
        }

        public string DestinationCountry
        {
            get
            {
                return this.destinationCountryField;
            }
            set
            {
                this.destinationCountryField = value;
            }
        }

        public string VehicleNumber
        {
            get
            {
                return this.vehicleNumberField;
            }
            set
            {
                this.vehicleNumberField = value;
            }
        }

        public string VehicleDate
        {
            get
            {
                return this.vehicleDateField;
            }
            set
            {
                this.vehicleDateField = value;
            }
        }

        public string PlaceOfLoading
        {
            get
            {
                return this.placeOfLoadingField;
            }
            set
            {
                this.placeOfLoadingField = value;
            }
        }

        public string PlaceOfDischarge
        {
            get
            {
                return this.placeOfDischargeField;
            }
            set
            {
                this.placeOfDischargeField = value;
            }
        }

        public string PlaceofDelivery
        {
            get
            {
                return this.placeofDeliveryField;
            }
            set
            {
                this.placeofDeliveryField = value;
            }
        }

        public string IncoTerms
        {
            get
            {
                return this.incoTermsField;
            }
            set
            {
                this.incoTermsField = value;
            }
        }

        public string TermsNote
        {
            get
            {
                return this.termsNoteField;
            }
            set
            {
                this.termsNoteField = value;
            }
        }

        public string ContainerKey
        {
            get
            {
                return this.containerKeyField;
            }
            set
            {
                this.containerKeyField = value;
            }
        }

        public string Signatory
        {
            get
            {
                return this.signatoryField;
            }
            set
            {
                this.signatoryField = value;
            }
        }

        public string PlaceofIssue
        {
            get
            {
                return this.placeofIssueField;
            }
            set
            {
                this.placeofIssueField = value;
            }
        }

        public string OpenQty
        {
            get
            {
                return this.openQtyField;
            }
            set
            {
                this.openQtyField = value;
            }
        }

        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        public string Notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        public string EffectiveDate
        {
            get
            {
                return this.effectiveDateField;
            }
            set
            {
                this.effectiveDateField = value;
            }
        }

        public string ContainerType
        {
            get
            {
                return this.containerTypeField;
            }
            set
            {
                this.containerTypeField = value;
            }
        }

        public string ContainerQty
        {
            get
            {
                return this.containerQtyField;
            }
            set
            {
                this.containerQtyField = value;
            }
        }

        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
        public string Priority
        {
            get
            {
                return this.priorityField;
            }
            set
            {
                this.priorityField = value;
            }
        }
        public string RMA
        {
            get
            {
                return this.rMAField;
            }
            set
            {
                this.rMAField = value;
            }
        }

        public string ExternalReceiptKey2
        {
            get
            {
                return this.externalReceiptKey2Field;
            }
            set
            {
                this.externalReceiptKey2Field = value;
            }
        }

        public string TransportationMode
        {
            get
            {
                return this.transportationModeField;
            }
            set
            {
                this.transportationModeField = value;
            }
        }

        public string ExpectedReceiptDate
        {
            get
            {
                return this.expectedReceiptDateField;
            }
            set
            {
                this.expectedReceiptDateField = value;
            }
        }

        public string ClosedDate
        {
            get
            {
                return this.closedDateField;
            }
            set
            {
                this.closedDateField = value;
            }
        }

        public string SUsr1
        {
            get
            {
                return this.sUsr1Field;
            }
            set
            {
                this.sUsr1Field = value;
            }
        }

        public string SUsr2
        {
            get
            {
                return this.sUsr2Field;
            }
            set
            {
                this.sUsr2Field = value;
            }
        }

        public string SUsr3
        {
            get
            {
                return this.sUsr3Field;
            }
            set
            {
                this.sUsr3Field = value;
            }
        }

        public string SUsr4
        {
            get
            {
                return this.sUsr4Field;
            }
            set
            {
                this.sUsr4Field = value;
            }
        }

        public string SUsr5
        {
            get
            {
                return this.sUsr5Field;
            }
            set
            {
                this.sUsr5Field = value;
            }
        }

        public string AllowAutoReceipt
        {
            get
            {
                return this.allowAutoReceiptField;
            }
            set
            {
                this.allowAutoReceiptField = value;
            }
        }

        public string CarrierCountry
        {
            get
            {
                return this.carrierCountryField;
            }
            set
            {
                this.carrierCountryField = value;
            }
        }

        public string CarrierPhone
        {
            get
            {
                return this.carrierPhoneField;
            }
            set
            {
                this.carrierPhoneField = value;
            }
        }

        public string DriverName
        {
            get
            {
                return this.driverNameField;
            }
            set
            {
                this.driverNameField = value;
            }
        }

        public string TrailerNumber
        {
            get
            {
                return this.trailerNumberField;
            }
            set
            {
                this.trailerNumberField = value;
            }
        }

        public string TrailerOwner
        {
            get
            {
                return this.trailerOwnerField;
            }
            set
            {
                this.trailerOwnerField = value;
            }
        }

        public string ArrivalDateTime
        {
            get
            {
                return this.arrivalDateTimeField;
            }
            set
            {
                this.arrivalDateTimeField = value;
            }
        }

        public string AdviceNumber
        {
            get
            {
                return this.adviceNumberField;
            }
            set
            {
                this.adviceNumberField = value;
            }
        }

        public string AdviceDate
        {
            get
            {
                return this.adviceDateField;
            }
            set
            {
                this.adviceDateField = value;
            }
        }

        public string PackingSlipNumber
        {
            get
            {
                return this.packingSlipNumberField;
            }
            set
            {
                this.packingSlipNumberField = value;
            }
        }

        public string APPOstringMENTKEY
        {
            get
            {
                return this.aPPOstringMENTKEYField;
            }
            set
            {
                this.aPPOstringMENTKEYField = value;
            }
        }

        public string Door
        {
            get
            {
                return this.doorField;
            }
            set
            {
                this.doorField = value;
            }
        }

        public string TEMPERATURE
        {
            get
            {
                return this.tEMPERATUREField;
            }
            set
            {
                this.tEMPERATUREField = value;
            }
        }

        public string EARLIESTSHIPDATE
        {
            get
            {
                return this.eARLIESTSHIPDATEField;
            }
            set
            {
                this.eARLIESTSHIPDATEField = value;
            }
        }

        public string REQUIREDSHIPDATE
        {
            get
            {
                return this.rEQUIREDSHIPDATEField;
            }
            set
            {
                this.rEQUIREDSHIPDATEField = value;
            }
        }

        public string PROMISEDSHIPDATE
        {
            get
            {
                return this.pROMISEDSHIPDATEField;
            }
            set
            {
                this.pROMISEDSHIPDATEField = value;
            }
        }

        public string PLANNEDSHIPDATE
        {
            get
            {
                return this.pLANNEDSHIPDATEField;
            }
            set
            {
                this.pLANNEDSHIPDATEField = value;
            }
        }

        public string EARLIESTDELIVERYDATE
        {
            get
            {
                return this.eARLIESTDELIVERYDATEField;
            }
            set
            {
                this.eARLIESTDELIVERYDATEField = value;
            }
        }

        public string REQUIREDDELIVERYDATE
        {
            get
            {
                return this.rEQUIREDDELIVERYDATEField;
            }
            set
            {
                this.rEQUIREDDELIVERYDATEField = value;
            }
        }

        public string PROMISEDDELIVERYDATE
        {
            get
            {
                return this.pROMISEDDELIVERYDATEField;
            }
            set
            {
                this.pROMISEDDELIVERYDATEField = value;
            }
        }

        public string PLANNEDDELIVERYDATE
        {
            get
            {
                return this.pLANNEDDELIVERYDATEField;
            }
            set
            {
                this.pLANNEDDELIVERYDATEField = value;
            }
        }

        public string TransportationService
        {
            get
            {
                return this.transportationServiceField;
            }
            set
            {
                this.transportationServiceField = value;
            }
        }

        public string ProNumber
        {
            get
            {
                return this.proNumberField;
            }
            set
            {
                this.proNumberField = value;
            }
        }

        public string FreightChargeBaseAmount
        {
            get
            {
                return this.freightChargeBaseAmountField;
            }
            set
            {
                this.freightChargeBaseAmountField = value;
            }
        }

        public string FREIGHTCHARGEAMOUNT
        {
            get
            {
                return this.fREIGHTCHARGEAMOUNTField;
            }
            set
            {
                this.fREIGHTCHARGEAMOUNTField = value;
            }
        }

        public string TMFREIGHTCHARGECURRENCY
        {
            get
            {
                return this.tMFREIGHTCHARGECURRENCYField;
            }
            set
            {
                this.tMFREIGHTCHARGECURRENCYField = value;
            }
        }

        public string FreightCostBaseAmount
        {
            get
            {
                return this.freightCostBaseAmountField;
            }
            set
            {
                this.freightCostBaseAmountField = value;
            }
        }

        public string FREIGHTCOSTAMOUNT
        {
            get
            {
                return this.fREIGHTCOSTAMOUNTField;
            }
            set
            {
                this.fREIGHTCOSTAMOUNTField = value;
            }
        }

        public string TMFREIGHTCOSTCURRENCY
        {
            get
            {
                return this.tMFREIGHTCOSTCURRENCYField;
            }
            set
            {
                this.tMFREIGHTCOSTCURRENCYField = value;
            }
        }

        public string CarrierRouteStatus
        {
            get
            {
                return this.carrierRouteStatusField;
            }
            set
            {
                this.carrierRouteStatusField = value;
            }
        }

        public string AddDate
        {
            get
            {
                return this.addDateField;
            }
            set
            {
                this.addDateField = value;
            }
        }

        public string EditDate
        {
            get
            {
                return this.editDateField;
            }
            set
            {
                this.editDateField = value;
            }
        }

        public string CUSTOMTYPE
        {
            get
            {
                return this.cUSTOMTYPEField;
            }
            set
            {
                this.cUSTOMTYPEField = value;
            }
        }

        public string AddWho
        {
            get
            {
                return this.addWhoField;
            }
            set
            {
                this.addWhoField = value;
            }
        }

        public string EditWho
        {
            get
            {
                return this.editWhoField;
            }
            set
            {
                this.editWhoField = value;
            }
        }

        public string Route
        {
            get
            {
                return this.routeField;
            }
            set
            {
                this.routeField = value;
            }
        }

        public string LOADNUMBER
        {
            get
            {
                return this.lOADNUMBERField;
            }
            set
            {
                this.lOADNUMBERField = value;
            }
        }

        public string BOXNUMBER
        {
            get
            {
                return this.bOXNUMBERField;
            }
            set
            {
                this.bOXNUMBERField = value;
            }
        }

        public string CARMODELS
        {
            get
            {
                return this.cARMODELSField;
            }
            set
            {
                this.cARMODELSField = value;
            }
        }

        public string CABINETNUMBER
        {
            get
            {
                return this.cABINETNUMBERField;
            }
            set
            {
                this.cABINETNUMBERField = value;
            }
        }

        public string PERFORMANCEDATA01
        {
            get
            {
                return this.pERFORMANCEDATA01Field;
            }
            set
            {
                this.pERFORMANCEDATA01Field = value;
            }
        }

        public string PERFORMANCEDATA02
        {
            get
            {
                return this.pERFORMANCEDATA02Field;
            }
            set
            {
                this.pERFORMANCEDATA02Field = value;
            }
        }

        public string PERFORMANCEDATA03
        {
            get
            {
                return this.pERFORMANCEDATA03Field;
            }
            set
            {
                this.pERFORMANCEDATA03Field = value;
            }
        }

        public string PERFORMANCEDATA04
        {
            get
            {
                return this.pERFORMANCEDATA04Field;
            }
            set
            {
                this.pERFORMANCEDATA04Field = value;
            }
        }

        public string PERFORMANCEDATA05
        {
            get
            {
                return this.pERFORMANCEDATA05Field;
            }
            set
            {
                this.pERFORMANCEDATA05Field = value;
            }
        }

        public string PERFORMANCEDATA06
        {
            get
            {
                return this.pERFORMANCEDATA06Field;
            }
            set
            {
                this.pERFORMANCEDATA06Field = value;
            }
        }

        public string PERFORMANCEDATA07
        {
            get
            {
                return this.pERFORMANCEDATA07Field;
            }
            set
            {
                this.pERFORMANCEDATA07Field = value;
            }
        }

        public string PERFORMANCEDATA08
        {
            get
            {
                return this.pERFORMANCEDATA08Field;
            }
            set
            {
                this.pERFORMANCEDATA08Field = value;
            }
        }

        public string PERFORMANCEDATA09
        {
            get
            {
                return this.pERFORMANCEDATA09Field;
            }
            set
            {
                this.pERFORMANCEDATA09Field = value;
            }
        }

        public string PERFORMANCEDATA10
        {
            get
            {
                return this.pERFORMANCEDATA10Field;
            }
            set
            {
                this.pERFORMANCEDATA10Field = value;
            }
        }

        public string PERFORMANCEDATA11
        {
            get
            {
                return this.pERFORMANCEDATA11Field;
            }
            set
            {
                this.pERFORMANCEDATA11Field = value;
            }
        }

        public string PERFORMANCEDATA12
        {
            get
            {
                return this.pERFORMANCEDATA12Field;
            }
            set
            {
                this.pERFORMANCEDATA12Field = value;
            }
        }

        public string PERFORMANCEDATA13
        {
            get
            {
                return this.pERFORMANCEDATA13Field;
            }
            set
            {
                this.pERFORMANCEDATA13Field = value;
            }
        }

        public string PERFORMANCEDATA14
        {
            get
            {
                return this.pERFORMANCEDATA14Field;
            }
            set
            {
                this.pERFORMANCEDATA14Field = value;
            }
        }

        public string PERFORMANCEDATA15
        {
            get
            {
                return this.pERFORMANCEDATA15Field;
            }
            set
            {
                this.pERFORMANCEDATA15Field = value;
            }
        }

        public string PERFORMANCEDATA16
        {
            get
            {
                return this.pERFORMANCEDATA16Field;
            }
            set
            {
                this.pERFORMANCEDATA16Field = value;
            }
        }

        public string PERFORMANCEDATA17
        {
            get
            {
                return this.pERFORMANCEDATA17Field;
            }
            set
            {
                this.pERFORMANCEDATA17Field = value;
            }
        }

        public string PERFORMANCEDATA18
        {
            get
            {
                return this.pERFORMANCEDATA18Field;
            }
            set
            {
                this.pERFORMANCEDATA18Field = value;
            }
        }

        public string PERFORMANCEDATA19
        {
            get
            {
                return this.pERFORMANCEDATA19Field;
            }
            set
            {
                this.pERFORMANCEDATA19Field = value;
            }
        }

        public string PERFORMANCEDATA20
        {
            get
            {
                return this.pERFORMANCEDATA20Field;
            }
            set
            {
                this.pERFORMANCEDATA20Field = value;
            }
        }

        public string SUsr6
        {
            get
            {
                return this.sUsr6Field;
            }
            set
            {
                this.sUsr6Field = value;
            }
        }

        public string SUsr7
        {
            get
            {
                return this.sUsr7Field;
            }
            set
            {
                this.sUsr7Field = value;
            }
        }

        public string SUsr8
        {
            get
            {
                return this.sUsr8Field;
            }
            set
            {
                this.sUsr8Field = value;
            }
        }

        public string SUsr9
        {
            get
            {
                return this.sUsr9Field;
            }
            set
            {
                this.sUsr9Field = value;
            }
        }

        public string SUsr10
        {
            get
            {
                return this.sUsr10Field;
            }
            set
            {
                this.sUsr10Field = value;
            }
        }

        public string SUSR11
        {
            get
            {
                return this.sUSR11Field;
            }
            set
            {
                this.sUSR11Field = value;
            }
        }

        public string SUSR12
        {
            get
            {
                return this.sUSR12Field;
            }
            set
            {
                this.sUSR12Field = value;
            }
        }

        public string SUSR13
        {
            get
            {
                return this.sUSR13Field;
            }
            set
            {
                this.sUSR13Field = value;
            }
        }

        public string SUSR14
        {
            get
            {
                return this.sUSR14Field;
            }
            set
            {
                this.sUSR14Field = value;
            }
        }

        public string SUSR15
        {
            get
            {
                return this.sUSR15Field;
            }
            set
            {
                this.sUSR15Field = value;
            }
        }

        public string SUSR16
        {
            get
            {
                return this.sUSR16Field;
            }
            set
            {
                this.sUSR16Field = value;
            }
        }

        public string SUSR17
        {
            get
            {
                return this.sUSR17Field;
            }
            set
            {
                this.sUSR17Field = value;
            }
        }

        public string SUSR18
        {
            get
            {
                return this.sUSR18Field;
            }
            set
            {
                this.sUSR18Field = value;
            }
        }

        public string SUSR19
        {
            get
            {
                return this.sUSR19Field;
            }
            set
            {
                this.sUSR19Field = value;
            }
        }

        public string SUSR20
        {
            get
            {
                return this.sUSR20Field;
            }
            set
            {
                this.sUSR20Field = value;
            }
        }

        public string SUSR21
        {
            get
            {
                return this.sUSR21Field;
            }
            set
            {
                this.sUSR21Field = value;
            }
        }

        public string SUSR22
        {
            get
            {
                return this.sUSR22Field;
            }
            set
            {
                this.sUSR22Field = value;
            }
        }

        public string SUSR23
        {
            get
            {
                return this.sUSR23Field;
            }
            set
            {
                this.sUSR23Field = value;
            }
        }

        public string SUSR24
        {
            get
            {
                return this.sUSR24Field;
            }
            set
            {
                this.sUSR24Field = value;
            }
        }

        public string SUSR25
        {
            get
            {
                return this.sUSR25Field;
            }
            set
            {
                this.sUSR25Field = value;
            }
        }

        public string SUSR26
        {
            get
            {
                return this.sUSR26Field;
            }
            set
            {
                this.sUSR26Field = value;
            }
        }

        public string SUSR27
        {
            get
            {
                return this.sUSR27Field;
            }
            set
            {
                this.sUSR27Field = value;
            }
        }

        public string SUSR28
        {
            get
            {
                return this.sUSR28Field;
            }
            set
            {
                this.sUSR28Field = value;
            }
        }

        public string SUSR29
        {
            get
            {
                return this.sUSR29Field;
            }
            set
            {
                this.sUSR29Field = value;
            }
        }

        public string SUSR30
        {
            get
            {
                return this.sUSR30Field;
            }
            set
            {
                this.sUSR30Field = value;
            }
        }

        public string SUSR31
        {
            get
            {
                return this.sUSR31Field;
            }
            set
            {
                this.sUSR31Field = value;
            }
        }

        public string SUSR32
        {
            get
            {
                return this.sUSR32Field;
            }
            set
            {
                this.sUSR32Field = value;
            }
        }

        public string SUSR33
        {
            get
            {
                return this.sUSR33Field;
            }
            set
            {
                this.sUSR33Field = value;
            }
        }

        public string SUSR34
        {
            get
            {
                return this.sUSR34Field;
            }
            set
            {
                this.sUSR34Field = value;
            }
        }

        public string SUSR35
        {
            get
            {
                return this.sUSR35Field;
            }
            set
            {
                this.sUSR35Field = value;
            }
        }
        [XmlElement("AdvancedShipNoticeDetail")]
        public List<AdvancedShipNoticeDetail> AdvancedShipNoticeDetail
        {
            get
            {
                if ((this.advancedShipNoticeDetailField == null))
                {
                    this.advancedShipNoticeDetailField = new List<AdvancedShipNoticeDetail>();
                }
                return this.advancedShipNoticeDetailField;
            }
            set
            {
                this.advancedShipNoticeDetailField = value;
            }
        }
    }

    public partial class AdvancedShipNoticeDetail
    {

        private string serialKeyField;

        private string whseIdField;

        private string receiptKeyField;

        private string receiptLineNumberField;

        private string sUBLINENUMBERField;

        private string externReceiptKeyField;

        private string externLineNoField;

        private string storerKeyField;

        private string pOKeyField;

        private string tariffKeyField;

        private string skuField;

        private string altSkuField;

        private string idField;

        private string statusField;

        private string dateReceivedField;

        private string qtyExpectedField;

        private string qtyAdjustedField;

        private string qtyReceivedField;

        private string uOMField;

        private string packKeyField;

        private string vesselKeyField;

        private string voyageKeyField;

        private string xdockKeyField;

        private string containerKeyField;

        private string toLocField;

        private string toLotField;

        private string toIdField;

        private string conditionCodeField;

        private string lottable01Field;

        private string lottable02Field;

        private string lottable03Field;

        private string lottable04Field;

        private string lottable05Field;

        private string lottable06Field;

        private string lottable07Field;

        private string lottable08Field;

        private string lottable09Field;

        private string lottable10Field;

        private string caseCntField;

        private string innerPackField;

        private string palletField;

        private string cubeField;

        private string grossWgtField;

        private string netWgtField;

        private string otherUnit1Field;

        private string otherUnit2Field;

        private string unitPriceField;

        private string extendedPriceField;

        private string effectiveDateField;

        private string fORTE_FLAGField;

        private string sUsr1Field;

        private string sUsr2Field;

        private string sUsr3Field;

        private string sUsr4Field;

        private string sUsr5Field;

        private string notesField;

        private string reasonCodeField;

        private string palletIDField;

        private string qtyRejectedField;

        private string typeField;


        private string rETURNTYPEField;

        private string rETURNREASONField;

        private string dISPOSITIONTYPEField;

        private string dISPOSITIONCODEField;

        private string rETURNCONDITIONField;

        private string qCRequiredField;

        private string qCQTYINSPECTEDField;

        private string qCQTYREJECTEDField;

        private string qCREJREASONField;

        private string qCSTATUSField;

        private string qCUSERField;

        private string qCAutoAdjustField;

        private string externalLotField;

        private string rMAField;

        private string packingSlipQtyField;

        private string iPSKEYField;

        private string supplierNameField;

        private string supplierKeyField;

        private string mATCHLOTTABLEField;

        private string rECEIPTDETAILIDField;

        private string pOLineNumberField;

        private string lOTTABLE11Field;

        private string lOTTABLE12Field;

        private string sOURCELOCATIONField;

        private string sourceVersionField;

        private string rEFERENCETYPEField;

        private string rEFERENCEDOCUMENTField;

        private string rEFERENCELOCATIONField;

        private string rEFERENCEVERSIONField;

        private string rEFERENCELINEField;

        private string cUBICMETERField;

        private string hUNDREDWEIGHTField;

        private string tAREWGTField;

        private string rEFERENCEACCOUNTINGENTITYField;

        private string rEFERENCESCHEDULELINEField;

        private string rEQUISITIONDOCUMENTField;

        private string rEQUISITIONACCOUNTINGENTITYField;

        private string rEQUISITIONLOCATIONField;

        private string requisitionVersionField;

        private string rEQUISITIONLINEField;

        private string rEQUISITIONSCHEDULELINEField;

        private string pURCHASEORDERDOCUMENTField;

        private string pURCHASEORDERACCOUNTINGENTITYField;

        private string pURCHASEORDERLOCATIONField;

        private string purchaseOrderVersionField;

        private string pURCHASEORDERLINEField;

        private string pURCHASEORDERSCHEDULELINEField;

        private string sALESORDERDOCUMENTField;

        private string sALESORDERACCOUNTINGENTITYField;

        private string sALESORDERLOCATIONField;

        private string salesOrderVersionField;

        private string sALESORDERLINEField;

        private string sALESORDERSCHEDULELINEField;

        private string pRODUCTIONORDERDOCUMENTField;

        private string pRODUCTIONORDERACCENTITYField;

        private string pRODUCTIONORDERLOCATIONField;

        private string productionOrderVersionField;

        private string pRODUCTIONORDERLINEField;

        private string pRODUCTIONORDERSCHEDULELINEField;

        private string tEMPERATUREField;

        private string addDateField;

        private string addWhoField;

        private string editDateField;

        private string editWhoField;

        private string lOTTABLE13Field;

        private string lOTTABLE14Field;

        private string lOTTABLE15Field;

        private string lOTTABLE16Field;

        private string lOTTABLE17Field;

        private string lOTTABLE18Field;

        private string lOTTABLE19Field;

        private string lOTTABLE20Field;

        private string lOTTABLE21Field;

        private string lOTTABLE22Field;

        private string lOTTABLE23Field;

        private string lOTTABLE24Field;

        private string lOTTABLE25Field;

        private string lOTTABLE26Field;

        private string lOTTABLE27Field;

        private string lOTTABLE28Field;

        private string lOTTABLE29Field;

        private string lOTTABLE30Field;

        private string lOTTABLE31Field;

        private string lOTTABLE32Field;

        private string lOTTABLE33Field;

        private string lOTTABLE34Field;

        private string lOTTABLE35Field;

        private string lOTTABLE36Field;

        private string lOTTABLE37Field;

        private string lOTTABLE38Field;

        private string lOTTABLE39Field;

        private string lOTTABLE40Field;

        private string lOTTABLE41Field;

        private string lOTTABLE42Field;

        private string lOTTABLE43Field;

        private string lOTTABLE44Field;

        private string lOTTABLE45Field;

        private string lOTTABLE46Field;

        private string lOTTABLE47Field;

        private string sUsr6Field;

        private string sUsr7Field;

        private string sUsr8Field;

        private string sUsr9Field;

        private string sUsr10Field;

        private string sUSR11Field;

        private string sUSR12Field;

        private string sUSR13Field;

        private string sUSR14Field;

        private string sUSR15Field;

        private string sUSR16Field;

        private string sUSR17Field;

        private string sUSR18Field;

        private string sUSR19Field;

        private string sUSR20Field;

        private string sUSRNUM01Field;

        private string sUSRNUM02Field;

        private string sUSRNUM03Field;

        private string sUSRNUM04Field;

        private string sUSRNUM05Field;

        private string bOXSNUMBERField;

        private string bOXQTYField;

        private string sTORER_SUSR2Field;

        private string sKU_DESCRField;

        public string SerialKey
        {
            get
            {
                return this.serialKeyField;
            }
            set
            {
                this.serialKeyField = value;
            }
        }

        public string WhseId
        {
            get
            {
                return this.whseIdField;
            }
            set
            {
                this.whseIdField = value;
            }
        }

        public string ReceiptKey
        {
            get
            {
                return this.receiptKeyField;
            }
            set
            {
                this.receiptKeyField = value;
            }
        }

        public string ReceiptLineNumber
        {
            get
            {
                return this.receiptLineNumberField;
            }
            set
            {
                this.receiptLineNumberField = value;
            }
        }

        public string SUBLINENUMBER
        {
            get
            {
                return this.sUBLINENUMBERField;
            }
            set
            {
                this.sUBLINENUMBERField = value;
            }
        }

        public string ExternReceiptKey
        {
            get
            {
                return this.externReceiptKeyField;
            }
            set
            {
                this.externReceiptKeyField = value;
            }
        }

        public string ExternLineNo
        {
            get
            {
                return this.externLineNoField;
            }
            set
            {
                this.externLineNoField = value;
            }
        }

        public string StorerKey
        {
            get
            {
                return this.storerKeyField;
            }
            set
            {
                this.storerKeyField = value;
            }
        }

        public string POKey
        {
            get
            {
                return this.pOKeyField;
            }
            set
            {
                this.pOKeyField = value;
            }
        }

        public string TariffKey
        {
            get
            {
                return this.tariffKeyField;
            }
            set
            {
                this.tariffKeyField = value;
            }
        }

        public string Sku
        {
            get
            {
                return this.skuField;
            }
            set
            {
                this.skuField = value;
            }
        }

        public string AltSku
        {
            get
            {
                return this.altSkuField;
            }
            set
            {
                this.altSkuField = value;
            }
        }

        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        public string DateReceived
        {
            get
            {
                return this.dateReceivedField;
            }
            set
            {
                this.dateReceivedField = value;
            }
        }

        public string QtyExpected
        {
            get
            {
                return this.qtyExpectedField;
            }
            set
            {
                this.qtyExpectedField = value;
            }
        }

        public string QtyAdjusted
        {
            get
            {
                return this.qtyAdjustedField;
            }
            set
            {
                this.qtyAdjustedField = value;
            }
        }

        public string QtyReceived
        {
            get
            {
                return this.qtyReceivedField;
            }
            set
            {
                this.qtyReceivedField = value;
            }
        }

        public string UOM
        {
            get
            {
                return this.uOMField;
            }
            set
            {
                this.uOMField = value;
            }
        }

        public string PackKey
        {
            get
            {
                return this.packKeyField;
            }
            set
            {
                this.packKeyField = value;
            }
        }

        public string VesselKey
        {
            get
            {
                return this.vesselKeyField;
            }
            set
            {
                this.vesselKeyField = value;
            }
        }

        public string VoyageKey
        {
            get
            {
                return this.voyageKeyField;
            }
            set
            {
                this.voyageKeyField = value;
            }
        }

        public string XdockKey
        {
            get
            {
                return this.xdockKeyField;
            }
            set
            {
                this.xdockKeyField = value;
            }
        }

        public string ContainerKey
        {
            get
            {
                return this.containerKeyField;
            }
            set
            {
                this.containerKeyField = value;
            }
        }

        public string ToLoc
        {
            get
            {
                return this.toLocField;
            }
            set
            {
                this.toLocField = value;
            }
        }

        public string ToLot
        {
            get
            {
                return this.toLotField;
            }
            set
            {
                this.toLotField = value;
            }
        }

        public string ToId
        {
            get
            {
                return this.toIdField;
            }
            set
            {
                this.toIdField = value;
            }
        }

        public string ConditionCode
        {
            get
            {
                return this.conditionCodeField;
            }
            set
            {
                this.conditionCodeField = value;
            }
        }

        public string Lottable01
        {
            get
            {
                return this.lottable01Field;
            }
            set
            {
                this.lottable01Field = value;
            }
        }

        public string Lottable02
        {
            get
            {
                return this.lottable02Field;
            }
            set
            {
                this.lottable02Field = value;
            }
        }

        public string Lottable03
        {
            get
            {
                return this.lottable03Field;
            }
            set
            {
                this.lottable03Field = value;
            }
        }

        public string Lottable04
        {
            get
            {
                return this.lottable04Field;
            }
            set
            {
                this.lottable04Field = value;
            }
        }

        public string Lottable05
        {
            get
            {
                return this.lottable05Field;
            }
            set
            {
                this.lottable05Field = value;
            }
        }

        public string Lottable06
        {
            get
            {
                return this.lottable06Field;
            }
            set
            {
                this.lottable06Field = value;
            }
        }

        public string Lottable07
        {
            get
            {
                return this.lottable07Field;
            }
            set
            {
                this.lottable07Field = value;
            }
        }

        public string Lottable08
        {
            get
            {
                return this.lottable08Field;
            }
            set
            {
                this.lottable08Field = value;
            }
        }

        public string Lottable09
        {
            get
            {
                return this.lottable09Field;
            }
            set
            {
                this.lottable09Field = value;
            }
        }

        public string Lottable10
        {
            get
            {
                return this.lottable10Field;
            }
            set
            {
                this.lottable10Field = value;
            }
        }

        public string CaseCnt
        {
            get
            {
                return this.caseCntField;
            }
            set
            {
                this.caseCntField = value;
            }
        }

        public string InnerPack
        {
            get
            {
                return this.innerPackField;
            }
            set
            {
                this.innerPackField = value;
            }
        }

        public string Pallet
        {
            get
            {
                return this.palletField;
            }
            set
            {
                this.palletField = value;
            }
        }

        public string Cube
        {
            get
            {
                return this.cubeField;
            }
            set
            {
                this.cubeField = value;
            }
        }

        public string GrossWgt
        {
            get
            {
                return this.grossWgtField;
            }
            set
            {
                this.grossWgtField = value;
            }
        }

        public string NetWgt
        {
            get
            {
                return this.netWgtField;
            }
            set
            {
                this.netWgtField = value;
            }
        }

        public string OtherUnit1
        {
            get
            {
                return this.otherUnit1Field;
            }
            set
            {
                this.otherUnit1Field = value;
            }
        }

        public string OtherUnit2
        {
            get
            {
                return this.otherUnit2Field;
            }
            set
            {
                this.otherUnit2Field = value;
            }
        }

        public string UnitPrice
        {
            get
            {
                return this.unitPriceField;
            }
            set
            {
                this.unitPriceField = value;
            }
        }

        public string ExtendedPrice
        {
            get
            {
                return this.extendedPriceField;
            }
            set
            {
                this.extendedPriceField = value;
            }
        }

        public string EffectiveDate
        {
            get
            {
                return this.effectiveDateField;
            }
            set
            {
                this.effectiveDateField = value;
            }
        }

        public string FORTE_FLAG
        {
            get
            {
                return this.fORTE_FLAGField;
            }
            set
            {
                this.fORTE_FLAGField = value;
            }
        }

        public string SUsr1
        {
            get
            {
                return this.sUsr1Field;
            }
            set
            {
                this.sUsr1Field = value;
            }
        }

        public string SUsr2
        {
            get
            {
                return this.sUsr2Field;
            }
            set
            {
                this.sUsr2Field = value;
            }
        }

        public string SUsr3
        {
            get
            {
                return this.sUsr3Field;
            }
            set
            {
                this.sUsr3Field = value;
            }
        }

        public string SUsr4
        {
            get
            {
                return this.sUsr4Field;
            }
            set
            {
                this.sUsr4Field = value;
            }
        }

        public string SUsr5
        {
            get
            {
                return this.sUsr5Field;
            }
            set
            {
                this.sUsr5Field = value;
            }
        }

        public string Notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        public string ReasonCode
        {
            get
            {
                return this.reasonCodeField;
            }
            set
            {
                this.reasonCodeField = value;
            }
        }

        public string PalletID
        {
            get
            {
                return this.palletIDField;
            }
            set
            {
                this.palletIDField = value;
            }
        }

        public string QtyRejected
        {
            get
            {
                return this.qtyRejectedField;
            }
            set
            {
                this.qtyRejectedField = value;
            }
        }

        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public string RETURNTYPE
        {
            get
            {
                return this.rETURNTYPEField;
            }
            set
            {
                this.rETURNTYPEField = value;
            }
        }

        public string RETURNREASON
        {
            get
            {
                return this.rETURNREASONField;
            }
            set
            {
                this.rETURNREASONField = value;
            }
        }

        public string DISPOSITIONTYPE
        {
            get
            {
                return this.dISPOSITIONTYPEField;
            }
            set
            {
                this.dISPOSITIONTYPEField = value;
            }
        }

        public string DISPOSITIONCODE
        {
            get
            {
                return this.dISPOSITIONCODEField;
            }
            set
            {
                this.dISPOSITIONCODEField = value;
            }
        }

        public string RETURNCONDITION
        {
            get
            {
                return this.rETURNCONDITIONField;
            }
            set
            {
                this.rETURNCONDITIONField = value;
            }
        }

        public string QCRequired
        {
            get
            {
                return this.qCRequiredField;
            }
            set
            {
                this.qCRequiredField = value;
            }
        }

        public string QCQTYINSPECTED
        {
            get
            {
                return this.qCQTYINSPECTEDField;
            }
            set
            {
                this.qCQTYINSPECTEDField = value;
            }
        }

        public string QCQTYREJECTED
        {
            get
            {
                return this.qCQTYREJECTEDField;
            }
            set
            {
                this.qCQTYREJECTEDField = value;
            }
        }

        public string QCREJREASON
        {
            get
            {
                return this.qCREJREASONField;
            }
            set
            {
                this.qCREJREASONField = value;
            }
        }

        public string QCSTATUS
        {
            get
            {
                return this.qCSTATUSField;
            }
            set
            {
                this.qCSTATUSField = value;
            }
        }

        public string QCUSER
        {
            get
            {
                return this.qCUSERField;
            }
            set
            {
                this.qCUSERField = value;
            }
        }

        public string QCAutoAdjust
        {
            get
            {
                return this.qCAutoAdjustField;
            }
            set
            {
                this.qCAutoAdjustField = value;
            }
        }

        public string ExternalLot
        {
            get
            {
                return this.externalLotField;
            }
            set
            {
                this.externalLotField = value;
            }
        }

        public string RMA
        {
            get
            {
                return this.rMAField;
            }
            set
            {
                this.rMAField = value;
            }
        }

        public string PackingSlipQty
        {
            get
            {
                return this.packingSlipQtyField;
            }
            set
            {
                this.packingSlipQtyField = value;
            }
        }

        public string IPSKEY
        {
            get
            {
                return this.iPSKEYField;
            }
            set
            {
                this.iPSKEYField = value;
            }
        }

        public string SupplierName
        {
            get
            {
                return this.supplierNameField;
            }
            set
            {
                this.supplierNameField = value;
            }
        }

        public string SupplierKey
        {
            get
            {
                return this.supplierKeyField;
            }
            set
            {
                this.supplierKeyField = value;
            }
        }

        public string MATCHLOTTABLE
        {
            get
            {
                return this.mATCHLOTTABLEField;
            }
            set
            {
                this.mATCHLOTTABLEField = value;
            }
        }

        public string RECEIPTDETAILID
        {
            get
            {
                return this.rECEIPTDETAILIDField;
            }
            set
            {
                this.rECEIPTDETAILIDField = value;
            }
        }

        public string POLineNumber
        {
            get
            {
                return this.pOLineNumberField;
            }
            set
            {
                this.pOLineNumberField = value;
            }
        }

        public string LOTTABLE11
        {
            get
            {
                return this.lOTTABLE11Field;
            }
            set
            {
                this.lOTTABLE11Field = value;
            }
        }

        public string LOTTABLE12
        {
            get
            {
                return this.lOTTABLE12Field;
            }
            set
            {
                this.lOTTABLE12Field = value;
            }
        }

        public string SOURCELOCATION
        {
            get
            {
                return this.sOURCELOCATIONField;
            }
            set
            {
                this.sOURCELOCATIONField = value;
            }
        }

        public string SourceVersion
        {
            get
            {
                return this.sourceVersionField;
            }
            set
            {
                this.sourceVersionField = value;
            }
        }

        public string REFERENCETYPE
        {
            get
            {
                return this.rEFERENCETYPEField;
            }
            set
            {
                this.rEFERENCETYPEField = value;
            }
        }

        public string REFERENCEDOCUMENT
        {
            get
            {
                return this.rEFERENCEDOCUMENTField;
            }
            set
            {
                this.rEFERENCEDOCUMENTField = value;
            }
        }

        public string REFERENCELOCATION
        {
            get
            {
                return this.rEFERENCELOCATIONField;
            }
            set
            {
                this.rEFERENCELOCATIONField = value;
            }
        }

        public string REFERENCEVERSION
        {
            get
            {
                return this.rEFERENCEVERSIONField;
            }
            set
            {
                this.rEFERENCEVERSIONField = value;
            }
        }

        public string REFERENCELINE
        {
            get
            {
                return this.rEFERENCELINEField;
            }
            set
            {
                this.rEFERENCELINEField = value;
            }
        }

        public string CUBICMETER
        {
            get
            {
                return this.cUBICMETERField;
            }
            set
            {
                this.cUBICMETERField = value;
            }
        }

        public string HUNDREDWEIGHT
        {
            get
            {
                return this.hUNDREDWEIGHTField;
            }
            set
            {
                this.hUNDREDWEIGHTField = value;
            }
        }

        public string TAREWGT
        {
            get
            {
                return this.tAREWGTField;
            }
            set
            {
                this.tAREWGTField = value;
            }
        }

        public string REFERENCEACCOUNTINGENTITY
        {
            get
            {
                return this.rEFERENCEACCOUNTINGENTITYField;
            }
            set
            {
                this.rEFERENCEACCOUNTINGENTITYField = value;
            }
        }

        public string REFERENCESCHEDULELINE
        {
            get
            {
                return this.rEFERENCESCHEDULELINEField;
            }
            set
            {
                this.rEFERENCESCHEDULELINEField = value;
            }
        }

        public string REQUISITIONDOCUMENT
        {
            get
            {
                return this.rEQUISITIONDOCUMENTField;
            }
            set
            {
                this.rEQUISITIONDOCUMENTField = value;
            }
        }

        public string REQUISITIONACCOUNTINGENTITY
        {
            get
            {
                return this.rEQUISITIONACCOUNTINGENTITYField;
            }
            set
            {
                this.rEQUISITIONACCOUNTINGENTITYField = value;
            }
        }

        public string REQUISITIONLOCATION
        {
            get
            {
                return this.rEQUISITIONLOCATIONField;
            }
            set
            {
                this.rEQUISITIONLOCATIONField = value;
            }
        }

        public string RequisitionVersion
        {
            get
            {
                return this.requisitionVersionField;
            }
            set
            {
                this.requisitionVersionField = value;
            }
        }

        public string REQUISITIONLINE
        {
            get
            {
                return this.rEQUISITIONLINEField;
            }
            set
            {
                this.rEQUISITIONLINEField = value;
            }
        }

        public string REQUISITIONSCHEDULELINE
        {
            get
            {
                return this.rEQUISITIONSCHEDULELINEField;
            }
            set
            {
                this.rEQUISITIONSCHEDULELINEField = value;
            }
        }

        public string PURCHASEORDERDOCUMENT
        {
            get
            {
                return this.pURCHASEORDERDOCUMENTField;
            }
            set
            {
                this.pURCHASEORDERDOCUMENTField = value;
            }
        }

        public string PURCHASEORDERACCOUNTINGENTITY
        {
            get
            {
                return this.pURCHASEORDERACCOUNTINGENTITYField;
            }
            set
            {
                this.pURCHASEORDERACCOUNTINGENTITYField = value;
            }
        }

        public string PURCHASEORDERLOCATION
        {
            get
            {
                return this.pURCHASEORDERLOCATIONField;
            }
            set
            {
                this.pURCHASEORDERLOCATIONField = value;
            }
        }

        public string PurchaseOrderVersion
        {
            get
            {
                return this.purchaseOrderVersionField;
            }
            set
            {
                this.purchaseOrderVersionField = value;
            }
        }

        public string PURCHASEORDERLINE
        {
            get
            {
                return this.pURCHASEORDERLINEField;
            }
            set
            {
                this.pURCHASEORDERLINEField = value;
            }
        }

        public string PURCHASEORDERSCHEDULELINE
        {
            get
            {
                return this.pURCHASEORDERSCHEDULELINEField;
            }
            set
            {
                this.pURCHASEORDERSCHEDULELINEField = value;
            }
        }

        public string SALESORDERDOCUMENT
        {
            get
            {
                return this.sALESORDERDOCUMENTField;
            }
            set
            {
                this.sALESORDERDOCUMENTField = value;
            }
        }

        public string SALESORDERACCOUNTINGENTITY
        {
            get
            {
                return this.sALESORDERACCOUNTINGENTITYField;
            }
            set
            {
                this.sALESORDERACCOUNTINGENTITYField = value;
            }
        }

        public string SALESORDERLOCATION
        {
            get
            {
                return this.sALESORDERLOCATIONField;
            }
            set
            {
                this.sALESORDERLOCATIONField = value;
            }
        }

        public string SalesOrderVersion
        {
            get
            {
                return this.salesOrderVersionField;
            }
            set
            {
                this.salesOrderVersionField = value;
            }
        }

        public string SALESORDERLINE
        {
            get
            {
                return this.sALESORDERLINEField;
            }
            set
            {
                this.sALESORDERLINEField = value;
            }
        }

        public string SALESORDERSCHEDULELINE
        {
            get
            {
                return this.sALESORDERSCHEDULELINEField;
            }
            set
            {
                this.sALESORDERSCHEDULELINEField = value;
            }
        }

        public string PRODUCTIONORDERDOCUMENT
        {
            get
            {
                return this.pRODUCTIONORDERDOCUMENTField;
            }
            set
            {
                this.pRODUCTIONORDERDOCUMENTField = value;
            }
        }

        public string PRODUCTIONORDERACCENTITY
        {
            get
            {
                return this.pRODUCTIONORDERACCENTITYField;
            }
            set
            {
                this.pRODUCTIONORDERACCENTITYField = value;
            }
        }

        public string PRODUCTIONORDERLOCATION
        {
            get
            {
                return this.pRODUCTIONORDERLOCATIONField;
            }
            set
            {
                this.pRODUCTIONORDERLOCATIONField = value;
            }
        }

        public string ProductionOrderVersion
        {
            get
            {
                return this.productionOrderVersionField;
            }
            set
            {
                this.productionOrderVersionField = value;
            }
        }

        public string PRODUCTIONORDERLINE
        {
            get
            {
                return this.pRODUCTIONORDERLINEField;
            }
            set
            {
                this.pRODUCTIONORDERLINEField = value;
            }
        }

        public string PRODUCTIONORDERSCHEDULELINE
        {
            get
            {
                return this.pRODUCTIONORDERSCHEDULELINEField;
            }
            set
            {
                this.pRODUCTIONORDERSCHEDULELINEField = value;
            }
        }

        public string TEMPERATURE
        {
            get
            {
                return this.tEMPERATUREField;
            }
            set
            {
                this.tEMPERATUREField = value;
            }
        }

        public string AddDate
        {
            get
            {
                return this.addDateField;
            }
            set
            {
                this.addDateField = value;
            }
        }

        public string AddWho
        {
            get
            {
                return this.addWhoField;
            }
            set
            {
                this.addWhoField = value;
            }
        }

        public string EditDate
        {
            get
            {
                return this.editDateField;
            }
            set
            {
                this.editDateField = value;
            }
        }

        public string EditWho
        {
            get
            {
                return this.editWhoField;
            }
            set
            {
                this.editWhoField = value;
            }
        }

        public string LOTTABLE13
        {
            get
            {
                return this.lOTTABLE13Field;
            }
            set
            {
                this.lOTTABLE13Field = value;
            }
        }

        public string LOTTABLE14
        {
            get
            {
                return this.lOTTABLE14Field;
            }
            set
            {
                this.lOTTABLE14Field = value;
            }
        }

        public string LOTTABLE15
        {
            get
            {
                return this.lOTTABLE15Field;
            }
            set
            {
                this.lOTTABLE15Field = value;
            }
        }

        public string LOTTABLE16
        {
            get
            {
                return this.lOTTABLE16Field;
            }
            set
            {
                this.lOTTABLE16Field = value;
            }
        }

        public string LOTTABLE17
        {
            get
            {
                return this.lOTTABLE17Field;
            }
            set
            {
                this.lOTTABLE17Field = value;
            }
        }

        public string LOTTABLE18
        {
            get
            {
                return this.lOTTABLE18Field;
            }
            set
            {
                this.lOTTABLE18Field = value;
            }
        }

        public string LOTTABLE19
        {
            get
            {
                return this.lOTTABLE19Field;
            }
            set
            {
                this.lOTTABLE19Field = value;
            }
        }

        public string LOTTABLE20
        {
            get
            {
                return this.lOTTABLE20Field;
            }
            set
            {
                this.lOTTABLE20Field = value;
            }
        }

        public string LOTTABLE21
        {
            get
            {
                return this.lOTTABLE21Field;
            }
            set
            {
                this.lOTTABLE21Field = value;
            }
        }

        public string LOTTABLE22
        {
            get
            {
                return this.lOTTABLE22Field;
            }
            set
            {
                this.lOTTABLE22Field = value;
            }
        }

        public string LOTTABLE23
        {
            get
            {
                return this.lOTTABLE23Field;
            }
            set
            {
                this.lOTTABLE23Field = value;
            }
        }

        public string LOTTABLE24
        {
            get
            {
                return this.lOTTABLE24Field;
            }
            set
            {
                this.lOTTABLE24Field = value;
            }
        }

        public string LOTTABLE25
        {
            get
            {
                return this.lOTTABLE25Field;
            }
            set
            {
                this.lOTTABLE25Field = value;
            }
        }

        public string LOTTABLE26
        {
            get
            {
                return this.lOTTABLE26Field;
            }
            set
            {
                this.lOTTABLE26Field = value;
            }
        }

        public string LOTTABLE27
        {
            get
            {
                return this.lOTTABLE27Field;
            }
            set
            {
                this.lOTTABLE27Field = value;
            }
        }

        public string LOTTABLE28
        {
            get
            {
                return this.lOTTABLE28Field;
            }
            set
            {
                this.lOTTABLE28Field = value;
            }
        }

        public string LOTTABLE29
        {
            get
            {
                return this.lOTTABLE29Field;
            }
            set
            {
                this.lOTTABLE29Field = value;
            }
        }

        public string LOTTABLE30
        {
            get
            {
                return this.lOTTABLE30Field;
            }
            set
            {
                this.lOTTABLE30Field = value;
            }
        }

        public string LOTTABLE31
        {
            get
            {
                return this.lOTTABLE31Field;
            }
            set
            {
                this.lOTTABLE31Field = value;
            }
        }

        public string LOTTABLE32
        {
            get
            {
                return this.lOTTABLE32Field;
            }
            set
            {
                this.lOTTABLE32Field = value;
            }
        }

        public string LOTTABLE33
        {
            get
            {
                return this.lOTTABLE33Field;
            }
            set
            {
                this.lOTTABLE33Field = value;
            }
        }

        public string LOTTABLE34
        {
            get
            {
                return this.lOTTABLE34Field;
            }
            set
            {
                this.lOTTABLE34Field = value;
            }
        }

        public string LOTTABLE35
        {
            get
            {
                return this.lOTTABLE35Field;
            }
            set
            {
                this.lOTTABLE35Field = value;
            }
        }

        public string LOTTABLE36
        {
            get
            {
                return this.lOTTABLE36Field;
            }
            set
            {
                this.lOTTABLE36Field = value;
            }
        }

        public string LOTTABLE37
        {
            get
            {
                return this.lOTTABLE37Field;
            }
            set
            {
                this.lOTTABLE37Field = value;
            }
        }

        public string LOTTABLE38
        {
            get
            {
                return this.lOTTABLE38Field;
            }
            set
            {
                this.lOTTABLE38Field = value;
            }
        }

        public string LOTTABLE39
        {
            get
            {
                return this.lOTTABLE39Field;
            }
            set
            {
                this.lOTTABLE39Field = value;
            }
        }

        public string LOTTABLE40
        {
            get
            {
                return this.lOTTABLE40Field;
            }
            set
            {
                this.lOTTABLE40Field = value;
            }
        }

        public string LOTTABLE41
        {
            get
            {
                return this.lOTTABLE41Field;
            }
            set
            {
                this.lOTTABLE41Field = value;
            }
        }

        public string LOTTABLE42
        {
            get
            {
                return this.lOTTABLE42Field;
            }
            set
            {
                this.lOTTABLE42Field = value;
            }
        }

        public string LOTTABLE43
        {
            get
            {
                return this.lOTTABLE43Field;
            }
            set
            {
                this.lOTTABLE43Field = value;
            }
        }

        public string LOTTABLE44
        {
            get
            {
                return this.lOTTABLE44Field;
            }
            set
            {
                this.lOTTABLE44Field = value;
            }
        }

        public string LOTTABLE45
        {
            get
            {
                return this.lOTTABLE45Field;
            }
            set
            {
                this.lOTTABLE45Field = value;
            }
        }

        public string LOTTABLE46
        {
            get
            {
                return this.lOTTABLE46Field;
            }
            set
            {
                this.lOTTABLE46Field = value;
            }
        }

        public string LOTTABLE47
        {
            get
            {
                return this.lOTTABLE47Field;
            }
            set
            {
                this.lOTTABLE47Field = value;
            }
        }

        public string SUsr6
        {
            get
            {
                return this.sUsr6Field;
            }
            set
            {
                this.sUsr6Field = value;
            }
        }

        public string SUsr7
        {
            get
            {
                return this.sUsr7Field;
            }
            set
            {
                this.sUsr7Field = value;
            }
        }

        public string SUsr8
        {
            get
            {
                return this.sUsr8Field;
            }
            set
            {
                this.sUsr8Field = value;
            }
        }

        public string SUsr9
        {
            get
            {
                return this.sUsr9Field;
            }
            set
            {
                this.sUsr9Field = value;
            }
        }

        public string SUsr10
        {
            get
            {
                return this.sUsr10Field;
            }
            set
            {
                this.sUsr10Field = value;
            }
        }

        public string SUSR11
        {
            get
            {
                return this.sUSR11Field;
            }
            set
            {
                this.sUSR11Field = value;
            }
        }

        public string SUSR12
        {
            get
            {
                return this.sUSR12Field;
            }
            set
            {
                this.sUSR12Field = value;
            }
        }

        public string SUSR13
        {
            get
            {
                return this.sUSR13Field;
            }
            set
            {
                this.sUSR13Field = value;
            }
        }

        public string SUSR14
        {
            get
            {
                return this.sUSR14Field;
            }
            set
            {
                this.sUSR14Field = value;
            }
        }

        public string SUSR15
        {
            get
            {
                return this.sUSR15Field;
            }
            set
            {
                this.sUSR15Field = value;
            }
        }

        public string SUSR16
        {
            get
            {
                return this.sUSR16Field;
            }
            set
            {
                this.sUSR16Field = value;
            }
        }

        public string SUSR17
        {
            get
            {
                return this.sUSR17Field;
            }
            set
            {
                this.sUSR17Field = value;
            }
        }

        public string SUSR18
        {
            get
            {
                return this.sUSR18Field;
            }
            set
            {
                this.sUSR18Field = value;
            }
        }

        public string SUSR19
        {
            get
            {
                return this.sUSR19Field;
            }
            set
            {
                this.sUSR19Field = value;
            }
        }

        public string SUSR20
        {
            get
            {
                return this.sUSR20Field;
            }
            set
            {
                this.sUSR20Field = value;
            }
        }

        public string SUSRNUM01
        {
            get
            {
                return this.sUSRNUM01Field;
            }
            set
            {
                this.sUSRNUM01Field = value;
            }
        }

        public string SUSRNUM02
        {
            get
            {
                return this.sUSRNUM02Field;
            }
            set
            {
                this.sUSRNUM02Field = value;
            }
        }

        public string SUSRNUM03
        {
            get
            {
                return this.sUSRNUM03Field;
            }
            set
            {
                this.sUSRNUM03Field = value;
            }
        }

        public string SUSRNUM04
        {
            get
            {
                return this.sUSRNUM04Field;
            }
            set
            {
                this.sUSRNUM04Field = value;
            }
        }

        public string SUSRNUM05
        {
            get
            {
                return this.sUSRNUM05Field;
            }
            set
            {
                this.sUSRNUM05Field = value;
            }
        }

        public string BOXSNUMBER
        {
            get
            {
                return this.bOXSNUMBERField;
            }
            set
            {
                this.bOXSNUMBERField = value;
            }
        }

        public string BOXQTY
        {
            get
            {
                return this.bOXQTYField;
            }
            set
            {
                this.bOXQTYField = value;
            }
        }

        public string STORER_SUSR2
        {
            get
            {
                return this.sTORER_SUSR2Field;
            }
            set
            {
                this.sTORER_SUSR2Field = value;
            }
        }

        public string SKU_DESCR
        {
            get
            {
                return this.sKU_DESCRField;
            }
            set
            {
                this.sKU_DESCRField = value;
            }
        }


        public AdvancedShipNoticeDetail Copy()
        {
            AdvancedShipNoticeDetail detail = new AdvancedShipNoticeDetail();
            var destProperties = detail.GetType().GetProperties();

            foreach (var sourceProperty in this.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name &&
                destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                )
                    {
                        destProperty.SetValue(detail, sourceProperty.GetValue(
                            this, new object[] { }), new object[] { });

                        break;
                    }
                }
            }
            return detail;
        }
    }
}

