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
    public class PARA_AirLineService : Service<PARA_AirLine>, IPARA_AirLineService
    {
        private readonly IRepositoryAsync<PARA_AirLine> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public PARA_AirLineService(IRepositoryAsync<PARA_AirLine> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PARA_AirLine").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PARA_AirLine item = new PARA_AirLine();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type para_airlinetype = item.GetType();
                        PropertyInfo propertyInfo = para_airlinetype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type para_airlinetype = item.GetType();
                        PropertyInfo propertyInfo = para_airlinetype.GetProperty(field.FieldName);
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
            var para_airline = this.Query(new PARA_AirLineQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = para_airline.Select(n => new
            {
                Id = n.Id,
                AirCode = n.AirCode,
                AirLine = n.AirLine,
                AirCompany = n.AirCompany,
                StarStation = n.StarStation,
                TransferStation = n.TransferStation,
                EndStation = n.EndStation,
                AirDate = n.AirDate,
                Description = n.Description,
                Status = n.Status,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(PARA_AirLine), datarows);
        }
    }
}