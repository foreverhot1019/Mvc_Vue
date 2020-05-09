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
    public class PARA_AirPortService : Service<PARA_AirPort>, IPARA_AirPortService
    {
        private readonly IRepositoryAsync<PARA_AirPort> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public PARA_AirPortService(IRepositoryAsync<PARA_AirPort> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PARA_AirPort").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PARA_AirPort item = new PARA_AirPort();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type para_airporttype = item.GetType();
                        PropertyInfo propertyInfo = para_airporttype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type para_airporttype = item.GetType();
                        PropertyInfo propertyInfo = para_airporttype.GetProperty(field.FieldName);
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
            var para_airport = this.Query(new PARA_AirPortQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = para_airport.Select(n => new
            {
                Id = n.Id,
                PortCode = n.PortCode,
                PortName = n.PortName,
                PortNameEng = n.PortNameEng,
                PortType = n.PortType,
                CountryCode = n.CountryCode,
                Description = n.Description,
                Status = n.Status,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(PARA_AirPort), datarows);
        }
    }
}