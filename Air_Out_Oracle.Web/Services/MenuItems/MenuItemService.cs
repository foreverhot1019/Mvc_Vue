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
using System.Reflection;

namespace AirOut.Web.Services
{
    public class MenuItemService : Service<MenuItem>, IMenuItemService
    {
        private readonly IRepositoryAsync<MenuItem> _repository;
        private readonly IDataTableImportMappingService _mappingservice;

        public MenuItemService(IRepositoryAsync<MenuItem> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<MenuItem> GetByParentId(int parentid)
        {
            return _repository.GetByParentId(parentid);
        }

        public IEnumerable<MenuItem> GetSubMenusByParentId(int parentid)
        {
            return _repository.GetSubMenusByParentId(parentid);
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "MenuItem").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                MenuItem item = new MenuItem();
                foreach (DataColumn col in datatable.Columns)
                {
                    var sourcefieldname = col.ColumnName;
                    var ColMappingWhere = mapping.Where(x => x.SourceFieldName == col.ToString() || x.FieldName == col.ToString());
                    if (ColMappingWhere.Any())
                    {
                        if (row[sourcefieldname] != DBNull.Value)
                        {
                            var OColMapping = ColMappingWhere.FirstOrDefault();
                            Type menuitemtype = item.GetType();
                            PropertyInfo propertyInfo = menuitemtype.GetProperty(OColMapping.FieldName);
                            if (propertyInfo != null)
                                propertyInfo.SetValue(item, Convert.ChangeType(row[sourcefieldname], propertyInfo.PropertyType), null);
                        }
                    }
                }

                this.Insert(item);
            }
        }

        public IEnumerable<MenuItem> CreateWithController()
        {
            List<MenuItem> list = new List<MenuItem>();

            Assembly asm = Assembly.GetExecutingAssembly();

            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                    .Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
            int i = 1000;
            string[] actions = new string[] { "Index" };
            foreach (var item in controlleractionlist.Where(x => actions.Contains(x.Action)))
            {
                MenuItem menu = new MenuItem();
                menu.Code = (i++).ToString("0000");
                menu.Description = "";
                menu.Title = item.Controller.Replace("Controller", "");
                menu.Url = "/" + item.Controller.Replace("Controller", "") + "/" + item.Action;
                menu.Action = item.Action;
                menu.Controller = item.Controller;
                menu.IsEnabled = true;
                if (!this.Queryable().Where(x => x.Url == menu.Url).Any())
                {
                    this.Insert(menu);
                }

                list.Add(menu);
            }

            return list;
        }


        public IEnumerable<MenuItem> ReBuildMenus()
        {
            foreach (var item in this.Queryable().ToList())
            {
                this.Delete(item);
            }
            return this.CreateWithController();
        }
    }
}



