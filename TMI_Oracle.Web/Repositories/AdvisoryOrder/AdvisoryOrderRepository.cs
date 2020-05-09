using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
    public static class AdvisoryOrderRepository
    {
        public static IEnumerable<AdvisoryOrder> GetByCustomerId(this IRepositoryAsync<AdvisoryOrder> repository, int customerid)
        {
            var query = repository.Queryable().Where(x => x.CustomerId == customerid);
            return query;
        }
        public static IEnumerable<AdvisoryOrder> GetByComponyId(this IRepositoryAsync<AdvisoryOrder> repository, int componyid)
        {
            var query = repository.Queryable().Where(x => x.ComponyId == componyid);
            return query;
        }
    }
}