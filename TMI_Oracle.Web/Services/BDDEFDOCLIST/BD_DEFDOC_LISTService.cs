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
    public class BD_DEFDOC_LISTService : Service<BD_DEFDOC_LIST>, IBD_DEFDOC_LISTService
    {
        private readonly IRepositoryAsync<BD_DEFDOC_LIST> _repository;
        private readonly IDataTableImportMappingService _mappingservice;

        public BD_DEFDOC_LISTService(IRepositoryAsync<BD_DEFDOC_LIST> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<BD_DEFDOC_LIST> GetByDOCID(int docid)
        {
            return _repository.GetByDOCID(docid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "BD_DEFDOC_LIST").ToList();

            foreach (DataRow row in datatable.Rows)
            {
                BD_DEFDOC_LIST item = new BD_DEFDOC_LIST();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type bd_defdoc_listtype = item.GetType();
                        PropertyInfo propertyInfo = bd_defdoc_listtype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type bd_defdoc_listtype = item.GetType();
                        PropertyInfo propertyInfo = bd_defdoc_listtype.GetProperty(field.FieldName);
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

            var bd_defdoc_list = this.Query(new BD_DEFDOC_LISTQuery().Withfilter(filters)).Include(p => p.BD_DEFDOC).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();

            var datarows = bd_defdoc_list.Select(n => new
            {
                BD_DEFDOCDOCCODE = (n.BD_DEFDOC == null ? "" : n.BD_DEFDOC.DOCCODE),
                ROWID = n.ID,
                DOCID = n.DOCID,
                LISTCODE = n.LISTCODE,
                LISTNAME = n.LISTNAME,
                ListFullName = n.ListFullName,
                REMARK = n.REMARK,
                STATUS = n.STATUS,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS == null ? "" : n.ADDTS.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS == null ? "" : n.EDITTS.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                R_CODE = n.R_CODE,
                ENAME = n.ENAME
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(BD_DEFDOC_LIST), datarows);

        }
    }
}



