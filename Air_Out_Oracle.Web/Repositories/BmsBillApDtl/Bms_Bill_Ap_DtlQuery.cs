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
   public class Bms_Bill_Ap_DtlQuery:QueryObject<Bms_Bill_Ap_Dtl>
    {
        public Bms_Bill_Ap_DtlQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                    x.Id.ToString().Contains(search) || 
                    x.Bms_Bill_Ap_Id.ToString().Contains(search) || 
                    x.Dzbh.Contains(search) || 
                    x.Line_No.ToString().Contains(search) || 
                    x.Line_Id.ToString().Contains(search) || 
                    x.Charge_Code.Contains(search) || 
                    x.Charge_Desc.Contains(search) || 
                    x.Unitprice.ToString().Contains(search) || 
                    x.Unitprice2.ToString().Contains(search) || 
                    x.Qty.ToString().Contains(search) || 
                    x.Account.ToString().Contains(search) || 
                    x.Account2.ToString().Contains(search) || 
                    x.Money_Code.Contains(search) || 
                    x.Summary.Contains(search) || 
                    x.Invoice_No.Contains(search) || 
                    x.Create_Status.ToString().Contains(search) || 
                    x.Invoice_Status.ToString().Contains(search) || 
                    x.Collate_Id.Contains(search) || 
                    x.Collate_Name.Contains(search) || 
                    x.Collate_Date.ToString().Contains(search) || 
                    x.Collate_Status.ToString().Contains(search) || 
                    x.Collate_No.Contains(search) || 
                    x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public Bms_Bill_Ap_DtlQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                    x.Id.ToString().Contains(search) || 
                    x.Bms_Bill_Ap_Id.ToString().Contains(search) || 
                    x.Dzbh.Contains(search) || 
                    x.Line_No.ToString().Contains(search) || 
                    x.Line_Id.ToString().Contains(search) || 
                    x.Charge_Code.Contains(search) || 
                    x.Charge_Desc.Contains(search) || 
                    x.Unitprice.ToString().Contains(search) || 
                    x.Unitprice2.ToString().Contains(search) || 
                    x.Qty.ToString().Contains(search) || 
                    x.Account.ToString().Contains(search) || 
                    x.Account2.ToString().Contains(search) || 
                    x.Money_Code.Contains(search) || 
                    x.Summary.Contains(search) || 
                    x.Invoice_No.Contains(search) || 
                    x.Create_Status.ToString().Contains(search) || 
                    x.Invoice_Status.ToString().Contains(search) || 
                    x.Collate_Id.Contains(search) || 
                    x.Collate_Name.Contains(search) || 
                    x.Collate_Date.ToString().Contains(search) || 
                    x.Collate_Status.ToString().Contains(search) || 
                    x.Collate_No.Contains(search) || 
                    x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public Bms_Bill_Ap_DtlQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					//if (rule.field == "OBms_Bill_Ap" && !string.IsNullOrEmpty(rule.value))
					//{	
					//    And(x => x.OBms_Bill_Ap == rule.value);
					//}
					if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Id == val);
                    }
					if (rule.field == "Bms_Bill_Ap_Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Bms_Bill_Ap_Id == val);
					}
					if (rule.field == "Dzbh" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Dzbh.StartsWith(rule.value));
					}
                    if (rule.field == "Ops_M_OrdId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Ops_M_OrdId == val);
                    }
                    if (rule.field == "Ops_H_OrdId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Ops_H_OrdId == val);
                    }
					if (rule.field == "Line_No" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Line_No == val);
					}
					if (rule.field == "Line_Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Line_Id == val);
					}
					if (rule.field == "Charge_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Charge_Code.StartsWith(rule.value));
					}
					if (rule.field == "Charge_Desc" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Charge_Desc.StartsWith(rule.value));
					}
					if (rule.field == "Unitprice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Unitprice == val);
					}
					if (rule.field == "Unitprice2" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Unitprice2 == val);
					}
					if (rule.field == "Qty" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Qty == val);
					}
					if (rule.field == "Account" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Account == val);
					}
					if (rule.field == "Account2" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Account2 == val);
					}
					if (rule.field == "Money_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Money_Code.StartsWith(rule.value));
					}
					if (rule.field == "Summary" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Summary.StartsWith(rule.value));
					}
					if (rule.field == "Invoice_No" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Invoice_No.StartsWith(rule.value));
					}
					if (rule.field == "Collate_Id" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Collate_Id.StartsWith(rule.value));
					}
					if (rule.field == "Collate_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Collate_Name.StartsWith(rule.value));
					}
					if (rule.field == "Collate_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Collate_Date == date);
					}
					if (rule.field == "_Collate_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Collate_Date >= date);
					}
					if (rule.field == "Collate_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Collate_Date <= date);
					}
					if (rule.field == "Collate_Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Collate_Status == val);
					}
					if (rule.field == "Collate_No" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Collate_No.StartsWith(rule.value));
					}
					if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.OperatingPoint == val);
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