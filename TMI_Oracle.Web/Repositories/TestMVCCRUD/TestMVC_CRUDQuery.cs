using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using System.Web.WebPages;
using TMI.Web.Models;
using TMI.Web.Extensions;

namespace TMI.Web.Repositories
{
   public class TestMVC_CRUDQuery:QueryObject<TestMVC_CRUD>
    {
        public TestMVC_CRUDQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Dzbh.Contains(search) || 
x.TestColumn1.Contains(search) || 
x.TestColumn2.Contains(search) || 
x.TestColumn3.Contains(search) || 
x.TestColumn4.Contains(search) || 
x.TestColumn5.Contains(search) );
            return this;
        }

		public TestMVC_CRUDQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Dzbh.Contains(search) || 
x.TestColumn1.Contains(search) || 
x.TestColumn2.Contains(search) || 
x.TestColumn3.Contains(search) || 
x.TestColumn4.Contains(search) || 
x.TestColumn5.Contains(search) );
            return this;
        }

		public TestMVC_CRUDQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Dzbh" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Dzbh.StartsWith(rule.value));
					}
					if (rule.field == "TestColumn1" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TestColumn1.StartsWith(rule.value));
					}
					if (rule.field == "TestColumn2" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TestColumn2.StartsWith(rule.value));
					}
					if (rule.field == "TestColumn3" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TestColumn3.StartsWith(rule.value));
					}
					if (rule.field == "TestColumn4" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TestColumn4.StartsWith(rule.value));
					}
					if (rule.field == "TestColumn5" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TestColumn5.StartsWith(rule.value));
					}
					//if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					//{
					//	 int val = Convert.ToInt32(rule.value);
					//	 And(x => x.OperatingPoint == val);
					//}
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