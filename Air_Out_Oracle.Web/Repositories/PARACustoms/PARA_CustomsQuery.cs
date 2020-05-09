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
   public class PARA_CustomsQuery:QueryObject<PARA_Customs>
    {
        public PARA_CustomsQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Customs_Code.Contains(search) || 
x.Customs_Name.Contains(search) || 
x.PinYinSimpleName.Contains(search) || 
x.Description.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_CustomsQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Customs_Code.Contains(search) || 
x.Customs_Name.Contains(search) || 
x.PinYinSimpleName.Contains(search) || 
x.Description.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_CustomsQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					if (rule.field == "Customs_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Customs_Code.StartsWith(rule.value));
					}
					if (rule.field == "Customs_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Customs_Name.StartsWith(rule.value));
					}
					if (rule.field == "PinYinSimpleName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.PinYinSimpleName.StartsWith(rule.value));
					}
					if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Description.StartsWith(rule.value));
					}
					if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Status == boolval);
					}
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
				}
			}
            return this;
		}
	}
}