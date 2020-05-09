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
    public class CusQuotedPriceDtlService : Service< CusQuotedPriceDtl >, ICusQuotedPriceDtlService
    {
        private readonly IRepositoryAsync<CusQuotedPriceDtl> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  CusQuotedPriceDtlService(IRepositoryAsync< CusQuotedPriceDtl> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "CusQuotedPriceDtl").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                CusQuotedPriceDtl item = new CusQuotedPriceDtl();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type cusquotedpricedtltype = item.GetType();
						PropertyInfo propertyInfo = cusquotedpricedtltype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type cusquotedpricedtltype = item.GetType();
						PropertyInfo propertyInfo = cusquotedpricedtltype.GetProperty(field.FieldName);
						if (defval.ToLower() == "now" && propertyInfo.PropertyType ==typeof(DateTime))
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

		public Stream ExportExcel(string filterRules = "",string sort = "Id", string order = "asc")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var cusquotedpricedtl = this.Query(new CusQuotedPriceDtlQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = cusquotedpricedtl.Select( n => new {  
Id = n.Id, 
CusQPSerialNo = n.CusQPSerialNo, 
CusQPId = n.CusQPId, 
QPSerialNo = n.QPSerialNo, 
QPId = n.QPId, 
FeeCode = n.FeeCode, 
FeeName = n.FeeName, 
StartPlace = n.StartPlace, 
TransitPlace = n.TransitPlace, 
EndPlace = n.EndPlace, 
AirLineCo = n.AirLineCo, 
AirLineNo = n.AirLineNo, 
WHBuyer = n.WHBuyer, 
ProxyOperator = n.ProxyOperator, 
DealWithArticle = n.DealWithArticle, 
BSA = n.BSA, 
CustomsType = n.CustomsType, 
InspectMark = n.InspectMark, 
GetOrdMark = n.GetOrdMark, 
MoorLevel = n.MoorLevel, 
BillingUnit = n.BillingUnit, 
Price = n.Price, 
CurrencyCode = n.CurrencyCode, 
FeeConditionVal1 = n.FeeConditionVal1, 
CalcSign1 = n.CalcSign1, 
FeeCondition = n.FeeCondition, 
CalcSign2 = n.CalcSign2, 
FeeConditionVal2 = n.FeeConditionVal2, 
CalcFormula = n.CalcFormula, 
FeeMin = n.FeeMin, 
FeeMax = n.FeeMax, 
StartDate = n.StartDate, 
EndDate = n.EndDate, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(CusQuotedPriceDtl), datarows);
        }
    }
}