

     
 
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using AirOut.Web.Models;
using AirOut.Web.Repositories;
using System.Data;

namespace AirOut.Web.Services
{
    public interface IMessageService:IService<Message>
    {

                  IEnumerable<Message> GetByNotificationId(int  notificationid);
        
         
 
		void ImportDataTable(DataTable datatable);

        void Append(string name, string subject,string key, string content, MessageType messageType);
        void Append(NotificationTag notificationTag, string subject, string key1, string key2, string content, MessageType messageType);
	}
}