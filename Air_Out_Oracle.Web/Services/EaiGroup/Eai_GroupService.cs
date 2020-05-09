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

using Newtonsoft.Json;
using AirOut.Web.Extensions;
using System.IO;

namespace AirOut.Web.Services
{
    public class Eai_GroupService : Service<Eai_Group>, IEai_GroupService
    {
        private readonly IRepositoryAsync<Eai_Group> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public Eai_GroupService(IRepositoryAsync<Eai_Group> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Eai_Group").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Eai_Group item = new Eai_Group();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type eai_grouptype = item.GetType();
                        PropertyInfo propertyInfo = eai_grouptype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type eai_grouptype = item.GetType();
                        PropertyInfo propertyInfo = eai_grouptype.GetProperty(field.FieldName);
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

        public Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var eai_group = this.Query(new Eai_GroupQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = eai_group.Select(n => new
            {
                Id = n.Id,
                Code = n.Code,
                Name = n.Name,
                Remark = n.Remark,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Eai_Group), datarows);
        }
    }
}