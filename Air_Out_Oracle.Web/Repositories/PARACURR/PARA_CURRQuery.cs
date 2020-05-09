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
   public class PARA_CURRQuery:QueryObject<PARA_CURR>
    {
        public PARA_CURRQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.ID.ToString().Contains(search) || 
                     x.CURR_CODE.Contains(search) || 
                     x.CURR_Name.Contains(search) || 
                     x.CURR_NameEng.Contains(search) || 
                     x.Money_CODE.Contains(search) || 
                     x.Description.Contains(search) || 
                     x.ADDWHO.Contains(search) || 
                     x.ADDTS.ToString().Contains(search) || 
                     x.EDITWHO.Contains(search) || 
                     x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_CURRQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.ID.ToString().Contains(search) || 
                     x.CURR_CODE.Contains(search) || 
                     x.CURR_Name.Contains(search) || 
                     x.CURR_NameEng.Contains(search) || 
                     x.Money_CODE.Contains(search) || 
                     x.Description.Contains(search) || 
                     x.ADDWHO.Contains(search) || 
                     x.ADDTS.ToString().Contains(search) || 
                     x.EDITWHO.Contains(search) || 
                     x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_CURRQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					if (rule.field == "ID" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.ID == val);
					}
					if (rule.field == "CURR_CODE" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CURR_CODE.StartsWith(rule.value));
					}
					if (rule.field == "CURR_Name" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CURR_Name.StartsWith(rule.value));
					}
					if (rule.field == "CURR_NameEng" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CURR_NameEng.StartsWith(rule.value));
					}
					if (rule.field == "Money_CODE" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Money_CODE.StartsWith(rule.value));
					}
					if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Description.StartsWith(rule.value));
					}
					if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusEnum>(rule.value);
						 //var boolval=Convert.ToBoolean(rule.value);
                         And(x => x.Status == EnumVal);
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