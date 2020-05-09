

     
 
 

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
    public interface ICodeItemService:IService<CodeItem>
    {

                  IEnumerable<CodeItem> GetByBaseCodeId(int  basecodeid);
        
         
 
		void ImportDataTable(DataTable datatable);
        void SaveHead(System.Data.DataTable datatable);
        IEnumerable<CodeItem> GetByCodeType(string codeType);
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");
	}
}