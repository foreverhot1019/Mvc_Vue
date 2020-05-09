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
   public class Warehouse_receiptQuery:QueryObject<Warehouse_receipt>
    {
        public Warehouse_receiptQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.Id.ToString().Contains(search) || 
                     x.Warehouse_Id.Contains(search) || 
                     x.Entry_Id.Contains(search) || 
                     x.Warehouse_Code.Contains(search) || 
                     x.Pieces_CK.ToString().Contains(search) || 
                     x.Weight_CK.ToString().Contains(search) || 
                     x.Volume_CK.ToString().Contains(search) || 
                     x.Packing.Contains(search) || 
                     x.Bulk_Weight_CK.ToString().Contains(search) || 
                     x.Closure_Remark.Contains(search) || 
                     x.Warehouse_Remark.Contains(search) || 
                     x.Consign_Code_CK.Contains(search) || 
                     x.MBL.Contains(search) || 
                     x.HBL.Contains(search) || 
                     x.Flight_Date_Want.ToString().Contains(search) || 
                     x.FLIGHT_No.Contains(search) || 
                     x.End_Port.Contains(search) || 
                     x.In_Date.ToString().Contains(search) || 
                     x.Out_Date.ToString().Contains(search) || 
                     x.CH_Name_CK.Contains(search) || 
                     x.Truck_Id.Contains(search) || 
                     x.Driver.Contains(search) || 
                     x.Remark.Contains(search) || 
                     x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public Warehouse_receiptQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.Id.ToString().Contains(search) || 
                     x.Warehouse_Id.Contains(search) || 
                     x.Entry_Id.Contains(search) || 
                     x.Warehouse_Code.Contains(search) || 
                     x.Pieces_CK.ToString().Contains(search) || 
                     x.Weight_CK.ToString().Contains(search) || 
                     x.Volume_CK.ToString().Contains(search) || 
                     x.Packing.Contains(search) || 
                     x.Bulk_Weight_CK.ToString().Contains(search) || 
                     x.Closure_Remark.Contains(search) || 
                     x.Warehouse_Remark.Contains(search) || 
                     x.Consign_Code_CK.Contains(search) || 
                     x.MBL.Contains(search) || 
                     x.HBL.Contains(search) || 
                     x.Flight_Date_Want.ToString().Contains(search) || 
                     x.FLIGHT_No.Contains(search) || 
                     x.End_Port.Contains(search) || 
                     x.In_Date.ToString().Contains(search) || 
                     x.Out_Date.ToString().Contains(search) || 
                     x.CH_Name_CK.Contains(search) || 
                     x.Truck_Id.Contains(search) || 
                     x.Driver.Contains(search) || 
                     x.Remark.Contains(search) || 
                     x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public Warehouse_receiptQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Warehouse_Id" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Warehouse_Id.StartsWith(rule.value));
					}
					if (rule.field == "Entry_Id" && !string.IsNullOrEmpty(rule.value))
					{
                        And(x => x.Entry_Id.Contains(rule.value));
					}
					if (rule.field == "Warehouse_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Warehouse_Code.StartsWith(rule.value));
					}
					if (rule.field == "Pieces_CK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_CK == val);
					}
					if (rule.field == "Weight_CK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_CK == val);
					}
					if (rule.field == "Volume_CK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_CK == val);
					}
					if (rule.field == "Packing" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Packing.StartsWith(rule.value));
					}
					if (rule.field == "Bulk_Weight_CK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bulk_Weight_CK == val);
					}
					if (rule.field == "Damaged_CK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Damaged_CK == boolval);
					}
					if (rule.field == "Dampness_CK" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Dampness_CK == boolval);
					}
					if (rule.field == "Deformation" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Deformation == boolval);
					}
					if (rule.field == "Is_GF" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_GF == boolval);
					}
					if (rule.field == "Closure_Remark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Closure_Remark.StartsWith(rule.value));
					}
					if (rule.field == "Is_QG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_QG == boolval);
					}
					if (rule.field == "Warehouse_Remark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Warehouse_Remark.StartsWith(rule.value));
					}
					if (rule.field == "Consign_Code_CK" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Consign_Code_CK.StartsWith(rule.value));
					}
					if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
					{
                         var val = Common.RemoveNotNumber(rule.value);
                         And(x => x.MBL.StartsWith(val));
                    }
                    if (rule.field == "MBLId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.MBLId == val);
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
					if (rule.field == "FLIGHT_No" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FLIGHT_No.StartsWith(rule.value));
					}
					if (rule.field == "End_Port" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.End_Port.StartsWith(rule.value));
					}
					if (rule.field == "In_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.In_Date == date);
					}
					if (rule.field == "_In_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.In_Date >= date);
					}
					if (rule.field == "In_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.In_Date <= date);
					}
					if (rule.field == "Out_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Out_Date == date);
					}
					if (rule.field == "_Out_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Out_Date >= date);
					}
					if (rule.field == "Out_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Out_Date <= date);
					}
					if (rule.field == "CH_Name_CK" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CH_Name_CK.StartsWith(rule.value));
					}
					if (rule.field == "Is_CustomerReturn" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_CustomerReturn == boolval);
					}
					if (rule.field == "Is_MyReturn" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_MyReturn == boolval);
					}
					if (rule.field == "Truck_Id" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Truck_Id.StartsWith(rule.value));
					}
					if (rule.field == "Driver" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Driver.StartsWith(rule.value));
					}
					if (rule.field == "Is_DamageUpload" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_DamageUpload == boolval);
					}
					if (rule.field == "Is_DeliveryUpload" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_DeliveryUpload == boolval);
					}
					if (rule.field == "Is_EntryUpload" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_EntryUpload == boolval);
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

                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusIsOrNoEnum>(rule.value);
                        And(x => x.Status == EnumVal);
                    }
                    if (rule.field == "Is_Binding" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
                         And(x => x.Is_Binding == boolval);
					}
					//if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
					//{
					//	 And(x => x.ADDID.StartsWith(rule.value));
					//}
					//if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
					//{
					//	 And(x => x.ADDWHO.StartsWith(rule.value));
					//}
					//if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.ADDTS == date);
					//}
                    if (rule.field == "_ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS >= date);
                    }
                    if (rule.field == "ADDTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS <= date);
                    }
					//if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
					//{
					//	 And(x => x.EDITWHO.StartsWith(rule.value));
					//}
					//if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					//{	
					//	 var date = Convert.ToDateTime(rule.value);
					//	 And(x => x.EDITTS == date);
					//}
                    if (rule.field == "_EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.EDITTS >= date);
                    }
                    if (rule.field == "EDITTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.EDITTS <= date);
                    }
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