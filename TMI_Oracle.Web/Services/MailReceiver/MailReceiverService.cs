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
using System.Reflection;

using Newtonsoft.Json;
using TMI.Web.Extensions;
using System.IO;

namespace TMI.Web.Services
{
    public class MailReceiverService : Service<MailReceiver>, IMailReceiverService
    {
        private readonly IRepositoryAsync<MailReceiver> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public MailReceiverService(IRepositoryAsync<MailReceiver> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }


        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "MailReceiver").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                MailReceiver item = new MailReceiver();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type mailreceivertype = item.GetType();
                        PropertyInfo propertyInfo = mailreceivertype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type mailreceivertype = item.GetType();
                        PropertyInfo propertyInfo = mailreceivertype.GetProperty(field.FieldName);
                        if (defval.ToLower() == "now" && propertyInfo.PropertyType == typeof(DateTime))
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(DateTime.Now, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(defval, propertyInfo.PropertyType), null);
                        }
                    }
                }
                this.Insert(item);
            }
        }

        public Stream ExportExcel(string filterRules = "", string sort = "ID", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var mailreceiver = this.Query(new MailReceiverQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = mailreceiver.Select(n => new { ID = n.ID, ErrType = n.ErrType, ErrMethod = n.ErrMethod, RecMailAddress = n.RecMailAddress, CCMailAddress = n.CCMailAddress, ADDID = n.ADDID, ADDWHO = n.ADDWHO, ADDTS = n.ADDTS, EDITWHO = n.EDITWHO, EDITTS = n.EDITTS, EDITID = n.EDITID }).ToList();

            return ExcelHelper.ExportExcel(typeof(MailReceiver), datarows);
        }
    }
}