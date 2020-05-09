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
    public class CusBusInfoService : Service<CusBusInfo>, ICusBusInfoService
    {
        private readonly IRepositoryAsync<CusBusInfo> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public CusBusInfoService(IRepositoryAsync<CusBusInfo> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "CusBusInfo").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                CusBusInfo item = new CusBusInfo();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type cusbusinfotype = item.GetType();
                        PropertyInfo propertyInfo = cusbusinfotype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type cusbusinfotype = item.GetType();
                        PropertyInfo propertyInfo = cusbusinfotype.GetProperty(field.FieldName);
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
            var cusbusinfo = this.Query(new CusBusInfoQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = cusbusinfo.Select(n => new
            {
                Id = n.Id,
                EnterpriseId = n.EnterpriseId,
                EnterpriseShortName = n.EnterpriseShortName,
                EnterpriseGroupCode = n.EnterpriseGroupCode,
                TopEnterpriseCode = n.TopEnterpriseCode,
                CIQID = n.CIQID,
                CHECKID = n.CHECKID,
                CustomsCode = n.CustomsCode,
                CHNName = n.CHNName,
                EngName = n.EngName,
                AddressCHN = n.AddressCHN,
                AddressEng = n.AddressEng,
                WebSite = n.WebSite,
                TradeTypeCode = n.TradeTypeCode,
                AreaCode = n.AreaCode,
                CountryCode = n.CountryCode,
                CopoKindCode = n.CopoKindCode,
                CorpartiPerson = n.CorpartiPerson,
                ResteredCapital = n.ResteredCapital,
                IsInternalCompany = n.IsInternalCompany,
                Description = n.Description,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(CusBusInfo), datarows);
        }
    }
}