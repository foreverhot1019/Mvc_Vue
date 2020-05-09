using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using TMI.Web.Models;
using TMI.Web.Repositories;

namespace TMI.Web.Services
{
    public interface IDataTableImportMappingService : IService<DataTableImportMapping>
    {
        IEnumerable<EntityInfo> GetAssemblyEntities();

        void GenerateByEnityName(string entityName);

        DataTableImportMapping FindMapping(string entitySetName, string sourceFieldName);

        string GetDisplayName(string entitySetName, string fieldName);

    }
}