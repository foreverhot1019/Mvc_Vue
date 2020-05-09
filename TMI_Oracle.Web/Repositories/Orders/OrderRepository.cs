using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
    public static class OrderRepository
    {
        public static IEnumerable<Order> GetByAdvisoryOrderId(this IRepositoryAsync<Order> repository, int advisoryorderid)
        {
            var query = repository.Queryable().Where(x => x.AdvisoryOrderId == advisoryorderid);
            return query;
        }

        public static IEnumerable<OrderCustomer> GetArrOrderCustomerByOrderId(this IRepositoryAsync<Order> repository, int orderid)
        {
            var ordercustomerRepository = repository.GetRepository<OrderCustomer>();
            return ordercustomerRepository.Queryable().Include(x => x.OCustomer).
                Include(x => x.OOrder).Where(n => n.OrderId == orderid);
        }
    }
}