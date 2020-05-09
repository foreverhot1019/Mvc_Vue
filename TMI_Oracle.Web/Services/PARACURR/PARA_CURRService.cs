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
    public class PARA_CURRService : Service< PARA_CURR >, IPARA_CURRService
    {
        private readonly IRepositoryAsync<PARA_CURR> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  PARA_CURRService(IRepositoryAsync< PARA_CURR> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PARA_CURR").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PARA_CURR item = new PARA_CURR();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type para_currtype = item.GetType();
						PropertyInfo propertyInfo = para_currtype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type para_currtype = item.GetType();
						PropertyInfo propertyInfo = para_currtype.GetProperty(field.FieldName);
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

		public Stream ExportExcel(string filterRules = "",string sort = "ID", string order = "asc")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var para_curr = this.Query(new PARA_CURRQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = para_curr.Select( n => new {  
ID = n.ID, 
CURR_CODE = n.CURR_CODE, 
CURR_Name = n.CURR_Name, 
CURR_NameEng = n.CURR_NameEng, 
Money_CODE = n.Money_CODE, 
Description = n.Description, 
Status = n.Status, 
ADDWHO = n.ADDWHO, 
ADDTS = n.ADDTS, 
EDITWHO = n.EDITWHO, 
EDITTS = n.EDITTS}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(PARA_CURR), datarows);
        }
    }
}