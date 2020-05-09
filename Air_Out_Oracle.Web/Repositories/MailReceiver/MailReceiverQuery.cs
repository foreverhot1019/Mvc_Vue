
                    
      
     
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
   public class MailReceiverQuery:QueryObject<MailReceiver>
    {
        public MailReceiverQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.ID.ToString().Contains(search) || x.ErrType.Contains(search) || x.ErrMethod.Contains(search) || x.RecMailAddress.Contains(search) || x.CCMailAddress.Contains(search) || x.ADDID.Contains(search) || x.ADDWHO.Contains(search) || x.ADDTS.ToString().Contains(search) || x.EDITWHO.Contains(search) || x.EDITTS.ToString().Contains(search) || x.EDITID.Contains(search) );
            return this;
        }

		public MailReceiverQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.ID.ToString().Contains(search) || x.ErrType.Contains(search) || x.ErrMethod.Contains(search) || x.RecMailAddress.Contains(search) || x.CCMailAddress.Contains(search) || x.ADDID.Contains(search) || x.ADDWHO.Contains(search) || x.ADDTS.ToString().Contains(search) || x.EDITWHO.Contains(search) || x.EDITTS.ToString().Contains(search) || x.EDITID.Contains(search) );
            return this;
        }

		public MailReceiverQuery Withfilter(IEnumerable<filterRule> filters)
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
				    
					
					
				    				
											if (rule.field == "ErrType"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.ErrType.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "ErrMethod"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.ErrMethod.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "RecMailAddress"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.RecMailAddress.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "CCMailAddress"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CCMailAddress.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "ADDID"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.ADDID.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "ADDWHO"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.ADDWHO.Contains(rule.value));
						}
				    
				    
					
					
				    				
					
				    
					
											if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
						{	
							var date = Convert.ToDateTime(rule.value) ;
							And(x => SqlFunctions.DateDiff("d", date, x.ADDTS)>=0);
						}
				   
				    				
											if (rule.field == "EDITWHO"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.EDITWHO.Contains(rule.value));
						}
				    
				    
					
					
				    				
					
				    
					
											if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
						{	
							var date = Convert.ToDateTime(rule.value) ;
							And(x => SqlFunctions.DateDiff("d", date, x.EDITTS)>=0);
						}
				   
				    				
											if (rule.field == "EDITID"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.EDITID.Contains(rule.value));
						}
				    
				    
					
					
				    									
                   
               }
           }
            return this;
        }
    }
}



