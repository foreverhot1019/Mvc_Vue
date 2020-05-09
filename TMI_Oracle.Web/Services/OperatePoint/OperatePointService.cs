
             
           
 




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
    public class OperatePointService : Service< OperatePoint >, IOperatePointService
    {

        private readonly IRepositoryAsync<OperatePoint> _repository;
		 private readonly IDataTableImportMappingService _mappingservice;
        public  OperatePointService(IRepositoryAsync< OperatePoint> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
                         public IEnumerable<OperatePointList>   GetOperatePointListsByOperatePointID (int operatepointid)
        {
            return _repository.GetOperatePointListsByOperatePointID(operatepointid);
        }
         
        

		public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                 
                OperatePoint item = new OperatePoint();
				var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "OperatePoint").ToList();

                foreach (var field in mapping)
                {
                 
						var defval = field.DefaultValue;
						var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
						if (contation && row[field.SourceFieldName] != DBNull.Value)
						{
							Type operatepointtype = item.GetType();
							PropertyInfo propertyInfo = operatepointtype.GetProperty(field.FieldName);
							propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
						}
						else if (!string.IsNullOrEmpty(defval))
						{
							Type operatepointtype = item.GetType();
							PropertyInfo propertyInfo = operatepointtype.GetProperty(field.FieldName);
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
                                   var operatepoint  = this.Query(new OperatePointQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
                        var datarows = operatepoint .Select(  n => new {  ID = n.ID , OperatePointCode = n.OperatePointCode , OperatePointName = n.OperatePointName , Description = n.Description , IsEnabled = n.IsEnabled , ADDTS = n.ADDTS , ADDID = n.ADDID , ADDWHO = n.ADDWHO , EDITWHO = n.EDITWHO , EDITID = n.EDITID , EDITTS = n.EDITTS }).ToList();
           
            return ExcelHelper.ExportExcel(typeof(OperatePoint), datarows);

        }
    }
}



