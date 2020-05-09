using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
	public static class SallerRepository  
    {
        /// <summary>
        /// 获取销售
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="sallerid"></param>
        /// <returns></returns>
		public static IEnumerable<CusBusInfo> GetArrCusBusInfoBySallerId (this IRepositoryAsync<Saller> repository,int sallerid)
        {
			var cusbusinfoRepository = repository.GetRepository<CusBusInfo>(); 
            return cusbusinfoRepository.Queryable().Include(x => x.OSaller).Where(n => n.SallerId == sallerid);
        }
	}
}