




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
using System.Reflection;
namespace AirOut.Web.Services
{
    public class MessageService : Service<Message>, IMessageService
    {

        private readonly IRepositoryAsync<Message> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        private readonly INotificationService _notifyService;
        public MessageService(IRepositoryAsync<Message> repository, IDataTableImportMappingService mappingservice, INotificationService _notifyService)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
            this._notifyService = _notifyService;
        }

        public IEnumerable<Message> GetByNotificationId(int notificationid)
        {
            return _repository.GetByNotificationId(notificationid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                Message item = new Message();
                var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Message").ToList();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type messagetype = item.GetType();
                        PropertyInfo propertyInfo = messagetype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type messagetype = item.GetType();
                        PropertyInfo propertyInfo = messagetype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(defval, propertyInfo.PropertyType), null);
                    }
                }
                this.Insert(item);
            }
        }

        public void Append(string name, string subject, string key, string content, MessageType messageType)
        {
            var notification = this._repository.GetRepository<Notification>().Queryable().Where(x => x.Name == name).FirstOrDefault();
            if (notification != null)
            {
                Message message = new Message();
                message.Content = content;
                message.Key1 = key;
                message.CreatedDate = DateTime.Now;
                message.NewDate = DateTime.Now;
                message.NotificationId = notification.Id;
                message.Subject = subject;
                message.Type = messageType.ToString();
                this.Insert(message);
            }
        }

        public void Append(NotificationTag notificationTag, string subject, string key1, string key2, string content, MessageType messageType)
        {
            string name = notificationTag.ToString();
            var notification = this._notifyService.Queryable().Where(x => x.Name == name).FirstOrDefault();
            if (notification != null)
            {
                Message message = new Message();
                message.Content = content;
                message.Key1 = key1;
                message.Key2 = key2;
                message.CreatedDate = DateTime.Now;
                message.NewDate = DateTime.Now;
                message.NotificationId = notification.Id;
                message.Subject = subject;
                message.Type = messageType.ToString();
                this.Insert(message);
            }
        }
    }
}



