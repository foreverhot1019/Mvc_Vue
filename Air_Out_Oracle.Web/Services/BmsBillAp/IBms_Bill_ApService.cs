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
    public interface IBms_Bill_ApService:IService<Bms_Bill_Ap>
    {
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        dynamic GetFromNAME(Bms_Bill_Ap OBms_Bill_Ap);

        /// <summary>
        /// 插入空总单数据
        /// 以便总单结算 分摊
        /// </summary>
        /// <param name="MBL"></param>
        string InsertFT_EntrustmentInfor(string MBL);

        /// <summary>
        /// 应收审核
        /// </summary>
        /// <param name="AuditId">应收账单Id</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        dynamic Audit(List<int> ArrAuditId, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess);
	}
}