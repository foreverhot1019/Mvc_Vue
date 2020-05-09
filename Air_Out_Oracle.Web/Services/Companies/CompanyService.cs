
             
           
 

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
namespace AirOut.Web.Services
{
    public class CompanyService : Service< Company >, ICompanyService
    {

        private readonly IRepositoryAsync<Company> _repository;
		 private readonly IDataTableImportMappingService _mappingservice;
        public  CompanyService(IRepositoryAsync< Company> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
                  
        

		public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                 
                Company item = new Company();
                var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Company").ToList();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type companytype = item.GetType();
                        PropertyInfo propertyInfo = companytype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type companytype = item.GetType();
                        PropertyInfo propertyInfo = companytype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(defval, propertyInfo.PropertyType), null);
                    }
                    

                }
                
                this.Insert(item);
               

            }
        }
    }
}



