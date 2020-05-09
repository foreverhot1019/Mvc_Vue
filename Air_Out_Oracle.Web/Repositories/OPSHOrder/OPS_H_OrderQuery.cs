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
   public class OPS_H_OrderQuery:QueryObject<OPS_H_Order>
    {
        public OPS_H_OrderQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Shipper_H.Contains(search) || 
x.Consignee_H.Contains(search) || 
x.Notify_Part_H.Contains(search) || 
x.Currency_H.Contains(search) || 
x.Bragainon_Article_H.Contains(search) || 
x.Pay_Mode_H.Contains(search) || 
x.Carriage_H.Contains(search) || 
x.Incidental_Expenses_H.Contains(search) || 
x.Declare_Value_Trans_H.Contains(search) || 
x.Declare_Value_Ciq_H.Contains(search) || 
x.Risk_H.Contains(search) || 
x.Marks_H.Contains(search) || 
x.EN_Name_H.Contains(search) || 
x.Pieces_H.ToString().Contains(search) || 
x.Weight_H.ToString().Contains(search) || 
x.Volume_H.ToString().Contains(search) || 
x.Charge_Weight_H.ToString().Contains(search) || 
x.HBL.Contains(search) || 
x.Operation_Id.Contains(search) || 
x.Ty_Type.Contains(search) || 
x.Lot_No.Contains(search) || 
x.Hbl_Feight.Contains(search) || 
x.ADDPoint.ToString().Contains(search) || 
x.EDITPoint.ToString().Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public OPS_H_OrderQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Shipper_H.Contains(search) || 
x.Consignee_H.Contains(search) || 
x.Notify_Part_H.Contains(search) || 
x.Currency_H.Contains(search) || 
x.Bragainon_Article_H.Contains(search) || 
x.Pay_Mode_H.Contains(search) || 
x.Carriage_H.Contains(search) || 
x.Incidental_Expenses_H.Contains(search) || 
x.Declare_Value_Trans_H.Contains(search) || 
x.Declare_Value_Ciq_H.Contains(search) || 
x.Risk_H.Contains(search) || 
x.Marks_H.Contains(search) || 
x.EN_Name_H.Contains(search) || 
x.Pieces_H.ToString().Contains(search) || 
x.Weight_H.ToString().Contains(search) || 
x.Volume_H.ToString().Contains(search) || 
x.Charge_Weight_H.ToString().Contains(search) || 
x.HBL.Contains(search) || 
x.Operation_Id.Contains(search) || 
x.Ty_Type.Contains(search) || 
x.Lot_No.Contains(search) || 
x.Hbl_Feight.Contains(search) || 
x.ADDPoint.ToString().Contains(search) || 
x.EDITPoint.ToString().Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public OPS_H_OrderQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Currency_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Currency_H.StartsWith(rule.value));
					}
					if (rule.field == "Bragainon_Article_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Bragainon_Article_H.StartsWith(rule.value));
					}
					if (rule.field == "Pay_Mode_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Pay_Mode_H.StartsWith(rule.value));
					}
					if (rule.field == "Carriage_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Carriage_H.StartsWith(rule.value));
					}
					if (rule.field == "Incidental_Expenses_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Incidental_Expenses_H.StartsWith(rule.value));
					}
					if (rule.field == "Declare_Value_Trans_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Declare_Value_Trans_H.StartsWith(rule.value));
					}
					if (rule.field == "Declare_Value_Ciq_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Declare_Value_Ciq_H.StartsWith(rule.value));
					}
					if (rule.field == "Risk_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Risk_H.StartsWith(rule.value));
					}
					if (rule.field == "Marks_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Marks_H.StartsWith(rule.value));
					}
					if (rule.field == "EN_Name_H" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EN_Name_H.StartsWith(rule.value));
					}
					if (rule.field == "Pieces_H" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_H == val);
					}
					if (rule.field == "Weight_H" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_H == val);
					}
					if (rule.field == "Volume_H" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_H == val);
					}
					if (rule.field == "Charge_Weight_H" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Charge_Weight_H == val);
                    }
                    if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.MBL.StartsWith(rule.value));
                    }
                    if (rule.field == "MBLId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.MBLId == val);
                    }
                    else if (rule.field == "MBLId" && rule.value == "null")
                    {
                        And(x => x.MBLId == null);
                    }
					if (rule.field == "HBL" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.HBL.StartsWith(rule.value));
					}
					if (rule.field == "Operation_Id" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Operation_Id.StartsWith(rule.value));
					}
					if (rule.field == "Is_Self" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_Self == boolval);
					}
					if (rule.field == "Ty_Type" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Ty_Type.StartsWith(rule.value));
					}
					if (rule.field == "Lot_No" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Lot_No.StartsWith(rule.value));
					}
					if (rule.field == "Hbl_Feight" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Hbl_Feight.StartsWith(rule.value));
					}
					if (rule.field == "Is_XC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_XC == boolval);
					}
					if (rule.field == "Is_BAS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_BAS == boolval);
					}
					if (rule.field == "Is_DCZ" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_DCZ == boolval);
					}
					if (rule.field == "Is_ZB" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_ZB == boolval);
                    }
                    if (rule.field == "Is_TG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Is_TG == boolval);
                    }
					if (rule.field == "ADDPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.ADDPoint == val);
					}
					if (rule.field == "EDITPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.EDITPoint == val);
					}
					if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.OperatingPoint == val);
					}
                    if (rule.field == "Batch_Num" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Batch_Num.StartsWith(rule.value));
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