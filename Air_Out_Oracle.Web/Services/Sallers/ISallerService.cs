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
    public interface ISallerService : IService<Saller>
    {
        /// <summary>
        /// 获取销售
        /// </summary>
        /// <param name="sallerid"></param>
        /// <returns></returns>
        IEnumerable<CusBusInfo> GetArrCusBusInfoBySallerId(int sallerid);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="datatable"></param>
        void ImportDataTable(DataTable datatable);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="filterRules"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc");
    }
}