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
using System.Reflection;

using Newtonsoft.Json;
using TMI.Web.Extensions;
using System.IO;

namespace TMI.Web.Services
{
    public class ChangeOrderHistoryService : Service<ChangeOrderHistory>, IChangeOrderHistoryService
    {
        private readonly IRepositoryAsync<ChangeOrderHistory> _repository;
        private readonly IDataTableImportMappingService _mappingservice;

        public ChangeOrderHistoryService(IRepositoryAsync<ChangeOrderHistory> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "ChangeOrderHistory").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                ChangeOrderHistory item = new ChangeOrderHistory();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type changeorderhistorytype = item.GetType();
                        PropertyInfo propertyInfo = changeorderhistorytype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type changeorderhistorytype = item.GetType();
                        PropertyInfo propertyInfo = changeorderhistorytype.GetProperty(field.FieldName);
                        if (defval.ToLower() == "now" && propertyInfo.PropertyType == typeof(DateTime))
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

        public Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var changeorderhistory = this.Query(new ChangeOrderHistoryQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = changeorderhistory.Select(n => new
            {
                Id = n.Id,
                Key_Id = n.Key_Id,
                TableName = n.TableName,
                ChangeType = n.ChangeType,
                InsertNum = n.InsertNum,
                UpdateNum = n.UpdateNum,
                DeleteNum = n.DeleteNum,
                Content = n.Content,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS,
                EDITID = n.EDITID,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(ChangeOrderHistory), datarows);
        }

        /// <summary>
        /// 插入 变更历史记录
        /// </summary>
        /// <param name="Key_Id"></param>
        /// <param name="TableName"></param>
        /// <param name="_EnumChangeType"></param>
        /// <param name="Content"></param>
        /// <param name="DeleteNum"></param>
        /// <param name="UpdateNum"></param>
        /// <param name="InsertNum"></param>
        /// <returns></returns>
        public string InsertChangeOrdHistory(int Key_Id, string TableName, ChangeOrderHistory.EnumChangeType _EnumChangeType, string Content, int DeleteNum = 0, int UpdateNum = 0, int InsertNum = 0)
        {
            string ErrMsg = "";
            try
            {
                WebdbContext OWebdbContext = new WebdbContext();
                ChangeOrderHistory OChangeOrderHistory = new ChangeOrderHistory();
                OChangeOrderHistory.ChangeType = _EnumChangeType;
                OChangeOrderHistory.DeleteNum = DeleteNum;
                OChangeOrderHistory.UpdateNum = UpdateNum;
                OChangeOrderHistory.InsertNum = InsertNum;
                OChangeOrderHistory.Key_Id = Key_Id;
                OChangeOrderHistory.TableName = TableName;
                OChangeOrderHistory.Content = Content;
                var Db_Set = OWebdbContext.ChangeOrderHistory.Attach(OChangeOrderHistory);
                Db_Set.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                OWebdbContext.Entry(OChangeOrderHistory).State = EntityState.Added;
                int ret = OWebdbContext.SaveChanges();

                if (ret > 0)
                {
                    string TakeDaysStr = System.Configuration.ConfigurationManager.AppSettings["ChangeOrdHistoryTakeDays"] ?? "";
                    int TakeDays = 30;
                    int.TryParse(TakeDaysStr, out TakeDays);
                    DateTime dt = DateTime.Now.AddDays(-TakeDays);
                    string StrDeltSQL = "delete from CHANGEORDERHISTORIES t where t.ADDTS < TO_DATE(:V_ADDTS,'yyyy-MM-dd hh24:mi:ss')";
                    Oracle.ManagedDataAccess.Client.OracleParameter[] ParamS = new Oracle.ManagedDataAccess.Client.OracleParameter[]{
                        new Oracle.ManagedDataAccess.Client.OracleParameter(":V_ADDTS",dt.ToString("yyyy-MM-dd")+" 23:59:59")
                    };
                    SQLDALHelper.OracleHelper.ExecuteNonQuery(CommandType.Text, StrDeltSQL, ParamS);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = Common.GetExceptionMsg(ex);
            }
            return ErrMsg;
        }

    }
}