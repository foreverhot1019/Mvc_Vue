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
using Repository.Pattern.Infrastructure;

namespace AirOut.Web.Services
{
    public class OPS_EntrustmentInforService : Service< OPS_EntrustmentInfor >, IOPS_EntrustmentInforService
    {
        private readonly IRepositoryAsync<OPS_EntrustmentInfor> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  OPS_EntrustmentInforService(IRepositoryAsync< OPS_EntrustmentInfor> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "OPS_EntrustmentInfor").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                OPS_EntrustmentInfor item = new OPS_EntrustmentInfor();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type ops_entrustmentinfortype = item.GetType();
						PropertyInfo propertyInfo = ops_entrustmentinfortype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type ops_entrustmentinfortype = item.GetType();
						PropertyInfo propertyInfo = ops_entrustmentinfortype.GetProperty(field.FieldName);
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
			var ops_entrustmentinfor = this.Query(new OPS_EntrustmentInforQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = ops_entrustmentinfor.Select( n => new {  
                                                            Id = n.Id, 
                                                            Consign_Code = n.Consign_Code, 
                                                            Custom_Code = n.Custom_Code, 
                                                            Area_Code = n.Area_Code, 
                                                            Entrustment_Name = n.Entrustment_Name, 
                                                            Entrustment_Code = n.Entrustment_Code, 
                                                            FWD_Code = n.FWD_Code, 
                                                            Consignee_Code = n.Consignee_Code, 
                                                            Carriage_Account_Code = n.Carriage_Account_Code, 
                                                            Incidental_Account_Code = n.Incidental_Account_Code, 
                                                            Depart_Port = n.Depart_Port, 
                                                            Transfer_Port = n.Transfer_Port, 
                                                            End_Port = n.End_Port, 
                                                            Shipper_H = n.Shipper_H, 
                                                            Consignee_H = n.Consignee_H, 
                                                            Notify_Part_H = n.Notify_Part_H, 
                                                            Shipper_M = n.Shipper_M, 
                                                            Consignee_M = n.Consignee_M, 
                                                            Notify_Part_M = n.Notify_Part_M, 
                                                            Pieces_TS = n.Pieces_TS, 
                                                            Weight_TS = n.Weight_TS, 
                                                            Pieces_SK = n.Pieces_SK, 
                                                            Slac_SK = n.Slac_SK, 
                                                            Weight_SK = n.Weight_SK, 
                                                            Pieces_DC = n.Pieces_DC, 
                                                            Slac_DC = n.Slac_DC, 
                                                            Weight_DC = n.Weight_DC, 
                                                            Pieces_Fact = n.Pieces_Fact, 
                                                            Weight_Fact = n.Weight_Fact, 
                                                            IS_MoorLevel = n.IS_MoorLevel, 
                                                            MoorLevel = n.MoorLevel, 
                                                            Volume_TS = n.Volume_TS, 
                                                            Charge_Weight_TS = n.Charge_Weight_TS, 
                                                            Bulk_Weight_TS = n.Bulk_Weight_TS, 
                                                            Volume_SK = n.Volume_SK, 
                                                            Charge_Weight_SK = n.Charge_Weight_SK, 
                                                            Bulk_Weight_SK = n.Bulk_Weight_SK, 
                                                            Bulk_Percent_SK = n.Bulk_Percent_SK, 
                                                            Account_Weight_SK = n.Account_Weight_SK, 
                                                            Volume_DC = n.Volume_DC, 
                                                            Charge_Weight_DC = n.Charge_Weight_DC, 
                                                            Bulk_Weight_DC = n.Bulk_Weight_DC, 
                                                            Bulk_Percent_DC = n.Bulk_Percent_DC, 
                                                            Account_Weight_DC = n.Account_Weight_DC, 
                                                            Volume_Fact = n.Volume_Fact, 
                                                            Charge_Weight_Fact = n.Charge_Weight_Fact, 
                                                            Bulk_Weight_Fact = n.Bulk_Weight_Fact, 
                                                            Bulk_Percent_Fact = n.Bulk_Percent_Fact, 
                                                            Account_Weight_Fact = n.Account_Weight_Fact, 
                                                            Marks_H = n.Marks_H, 
                                                            EN_Name_H = n.EN_Name_H, 
                                                            Book_Flat_Code = n.Book_Flat_Code, 
                                                            Airways_Code = n.Airways_Code,
                                                            FLIGHT_No = n.Flight_No, 
                                                            MBL = n.MBL, 
                                                            HBL = n.HBL, 
                                                            Flight_Date_Want = n.Flight_Date_Want, 
                                                            Book_Remark = n.Book_Remark, 
                                                            Delivery_Point = n.Delivery_Point, 
                                                            Warehouse_Code = n.Warehouse_Code, 
                                                            RK_Date = n.RK_Date, 
                                                            CK_Date = n.CK_Date, 
                                                            CH_Name = n.CH_Name, 
                                                            AMS = n.AMS, 
                                                            Status = n.Status, 
                                                            Remark = n.Remark, 
                                                            OperatingPoint = n.OperatingPoint}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(OPS_EntrustmentInfor), datarows);
        }

