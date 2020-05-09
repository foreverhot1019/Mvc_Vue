using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
	public static class Bms_Bill_Ar_DtlRepository  
    {
        public static IEnumerable<Bms_Bill_Ar_Dtl> GetByBmsBillArId(this IRepositoryAsync<Bms_Bill_Ar_Dtl> repository, int bmsbillarid)
        {
             var query= repository.Queryable().Where(x => x.Bms_Bill_Ar_Id==bmsbillarid);
             return query;
        }
	}
}