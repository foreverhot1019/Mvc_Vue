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
    public class PlanePersonService : Service< PlanePerson >, IPlanePersonService
    {
        private readonly IRepositoryAsync<PlanePerson> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  PlanePersonService(IRepositoryAsync< PlanePerson> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
		
		public IEnumerable<PlanePerson> GetByAirTicketOrderId(int airticketorderid)
        {
			return _repository.GetByAirTicketOrderId(airticketorderid);
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PlanePerson").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PlanePerson item = new PlanePerson();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type planepersontype = item.GetType();
						PropertyInfo propertyInfo = planepersontype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type planepersontype = item.GetType();
						PropertyInfo propertyInfo = planepersontype.GetProperty(field.FieldName);
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
			var planeperson = this.Query(new PlanePersonQuery().Withfilter(filters)).Include(p => p.OAirTicket).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = planeperson.Select( n => new {  
OAirTicketAirTicketNo = (n.OAirTicket==null?"": n.OAirTicket.AirTicketNo), 
Id = n.Id, 
AirTicketOrderId = n.AirTicketOrderId, 
NameChs = n.NameChs, 
LastNameEng = n.LastNameEng, 
FirstNameEng = n.FirstNameEng, 
OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(PlanePerson), datarows);
        }
    }
}