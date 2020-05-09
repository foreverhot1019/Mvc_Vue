
             
           
 




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
    public class MenuActionService : Service< MenuAction >, IMenuActionService
    {

        private readonly IRepositoryAsync<MenuAction> _repository;
		 private readonly IDataTableImportMappingService _mappingservice;
        public  MenuActionService(IRepositoryAsync< MenuAction> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
                  
        

		public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                 
                MenuAction item = new MenuAction();
				var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "MenuAction").ToList();

                foreach (var field in mapping)
                {
                 
						var defval = field.DefaultValue;
						var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
						if (contation && row[field.SourceFieldName] != DBNull.Value)
						{
							Type menuactiontype = item.GetType();
							PropertyInfo propertyInfo = menuactiontype.GetProperty(field.FieldName);
							propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
						}
						else if (!string.IsNullOrEmpty(defval))
						{
							Type menuactiontype = item.GetType();
							PropertyInfo propertyInfo = menuactiontype.GetProperty(field.FieldName);
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
                                   var menuactions  = this.Query(new MenuActionQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
                        var datarows = menuactions .Select(  n => new {  Id = n.Id , Name = n.Name , Code = n.Code , Sort = n.Sort , IsEnabled = n.IsEnabled , Description = n.Description }).ToList();
           
            return ExcelHelper.ExportExcel(typeof(MenuAction), datarows);

        }
    }
}



