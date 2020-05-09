

     
 
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using TMI.Web.Models;
using TMI.Web.Repositories;
using System.Data;

namespace TMI.Web.Services
{
    public interface INotificationService:IService<Notification>
    {

         
                 IEnumerable<Message>   GetMessagesByNotificationId (int notificationid);
         
         
 
		void ImportDataTable(DataTable datatable);
	}
}