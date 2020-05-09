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
    public interface IChangeOrderHistoryService : IService<ChangeOrderHistory>
    {
        void ImportDataTable(DataTable datatable);

        Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc");

        string InsertChangeOrdHistory(int Key_Id, string TableName, ChangeOrderHistory.EnumChangeType _EnumChangeType, string Content, int DeleteNum = 0, int UpdateNum = 0, int InsertNum = 0);
    }
}