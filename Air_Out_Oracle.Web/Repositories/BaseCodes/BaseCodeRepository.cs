﻿



using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;

using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
    public static class BaseCodeRepository
    {


        public static IEnumerable<CodeItem> GetCodeItemsByBaseCodeId(this IRepositoryAsync<BaseCode> repository, int basecodeid)
        {
            var query = repository.Query(n => n.Id == basecodeid).Include(x => x.CodeItems).Select().FirstOrDefault();
            return (query == null ? null : query.CodeItems);
           
        }

        public static IEnumerable<CodeItem> GetCodeItemsByCodeType(this IRepositoryAsync<BaseCode> repository, string codeType)
        {
            var query = repository.Query(n => n.CodeType == codeType).Include(x => x.CodeItems).Select().FirstOrDefault();
            return query.CodeItems;
         
        }

    }
}



