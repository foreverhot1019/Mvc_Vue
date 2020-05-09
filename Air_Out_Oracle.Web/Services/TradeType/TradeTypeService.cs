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
    public class TradeTypeService : Service< TradeType >, ITradeTypeService
    {
        private readonly IRepositoryAsync<TradeType> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  TradeTypeService(IRepositoryAsync< TradeType> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "TradeType").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                TradeType item = new TradeType();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type tradetypetype = item.GetType();
						PropertyInfo propertyInfo = tradetypetype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type tradetypetype = item.GetType();
						PropertyInfo propertyInfo = tradetypetype.GetProperty(field.FieldName);
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
			var tradetype = this.Query(new TradeTypeQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = tradetype.Select( n => new {  
ID = n.ID, 
Code = n.Code, 
Name = n.Name, 
Description = n.Description, 
Status = n.Status, 
ADDWHO = n.ADDWHO, 
ADDTS = n.ADDTS, 
EDITWHO = n.EDITWHO, 
EDITTS = n.EDITTS}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(TradeType), datarows);
        }
    }
}