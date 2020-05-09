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
   public class DocumentManagementQuery:QueryObject<DocumentManagement>
    {
        public DocumentManagementQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Operation_ID.Contains(search) || 
x.DZ_Type.Contains(search) || 
x.Doc_NO.Contains(search) || 
x.Trade_Mode.Contains(search) || 
x.Return_Print.ToString().Contains(search) || 
x.QTY.ToString().Contains(search) || 
x.BG_TT.Contains(search) || 
x.Ping_Name.Contains(search) || 
x.Return_Date.ToString().Contains(search) || 
x.Print_Date.ToString().Contains(search) || 
x.Return_Customer_Date.ToString().Contains(search) || 
x.Remark.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public DocumentManagementQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Operation_ID.Contains(search) || 
x.DZ_Type.Contains(search) || 
x.Doc_NO.Contains(search) || 
x.Trade_Mode.Contains(search) || 
x.Return_Print.ToString().Contains(search) || 
x.QTY.ToString().Contains(search) || 
x.BG_TT.Contains(search) || 
x.Ping_Name.Contains(search) || 
x.Return_Date.ToString().Contains(search) || 
x.Print_Date.ToString().Contains(search) || 
x.Return_Customer_Date.ToString().Contains(search) || 
x.Remark.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public DocumentManagementQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Operation_ID" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Operation_ID.StartsWith(rule.value));
					}
					if (rule.field == "DZ_Type" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.DZ_Type.StartsWith(rule.value));
					}
					if (rule.field == "Doc_NO" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Doc_NO.Contains(rule.value));
					}
					if (rule.field == "Trade_Mode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Trade_Mode.StartsWith(rule.value));
					}
					if (rule.field == "Return_Print" && !string.IsNullOrEmpty(rule.value))
					{
                         And(x => x.Return_Print == rule.value);
					}
					if (rule.field == "QTY" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.QTY == val);
					}
					if (rule.field == "BG_TT" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.BG_TT.StartsWith(rule.value));
					}
					if (rule.field == "Ping_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Ping_Name.StartsWith(rule.value));
					}
					if (rule.field == "Return_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Return_Date == date);
					}
					if (rule.field == "_Return_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Return_Date >= date);
					}
					if (rule.field == "Return_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Return_Date <= date);
					}
					if (rule.field == "Print_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Print_Date == date);
					}
					if (rule.field == "_Print_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Print_Date >= date);
					}
					if (rule.field == "Print_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Print_Date <= date);
					}

					if (rule.field == "Return_Customer_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Return_Customer_Date == date);
					}
					if (rule.field == "_Return_Customer_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Return_Customer_Date >= date);
					}
					if (rule.field == "Return_Customer_Date_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Return_Customer_Date <= date);
					}
                    if (rule.field == "Is_Return_Customer" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_Return_Customer == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_Return_Customer == boolval || x.Is_Return_Customer == null));
                        }
                    }
                    if (rule.field == "Is_Return" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_Return == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_Return == boolval || x.Is_Return == null));
                        }
                    }
                    if (rule.field == "Entrustment_Code" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Entrustment_Code.StartsWith(rule.value));
                    }
                    if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.MBL.StartsWith(rule.value));
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
                    if (rule.field == "SignReceipt_Code" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SignReceipt_Code.StartsWith(rule.value));
                    }
                    if (rule.field == "Is_Print" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        if (boolval == true)
                        {
                            And(x => x.Is_Print == boolval);
                        }
                        else
                        {
                            And(x => (x.Is_Print == boolval || x.Is_Print == null));
                        }
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
					//if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
					//{
					//	 And(x => x.ADDID.StartsWith(rule.value));
					//}
					if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.ADDWHO.StartsWith(rule.value));
					}
					if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ADDTS == date);
					}
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
					if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EDITWHO.StartsWith(rule.value));
					}
					if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EDITTS == date);
					}
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