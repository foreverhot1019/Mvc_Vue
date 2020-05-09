using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
	public static class Bms_Bill_Ap_DtlRepository  
    {
        public static IEnumerable<Bms_Bill_Ap_Dtl> GetByBmsBillApId(this IRepositoryAsync<Bms_Bill_Ap_Dtl> repository, int bmsbillapid)
        {
             var query= repository.Queryable().Where(x => x.Bms_Bill_Ap_Id==bmsbillapid);
             return query;
        }
	}
}