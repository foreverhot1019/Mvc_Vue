using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.IO;
using AirOut.Web.Models;
using AirOut.Web.Repositories;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace AirOut.Web.Services
{
    public interface IBD_DEFDOC_LISTService : IService<BD_DEFDOC_LIST>
    {
        IEnumerable<BD_DEFDOC_LIST> GetByDOCID(int docid);

        void ImportDataTable(DataTable datatable);

        Stream ExportExcel(string filterRules = "", string sort = "ROWID", string order = "asc");
    }
}