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
    public interface IContactsService:IService<Contacts>
    {
				 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");
        
        /// <summary>
        /// 新增 客户联系人
        /// </summary>
        /// <param name="ContactInfo">联系信息</param>
        /// <param name="ContactType">类型</param>
        /// <param name="CusBusInfoId">客户ID</param>
        void InsertContact(string ContactInfo, string ContactType, string CusBusInfoId);
	}
}