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
    public class CodeItemService : Service<CodeItem>, ICodeItemService
    {
        private readonly IRepositoryAsync<CodeItem> _repository;
        private readonly IDataTableImportMappingService _mappingservice;

        public CodeItemService(IRepositoryAsync<CodeItem> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public IEnumerable<CodeItem> GetByBaseCodeId(int basecodeid)
        {
            return _repository.GetByBaseCodeId(basecodeid);
        }

        public IEnumerable<CodeItem> GetByCodeType(string codeType)
        {
            return _repository.Queryable().Where(x => x.BaseCode.CodeType == codeType);
        }

        public void SaveHead(System.Data.DataTable datatable)
        {
            var codelist = new List<BaseCode>();
            var coderepository = this._repository.GetRepository<BaseCode>();
            var codetypes = datatable.AsEnumerable().Where(row => row["基础代码"].ToString() != "").Select(row => row["基础代码"].ToString()).Distinct();
            foreach (var codetype in codetypes)
            {
                var basecode = coderepository.Queryable().Where(x => x.CodeType == codetype).FirstOrDefault();
                if (basecode == null)
                {
                    basecode = new BaseCode();
                    basecode.CodeType = codetype.ToString();
                    basecode.Description = codetype.ToString();
                    basecode.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                }
                codelist.Add(basecode);

            }
            if (codelist.Where(x => x.ObjectState == Repository.Pattern.Infrastructure.ObjectState.Added).Any())
                coderepository.InsertGraphRange(codelist.Where(x => x.ObjectState == Repository.Pattern.Infrastructure.ObjectState.Added).ToArray());
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var codelist = new List<BaseCode>();
            var coderepository = this._repository.GetRepository<BaseCode>();
            var codetypes = datatable.AsEnumerable().Select(row => row["基础代码"].ToString()).Distinct();
            foreach (var codetype in codetypes)
            {
                var basecode = coderepository.Queryable().Where(x => x.CodeType == codetype).FirstOrDefault();
                if (basecode == null)
                {
                    basecode = new BaseCode();
                    basecode.CodeType = codetype.ToString();
                    basecode.Description = codetype.ToString();
                    basecode.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;


                }
                codelist.Add(basecode);

            }


            foreach (DataRow row in datatable.Rows)
            {
                if (row["基础代码"].ToString() == "")
                    continue;
                CodeItem item = new CodeItem();
                var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "CodeItem").ToList();
                string codetype = string.Empty;

                foreach (var field in mapping)
                {

                    if (field.FieldName == "BaseCodeId")
                    {
                        codetype = row[field.SourceFieldName].ToString();
                        var baseCode = codelist.Where(x => x.CodeType == codetype).First();
                        item.BaseCodeId = baseCode.Id;
                    }
                    else
                    {
                        var defval = field.DefaultValue;
                        var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                        if (contation && row[field.SourceFieldName] != DBNull.Value)
                        {
                            Type codeitemtype = item.GetType();
                            PropertyInfo propertyInfo = codeitemtype.GetProperty(field.FieldName);
                            propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                        }
                        else if (!string.IsNullOrEmpty(defval))
                        {
                            Type codeitemtype = item.GetType();
                            PropertyInfo propertyInfo = codeitemtype.GetProperty(field.FieldName);
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
                }

                this.Insert(item);


            }
        }

        public Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);

            var codeitems = this.Query(new CodeItemQuery().Withfilter(filters)).Include(p => p.BaseCode).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();

            var datarows = codeitems.Select(n => new { BaseCodeCodeType = (n.BaseCode == null ? "" : n.BaseCode.CodeType), Id = n.Id, Code = n.Code, Text = n.Text, Description = n.Description, BaseCodeId = n.BaseCodeId }).ToList();

            return ExcelHelper.ExportExcel(typeof(CodeItem), datarows);

        }
    }
}



