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
    public class AirLineService : Service< AirLine >, IAirLineService
    {
        private readonly IRepositoryAsync<AirLine> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  AirLineService(IRepositoryAsync< AirLine> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
		
		public IEnumerable<AirLine> GetByAirTicketOrderId(int airticketorderid)
        {
			return _repository.GetByAirTicketOrderId(airticketorderid);
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "AirLine").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                AirLine item = new AirLine();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type airlinetype = item.GetType();
						PropertyInfo propertyInfo = airlinetype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type airlinetype = item.GetType();
						PropertyInfo propertyInfo = airlinetype.GetProperty(field.FieldName);
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
			var airline = this.Query(new AirLineQuery().Withfilter(filters)).Include(p => p.OAirTicket).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = airline.Select( n => new {  
OAirTicketAirTicketNo = (n.OAirTicket==null?"": n.OAirTicket.AirTicketNo), 
Id = n.Id, 
AirTicketOrderId = n.AirTicketOrderId, 
AirLineType = n.AirLineType, 
AirCompany = n.AirCompany, 
Flight_No = n.Flight_No, 
City = n.City, 
Position = n.Position, 
Flight_Date_Want = n.Flight_Date_Want, 
TakeOffTime = n.TakeOffTime, 
ArrivalTime = n.ArrivalTime, 
TicketPrice = n.TicketPrice, 
BillTaxAmount = n.BillTaxAmount, 
CostMoney = n.CostMoney, 
SellPrice = n.SellPrice, 
TicketNum = n.TicketNum, 
Profit = n.Profit, 
Insurance = n.Insurance, 
Is_Endorse = n.Is_Endorse, 
EndorseDate = n.EndorseDate, 
EndorseWho = n.EndorseWho, 
Is_ReturnTicket = n.Is_ReturnTicket, 
ReturnTicketDate = n.ReturnTicketDate, 
ReturnTicketWho = n.ReturnTicketWho, 
Is_Cancel = n.Is_Cancel, 
CancelDate = n.CancelDate, 
CancelWho = n.CancelWho, 
Remark = n.Remark, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(AirLine), datarows);
        }
    }
}