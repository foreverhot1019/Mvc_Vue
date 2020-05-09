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

namespace TMI.Web.Services
{
    public interface IMenuItemService : IService<MenuItem>
    {
        IEnumerable<MenuItem> GetByParentId(int parentid);

        IEnumerable<MenuItem> GetSubMenusByParentId(int parentid);

        void ImportDataTable(DataTable datatable);

        IEnumerable<MenuItem> CreateWithController();

        IEnumerable<MenuItem> ReBuildMenus();
    }
}