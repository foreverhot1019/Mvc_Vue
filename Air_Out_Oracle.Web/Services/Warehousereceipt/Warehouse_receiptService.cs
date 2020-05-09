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
    public class Warehouse_receiptService : Service< Warehouse_receipt >, IWarehouse_receiptService
    {
        private readonly IRepositoryAsync<Warehouse_receipt> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  Warehouse_receiptService(IRepositoryAsync< Warehouse_receipt> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Warehouse_receipt").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Warehouse_receipt item = new Warehouse_receipt();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type warehouse_receipttype = item.GetType();
						PropertyInfo propertyInfo = warehouse_receipttype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type warehouse_receipttype = item.GetType();
						PropertyInfo propertyInfo = warehouse_receipttype.GetProperty(field.FieldName);
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
			var warehouse_receipt = this.Query(new Warehouse_receiptQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = warehouse_receipt.Select( n => new {  
                                                Id = n.Id, 
                                                Warehouse_Id = n.Warehouse_Id, 
                                                Entry_Id = n.Entry_Id, 
                                                Warehouse_Code = n.Warehouse_Code, 
                                                Pieces_CK = n.Pieces_CK, 
                                                Weight_CK = n.Weight_CK, 
                                                Volume_CK = n.Volume_CK, 
                                                Packing = n.Packing, 
                                                Bulk_Weight_CK = n.Bulk_Weight_CK, 
                                                Damaged_CK = n.Damaged_CK, 
                                                Dampness_CK = n.Dampness_CK, 
                                                Deformation = n.Deformation, 
                                                Is_GF = n.Is_GF, 
                                                Closure_Remark = n.Closure_Remark, 
                                                Is_QG = n.Is_QG, 
                                                Warehouse_Remark = n.Warehouse_Remark, 
                                                Consign_Code_CK = n.Consign_Code_CK, 
                                                MBL = n.MBL, 
                                                HBL = n.HBL, 
                                                Flight_Date_Want = n.Flight_Date_Want, 
                                                FLIGHT_No = n.FLIGHT_No, 
                                                End_Port = n.End_Port, 
                                                In_Date = n.In_Date, 
                                                Out_Date = n.Out_Date, 
                                                CH_Name_CK = n.CH_Name_CK, 
                                                Is_CustomerReturn = n.Is_CustomerReturn, 
                                                Is_MyReturn = n.Is_MyReturn, 
                                                Truck_Id = n.Truck_Id, 
                                                Driver = n.Driver, 
                                                Is_DamageUpload = n.Is_DamageUpload, 
                                                Is_DeliveryUpload = n.Is_DeliveryUpload, 
                                                Is_EntryUpload = n.Is_EntryUpload, 
                                                Status = n.Status, 
                                                Remark = n.Remark, 
                                                OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(Warehouse_receipt), datarows);
        }
    }
}