        /// <summary>
        /// 保存退关或取消退关
        /// </summary>
        /// <param name="ArrId">退关 委托Id</param>
        /// <param name="NeedConfirm">强制删除</param>
        /// <returns></returns>
        public string Batch_TG(IEnumerable<int> ArrId,bool NeedConfirm = false)
        {
            try
            {
                var bms_Bill_ArRep = _repository.GetRepository<Bms_Bill_Ar>();
                var bms_Bill_ApRep = _repository.GetRepository<Bms_Bill_Ap>();
                var idarr = ArrId.Distinct().Select(x => (int?)x);
                //var ArrMBL = _repository.Queryable().Where(x => idarr.Contains(x.MBLId)).Select(x => x.MBL).ToList();
                //ArrMBL = ArrMBL.Distinct().ToList();
                //var ArrOPS_EttInfor = _repository.Queryable().Where(x => ArrMBL.Contains(x.MBL)).Include(x=>x.OPS_M_Order).ToList().Where(x=>!x.Is_TG);
                var ArrOPS_EttInfor = _repository.Queryable().Where(x => idarr.Contains(x.MBLId)).Include(x => x.OPS_M_Order).ToList().Where(x => !x.Is_TG);
                var ArrMBLId = ArrOPS_EttInfor.Select(x => x.MBLId).Distinct();
                var ArrBmsAr = bms_Bill_ArRep.Queryable().Where(x => ArrMBLId.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                var ArrBmsAp = bms_Bill_ApRep.Queryable().Where(x => ArrMBLId.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                //总单应付行分摊数据
                var ArrBmsAp_IdZD = ArrBmsAp.Where(x => x.IsMBLJS).Select(x => x.Id);
                var ArrBmsApZD = bms_Bill_ApRep.Queryable().Where(x => !x.IsMBLJS && ArrBmsAp_IdZD.Contains(x.FTParentId)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();

                //反向从依赖注入中取上下文dbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                if (ArrOPS_EttInfor.Any(item => !item.OPS_M_Order.OPS_BMS_Status && (item.Pieces_Fact > 0 || item.Weight_Fact > 0 || item.Volume_Fact > 0)))
                {
                    return "退关订单中，实际进仓数据（件毛体）含有数据，不能退关" ;
                }
                if (ArrBmsAr.Any(x => x.Sumbmit_Status) || ArrBmsAp.Any(x => x.Sumbmit_Status))
                {
                    return "退关订单中，应付账单含有 已提交 账单，不能退关";
                }
                if (ArrBmsAr.Any(x => x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess) && !NeedConfirm)
                {
                    return "退关订单中，应收账单含有 已审核 账单，是否强制删除?";//NeedConfirm = true
                }
                if (ArrBmsAp.Any(x => x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess) && !NeedConfirm)
                {
                    return "退关订单中，应付账单含有 已审核 账单，是否强制删除?";//NeedConfirm = true
                }

                foreach (var item in ArrOPS_EttInfor)
                {
                    if (item != null)
                    {
                        int mblid = item.MBLId.ToInt32OrDefault();
                        var ArData = ArrBmsAr.Where(x => x.Ops_M_OrdId == mblid);
                        var ApData = ArrBmsAp.Where(x => x.Ops_M_OrdId == mblid);
                        var OOPS_M_Ord = new OPS_M_Order();
                        if (item.OPS_M_Order != null && item.OPS_M_Order.Id > 0)
                        {
                            OOPS_M_Ord = item.OPS_M_Order;
                        }
                        #region 总单应付行 删除 分摊数据

                        if (OOPS_M_Ord.Id > 0 && OOPS_M_Ord.OPS_BMS_Status)
                        {
                            //总单应付行 删除 分摊数据
                            var QApDataID = ApData.Where(x => x.IsMBLJS).Select(x => x.Id);
                            var QArrBmsApZD = ArrBmsApZD.Where(x => QApDataID.Contains(x.FTParentId));
                            foreach (var ap in QArrBmsApZD)
                            {
                                List<Bms_Bill_Ap_Dtl> ArrBms_Bill_Ap_Dtl = new List<Bms_Bill_Ap_Dtl>();
                                if (ap.ArrBms_Bill_Ap_Dtl != null && ap.ArrBms_Bill_Ap_Dtl.Any())
                                {
                                    ArrBms_Bill_Ap_Dtl = ArrBms_Bill_Ap_Dtl.Concat(ap.ArrBms_Bill_Ap_Dtl).ToList();
                                    foreach (var apDtl in ArrBms_Bill_Ap_Dtl)
                                    {
                                        var apDtlentry = WebdbContxt.Entry(apDtl);
                                        apDtlentry.State = EntityState.Deleted;
                                        apDtl.ObjectState = ObjectState.Deleted;
                                    }
                                }
                                var apentry = WebdbContxt.Entry(ap);
                                apentry.State = EntityState.Deleted;
                                ap.ObjectState = ObjectState.Deleted;
                            }
                        }

                        #endregion
                        foreach (var ar in ArData)
                        {
                            List<Bms_Bill_Ar_Dtl> ArrBms_Bill_Ar_Dtl = new List<Bms_Bill_Ar_Dtl>();
                            if (ar.ArrBms_Bill_Ar_Dtl != null && ar.ArrBms_Bill_Ar_Dtl.Any())
                            {
                                ArrBms_Bill_Ar_Dtl = ArrBms_Bill_Ar_Dtl.Concat(ar.ArrBms_Bill_Ar_Dtl).ToList();
                                foreach (var arDtl in ArrBms_Bill_Ar_Dtl)
                                {
                                    var arDtlentry = WebdbContxt.Entry(arDtl);
                                    arDtlentry.State = EntityState.Deleted;
                                    //arDtl.ObjectState = ObjectState.Deleted;
                                }
                            }
                            var arentry = WebdbContxt.Entry(ar);
                            arentry.State = EntityState.Deleted;
                            //ar.ObjectState = ObjectState.Deleted;
                        }
                        foreach (var ap in ApData)
                        {
                            List<Bms_Bill_Ap_Dtl> ArrBms_Bill_Ap_Dtl = new List<Bms_Bill_Ap_Dtl>();
                            if (ap.ArrBms_Bill_Ap_Dtl != null && ap.ArrBms_Bill_Ap_Dtl.Any())
                            {
                                ArrBms_Bill_Ap_Dtl = ArrBms_Bill_Ap_Dtl.Concat(ap.ArrBms_Bill_Ap_Dtl).ToList();
                                foreach (var apDtl in ArrBms_Bill_Ap_Dtl)
                                {
                                    var apDtlentry = WebdbContxt.Entry(apDtl);
                                    apDtlentry.State = EntityState.Deleted;
                                    //apDtl.ObjectState = ObjectState.Deleted;
                                }
                            }
                            var apentry = WebdbContxt.Entry(ap);
                            apentry.State = EntityState.Deleted;
                            //ap.ObjectState = ObjectState.Deleted;
                        }
                        //退关 标志
                        var entry = WebdbContxt.Entry(item);
                        entry.State = EntityState.Unchanged;
                        item.ObjectState = ObjectState.Modified;
                        item.Is_TG = true;
                        entry.Property(x => x.Is_TG).IsModified = true;
                    }
                }
                WebdbContxt.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                string ErrMSg = Common.GetExceptionMsg(ex);
                Common.WriteLog_Local("退关失败Batch_TG：" + ErrMSg, "OPS_EttInfor", true, true);
                return ErrMSg;
            }
        }


        /// <summary>
        /// 委托日志写入 记录字段
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="OldSource">旧的数据源</param>
        /// <param name="NowSource">新的数据源</param>
        /// <returns></returns>
        public string DifferenceComparisonE(OPS_EntrustmentInfor OldSource, OPS_EntrustmentInfor NowSource)
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
            if (OldSource.Pieces_Fact != NowSource.Pieces_Fact)
            {
                dcstr += "Pieces_Fact:" + NowSource.Pieces_Fact + ",";
            }
            if (OldSource.Weight_Fact != NowSource.Weight_Fact)
            {
                dcstr += "Weight_Fact:" + NowSource.Weight_Fact + ",";
            }
            if (OldSource.Volume_Fact != NowSource.Volume_Fact)
            {
                dcstr += "Volume_Fact:" + NowSource.Volume_Fact + ",";
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

    }
}