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
    public interface IBms_Bill_Ar_DtlService:IService<Bms_Bill_Ar_Dtl>
    {
		IEnumerable<Bms_Bill_Ar_Dtl> GetByBmsBillArId(int bmsbillarid);
    			 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "Id", string order = "asc");
	}
}