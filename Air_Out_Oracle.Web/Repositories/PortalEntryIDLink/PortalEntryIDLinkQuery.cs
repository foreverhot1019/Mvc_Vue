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
   public class PortalEntryIDLinkQuery:QueryObject<PortalEntryIDLink>
    {
        public PortalEntryIDLinkQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.ID.ToString().Contains(search) || 
x.UserId.Contains(search) || 
x.DepartMent.Contains(search) || 
x.EntryID.Contains(search) );
            return this;
        }

		public PortalEntryIDLinkQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.ID.ToString().Contains(search) || 
x.UserId.Contains(search) || 
x.DepartMent.Contains(search) || 
x.EntryID.Contains(search) );
            return this;
        }

		public PortalEntryIDLinkQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "UserId" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.UserId.StartsWith(rule.value));
					}
					if (rule.field == "DepartMent" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.DepartMent.StartsWith(rule.value));
					}
					if (rule.field == "EntryID" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EntryID.StartsWith(rule.value));
					}
				}
			}
            return this;
		}
	}
}