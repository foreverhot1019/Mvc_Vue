using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
	public static class AirTicketOrderRepository  
    {
        
		public static IEnumerable<AirLine> GetArrAirLineByAirTicketOrderId (this IRepositoryAsync<AirTicketOrder> repository,int airticketorderid)
        {
			var airlineRepository = repository.GetRepository<AirLine>(); 
            return airlineRepository.Queryable().Include(x => x.OAirTicket).Where(n => n.AirTicketOrderId == airticketorderid);
        }
        
		public static IEnumerable<PlanePerson> GetArrPlanePersonByAirTicketOrderId (this IRepositoryAsync<AirTicketOrder> repository,int airticketorderid)
        {
			var planepersonRepository = repository.GetRepository<PlanePerson>(); 
            return planepersonRepository.Queryable().Include(x => x.OAirTicket).Where(n => n.AirTicketOrderId == airticketorderid);
        }
	}
}