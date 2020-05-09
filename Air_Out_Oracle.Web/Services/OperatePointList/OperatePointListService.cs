







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
    public class OperatePointListService : Service<OperatePointList>, IOperatePointListService
    {

        private readonly IRepositoryAsync<OperatePointList> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public OperatePointListService(IRepositoryAsync<OperatePointList> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<OperatePointList> GetByOperatePointID(int operatepointid)
        {
            return _repository.GetByOperatePointID(operatepointid);
        }



        public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {

                OperatePointList item = new OperatePointList();
                var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "OperatePointList").ToList();

                foreach (var field in mapping)
                {

                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type operatepointlisttype = item.GetType();
                        PropertyInfo propertyInfo = operatepointlisttype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type operatepointlisttype = item.GetType();
                        PropertyInfo propertyInfo = operatepointlisttype.GetProperty(field.FieldName);
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

            var operatepointlist = this.Query(new OperatePointListQuery().Withfilter(filters)).Include(p => p.OperatePoint).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();

            var datarows = operatepointlist.Select(n => new
            {
                OperatePointOperatePointCode = (n.OperatePoint == null ? "" : n.OperatePoint.OperatePointCode),
                ID = n.ID,
                OperatePointID = n.OperatePointID,
                OperatePointCode = n.OperatePointCode,
                CompanyCode = n.CompanyCode,
                CompanyName = n.CompanyName,
                IsEnabled = n.IsEnabled,
                ADDTS = n.ADDTS,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                EDITWHO = n.EDITWHO,
                EDITID = n.EDITID,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(OperatePointList), datarows);

        }
    }
}



