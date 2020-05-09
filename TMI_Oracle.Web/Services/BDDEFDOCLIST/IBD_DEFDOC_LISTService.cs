using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.IO;
using TMI.Web.Models;
using TMI.Web.Repositories;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace TMI.Web.Services
{
    public interface IBD_DEFDOC_LISTService : IService<BD_DEFDOC_LIST>
    {
        IEnumerable<BD_DEFDOC_LIST> GetByDOCID(int docid);

        void ImportDataTable(DataTable datatable);

        Stream ExportExcel(string filterRules = "", string sort = "ROWID", string order = "asc");
    }
}