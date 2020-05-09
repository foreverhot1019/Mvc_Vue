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
  public static class MessageRepository  
    {
 
                 public static IEnumerable<Message> GetByNotificationId(this IRepositoryAsync<Message> repository, int notificationid)
         {
             var query= repository
                .Queryable()
                .Where(x => x.NotificationId==notificationid);
             return query;

         }
             
        
         
	}
}



