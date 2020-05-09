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
   public class Bms_Bill_ApQuery:QueryObject<Bms_Bill_Ap>
    {
        public Bms_Bill_ApQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                    x.Id.ToString().Contains(search) || 
                    x.Dzbh.Contains(search) || 
                    x.Line_No.ToString().Contains(search) || 
                    x.Bill_Account.ToString().Contains(search) || 
                    x.Bill_Account2.ToString().Contains(search) || 
                    x.Money_Code.Contains(search) || 
                    x.Payway.Contains(search) || 
                    x.Bill_Object_Id.Contains(search) || 
                    x.Bill_Type.Contains(search) || 
                    x.Bill_Date.ToString().Contains(search) || 
                    x.Summary.Contains(search) || 
                    x.Remark.Contains(search) || 
                    x.Create_Status.ToString().Contains(search) || 
                    x.Bill_Object_Name.Contains(search) || 
                    x.Org_Money_Code.Contains(search) || 
                    x.Org_Bill_Account2.ToString().Contains(search) || 
                    x.Nowent_Acc.ToString().Contains(search) || 
                    x.Dsdf_Status.ToString().Contains(search) || 
                    x.AuditId.Contains(search) || 
                    x.AuditName.Contains(search) || 
                    x.AuditDate.ToString().Contains(search) || 
                    x.Cancel_Id.Contains(search) || 
                    x.Cancel_Name.Contains(search) || 
                    x.Cancel_Date.ToString().Contains(search) || 
                    x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public Bms_Bill_ApQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                    x.Id.ToString().Contains(search) || 
                    x.Dzbh.Contains(search) || 
                    x.Line_No.ToString().Contains(search) || 
                    x.Bill_Account.ToString().Contains(search) || 
                    x.Bill_Account2.ToString().Contains(search) || 
                    x.Money_Code.Contains(search) || 
                    x.Payway.Contains(search) || 
                    x.Bill_Object_Id.Contains(search) || 
                    x.Bill_Type.Contains(search) || 
                    x.Bill_Date.ToString().Contains(search) || 
                    x.Summary.Contains(search) || 
                    x.Remark.Contains(search) || 
                    x.Create_Status.ToString().Contains(search) || 
                    x.Bill_Object_Name.Contains(search) || 
                    x.Org_Money_Code.Contains(search) || 
                    x.Org_Bill_Account2.ToString().Contains(search) || 
                    x.Nowent_Acc.ToString().Contains(search) || 
                    x.Dsdf_Status.ToString().Contains(search) || 
                    x.AuditId.Contains(search) || 
                    x.AuditName.Contains(search) || 
                    x.AuditDate.ToString().Contains(search) || 
                    x.Cancel_Id.Contains(search) || 
                    x.Cancel_Name.Contains(search) || 
                    x.Cancel_Date.ToString().Contains(search) || 
                    x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public Bms_Bill_ApQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Dzbh" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Dzbh.StartsWith(rule.value));
					}
					if (rule.field == "Line_No" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Line_No == val);
					}
					if (rule.field == "Bill_Account" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bill_Account == val);
					}
					if (rule.field == "Bill_Account2" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Bill_Account2 == val);
					}
					if (rule.field == "Money_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Money_Code.StartsWith(rule.value));
					}
					if (rule.field == "Payway" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Payway.StartsWith(rule.value));
					}
					if (rule.field == "Bill_Object_Id" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Bill_Object_Id.StartsWith(rule.value));
					}
					if (rule.field == "Bill_Type" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Bill_Type.StartsWith(rule.value));
					}
					if (rule.field == "Bill_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Bill_Date == date);
					}
					if (rule.field == "_Bill_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Bill_Date >= date);
					}
					if (rule.field == "Bill_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Bill_Date <= date);
					}
					if (rule.field == "Summary" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Summary.StartsWith(rule.value));
					}
					if (rule.field == "Remark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Remark.StartsWith(rule.value));
					}
					if (rule.field == "Create_Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
                         And(x => x.Create_Status == (AirOutEnumType.Bms_BillCreate_Status)Enum.Parse(typeof(AirOutEnumType.Bms_BillCreate_Status), rule.value));
					}
					if (rule.field == "Bill_Object_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Bill_Object_Name.StartsWith(rule.value));
					}
					if (rule.field == "Org_Money_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Org_Money_Code.StartsWith(rule.value));
					}
					if (rule.field == "Org_Bill_Account2" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Org_Bill_Account2 == val);
					}
					if (rule.field == "Nowent_Acc" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Nowent_Acc == val);
					}
					if (rule.field == "Dsdf_Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Dsdf_Status == val);
					}
					if (rule.field == "AuditId" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AuditId.StartsWith(rule.value));
					}
					if (rule.field == "AuditName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AuditName.StartsWith(rule.value));
					}
					if (rule.field == "AuditDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.AuditDate == date);
					}
					if (rule.field == "_AuditDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.AuditDate >= date);
					}
					if (rule.field == "AuditDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.AuditDate <= date);
					}
					if (rule.field == "Cancel_Id" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Cancel_Id.StartsWith(rule.value));
					}
					if (rule.field == "Cancel_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Cancel_Name.StartsWith(rule.value));
					}
					if (rule.field == "Cancel_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Cancel_Date == date);
					}
					if (rule.field == "_Cancel_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Cancel_Date >= date);
					}
					if (rule.field == "Cancel_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Cancel_Date <= date);
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