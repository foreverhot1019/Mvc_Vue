
     
 
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using AirOut.Web.Models;
using AirOut.Web.Repositories;

namespace AirOut.Web.Services
{
    public interface IDataTableImportMappingService:IService<DataTableImportMapping>
    {

        IEnumerable<EntityInfo> GetAssemblyEntities();
        void GenerateByEnityName(string entityName);

        DataTableImportMapping FindMapping(string entitySetName, string sourceFieldName);

        string GetDisplayName(string entitySetName, string fieldName);
         
 
	}
}