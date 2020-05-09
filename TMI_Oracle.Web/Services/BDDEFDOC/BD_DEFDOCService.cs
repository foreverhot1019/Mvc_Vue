using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Data;
using System.Reflection;

using Repository.Pattern.Repositories;
using Service.Pattern;
using TMI.Web.Models;
using TMI.Web.Repositories;
using Newtonsoft.Json;
using TMI.Web.Extensions;

namespace TMI.Web.Services
{
    public class BD_DEFDOCService : Service<BD_DEFDOC>, IBD_DEFDOCService
    {
        private readonly IRepositoryAsync<BD_DEFDOC> _repository;
        private readonly IDataTableImportMappingService _mappingservice;

        public BD_DEFDOCService(IRepositoryAsync<BD_DEFDOC> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<BD_DEFDOC_LIST> GetBD_DEFDOC_LISTByDOCID(int docid)
        {
            return _repository.GetBD_DEFDOC_LISTByDOCID(docid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "BD_DEFDOC").ToList();

            foreach (DataRow row in datatable.Rows)
            {
                BD_DEFDOC item = new BD_DEFDOC();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type bd_defdoctype = item.GetType();
                        PropertyInfo propertyInfo = bd_defdoctype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type bd_defdoctype = item.GetType();
                        PropertyInfo propertyInfo = bd_defdoctype.GetProperty(field.FieldName);
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

        public Stream ExportExcel(string filterRules = "", string sort = "ROWID", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var bd_defdoc = this.Query(new BD_DEFDOCQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = bd_defdoc.Select(n => new
            {
                ROWID = n.ID,
                DOCCODE = n.DOCCODE,
                DOCNAME = n.DOCNAME,
                REMARK = n.REMARK,
                STATUS = n.STATUS,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS == null ? "" : n.ADDTS.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS == null ? "" : n.EDITTS.Value.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(BD_DEFDOC), datarows);

        }
    }
}



