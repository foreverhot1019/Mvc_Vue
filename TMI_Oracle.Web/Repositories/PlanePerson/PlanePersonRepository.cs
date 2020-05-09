using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
	public static class PlanePersonRepository  
    {
        public static IEnumerable<PlanePerson> GetByAirTicketOrderId(this IRepositoryAsync<PlanePerson> repository, int airticketorderid)
        {
             var query= repository.Queryable().Where(x => x.AirTicketOrderId==airticketorderid);
             return query;
        }
	}
}