
                    
      
     
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
   public class UserOperatePointLinkQuery:QueryObject<UserOperatePointLink>
    {
        public UserOperatePointLinkQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.ID.ToString().Contains(search) || x.UserId.Contains(search) || x.OperateOpintId.ToString().Contains(search) );
            return this;
        }

		public UserOperatePointLinkQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.ID.ToString().Contains(search) || x.UserId.Contains(search) || x.OperateOpintId.ToString().Contains(search) );
            return this;
        }

		public UserOperatePointLinkQuery Withfilter(IEnumerable<filterRule> filters)
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
				    
					
					
				    				
											if (rule.field == "UserId"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.UserId.Contains(rule.value));
						}
				    
				    
					
					
				    				
					
				    						if (rule.field == "OperateOpintId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							int val = Convert.ToInt32(rule.value);
							And(x => x.OperateOpintId == val);
						}
				    
					
					
				    									
                   
               }
           }
            return this;
        }
    }
}



