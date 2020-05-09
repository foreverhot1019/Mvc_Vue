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
   public class PARA_PackageQuery:QueryObject<PARA_Package>
    {
        public PARA_PackageQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.PackageCode.Contains(search) || 
x.PackageName.Contains(search) || 
x.Description.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_PackageQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.PackageCode.Contains(search) || 
x.PackageName.Contains(search) || 
x.Description.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PARA_PackageQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "PackageCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.PackageCode.StartsWith(rule.value));
					}
					if (rule.field == "PackageName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.PackageName.StartsWith(rule.value));
					}
					if (rule.field == "IsWood" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.IsWood == boolval);
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