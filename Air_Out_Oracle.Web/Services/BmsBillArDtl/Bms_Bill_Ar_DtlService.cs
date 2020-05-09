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
    public class Bms_Bill_Ar_DtlService : Service<Bms_Bill_Ar_Dtl>, IBms_Bill_Ar_DtlService
    {
        private readonly IRepositoryAsync<Bms_Bill_Ar_Dtl> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public Bms_Bill_Ar_DtlService(IRepositoryAsync<Bms_Bill_Ar_Dtl> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<Bms_Bill_Ar_Dtl> GetByBmsBillArId(int bmsbillarid)
        {
            return _repository.GetByBmsBillArId(bmsbillarid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Bms_Bill_Ar_Dtl").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Bms_Bill_Ar_Dtl item = new Bms_Bill_Ar_Dtl();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type bms_bill_ar_dtltype = item.GetType();
                        PropertyInfo propertyInfo = bms_bill_ar_dtltype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type bms_bill_ar_dtltype = item.GetType();
                        PropertyInfo propertyInfo = bms_bill_ar_dtltype.GetProperty(field.FieldName);
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
            var bms_bill_ar_dtl = this.Query(new Bms_Bill_Ar_DtlQuery().Withfilter(filters)).Include(p => p.OBms_Bill_Ar).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = bms_bill_ar_dtl.Select(n => new
            {
                OBms_Bill_ArDzbh = (n.OBms_Bill_Ar == null ? "" : n.OBms_Bill_Ar.Dzbh),
                Id = n.Id,
                Bms_Bill_Ar_Id = n.Bms_Bill_Ar_Id,
                Dzbh = n.Dzbh,
                Line_No = n.Line_No,
                Line_Id = n.Line_Id,
                Charge_Code = n.Charge_Code,
                Charge_Desc = n.Charge_Desc,
                Unitprice = n.Unitprice,
                Unitprice2 = n.Unitprice2,
                Qty = n.Qty,
                Account = n.Account,
                Account2 = n.Account2,
                Money_Code = n.Money_Code,
                Summary = n.Summary,
                Collate_Id = n.Collate_Id,
                Collate_Name = n.Collate_Name,
                Collate_Date = n.Collate_Date,
                Collate_Status = n.Collate_Status,
                Collate_No = n.Collate_No,
                User_Id = n.User_Id,
                User_Name = n.User_Name,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Bms_Bill_Ar_Dtl), datarows);
        }
    }
}