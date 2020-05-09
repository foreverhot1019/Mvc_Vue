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
   public class OPS_M_OrderQuery:QueryObject<OPS_M_Order>
    {
        public OPS_M_OrderQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.MBL.Contains(search) || 
x.Airways_Code.Contains(search) || 
x.FWD_Code.Contains(search) || 
x.Shipper_M.Contains(search) || 
x.Consignee_M.Contains(search) || 
x.Notify_Part_M.Contains(search) || 
x.Depart_Port.Contains(search) || 
x.End_Port.Contains(search) || 
x.Flight_No.Contains(search) || 
x.Flight_Date_Want.ToString().Contains(search) || 
x.Currency_M.Contains(search) || 
x.Bragainon_Article_M.Contains(search) || 
x.Pay_Mode_M.Contains(search) || 
x.Carriage_M.Contains(search) || 
x.Incidental_Expenses_M.Contains(search) || 
x.Declare_Value_Trans_M.Contains(search) || 
x.Declare_Value_Ciq_M.Contains(search) || 
x.Risk_M.Contains(search) || 
x.Marks_M.Contains(search) || 
x.EN_Name_M.Contains(search) || 
x.Hand_Info_M.Contains(search) || 
x.Signature_Agent_M.Contains(search) || 
x.Rate_Class_M.Contains(search) || 
x.Air_Frae.ToString().Contains(search) || 
x.AWC.ToString().Contains(search) || 
x.Pieces_M.ToString().Contains(search) || 
x.Weight_M.ToString().Contains(search) || 
x.Volume_M.ToString().Contains(search) || 
x.Charge_Weight_M.ToString().Contains(search) || 
x.Price_Article.Contains(search) || 
x.CCC.Contains(search) || 
x.File_M.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public OPS_M_OrderQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.MBL.Contains(search) || 
x.Airways_Code.Contains(search) || 
x.FWD_Code.Contains(search) || 
x.Shipper_M.Contains(search) || 
x.Consignee_M.Contains(search) || 
x.Notify_Part_M.Contains(search) || 
x.Depart_Port.Contains(search) || 
x.End_Port.Contains(search) || 
x.Flight_No.Contains(search) || 
x.Flight_Date_Want.ToString().Contains(search) || 
x.Currency_M.Contains(search) || 
x.Bragainon_Article_M.Contains(search) || 
x.Pay_Mode_M.Contains(search) || 
x.Carriage_M.Contains(search) || 
x.Incidental_Expenses_M.Contains(search) || 
x.Declare_Value_Trans_M.Contains(search) || 
x.Declare_Value_Ciq_M.Contains(search) || 
x.Risk_M.Contains(search) || 
x.Marks_M.Contains(search) || 
x.EN_Name_M.Contains(search) || 
x.Hand_Info_M.Contains(search) || 
x.Signature_Agent_M.Contains(search) || 
x.Rate_Class_M.Contains(search) || 
x.Air_Frae.ToString().Contains(search) || 
x.AWC.ToString().Contains(search) || 
x.Pieces_M.ToString().Contains(search) || 
x.Weight_M.ToString().Contains(search) || 
x.Volume_M.ToString().Contains(search) || 
x.Charge_Weight_M.ToString().Contains(search) || 
x.Price_Article.Contains(search) || 
x.CCC.Contains(search) || 
x.File_M.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public OPS_M_OrderQuery Withfilter(IEnumerable<filterRule> filters)
        {

            And(x => x.Id != 144);
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Id == val);
					}
					if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.MBL.StartsWith(rule.value));
					}
					if (rule.field == "Airways_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Airways_Code.StartsWith(rule.value));
					}
					if (rule.field == "FWD_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FWD_Code.StartsWith(rule.value));
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
					if (rule.field == "Depart_Port" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Depart_Port.StartsWith(rule.value));
					}
					if (rule.field == "End_Port" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.End_Port.StartsWith(rule.value));
					}
					if (rule.field == "Flight_No" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Flight_No.StartsWith(rule.value));
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
					if (rule.field == "Currency_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Currency_M.StartsWith(rule.value));
					}
					if (rule.field == "Bragainon_Article_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Bragainon_Article_M.StartsWith(rule.value));
					}
					if (rule.field == "Pay_Mode_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Pay_Mode_M.StartsWith(rule.value));
					}
					if (rule.field == "Carriage_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Carriage_M.StartsWith(rule.value));
					}
					if (rule.field == "Incidental_Expenses_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Incidental_Expenses_M.StartsWith(rule.value));
					}
					if (rule.field == "Declare_Value_Trans_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Declare_Value_Trans_M.StartsWith(rule.value));
					}
					if (rule.field == "Declare_Value_Ciq_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Declare_Value_Ciq_M.StartsWith(rule.value));
					}
					if (rule.field == "Risk_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Risk_M.StartsWith(rule.value));
					}
					if (rule.field == "Marks_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Marks_M.StartsWith(rule.value));
					}
					if (rule.field == "EN_Name_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EN_Name_M.StartsWith(rule.value));
					}
					if (rule.field == "Hand_Info_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Hand_Info_M.StartsWith(rule.value));
					}
					if (rule.field == "Signature_Agent_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Signature_Agent_M.StartsWith(rule.value));
					}
					if (rule.field == "Rate_Class_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Rate_Class_M.StartsWith(rule.value));
					}
					if (rule.field == "Air_Frae" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Air_Frae == val);
					}
					if (rule.field == "AWC" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.AWC == val);
					}
					if (rule.field == "Pieces_M" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_M == val);
					}
					if (rule.field == "Weight_M" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_M == val);
					}
					if (rule.field == "Volume_M" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_M == val);
					}
					if (rule.field == "Charge_Weight_M" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Charge_Weight_M == val);
					}
					if (rule.field == "Price_Article" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Price_Article.StartsWith(rule.value));
					}
					if (rule.field == "CCC" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CCC.StartsWith(rule.value));
					}
					if (rule.field == "File_M" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.File_M.StartsWith(rule.value));
					}
					if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.OperatingPoint == val);
					}
                    if (rule.field == "SendOut_ZD" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.SendOut_ZD == boolval);
                        }
                        else
                        {
                            And(x => (x.SendOut_ZD == boolval || x.SendOut_ZD == null));
                        }
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