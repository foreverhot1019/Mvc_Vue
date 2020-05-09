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
    public class FinanceMoneyService : Service<FinanceMoney>, IFinanceMoneyService
    {
        private readonly IRepositoryAsync<FinanceMoney> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public FinanceMoneyService(IRepositoryAsync<FinanceMoney> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<FinanceMoney> GetByOrderId(int orderid)
        {
            return _repository.GetByOrderId(orderid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "FinanceMoney").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                FinanceMoney item = new FinanceMoney();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type financemoneytype = item.GetType();
                        PropertyInfo propertyInfo = financemoneytype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type financemoneytype = item.GetType();
                        PropertyInfo propertyInfo = financemoneytype.GetProperty(field.FieldName);
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
            var financemoney = this.Query(new FinanceMoneyQuery().Withfilter(filters)).Include(p => p.OOrder).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = financemoney.Select(n => new
            {
                OOrderOrderNo = (n.OOrder == null ? "" : n.OOrder.OrderNo),
                Id = n.Id,
                OrderId = n.OrderId,
                SupplierName = n.SupplierName,
                ServiceProject = n.ServiceProject,
                Price = n.Price,
                Num = n.Num,
                TotalAmount = n.TotalAmount,
                RequestAmount = n.RequestAmount,
                ExcessAmount = n.ExcessAmount,
                Remark = n.Remark,
                AuditStatus = n.AuditStatus,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(FinanceMoney), datarows);
        }
    }
}