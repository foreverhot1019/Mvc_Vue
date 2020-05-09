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
    public class OPS_M_OrderService : Service<OPS_M_Order>, IOPS_M_OrderService
    {
        private readonly IRepositoryAsync<OPS_M_Order> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        private readonly IBms_Bill_ApService _bms_bill_apService;

        public OPS_M_OrderService(IRepositoryAsync<OPS_M_Order> repository, IDataTableImportMappingService mappingservice, IBms_Bill_ApService bms_bill_apservice)
            : base(repository)
        {
            _repository = repository;
			_mappingservice = mappingservice;
            _bms_bill_apService = bms_bill_apservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "OPS_M_Order").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                OPS_M_Order item = new OPS_M_Order();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type ops_m_ordertype = item.GetType();
						PropertyInfo propertyInfo = ops_m_ordertype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type ops_m_ordertype = item.GetType();
						PropertyInfo propertyInfo = ops_m_ordertype.GetProperty(field.FieldName);
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
            var ops_m_order = this.Query(new OPS_M_OrderQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = ops_m_order.Select(n => new
            {
                                                Id = n.Id, 
                                                MBL = n.MBL, 
                                                Airways_Code = n.Airways_Code, 
                                                FWD_Code = n.FWD_Code, 
                                                Shipper_M = n.Shipper_M, 
                                                Consignee_M = n.Consignee_M, 
                                                Notify_Part_M = n.Notify_Part_M, 
                                                Depart_Port = n.Depart_Port, 
                                                End_Port = n.End_Port, 
                                                Flight_No = n.Flight_No, 
                                                Flight_Date_Want = n.Flight_Date_Want, 
                                                Currency_M = n.Currency_M, 
                                                Bragainon_Article_M = n.Bragainon_Article_M, 
                                                Pay_Mode_M = n.Pay_Mode_M, 
                                                Carriage_M = n.Carriage_M, 
                                                Incidental_Expenses_M = n.Incidental_Expenses_M, 
                                                Declare_Value_Trans_M = n.Declare_Value_Trans_M, 
                                                Declare_Value_Ciq_M = n.Declare_Value_Ciq_M, 
                                                Risk_M = n.Risk_M, 
                                                Marks_M = n.Marks_M, 
                                                EN_Name_M = n.EN_Name_M, 
                                                Hand_Info_M = n.Hand_Info_M, 
                                                Signature_Agent_M = n.Signature_Agent_M, 
                                                Rate_Class_M = n.Rate_Class_M, 
                                                Air_Frae = n.Air_Frae, 
                                                AWC = n.AWC, 
                                                Pieces_M = n.Pieces_M, 
                                                Weight_M = n.Weight_M, 
                                                Volume_M = n.Volume_M, 
                                                Charge_Weight_M = n.Charge_Weight_M, 
                                                Price_Article = n.Price_Article, 
                                                CCC = n.CCC, 
                                                File_M = n.File_M, 
                                                Status = n.Status, 
                OperatingPoint = n.OperatingPoint
            }).ToList();
           
            return ExcelHelper.ExportExcel(typeof(OPS_M_Order), datarows);
        }

        /// <summary>
        /// 主单日志写入 记录字段
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="OldSource">旧的数据源</param>
        /// <param name="NowSource">新的数据源</param>
        /// <returns></returns>
        public string DifferenceComparisonM(OPS_M_Order OldSource, OPS_M_Order NowSource)
        {
            string dcstr = "";
            if (OldSource == null || NowSource == null)
            {
                return dcstr;
            }
            if (OldSource.MBL != NowSource.MBL)
            {
                dcstr += "MBL:" + NowSource.MBL + ",";
            }
            if (OldSource.Currency_M != NowSource.Currency_M)
            {
                dcstr += "Currency_M:" + NowSource.Currency_M + ",";
            }
            if (OldSource.Bragainon_Article_M != NowSource.Bragainon_Article_M)
            {
                dcstr += "Bragainon_Article_M:" + NowSource.Bragainon_Article_M + ",";
            }
            if (OldSource.Flight_Date_Want != NowSource.Flight_Date_Want)
            {
                dcstr += "Flight_Date_Want:" + NowSource.Flight_Date_Want + ",";
            }
            if (OldSource.Flight_No != NowSource.Flight_No)
            {
                dcstr += "Flight_No:" + NowSource.Flight_No + ",";
            }

            return dcstr;
        }

        /// <summary>
        /// 修改总单号
        /// </summary>
        /// <param name="ArrOps_M_Ids"></param>
        /// <param name="NewMBL"></param>
        public string EditMBL(IEnumerable<int> ArrOps_M_Ids, string NewMBL)
        {
            string ErrMsg = "";

            try
            {
                if ((ArrOps_M_Ids == null || !ArrOps_M_Ids.Any()) || string.IsNullOrWhiteSpace(NewMBL))
                    ErrMsg = "要修改的数据为空，或新的总单号为空！";
                else
                {
                    NewMBL = Common.RemoveNotNumber(NewMBL);
                    var tf = Common.validataMBL(NewMBL);
                    if (!tf)
                        ErrMsg = "新的总单号格式不正确！";
                    else
                    {
                        var dbAppContxt = _repository.getDbContext();
                        //var OEntry = dbAppContxt.Entry();
                        var ArrOps_M_Order = _repository.Queryable().Where(x => ArrOps_M_Ids.Contains(x.Id)).ToList();
                        var inIdParam = string.Join(",", ArrOps_M_Ids);
                        List<string> ArrSQL = new List<string>(){
                            "update OPS_M_Orders t set t.MBL =:V_MBL  where t.id in ("+inIdParam+")",
                            "update OPS_EntrustmentInfors t set t.MBL =:V_MBL  where t.MBLId in ("+inIdParam+")",
                            "update OPS_H_Orders t set t.MBL =:V_MBL  where t.MBLId in ("+inIdParam+")",
                        };
                        var ArrOrclParam = new List<Oracle.ManagedDataAccess.Client.OracleParameter>(){
                            new Oracle.ManagedDataAccess.Client.OracleParameter(":V_MBL",NewMBL),
                            //new Oracle.ManagedDataAccess.Client.OracleParameter(":V_Ids",string.Join(",",ArrOps_M_Ids))
                        };
                        int ret = SQLDALHelper.OracleHelper.ExecuteSqlTran(ArrSQL, ArrOrclParam.ToArray());
                        ////插入总单应付行
                        //ErrMsg = _bms_bill_apService.InsertFT_EntrustmentInfor(NewMBL);
                        //if (!string.IsNullOrEmpty(ErrMsg))
                        //{
                        //    ErrMsg = "修改总单号成功，但插入总单应付行失败" + ErrMsg;
                        //}
                    }
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