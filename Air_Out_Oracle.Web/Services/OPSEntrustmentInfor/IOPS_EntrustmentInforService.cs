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
    public interface IOPS_EntrustmentInforService:IService<OPS_EntrustmentInfor>
    {
				 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");

        /// <summary>
        /// 保存退关或取消退关
        /// </summary>
        /// <param name="ArrId">退关 委托Id</param>
        /// <param name="NeedConfirm">强制删除</param>
        /// <returns></returns>
        string Batch_TG(IEnumerable<int> ArrId, bool NeedConfirm = false);

        //委托信息日志写入 
        string DifferenceComparisonE(OPS_EntrustmentInfor OldSource, OPS_EntrustmentInfor NowSource);
	}
}