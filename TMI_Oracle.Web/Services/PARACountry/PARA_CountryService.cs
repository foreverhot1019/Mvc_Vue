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
    public class PARA_CountryService : Service< PARA_Country >, IPARA_CountryService
    {
        private readonly IRepositoryAsync<PARA_Country> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  PARA_CountryService(IRepositoryAsync< PARA_Country> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PARA_Country").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PARA_Country item = new PARA_Country();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type para_countrytype = item.GetType();
						PropertyInfo propertyInfo = para_countrytype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type para_countrytype = item.GetType();
						PropertyInfo propertyInfo = para_countrytype.GetProperty(field.FieldName);
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

		public Stream ExportExcel(string filterRules = "",string sort = "COUNTRY_CO", string order = "asc")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var para_country = this.Query(new PARA_CountryQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = para_country.Select( n => new {  
COUNTRY_CO = n.COUNTRY_CO, 
COUNTRY_EN = n.COUNTRY_EN, 
COUNTRY_NA = n.COUNTRY_NA, 
EXAM_MARK = n.EXAM_MARK, 
HIGH_LOW = n.HIGH_LOW, 
Status = n.Status}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(PARA_Country), datarows);
        }
    }
}