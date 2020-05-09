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
    public class CustomerService : Service<Customer>, ICustomerService
    {
        private readonly IRepositoryAsync<Customer> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public CustomerService(IRepositoryAsync<Customer> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<Customer> GetByComponyId(int componyid)
        {
            return _repository.GetByComponyId(componyid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Customer").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Customer item = new Customer();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type customertype = item.GetType();
                        PropertyInfo propertyInfo = customertype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type customertype = item.GetType();
                        PropertyInfo propertyInfo = customertype.GetProperty(field.FieldName);
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
            var customer = this.Query(new CustomerQuery().Withfilter(filters)).Include(p => p.OCompany).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = customer.Select(n => new
            {
                OCompanyName = (n.OCompany == null ? "" : n.OCompany.Name),
                Id = n.Id,
                NameChs = n.NameChs,
                NamePinYin = n.NamePinYin,
                NameEng = n.NameEng,
                Sex = n.Sex,
                Birthday = n.Birthday,
                AirLineMember = n.AirLineMember,
                ContactType = n.ContactType,
                Contact = n.Contact,
                ActiveStatus = n.ActiveStatus,
                Saller = n.Saller,
                OP = n.OP,
                CustomerLevel = n.CustomerLevel,
                CustomerSource = n.CustomerSource,
                CustomerType = n.CustomerType,
                ComponyName = n.ComponyName,
                ComponyId = n.ComponyId,
                IdCard = n.IdCard,
                IdCardLimit_S = n.IdCardLimit_S,
                IdCardLimit_E = n.IdCardLimit_E,
                IdCardPhoto_A = n.IdCardPhoto_A,
                IdCardPhoto_B = n.IdCardPhoto_B,
                Passpord = n.Passpord,
                PasspordLimit_S = n.PasspordLimit_S,
                PasspordLimit_E = n.PasspordLimit_E,
                PasspordPhoto_A = n.PasspordPhoto_A,
                PasspordPhoto_B = n.PasspordPhoto_B,
                HK_MacauPass = n.HK_MacauPass,
                HK_MacauPassLimit_S = n.HK_MacauPassLimit_S,
                HK_MacauPassLimit_E = n.HK_MacauPassLimit_E,
                HK_MacauPassPhoto_A = n.HK_MacauPassPhoto_A,
                HK_MacauPassPhoto_B = n.HK_MacauPassPhoto_B,
                TaiwanPass = n.TaiwanPass,
                TaiwanPassLimit_S = n.TaiwanPassLimit_S,
                TaiwanPassLimit_E = n.TaiwanPassLimit_E,
                TaiwanPassPhoto_A = n.TaiwanPassPhoto_A,
                TaiwanPassPhoto_B = n.TaiwanPassPhoto_B,
                TWIdCard = n.TWIdCard,
                TWIdCardLimit_S = n.TWIdCardLimit_S,
                TWIdCardLimit_E = n.TWIdCardLimit_E,
                TWIdCardPhoto_A = n.TWIdCardPhoto_A,
                TWIdCardPhoto_B = n.TWIdCardPhoto_B,
                HometownPass = n.HometownPass,
                HometownPassLimit_S = n.HometownPassLimit_S,
                HometownPassLimit_E = n.HometownPassLimit_E,
                HometownPassPhoto_A = n.HometownPassPhoto_A,
                HometownPassPhoto_B = n.HometownPassPhoto_B,
                Remark = n.Remark,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Customer), datarows);
        }
    }
}