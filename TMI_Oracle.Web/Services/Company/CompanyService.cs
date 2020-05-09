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
    public class CompanyService : Service<Company>, ICompanyService
    {
        private readonly IRepositoryAsync<Company> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public CompanyService(IRepositoryAsync<Company> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Company").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Company item = new Company();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type companytype = item.GetType();
                        PropertyInfo propertyInfo = companytype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type companytype = item.GetType();
                        PropertyInfo propertyInfo = companytype.GetProperty(field.FieldName);
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
            var company = this.Query(new CompanyQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = company.Select(n => new
            {
                Id = n.Id,
                CIQID = n.CIQID,
                Name = n.Name,
                SimpleName = n.SimpleName,
                Eng_Name = n.Eng_Name,
                Address = n.Address,
                City = n.City,
                Province = n.Province,
                RegisterDate = n.RegisterDate,
                Logo = n.Logo,
                ContractStart = n.ContractStart,
                ContractEnd = n.ContractEnd,
                AuthorizeAmount = n.AuthorizeAmount,
                Currency = n.Currency,
                CheckBillDate = n.CheckBillDate,
                PayPalDate = n.PayPalDate,
                InvoiceType = n.InvoiceType,
                Remark = n.Remark,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Company), datarows);
        }
    }
}