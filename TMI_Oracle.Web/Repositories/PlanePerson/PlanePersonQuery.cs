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
   public class PlanePersonQuery:QueryObject<PlanePerson>
    {
        public PlanePersonQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.AirTicketOrderId.ToString().Contains(search) || 
x.NameChs.Contains(search) || 
x.LastNameEng.Contains(search) || 
x.FirstNameEng.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public PlanePersonQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.AirTicketOrderId.ToString().Contains(search) || 
x.NameChs.Contains(search) || 
x.LastNameEng.Contains(search) || 
x.FirstNameEng.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public PlanePersonQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					//if (rule.field == "OAirTicket" && !string.IsNullOrEmpty(rule.value))
					//{	
					//    And(x => x.OAirTicket == rule.value);
					//}
					if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Id == val);
					}
					if (rule.field == "AirTicketOrderId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.AirTicketOrderId == val);
					}
					if (rule.field == "NameChs" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.NameChs.StartsWith(rule.value));
					}
					if (rule.field == "LastNameEng" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.LastNameEng.StartsWith(rule.value));
					}
					if (rule.field == "FirstNameEng" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FirstNameEng.StartsWith(rule.value));
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