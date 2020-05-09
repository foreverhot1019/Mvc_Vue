using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using System.Web.WebPages;
using AirOut.Web.Models;
using AirOut.Web.Extensions;

namespace AirOut.Web.Repositories
{
   public class OPS_EntrustmentInforQuery:QueryObject<OPS_EntrustmentInfor>
    {
        public OPS_EntrustmentInforQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                x.Id.ToString().Contains(search) || 
                x.Consign_Code.Contains(search) || 
                x.Custom_Code.Contains(search) || 
                x.Area_Code.Contains(search) || 
                x.Entrustment_Name.Contains(search) || 
                x.Entrustment_Code.Contains(search) || 
                x.FWD_Code.Contains(search) || 
                x.Consignee_Code.Contains(search) || 
                x.Carriage_Account_Code.Contains(search) || 
                x.Incidental_Account_Code.Contains(search) || 
                x.Depart_Port.Contains(search) || 
                x.Transfer_Port.Contains(search) || 
                x.End_Port.Contains(search) || 
                x.Shipper_H.Contains(search) || 
                x.Consignee_H.Contains(search) || 
                x.Notify_Part_H.Contains(search) || 
                x.Shipper_M.Contains(search) || 
                x.Consignee_M.Contains(search) || 
                x.Notify_Part_M.Contains(search) || 
                x.Pieces_TS.ToString().Contains(search) || 
                x.Weight_TS.ToString().Contains(search) || 
                x.Pieces_SK.ToString().Contains(search) || 
                x.Slac_SK.ToString().Contains(search) || 
                x.Weight_SK.ToString().Contains(search) || 
                x.Pieces_DC.ToString().Contains(search) || 
                x.Slac_DC.ToString().Contains(search) || 
                x.Weight_DC.ToString().Contains(search) || 
                x.Pieces_Fact.ToString().Contains(search) || 
                x.Weight_Fact.ToString().Contains(search) || 
                x.MoorLevel.Contains(search) || 
                x.Volume_TS.ToString().Contains(search) || 
                x.Charge_Weight_TS.ToString().Contains(search) || 
                x.Bulk_Weight_TS.ToString().Contains(search) || 
                x.Volume_SK.ToString().Contains(search) || 
                x.Charge_Weight_SK.ToString().Contains(search) || 
                x.Bulk_Weight_SK.ToString().Contains(search) || 
                x.Bulk_Percent_SK.ToString().Contains(search) || 
                x.Account_Weight_SK.ToString().Contains(search) || 
                x.Volume_DC.ToString().Contains(search) || 
                x.Charge_Weight_DC.ToString().Contains(search) || 
                x.Bulk_Weight_DC.ToString().Contains(search) || 
                x.Bulk_Percent_DC.ToString().Contains(search) || 
                x.Account_Weight_DC.ToString().Contains(search) || 
                x.Volume_Fact.ToString().Contains(search) || 
                x.Charge_Weight_Fact.ToString().Contains(search) || 
                x.Bulk_Weight_Fact.ToString().Contains(search) || 
                x.Bulk_Percent_Fact.ToString().Contains(search) || 
                x.Account_Weight_Fact.ToString().Contains(search) || 
                x.Marks_H.Contains(search) || 
                x.EN_Name_H.Contains(search) || 
                x.Book_Flat_Code.Contains(search) || 
                x.Airways_Code.Contains(search) ||
                x.Flight_No.Contains(search) || 
                x.MBL.Contains(search) || 
                x.HBL.Contains(search) || 
                x.Flight_Date_Want.ToString().Contains(search) || 
                x.Book_Remark.Contains(search) || 
                x.Delivery_Point.Contains(search) || 
                x.Warehouse_Code.Contains(search) || 
                x.RK_Date.ToString().Contains(search) || 
                x.CK_Date.ToString().Contains(search) || 
                x.CH_Name.Contains(search) || 
                x.AMS.ToString().Contains(search) || 
                x.Remark.Contains(search) || 
                x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public OPS_EntrustmentInforQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                x.Id.ToString().Contains(search) || 
                x.Consign_Code.Contains(search) || 
                x.Custom_Code.Contains(search) || 
                x.Area_Code.Contains(search) || 
                x.Entrustment_Name.Contains(search) || 
                x.Entrustment_Code.Contains(search) || 
                x.FWD_Code.Contains(search) || 
                x.Consignee_Code.Contains(search) || 
                x.Carriage_Account_Code.Contains(search) || 
                x.Incidental_Account_Code.Contains(search) || 
                x.Depart_Port.Contains(search) || 
                x.Transfer_Port.Contains(search) || 
                x.End_Port.Contains(search) || 
                x.Shipper_H.Contains(search) || 
                x.Consignee_H.Contains(search) || 
                x.Notify_Part_H.Contains(search) || 
                x.Shipper_M.Contains(search) || 
                x.Consignee_M.Contains(search) || 
                x.Notify_Part_M.Contains(search) || 
                x.Pieces_TS.ToString().Contains(search) || 
                x.Weight_TS.ToString().Contains(search) || 
                x.Pieces_SK.ToString().Contains(search) || 
                x.Slac_SK.ToString().Contains(search) || 
                x.Weight_SK.ToString().Contains(search) || 
                x.Pieces_DC.ToString().Contains(search) || 
                x.Slac_DC.ToString().Contains(search) || 
                x.Weight_DC.ToString().Contains(search) || 
                x.Pieces_Fact.ToString().Contains(search) || 
                x.Weight_Fact.ToString().Contains(search) || 
                x.MoorLevel.Contains(search) || 
                x.Volume_TS.ToString().Contains(search) || 
                x.Charge_Weight_TS.ToString().Contains(search) || 
                x.Bulk_Weight_TS.ToString().Contains(search) || 
                x.Volume_SK.ToString().Contains(search) || 
                x.Charge_Weight_SK.ToString().Contains(search) || 
                x.Bulk_Weight_SK.ToString().Contains(search) || 
                x.Bulk_Percent_SK.ToString().Contains(search) || 
                x.Account_Weight_SK.ToString().Contains(search) || 
                x.Volume_DC.ToString().Contains(search) || 
                x.Charge_Weight_DC.ToString().Contains(search) || 
                x.Bulk_Weight_DC.ToString().Contains(search) || 
                x.Bulk_Percent_DC.ToString().Contains(search) || 
                x.Account_Weight_DC.ToString().Contains(search) || 
                x.Volume_Fact.ToString().Contains(search) || 
                x.Charge_Weight_Fact.ToString().Contains(search) || 
                x.Bulk_Weight_Fact.ToString().Contains(search) || 
                x.Bulk_Percent_Fact.ToString().Contains(search) || 
                x.Account_Weight_Fact.ToString().Contains(search) || 
                x.Marks_H.Contains(search) || 
                x.EN_Name_H.Contains(search) || 
                x.Book_Flat_Code.Contains(search) || 
                x.Airways_Code.Contains(search) ||
                x.Flight_No.Contains(search) || 
                x.MBL.Contains(search) || 
                x.HBL.Contains(search) || 
                x.Flight_Date_Want.ToString().Contains(search) || 
                x.Book_Remark.Contains(search) || 
                x.Delivery_Point.Contains(search) || 
                x.Warehouse_Code.Contains(search) || 
                x.RK_Date.ToString().Contains(search) || 
                x.CK_Date.ToString().Contains(search) || 
                x.CH_Name.Contains(search) || 
                x.AMS.ToString().Contains(search) || 
                x.Remark.Contains(search) || 
                x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public OPS_EntrustmentInforQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Id == val);
                    }
                    if (rule.field == "MBLId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.MBLId == val);
                    }
                    if (rule.field == "Only_Self" && !string.IsNullOrEmpty(rule.value))
                    {
                        var CurrentAppUser = AirOut.Web.Controllers.Utility.CurrentAppUser;
                        if (CurrentAppUser != null && !string.IsNullOrEmpty(CurrentAppUser.Id))
                        {
                            var name = CurrentAppUser.Id;
                            And(x => x.ADDID == name);
                        }
                    }
                    else if (rule.field == "MBLId" && rule.value == "null")
                    {
                        And(x => x.MBLId == null);
                    }
                    if (rule.field == "Operation_Id" && !string.IsNullOrEmpty(rule.value))
					{
                        And(x => x.Operation_Id.StartsWith(rule.value));
					}
					if (rule.field == "Consign_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Consign_Code.StartsWith(rule.value));
					}
					if (rule.field == "Custom_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Custom_Code.StartsWith(rule.value));
					}
					if (rule.field == "Area_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Area_Code.StartsWith(rule.value));
					}
					if (rule.field == "Entrustment_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Entrustment_Name.StartsWith(rule.value));
					}
					if (rule.field == "Entrustment_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Entrustment_Code.StartsWith(rule.value));
					}
					if (rule.field == "FWD_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FWD_Code.StartsWith(rule.value));
					}
					if (rule.field == "Consignee_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Consignee_Code.StartsWith(rule.value));
					}
					if (rule.field == "Carriage_Account_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Carriage_Account_Code.StartsWith(rule.value));
					}
					if (rule.field == "Incidental_Account_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Incidental_Account_Code.StartsWith(rule.value));
					}
					if (rule.field == "Depart_Port" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Depart_Port.StartsWith(rule.value));
					}
					if (rule.field == "Transfer_Port" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Transfer_Port.StartsWith(rule.value));
					}
					if (rule.field == "End_Port" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.End_Port.StartsWith(rule.value));
					}
					if (rule.field == "Shipper_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Shipper_H.StartsWith(rule.value));
					}
					if (rule.field == "Consignee_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Consignee_H.StartsWith(rule.value));
					}
					if (rule.field == "Notify_Part_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Notify_Part_H.StartsWith(rule.value));
					}
					if (rule.field == "Shipper_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Shipper_M.StartsWith(rule.value));
					}
					if (rule.field == "Consignee_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Consignee_M.StartsWith(rule.value));
					}
					if (rule.field == "Notify_Part_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Notify_Part_M.StartsWith(rule.value));
					}
					if (rule.field == "Pieces_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_TS == val);
					}
					if (rule.field == "Weight_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_TS == val);
					}
					if (rule.field == "Pieces_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_SK == val);
					}
					if (rule.field == "Slac_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Slac_SK == val);
					}
					if (rule.field == "Weight_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_SK == val);
					}
					if (rule.field == "Pieces_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_DC == val);
					}
					if (rule.field == "Slac_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Slac_DC == val);
					}
					if (rule.field == "Weight_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_DC == val);
					}
					if (rule.field == "Pieces_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_Fact == val);
					}
					if (rule.field == "Weight_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_Fact == val);
					}
					if (rule.field == "MoorLevel" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.MoorLevel.StartsWith(rule.value));
					}
					if (rule.field == "Volume_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_TS == val);
					}
					if (rule.field == "Charge_Weight_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Charge_Weight_TS == val);
					}
					if (rule.field == "Bulk_Weight_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Weight_TS == val);
					}
					if (rule.field == "Volume_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_SK == val);
					}
					if (rule.field == "Charge_Weight_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Charge_Weight_SK == val);
					}
					if (rule.field == "Bulk_Weight_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Weight_SK == val);
					}
					if (rule.field == "Bulk_Percent_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Percent_SK == val);
					}
					if (rule.field == "Account_Weight_SK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Account_Weight_SK == val);
					}
					if (rule.field == "Volume_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_DC == val);
					}
					if (rule.field == "Charge_Weight_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Charge_Weight_DC == val);
					}
					if (rule.field == "Bulk_Weight_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Weight_DC == val);
					}
					if (rule.field == "Bulk_Percent_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Percent_DC == val);
					}
					if (rule.field == "Account_Weight_DC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Account_Weight_DC == val);
					}
					if (rule.field == "Volume_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_Fact == val);
					}
					if (rule.field == "Charge_Weight_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Charge_Weight_Fact == val);
					}
					if (rule.field == "Bulk_Weight_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Weight_Fact == val);
					}
					if (rule.field == "Bulk_Percent_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Percent_Fact == val);
					}
					if (rule.field == "Account_Weight_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Account_Weight_Fact == val);
					}
					if (rule.field == "Marks_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Marks_H.StartsWith(rule.value));
					}
					if (rule.field == "EN_Name_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EN_Name_H.StartsWith(rule.value));
					}
					if (rule.field == "Book_Flat_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Book_Flat_Code.StartsWith(rule.value));
					}
					if (rule.field == "Airways_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Airways_Code.StartsWith(rule.value));
					}
					if (rule.field == "FLIGHT_No" && !string.IsNullOrEmpty(rule.value))
					{
                        And(x => x.Flight_No.StartsWith(rule.value));
					}
					if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
					{
                        var val = Common.RemoveNotNumber(rule.value);
                        And(x => x.MBL.StartsWith(val));
					}
					if (rule.field == "HBL" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.HBL.StartsWith(rule.value));
					}
					if (rule.field == "Flight_Date_Want" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Flight_Date_Want == date);
					}
					if (rule.field == "_Flight_Date_Want" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Flight_Date_Want >= date);
					}
					if (rule.field == "Flight_Date_Want_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Flight_Date_Want <= date);
					}
					if (rule.field == "Book_Remark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Book_Remark.StartsWith(rule.value));
					}
					if (rule.field == "Delivery_Point" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Delivery_Point.StartsWith(rule.value));
					}
					if (rule.field == "Warehouse_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Warehouse_Code.StartsWith(rule.value));
					}
					if (rule.field == "RK_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.RK_Date == date);
					}
					if (rule.field == "_RK_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.RK_Date >= date);
					}
					if (rule.field == "RK_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.RK_Date <= date);
					}
					if (rule.field == "CK_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.CK_Date == date);
					}
					if (rule.field == "_CK_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.CK_Date >= date);
					}
					if (rule.field == "CK_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.CK_Date <= date);
					}
					if (rule.field == "CH_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CH_Name.StartsWith(rule.value));
					}
					if (rule.field == "AMS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.AMS == val);
                    }
                    if (rule.field == "Lot_No" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Lot_No.StartsWith(rule.value));
                    }
                    if (rule.field == "Batch_Num" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Batch_Num.StartsWith(rule.value));
                    }
					if (rule.field == "Remark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Remark.StartsWith(rule.value));
					}
					if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.OperatingPoint == val);
					}

                    if (rule.field == "Is_BAS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Is_BAS == boolval);
                    }
                    if (rule.field == "Is_DCZ" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Is_DCZ == boolval);
                    }
                    if (rule.field == "Is_ZB" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Is_ZB == boolval);
                    }
                    if (rule.field == "Is_TG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Is_TG == boolval);
                    }
                    if (rule.field == "Is_HDQ" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_HDQ == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_HDQ == boolval || x.Is_HDQ == null));
                        }
                    }
                    if (rule.field == "Is_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_BG == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_BG == boolval || x.Is_BG == null));
                        }
                    }
                    if (rule.field == "Is_BQ" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_BQ == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_BQ == boolval || x.Is_BQ == null));
                        }
                    }
                    if (rule.field == "Is_OutGoing" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_OutGoing == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_OutGoing == boolval || x.Is_OutGoing == null));
                        }
                    }
                    if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDID.StartsWith(rule.value));
                    }
                    if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDWHO.StartsWith(rule.value));
                    }
					//if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.ADDTS == date);
					//}
					//if (rule.field == "_ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.ADDTS >= date);
					//}
					//if (rule.field == "ADDTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.ADDTS <= date);
					//}
					//if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
					//{
					//	 And(x => x.EDITWHO.StartsWith(rule.value));
					//}
					//if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.EDITTS == date);
					//}
					//if (rule.field == "_EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.EDITTS >= date);
					//}
					//if (rule.field == "EDITTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.EDITTS <= date);
					//}
					//if (rule.field == "EDITID" && !string.IsNullOrEmpty(rule.value))
					//{
					//	 And(x => x.EDITID.StartsWith(rule.value));
					//}
				}
			}
            return this;
		}
	}
}