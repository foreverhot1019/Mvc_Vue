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
    public class DailyRateService : Service< DailyRate >, IDailyRateService
    {
        private readonly IRepositoryAsync<DailyRate> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  DailyRateService(IRepositoryAsync< DailyRate> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "DailyRate").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                DailyRate item = new DailyRate();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type dailyratetype = item.GetType();
						PropertyInfo propertyInfo = dailyratetype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type dailyratetype = item.GetType();
						PropertyInfo propertyInfo = dailyratetype.GetProperty(field.FieldName);
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
			var dailyrate = this.Query(new DailyRateQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = dailyrate.Select( n => new {  
Id = n.Id, 
LocalCurrency = n.LocalCurrency, 
LocalCurrCode = n.LocalCurrCode, 
ForeignCurrency = n.ForeignCurrency, 
ForeignCurrCode = n.ForeignCurrCode, 
PriceType = n.PriceType, 
BankName = n.BankName, 
Price = n.Price, 
ScrapyDate = n.ScrapyDate, 
Description = n.Description, 
Status = n.Status, 
ADDWHO = n.ADDWHO, 
ADDTS = n.ADDTS, 
EDITWHO = n.EDITWHO, 
EDITTS = n.EDITTS}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(DailyRate), datarows);
        }
    }
}