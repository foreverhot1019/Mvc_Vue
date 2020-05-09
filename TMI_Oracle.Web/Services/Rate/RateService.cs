﻿using System;
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
    public class RateService : Service<Rate>, IRateService
    {
        private readonly IRepositoryAsync<Rate> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public RateService(IRepositoryAsync<Rate> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Rate").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Rate item = new Rate();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type ratetype = item.GetType();
                        PropertyInfo propertyInfo = ratetype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type ratetype = item.GetType();
                        PropertyInfo propertyInfo = ratetype.GetProperty(field.FieldName);
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
            var rate = this.Query(new RateQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = rate.Select(n => new
            {
                Id = n.Id,
                LocalCurrency = n.LocalCurrency,
                LocalCurrCode = n.LocalCurrCode,
                ForeignCurrency = n.ForeignCurrency,
                ForeignCurrCode = n.ForeignCurrCode,
                Year = n.Year,
                Month = n.Month,
                RecRate = n.RecRate,
                PayRate = n.PayRate,
                Description = n.Description,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Rate), datarows);
        }
    }
}