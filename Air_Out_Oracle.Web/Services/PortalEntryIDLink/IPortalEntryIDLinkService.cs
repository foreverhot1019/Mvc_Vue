﻿using System;
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
    public interface IPortalEntryIDLinkService:IService<PortalEntryIDLink>
    {
				 
		void ImportDataTable(DataTable datatable);
		 
		Stream ExportExcel( string filterRules = "",string sort = "ID", string order = "asc");
	}
}