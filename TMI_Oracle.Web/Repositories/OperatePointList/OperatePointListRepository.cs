﻿





















                    

      
    
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;

using TMI.Web.Models;

namespace TMI.Web.Repositories
{
  public static class OperatePointListRepository  
    {
 
        
         public static IEnumerable<OperatePointList> GetByOperatePointID(this IRepositoryAsync<OperatePointList> repository, int operatepointid)
         {
             var query= repository
                .Queryable()
                .Where(x => x.OperatePointID==operatepointid);
             return query;

         }
             
        

         
	}
}



