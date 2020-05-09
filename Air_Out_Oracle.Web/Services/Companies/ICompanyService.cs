

     
 
 
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

namespace AirOut.Web.Services
{
    public interface ICompanyService:IService<Company>
    {

         
         
 
		void ImportDataTable(DataTable datatable);
	}
}