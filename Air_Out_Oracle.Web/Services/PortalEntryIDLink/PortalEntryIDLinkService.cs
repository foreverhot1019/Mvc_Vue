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

using Newtonsoft.Json;
using AirOut.Web.Extensions;
using System.IO;

namespace AirOut.Web.Services
{
    public class PortalEntryIDLinkService : Service< PortalEntryIDLink >, IPortalEntryIDLinkService
    {
        private readonly IRepositoryAsync<PortalEntryIDLink> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  PortalEntryIDLinkService(IRepositoryAsync< PortalEntryIDLink> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PortalEntryIDLink").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PortalEntryIDLink item = new PortalEntryIDLink();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type portalentryidlinktype = item.GetType();
						PropertyInfo propertyInfo = portalentryidlinktype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type portalentryidlinktype = item.GetType();
						PropertyInfo propertyInfo = portalentryidlinktype.GetProperty(field.FieldName);
						if (defval.ToLower() == "now" && propertyInfo.PropertyType ==typeof(DateTime))
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(DateTime.Now, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(defval, propertyInfo.PropertyType), null);
                        }
					}
                }
                
                this.Insert(item);
            }
        }

		public Stream ExportExcel(string filterRules = "",string sort = "ID", string order = "asc")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var portalentryidlink = this.Query(new PortalEntryIDLinkQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = portalentryidlink.Select( n => new {  
ID = n.ID, 
UserId = n.UserId, 
DepartMent = n.DepartMent, 
EntryID = n.EntryID}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(PortalEntryIDLink), datarows);
        }
    }
}