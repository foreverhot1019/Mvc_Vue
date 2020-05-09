
                    
      
    
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;

using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
  public static class NotificationRepository  
    {
 
        
                public static IEnumerable<Message>   GetMessagesByNotificationId (this IRepositoryAsync<Notification> repository,int notificationid)
        {
			var messageRepository = repository.GetRepository<Message>(); 
            return messageRepository.Queryable().Include(x => x.Notification).Where(n => n.NotificationId == notificationid);
        }
         
	}
}



