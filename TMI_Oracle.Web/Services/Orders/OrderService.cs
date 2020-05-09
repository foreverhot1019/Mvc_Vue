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
    public class OrderService : Service<Order>, IOrderService
    {
        private readonly IRepositoryAsync<Order> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public OrderService(IRepositoryAsync<Order> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<Order> GetByAdvisoryOrderId(int advisoryorderid)
        {
            return _repository.GetByAdvisoryOrderId(advisoryorderid);
        }

        public IEnumerable<OrderCustomer> GetArrOrderCustomerByOrderId(int orderid)
        {
            return _repository.GetArrOrderCustomerByOrderId(orderid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Order").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Order item = new Order();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type ordertype = item.GetType();
                        PropertyInfo propertyInfo = ordertype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type ordertype = item.GetType();
                        PropertyInfo propertyInfo = ordertype.GetProperty(field.FieldName);
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
            var orders = this.Query(new OrderQuery().Withfilter(filters)).Include(p => p.OAdvisoryOrder).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = orders.Select(n => new
            {
                OAdvisoryOrderOrderNo = (n.OAdvisoryOrder == null ? "" : n.OAdvisoryOrder.OrderNo),
                Id = n.Id,
                OrderNo = n.OrderNo,
                AdvisoryOrderId = n.AdvisoryOrderId,
                AdvsyOrdNo = n.AdvsyOrdNo,
                TravleOrdType = n.TravleOrdType,
                TravleType = n.TravleType,
                STravleDate = n.STravleDate,
                ETravleDate = n.ETravleDate,
                RouteNo = n.RouteNo,
                RouteName = n.RouteName,
                Saller = n.Saller,
                OP = n.OP,
                ForeCastPayDate = n.ForeCastPayDate,
                SupplyierNo = n.SupplyierNo,
                SupplierName = n.SupplierName,
                Contact = n.Contact,
                PayType = n.PayType,
                RoutePhoto = n.RoutePhoto,
                Remark = n.Remark,
                Status = n.Status,
                AuditStatus = n.AuditStatus,
                AdultNum = n.AdultNum,
                AdultActualNum = n.AdultActualNum,
                AdultPrice = n.AdultPrice,
                BoyNum = n.BoyNum,
                BoyActualNum = n.BoyActualNum,
                BoyPrice = n.BoyPrice,
                ChildNum = n.ChildNum,
                ChildActualNum = n.ChildActualNum,
                ChildPrice = n.ChildPrice,
                PriceRepair = n.PriceRepair,
                PriceRepairRemark = n.PriceRepairRemark,
                TotalRemark = n.TotalRemark,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Order), datarows);
        }
    }
}