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
    public class CustomsInspectionService : Service< CustomsInspection >, ICustomsInspectionService
    {
        private readonly IRepositoryAsync<CustomsInspection> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  CustomsInspectionService(IRepositoryAsync< CustomsInspection> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "CustomsInspection").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                CustomsInspection item = new CustomsInspection();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type customsinspectiontype = item.GetType();
						PropertyInfo propertyInfo = customsinspectiontype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type customsinspectiontype = item.GetType();
						PropertyInfo propertyInfo = customsinspectiontype.GetProperty(field.FieldName);
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
			var customsinspection = this.Query(new CustomsInspectionQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = customsinspection.Select( n => new {  
Id = n.Id, 
Operation_ID = n.Operation_ID, 
Flight_NO = n.Flight_NO, 
Flight_Date_Want = n.Flight_Date_Want, 
MBL = n.MBL, 
Consign_Code_CK = n.Consign_Code_CK, 
Book_Flat_Code = n.Book_Flat_Code, 
Customs_Declaration = n.Customs_Declaration, 
Num_BG = n.Num_BG, 
Remarks_BG = n.Remarks_BG, 
Customs_Broker_BG = n.Customs_Broker_BG, 
Customs_Date_BG = n.Customs_Date_BG, 
Pieces_TS = n.Pieces_TS, 
Weight_TS = n.Weight_TS, 
Volume_TS = n.Volume_TS, 
Pieces_Fact = n.Pieces_Fact, 
Weight_Fact = n.Weight_Fact, 
Volume_Fact = n.Volume_Fact, 
Pieces_BG = n.Pieces_BG, 
Weight_BG = n.Weight_BG, 
Volume_BG = n.Volume_BG, 
IS_Checked_BG = n.IS_Checked_BG, 
Check_QTY = n.Check_QTY, 
Check_Date = n.Check_Date, 
Fileupload = n.Fileupload, 
Status = n.Status, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(CustomsInspection), datarows);
        }
    }
}