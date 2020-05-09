
             
           
 




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
    public class UserOperatePointLinkService : Service< UserOperatePointLink >, IUserOperatePointLinkService
    {

        private readonly IRepositoryAsync<UserOperatePointLink> _repository;
		 private readonly IDataTableImportMappingService _mappingservice;
        public  UserOperatePointLinkService(IRepositoryAsync< UserOperatePointLink> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
                  
        

		public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                 
                UserOperatePointLink item = new UserOperatePointLink();
				var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "UserOperatePointLink").ToList();

                foreach (var field in mapping)
                {
                 
						var defval = field.DefaultValue;
						var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
						if (contation && row[field.SourceFieldName] != DBNull.Value)
						{
							Type useroperatepointlinktype = item.GetType();
							PropertyInfo propertyInfo = useroperatepointlinktype.GetProperty(field.FieldName);
							propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
						}
						else if (!string.IsNullOrEmpty(defval))
						{
							Type useroperatepointlinktype = item.GetType();
							PropertyInfo propertyInfo = useroperatepointlinktype.GetProperty(field.FieldName);
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
                                   var useroperatepointlink  = this.Query(new UserOperatePointLinkQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
                        var datarows = useroperatepointlink .Select(  n => new {  ID = n.ID , UserId = n.UserId , OperateOpintId = n.OperateOpintId }).ToList();
           
            return ExcelHelper.ExportExcel(typeof(UserOperatePointLink), datarows);

        }
    }
}



