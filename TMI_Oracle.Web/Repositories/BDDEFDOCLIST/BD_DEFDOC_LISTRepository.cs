using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
    public static class BD_DEFDOC_LISTRepository
    {
        public static IEnumerable<BD_DEFDOC_LIST> GetByDOCID(this IRepositoryAsync<BD_DEFDOC_LIST> repository, int docid)
        {
            var query = repository.Queryable()
               .Where(x => x.DOCID == docid);
            return query;

        }

    }
}



