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
    public interface IBms_Bill_ArService:IService<Bms_Bill_Ar>
    {
		void ImportDataTable(DataTable datatable);

        Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc");

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        dynamic GetFromNAME(Bms_Bill_Ar OBms_Bill_Ar);

        /// <summary>
        /// 计算 价，税，价税合计
        /// </summary>
        /// <param name="Bill_HasTax">是否含税</param>
        /// <param name="Bill_TaxRate">税率</param>
        /// <param name="Bill_Account2">实际金额</param>
        /// <returns></returns>
        dynamic CalcTaxRate(bool Bill_HasTax, decimal Bill_TaxRate, decimal Bill_Account2);
        
        /// <summary>
        /// 应收审核
        /// </summary>
        /// <param name="AuditId">应收账单Id</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        dynamic Audit(List<int> ArrAuditId, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess);
	}
}