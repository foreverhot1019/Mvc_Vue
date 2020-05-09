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
    public interface IQuotedPriceService:IService<QuotedPrice>
    {
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OQuotedPrice"></param>
        /// <returns></returns>
        dynamic GetFromNAME(QuotedPrice OQuotedPrice);
	}
}