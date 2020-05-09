using System;
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
    public interface IBD_DEFDOCService : IService<BD_DEFDOC>
    {
        IEnumerable<BD_DEFDOC_LIST> GetBD_DEFDOC_LISTByDOCID(int docid);

        void ImportDataTable(DataTable datatable);

        Stream ExportExcel(string filterRules = "", string sort = "ROWID", string order = "asc");
    }
}