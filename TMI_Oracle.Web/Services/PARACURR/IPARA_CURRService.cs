﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using TMI.Web.Models;
using TMI.Web.Repositories;
using System.Data;
using System.IO;

namespace TMI.Web.Services
{
    public interface IPARA_CURRService:IService<PARA_CURR>
    {
				 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "ID", string order = "asc");
	}
}