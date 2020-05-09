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
    public class SallerService : Service<Saller>, ISallerService
    {
        private readonly IRepositoryAsync<Saller> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public SallerService(IRepositoryAsync<Saller> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        /// <summary>
        /// 获取销售
        /// </summary>
        /// <param name="sallerid"></param>
        /// <returns></returns>
        public IEnumerable<CusBusInfo> GetArrCusBusInfoBySallerId(int sallerid)
        {
            return _repository.GetArrCusBusInfoBySallerId(sallerid);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="datatable"></param>
        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Saller").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Saller item = new Saller();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type sallertype = item.GetType();
                        PropertyInfo propertyInfo = sallertype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type sallertype = item.GetType();
                        PropertyInfo propertyInfo = sallertype.GetProperty(field.FieldName);
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

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="filterRules"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var sallers = this.Query(new SallerQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = sallers.Select(n => new
            {
                Id = n.Id,
                Name = n.Name,
                PhoneNumber = n.PhoneNumber,
                Company = n.Company,
                Address = n.Address,
                Description = n.Description,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Saller), datarows);
        }
    }
}