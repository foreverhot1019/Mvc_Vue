using AirOut.Web.App_Start;
using AirOut.Web.Models;
using AirOut.Web.Services;
using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirOut.Web
{
    internal static class WebRequestContext
    {
        internal static WebdbContext Current
        {
            get
            {
                if (!HttpContext.Current.Items.Contains("WebdbContext"))
                {
                    WebdbContext db = new WebdbContext();
                    db.Database.Initialize(true);
                    HttpContext.Current.Items.Add("WebdbContext", db);
                }
                return HttpContext.Current.Items["WebdbContext"] as WebdbContext;
            }
        }
    }

    public static class EventLogHelper
    {
        public static void RawWrite(NotificationTag notificationTag, string subject, string key1, string key2, string content, MessageType messageType)
        {
            var db = new WebdbContext();
            var NofQ = db.Notification.Where(x => x.Name == notificationTag.ToString()).Select(x => x.Id).Take(1);
            Message msg = new Message();
            msg.Subject = subject;
            msg.Content = content.Replace("'", "");
            msg.Type = messageType.ToString();
            msg.NewDate = DateTime.Now;
            msg.NotificationId = NofQ.Any() ? NofQ.FirstOrDefault() : 0;
            msg.CreatedDate = DateTime.Now;
            msg.Key1 = key1;
            msg.Key2 = key2;
            msg.IsSended = false;
            msg.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
            db.Message.Add(msg);
            db.SaveChanges();

//            using (var db = new WebdbContext().Database.Connection)
//            {
//                db.Open();
//                var cmd = db.CreateCommand();
//                string sql = string.Format("SELECT ID FROM [dbo].[NOTIFICATIONS] WHERE [NAME]=N'{0}' ", notificationTag.ToString());
//                cmd.CommandText = sql;
//                object result = cmd.ExecuteScalar();
//                if (result != null && DBNull.Value != result)
//                {
//                    int id = Convert.ToInt32(result);
//                    sql = string.Format(@"INSERT INTO [dbo].[Messages]
//                        ([Subject]
//                        ,[Content]
//                        ,[Type]
//                        ,[NewDate]
//                        ,[NotificationId]
//                        ,[CreatedDate]
//                        ,[Key1]
//                        ,[Key2]
//                        ,[IsSended])
//                    VALUES
//                        (N'{0}'
//                        ,N'{1}'
//                        ,N'{2}'
//                        ,N'{3}'
//                        ,{4}
//                        ,'{5}'
//                        ,N'{6}'
//                        ,N'{7}',0)", subject,
//                         content.Replace("'", ""),
//                         messageType.ToString(),
//                         DateTime.Now.ToString("yyyy/MM-dd HH:mm:ss"),
//                         id,
//                          DateTime.Now.ToString("yyyy/MM-dd HH:mm:ss"),
//                          key1,
//                          key2
//                         );

//                    cmd.CommandText = sql;
//                    cmd.ExecuteNonQuery();
//                }
//                db.Close();
//            }
        }

        public static void Write(NotificationTag notificationTag, string subject, string key1, string key2, string content, MessageType messageType)
        {
            var unitofWork = UnityConfig.GetConfiguredContainer().Resolve(typeof(UnitOfWork), "UnitOfWork") as UnitOfWork;
            var messageService = UnityConfig.GetConfiguredContainer().Resolve(typeof(MessageService), "MessageService") as MessageService;
            messageService.Append(notificationTag, subject, key1, key2, content, messageType);
            unitofWork.SaveChanges();
            //_unitOfWork.SaveChanges();

            //using (WebdbContext db = UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext)
            //{

            //    string notificationName = notificationTag.ToString();


            //    var notification = db.Notifications.Where(x => x.Name == notificationName).FirstOrDefault();
            //    if (notification == null)
            //    {
            //        notification = new Notification();
            //        notification.Name = notificationName;
            //        notification.Type = NotificationType.Email.ToString();
            //        notification.Receiver = "test@test.com";
            //        notification.Sender = "test@test.com";
            //        notification.AuthUser = "";
            //        notification.AuthPassword = "";
            //        notification.Host = "";
            //        Message message = new Message();
            //        message.Content = content;
            //        message.Key1 = key1;
            //        message.Key2 = key2;
            //        message.CreatedDate = DateTime.Now;
            //        message.NewDate = DateTime.Now;
            //        message.NotificationId = notification.Id;
            //        message.Subject = subject;
            //        message.Type = messageType.ToString();
            //        notification.Messages.Add(message);
            //        db.Notifications.Add(notification);
            //    }
            //    else
            //    {
            //        Message message = new Message();
            //        message.Content = content;
            //        message.Key1 = key1;
            //        message.Key2 = key2;
            //        message.CreatedDate = DateTime.Now;
            //        message.NewDate = DateTime.Now;
            //        message.NotificationId = notification.Id;
            //        message.Subject = subject;
            //        message.Type = messageType.ToString();
            //        //notification.Messages.Add(message);
            //        db.Messages.Add(message);
            //    }
            //    db.SaveChanges();
            //}
        }
    }
}