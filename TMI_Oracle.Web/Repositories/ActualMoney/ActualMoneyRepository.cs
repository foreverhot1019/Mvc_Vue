using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
	public static class ActualMoneyRepository  
    {
        public static IEnumerable<ActualMoney> GetByOrderId(this IRepositoryAsync<ActualMoney> repository, int orderid)
        {
             var query= repository.Queryable().Where(x => x.OrderId==orderid);
             return query;
        }
	}
}