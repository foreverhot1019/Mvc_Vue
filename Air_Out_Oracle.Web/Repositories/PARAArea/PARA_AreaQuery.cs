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
   public class PARA_AreaQuery:QueryObject<PARA_Area>
    {
        public PARA_AreaQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                x.ID.ToString().Contains(search) || 
                x.AreaCode.Contains(search) || 
                x.AreaName.Contains(search) || 
                x.ADDWHO.Contains(search) || 
                x.ADDTS.ToString().Contains(search) || 
                x.EDITWHO.Contains(search) || 
                x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_AreaQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                x.ID.ToString().Contains(search) || 
                x.AreaCode.Contains(search) || 
                x.AreaName.Contains(search) || 
                x.ADDWHO.Contains(search) || 
                x.ADDTS.ToString().Contains(search) || 
                x.EDITWHO.Contains(search) || 
                x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_AreaQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "AreaCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AreaCode.StartsWith(rule.value));
					}
					if (rule.field == "AreaName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AreaName.StartsWith(rule.value));
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