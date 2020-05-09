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
using System.IO;

namespace TMI.Web.Services
{
    public interface IAirTicketOrderService:IService<AirTicketOrder>
    {
		
		IEnumerable<AirLine> GetArrAirLineByAirTicketOrderId(int airticketorderid);
	
		IEnumerable<PlanePerson> GetArrPlanePersonByAirTicketOrderId(int airticketorderid);
			 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");
	}
}