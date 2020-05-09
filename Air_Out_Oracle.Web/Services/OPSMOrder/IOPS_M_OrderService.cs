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
    public interface IOPS_M_OrderService:IService<OPS_M_Order>
    {
		//导入
		void ImportDataTable(DataTable datatable);
		 
        //导出
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");

        //总单信息日志写入 
        string DifferenceComparisonM(OPS_M_Order OldSource, OPS_M_Order NowSource);

        /// <summary>
        /// 修改总单号
        /// </summary>
        /// <param name="ArrOps_M_Ids"></param>
        /// <param name="NewMBL"></param>
        string EditMBL(IEnumerable<int> ArrOps_M_Ids, string NewMBL);
	}
}