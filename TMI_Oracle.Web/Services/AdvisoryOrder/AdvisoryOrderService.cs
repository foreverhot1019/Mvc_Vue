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
    public class AdvisoryOrderService : Service<AdvisoryOrder>, IAdvisoryOrderService
    {
        private readonly IRepositoryAsync<AdvisoryOrder> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public AdvisoryOrderService(IRepositoryAsync<AdvisoryOrder> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<AdvisoryOrder> GetByCustomerId(int customerid)
        {
            return _repository.GetByCustomerId(customerid);
        }

        public IEnumerable<AdvisoryOrder> GetByComponyId(int componyid)
        {
            return _repository.GetByComponyId(componyid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "AdvisoryOrder").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                AdvisoryOrder item = new AdvisoryOrder();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type advisoryordertype = item.GetType();
                        PropertyInfo propertyInfo = advisoryordertype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type advisoryordertype = item.GetType();
                        PropertyInfo propertyInfo = advisoryordertype.GetProperty(field.FieldName);
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
            var advisoryorder = this.Query(new AdvisoryOrderQuery().Withfilter(filters)).Include(p => p.OCompany).Include(p => p.OCustomer).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = advisoryorder.Select(n => new
            {
                OCompanyCIQID = (n.OCompany == null ? "" : n.OCompany.CIQID),
                OCustomerNameChs = (n.OCustomer == null ? "" : n.OCustomer.NameChs),
                Id = n.Id,
                OrderNo = n.OrderNo,
                TravleType = n.TravleType,
                RouteNo = n.RouteNo,
                //STravleDate = n.STravleDate,
                //ETravleDate = n.ETravleDate,
                TravlePersonNum = n.TravlePersonNum,
                UnitPrice = n.UnitPrice,
                TotalPrice = n.TotalPrice,
                RoutePhoto = n.RoutePhoto,
                Remark = n.Remark,
                Status = n.Status,
                CustomerId = n.CustomerId,
                CustomerName = n.CustomerName,
                ContactType = n.ContactType,
                Contact = n.Contact,
                Saller = n.Saller,
                OP = n.OP,
                CustomerType = n.CustomerType,
                CustomerSource = n.CustomerSource,
                ActiveStatus = n.ActiveStatus,
                CustomerLevel = n.CustomerLevel,
                ComponyName = n.ComponyName,
                ComponyId = n.ComponyId,
                n.TravleOrdType,
                n.RouteName ,
                n.CustomerRequire,
                OperatingPoint = n.OperatingPoint,
                n.ADDID,
                n.ADDTS,n.ADDWHO,
                n.EDITID,
                n.EDITTS,
                n.EDITWHO,
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(AdvisoryOrder), datarows);
        }
    }
}