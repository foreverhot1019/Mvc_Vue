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
    public interface IMessageService : IService<Message>
    {
        IEnumerable<Message> GetByNotificationId(int notificationid);

        void ImportDataTable(DataTable datatable);

        void Append(string name, string subject, string key, string content, TMI.Web.Models.EnumType.MessageType messageType);

        void Append(TMI.Web.Models.EnumType.NotificationTag notificationTag, string subject, string key1, string key2, string content, TMI.Web.Models.EnumType.MessageType messageType);
    }
}