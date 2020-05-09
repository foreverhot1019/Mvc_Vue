using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;

using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
    public static class CodeItemRepository
    {
        public static IEnumerable<CodeItem> GetByBaseCodeId(this IRepositoryAsync<CodeItem> repository, int basecodeid)
        {
            var query = repository
               .Queryable()
               .Where(x => x.BaseCodeId == basecodeid);
            return query;

        }



    }
}



