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
    public class TestMVC_CRUDService : Service< TestMVC_CRUD >, ITestMVC_CRUDService
    {
        private readonly IRepositoryAsync<TestMVC_CRUD> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  TestMVC_CRUDService(IRepositoryAsync< TestMVC_CRUD> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "TestMVC_CRUD").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                TestMVC_CRUD item = new TestMVC_CRUD();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type testmvc_crudtype = item.GetType();
						PropertyInfo propertyInfo = testmvc_crudtype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type testmvc_crudtype = item.GetType();
						PropertyInfo propertyInfo = testmvc_crudtype.GetProperty(field.FieldName);
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
			var testmvc_crud = this.Query(new TestMVC_CRUDQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = testmvc_crud.Select( n => new {  
Id = n.Id, 
Dzbh = n.Dzbh, 
TestColumn1 = n.TestColumn1, 
TestColumn2 = n.TestColumn2, 
TestColumn3 = n.TestColumn3, 
TestColumn4 = n.TestColumn4, 
TestColumn5 = n.TestColumn5}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(TestMVC_CRUD), datarows);
        }
    }
}