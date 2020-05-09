
                    
      
     
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
   public class NotificationQuery:QueryObject<Notification>
    {
        public NotificationQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.Id.ToString().Contains(search) || x.Name.Contains(search) || x.Description.Contains(search) || x.Type.Contains(search) || x.Sender.Contains(search) || x.Receiver.Contains(search) || x.Schedule.Contains(search) || x.AuthUser.Contains(search) || x.AuthPassword.Contains(search) || x.Host.Contains(search) );
            return this;
        }

		public NotificationQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.Id.ToString().Contains(search) || x.Name.Contains(search) || x.Description.Contains(search) || x.Type.Contains(search) || x.Sender.Contains(search) || x.Receiver.Contains(search) || x.Schedule.Contains(search) || x.AuthUser.Contains(search) || x.AuthPassword.Contains(search) || x.Host.Contains(search) );
            return this;
        }

		public NotificationQuery Withfilter(IEnumerable<filterRule> filters)
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
				    
					
					
				    				
											if (rule.field == "Name"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Name.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Description"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Description.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Type"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Type.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Sender"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Sender.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Receiver"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Receiver.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Schedule"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Schedule.Contains(rule.value));
						}
				    
				    
					
					
				    				
					
				    
					
					
				    						if (rule.field == "Disabled" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
						{	
							 var boolval=Convert.ToBoolean(rule.value);
							 And(x => x.Disabled == boolval);
						}
				   				
											if (rule.field == "AuthUser"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.AuthUser.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "AuthPassword"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.AuthPassword.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Host"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Host.Contains(rule.value));
						}
				    
				    
					
					
				    									
                   
               }
           }
            return this;
        }
    }
}



