using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using AirOut.Web.Models;
using AirOut.Web.Repositories;
using System.Data;
using System.IO;

namespace AirOut.Web.Services
{
    public interface ICostMoneyService:IService<CostMoney>
    {
				 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");
        
        /// <summary>
        /// 应付自动结算
        /// </summary>
        /// <param name="OOPS_M_Order">接单数据</param>
        /// <param name="ArrFeeType">费用</param>
        /// <returns></returns>
        string AutoAddFee(OPS_M_Order OOPS_M_Order, IEnumerable<String> ArrFeeType = null);
	}
}