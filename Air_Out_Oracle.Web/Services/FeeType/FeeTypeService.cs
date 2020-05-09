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
    public class FeeTypeService : Service< FeeType >, IFeeTypeService
    {
        private readonly IRepositoryAsync<FeeType> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  FeeTypeService(IRepositoryAsync< FeeType> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "FeeType").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                FeeType item = new FeeType();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type feetypetype = item.GetType();
						PropertyInfo propertyInfo = feetypetype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type feetypetype = item.GetType();
						PropertyInfo propertyInfo = feetypetype.GetProperty(field.FieldName);
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
			var feetype = this.Query(new FeeTypeQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = feetype.Select( n => new {  
Id = n.Id, 
FeeCode = n.FeeCode, 
FeeName = n.FeeName, 
FeeEName = n.FeeEName, 
InspectionFee = n.InspectionFee, 
CustomsFee = n.CustomsFee, 
MathDate = n.MathDate, 
Description = n.Description, 
Status = n.Status, 
OperatingPoint = n.OperatingPoint, 
ADDWHO = n.ADDWHO, 
ADDTS = n.ADDTS, 
EDITWHO = n.EDITWHO, 
EDITTS = n.EDITTS}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(FeeType), datarows);
        }
    }
}