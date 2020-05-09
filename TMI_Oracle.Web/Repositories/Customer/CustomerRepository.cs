﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
	public static class CustomerRepository  
    {
        public static IEnumerable<Customer> GetByComponyId(this IRepositoryAsync<Customer> repository, int componyid)
        {
             var query= repository.Queryable().Where(x => x.ComponyId==componyid);
             return query;
        }
	}
}