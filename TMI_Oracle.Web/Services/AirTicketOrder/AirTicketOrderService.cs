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
    public class AirTicketOrderService : Service< AirTicketOrder >, IAirTicketOrderService
    {
        private readonly IRepositoryAsync<AirTicketOrder> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  AirTicketOrderService(IRepositoryAsync< AirTicketOrder> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }

        public IEnumerable<AirLine>   GetArrAirLineByAirTicketOrderId (int airticketorderid)
        {
            return _repository.GetArrAirLineByAirTicketOrderId(airticketorderid);
        }

        public IEnumerable<PlanePerson>   GetArrPlanePersonByAirTicketOrderId (int airticketorderid)
        {
            return _repository.GetArrPlanePersonByAirTicketOrderId(airticketorderid);
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "AirTicketOrder").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                AirTicketOrder item = new AirTicketOrder();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type airticketordertype = item.GetType();
						PropertyInfo propertyInfo = airticketordertype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type airticketordertype = item.GetType();
						PropertyInfo propertyInfo = airticketordertype.GetProperty(field.FieldName);
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
			var airticketorder = this.Query(new AirTicketOrderQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = airticketorder.Select( n => new {  
Id = n.Id, 
AirTicketNo = n.AirTicketNo, 
CompanyCIQID = n.CompanyCIQID, 
CompanyName = n.CompanyName, 
Saller = n.Saller, 
AirTicketOrdType = n.AirTicketOrdType, 
TicketType = n.TicketType, 
PlanePerson = n.PlanePerson, 
PNR = n.PNR, 
TravlePersonNum = n.TravlePersonNum, 
ExpectPaymentDate = n.ExpectPaymentDate, 
SupplierName = n.SupplierName, 
AirTicketNo_Org = n.AirTicketNo_Org, 
Status = n.Status, 
AuditStatus = n.AuditStatus, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(AirTicketOrder), datarows);
        }
    }
}