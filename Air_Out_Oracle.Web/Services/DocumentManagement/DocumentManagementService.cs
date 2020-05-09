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
    public class DocumentManagementService : Service< DocumentManagement >, IDocumentManagementService
    {
        private readonly IRepositoryAsync<DocumentManagement> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  DocumentManagementService(IRepositoryAsync< DocumentManagement> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "DocumentManagement").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                DocumentManagement item = new DocumentManagement();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type documentmanagementtype = item.GetType();
						PropertyInfo propertyInfo = documentmanagementtype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type documentmanagementtype = item.GetType();
						PropertyInfo propertyInfo = documentmanagementtype.GetProperty(field.FieldName);
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

		public Stream ExportExcel(string filterRules = "",string sort = "Id", string order = "asc")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var documentmanagement = this.Query(new DocumentManagementQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = documentmanagement.Select( (n,idx) => new {  
                Id = idx++,
                Doc_NO = n.Doc_NO,
                MBL = n.MBL,
                Ping_Name = n.Ping_Name,
                BG_TT = n.BG_TT,
                Flight_Date_Want = n.Flight_Date_Want,
                Entrustment_Name = n.Entrustment_Name,
                Return_Date = n.Return_Date,
                Return_Customer_Date = n.Return_Customer_Date,
                Operation_ID = n.Operation_ID,
                SignReceipt_Code = n.SignReceipt_Code,
                Trade_Mode = n.Trade_Mode,
                ReturnWHO = n.ReturnWHO,
                Entrustment_Code = n.Entrustment_Code
             }).ToList();

            return ExcelHelper.ExportExcel(typeof(DocumentManagement), datarows, "ExportDocumentManagementList");
            //return ExcelHelper.ExportExcel(typeof(DocumentManagement), datarows);
        }
    }
}