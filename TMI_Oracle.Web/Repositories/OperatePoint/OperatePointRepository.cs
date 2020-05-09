





















                    

      
    
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;

using TMI.Web.Models;

namespace TMI.Web.Repositories
{
  public static class OperatePointRepository  
    {
 
        

        
        public static IEnumerable<OperatePointList>   GetOperatePointListsByOperatePointID (this IRepositoryAsync<OperatePoint> repository,int operatepointid)
        {
			var operatepointlistRepository = repository.GetRepository<OperatePointList>(); 
            return operatepointlistRepository.Queryable().Include(x => x.OperatePoint).Where(n => n.OperatePointID == operatepointid);
        }
         
	}
}



