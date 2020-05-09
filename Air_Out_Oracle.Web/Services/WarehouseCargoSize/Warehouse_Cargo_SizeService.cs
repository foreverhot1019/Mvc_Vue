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
    public class Warehouse_Cargo_SizeService : Service< Warehouse_Cargo_Size >, IWarehouse_Cargo_SizeService
    {
        private readonly IRepositoryAsync<Warehouse_Cargo_Size> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  Warehouse_Cargo_SizeService(IRepositoryAsync< Warehouse_Cargo_Size> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Warehouse_Cargo_Size").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Warehouse_Cargo_Size item = new Warehouse_Cargo_Size();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type warehouse_cargo_sizetype = item.GetType();
						PropertyInfo propertyInfo = warehouse_cargo_sizetype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type warehouse_cargo_sizetype = item.GetType();
						PropertyInfo propertyInfo = warehouse_cargo_sizetype.GetProperty(field.FieldName);
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
			var warehouse_cargo_size = this.Query(new Warehouse_Cargo_SizeQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = warehouse_cargo_size.Select( n => new {  
Id = n.Id, 
Warehouse_Receipt_Id = n.Warehouse_Receipt_Id, 
Entry_Id = n.Entry_Id, 
CM_Length = n.CM_Length, 
CM_Width = n.CM_Width, 
CM_Height = n.CM_Height, 
CM_Piece = n.CM_Piece, 
Status = n.Status, 
Remark = n.Remark, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(Warehouse_Cargo_Size), datarows);
        }
    }
}