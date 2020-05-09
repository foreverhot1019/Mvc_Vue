using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using TMI.Web.Models;

namespace TMI.Web.Repositories
{
    public static class BD_DEFDOCRepository
    {
        public static IEnumerable<BD_DEFDOC_LIST> GetBD_DEFDOC_LISTByDOCID(this IRepositoryAsync<BD_DEFDOC> repository, int docid)
        {
            var bd_defdoc_listRepository = repository.GetRepository<BD_DEFDOC_LIST>();
            return bd_defdoc_listRepository.Queryable().Include(x => x.BD_DEFDOC).Where(n => n.DOCID == docid);
        }

    }
}



