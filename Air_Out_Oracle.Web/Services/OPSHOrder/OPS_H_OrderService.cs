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
    public class OPS_H_OrderService : Service< OPS_H_Order >, IOPS_H_OrderService
    {
        private readonly IRepositoryAsync<OPS_H_Order> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  OPS_H_OrderService(IRepositoryAsync< OPS_H_Order> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "OPS_H_Order").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                OPS_H_Order item = new OPS_H_Order();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type ops_h_ordertype = item.GetType();
						PropertyInfo propertyInfo = ops_h_ordertype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type ops_h_ordertype = item.GetType();
						PropertyInfo propertyInfo = ops_h_ordertype.GetProperty(field.FieldName);
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
			var ops_h_order = this.Query(new OPS_H_OrderQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = ops_h_order.Select( n => new {  
Id = n.Id, 
Shipper_H = n.Shipper_H, 
Consignee_H = n.Consignee_H, 
Notify_Part_H = n.Notify_Part_H, 
Currency_H = n.Currency_H, 
Bragainon_Article_H = n.Bragainon_Article_H, 
Pay_Mode_H = n.Pay_Mode_H, 
Carriage_H = n.Carriage_H, 
Incidental_Expenses_H = n.Incidental_Expenses_H, 
Declare_Value_Trans_H = n.Declare_Value_Trans_H, 
Declare_Value_Ciq_H = n.Declare_Value_Ciq_H, 
Risk_H = n.Risk_H, 
Marks_H = n.Marks_H, 
EN_Name_H = n.EN_Name_H, 
Pieces_H = n.Pieces_H, 
Weight_H = n.Weight_H, 
Volume_H = n.Volume_H, 
Charge_Weight_H = n.Charge_Weight_H, 
HBL = n.HBL, 
Operation_Id = n.Operation_Id, 
Is_Self = n.Is_Self, 
Ty_Type = n.Ty_Type, 
Lot_No = n.Lot_No, 
Hbl_Feight = n.Hbl_Feight, 
Is_XC = n.Is_XC, 
Is_BAS = n.Is_BAS, 
Is_DCZ = n.Is_DCZ, 
Is_ZB = n.Is_ZB, 
ADDPoint = n.ADDPoint, 
EDITPoint = n.EDITPoint, 
Status = n.Status, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(OPS_H_Order), datarows);
        }
    }
}