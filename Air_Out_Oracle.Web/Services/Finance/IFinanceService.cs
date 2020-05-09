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
    public interface IFinanceService
    {
        void ImportDataTable(DataTable datatable);

        Stream ExportExcel(string filterRules = "", string sort = "ID", string order = "asc");

        IQueryable<Finance> GetData(string filterRules = "", IEnumerable<AirOut.Web.Extensions.filterRule> AddfilterRules = null);

        /// <summary>
        /// 获取费用明细搜索 结果
        /// </summary>
        /// <param name="ArrFinance">搜索应收/付头数据</param>
        /// <param name="filterRules">搜索条件</param>
        /// <returns></returns>
        IQueryable<FinanceDtl> GetDtlData(List<Finance> ArrFinance, string filterRules = "");

        /// <summary>
        /// 获取应收应付 金额（按币种）
        /// </summary>
        /// <param name="ArrFinance"></param>
        /// <returns></returns>
        IEnumerable<Bill_AccountTotalByMoney_Code> GetArApBillAccountByFlight_Date(List<Finance> ArrFinance);
    }
}