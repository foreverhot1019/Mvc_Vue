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
    public class PARA_AreaService : Service<PARA_Area>, IPARA_AreaService
    {
        private readonly IRepositoryAsync<PARA_Area> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public PARA_AreaService(IRepositoryAsync<PARA_Area> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "PARA_Area").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                PARA_Area item = new PARA_Area();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type para_areatype = item.GetType();
                        PropertyInfo propertyInfo = para_areatype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type para_areatype = item.GetType();
                        PropertyInfo propertyInfo = para_areatype.GetProperty(field.FieldName);
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

        public Stream ExportExcel(string filterRules = "", string sort = "ID", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var para_area = this.Query(new PARA_AreaQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = para_area.Select(n => new
            {
                ID = n.ID,
                AreaCode = n.AreaCode,
                AreaName = n.AreaName
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(PARA_Area), datarows);
        }

        /// <summary>
        /// 获取区域+口岸（起始的/目的地使用）
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public IEnumerable<ComboDridListModel> GetComboDridListModel_FromCache()
        {
            IEnumerable<ComboDridListModel> QData = (IEnumerable<ComboDridListModel>)CacheHelper.GetCache("AddressPlace");
            if (QData == null || !QData.Any())
            {
                //区域
                var para_Area = (List<PARA_Area>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area);
                var Qpara_Area = para_Area.Where(x => x.AreaCode == null || x.AreaName == null);
                foreach (var item in Qpara_Area)
                {
                    item.AreaCode = item.AreaCode ?? "";
                    item.AreaName = item.AreaName ?? "";
                }
                //口岸
                var para_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
                var Qpara_AirPort = para_AirPort.Where(x => x.PortCode == null || x.PortName == null);
                foreach (var item in Qpara_AirPort)
                {
                    item.PortCode = item.PortCode ?? "";
                    item.PortName = item.PortName ?? "";
                }
                //汇总
                QData = para_Area.Select(n => new ComboDridListModel { ID = n.ID.ToString(), TEXT = n.AreaName }).
                    Concat(para_AirPort.Select(n => new ComboDridListModel { ID = n.PortCode, TEXT = n.PortName }));
                CacheHelper.SetCache("AddressPlace", QData);
            }
            return QData;
        }
    }
}