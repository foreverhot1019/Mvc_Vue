
                    
      
     
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
   public class CompanyQuery:QueryObject<Company>
    {
        public CompanyQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.Id.ToString().Contains(search) || x.Name.Contains(search) || x.Address.Contains(search) || x.City.Contains(search) || x.Province.Contains(search) || x.RegisterDate.ToString().Contains(search) || x.Logo.Contains(search) );
            return this;
        }

		public CompanyQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  x.Id.ToString().Contains(search) || x.Name.Contains(search) || x.Address.Contains(search) || x.City.Contains(search) || x.Province.Contains(search) || x.RegisterDate.ToString().Contains(search) || x.Logo.Contains(search) );
            return this;
        }

		public CompanyQuery Withfilter(IEnumerable<filterRule> filters)
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
				    
				    
					
					
				    				
											if (rule.field == "Address"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Address.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "City"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.City.Contains(rule.value));
						}
				    
				    
					
					
				    				
											if (rule.field == "Province"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Province.Contains(rule.value));
						}
				    
				    
					
					
				    				
					
				    
					
											if (rule.field == "RegisterDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
						{	
							var date = Convert.ToDateTime(rule.value) ;
							And(x => SqlFunctions.DateDiff("d", date, x.RegisterDate)>=0);
						}
				   
				    				
											if (rule.field == "Logo"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Logo.Contains(rule.value));
						}
				    
				    
					
					
				    									
                   
               }
           }
            return this;
        }
    }
}



