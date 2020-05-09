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
    public class Bms_Bill_ApService : Service<Bms_Bill_Ap>, IBms_Bill_ApService
    {
        private readonly IRepositoryAsync<Bms_Bill_Ap> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public Bms_Bill_ApService(IRepositoryAsync<Bms_Bill_Ap> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Bms_Bill_Ap").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Bms_Bill_Ap item = new Bms_Bill_Ap();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type bms_bill_aptype = item.GetType();
                        PropertyInfo propertyInfo = bms_bill_aptype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type bms_bill_aptype = item.GetType();
                        PropertyInfo propertyInfo = bms_bill_aptype.GetProperty(field.FieldName);
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
            var bms_bill_ap = this.Query(new Bms_Bill_ApQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = bms_bill_ap.Select(n => new
            {
                Id = n.Id,
                Dzbh = n.Dzbh,
                Line_No = n.Line_No,
                Bill_Account = n.Bill_Account,
                Bill_Account2 = n.Bill_Account2,
                Money_Code = n.Money_Code,
                Payway = n.Payway,
                Bill_Object_Id = n.Bill_Object_Id,
                Bill_Type = n.Bill_Type,
                Bill_Date = n.Bill_Date,
                Summary = n.Summary,
                Remark = n.Remark,
                Create_Status = n.Create_Status,
                Bill_Object_Name = n.Bill_Object_Name,
                Org_Money_Code = n.Org_Money_Code,
                Org_Bill_Account2 = n.Org_Bill_Account2,
                Nowent_Acc = n.Nowent_Acc,
                Dsdf_Status = n.Dsdf_Status,
                AuditId = n.AuditId,
                AuditName = n.AuditName,
                AuditDate = n.AuditDate,
                AuditStatus = n.AuditStatus,
                Cancel_Id = n.Cancel_Id,
                Cancel_Name = n.Cancel_Name,
                Cancel_Date = n.Cancel_Date,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Bms_Bill_Ap), datarows);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(Bms_Bill_Ap OBms_Bill_Ap)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            if (!string.IsNullOrEmpty(OBms_Bill_Ap.Bill_TaxRateType))
            {
                var OBill_TaxRateType = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Bill_TaxRateType" && x.LISTCODE == OBms_Bill_Ap.Bill_TaxRateType).FirstOrDefault();
                ODynamic.Bill_TaxRateTypeNAME = OBill_TaxRateType == null ? "" : OBill_TaxRateType.LISTNAME + (string.IsNullOrEmpty(OBill_TaxRateType.ENAME) ? "" : "-" + OBill_TaxRateType.ENAME);
            }
            return ODynamic;
        }

        /// <summary>
        /// 插入空总单数据
        /// 以便总单结算 分摊
        /// </summary>
        /// <param name="MBL">总单号</param>
        public string InsertFT_EntrustmentInfor(string MBL)
        {
            string ErrMsg = "";
            IEnumerable<Notification> ArrNotification = new List<Notification>();
            try
            {
                MBL = Common.RemoveNotNumber(MBL);
                var tf = Common.validataMBL(MBL);
                if (!tf)
                {
                    ErrMsg = "总单号格式不正确！";
                    return ErrMsg;
                }
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                var OPS_M_OrderRep = _repository.GetRepository<OPS_M_Order>();
                var OPS_EntrustmentInforRep = _repository.GetRepository<OPS_EntrustmentInfor>();
                ArrNotification = (IEnumerable<Notification>)CacheHelper.Get_SetCache(Common.CacheNameS.Notification);
                var SDate = DateTime.Now.Date.AddDays(-15);
                var EDate = DateTime.Now.Date.AddDays(16);
                var QOPS_EntrustmentInfor = OPS_EntrustmentInforRep.Queryable().AsNoTracking().Where(x => !x.Is_TG && x.Flight_Date_Want > SDate && x.Flight_Date_Want < EDate && x.MBL == MBL).ToList();
                //QOPS_EntrustmentInfor.Select(x=>x.MBLId);
                var QOPS_M_Order = OPS_M_OrderRep.Queryable().AsNoTracking().Where(x => x.OPS_EntrustmentInfors.Any(n => !n.Is_TG) && x.Flight_Date_Want > SDate && x.Flight_Date_Want < EDate && x.MBL == MBL).ToList();
                
                if (QOPS_EntrustmentInfor != null && QOPS_EntrustmentInfor.Any() && QOPS_EntrustmentInfor.Count > 1)
                {
                    if (!QOPS_M_Order.Any(x => x.OPS_BMS_Status))
                    {
                        var OOPS_EntrustmentInfor = QOPS_EntrustmentInfor.FirstOrDefault();
                        OPS_M_Order OOPS_M_Order = new Models.OPS_M_Order();
                        OOPS_M_Order.MBL = MBL;
                        OOPS_M_Order.OPS_BMS_Status = true;
                        OOPS_M_Order.Flight_No = OOPS_EntrustmentInfor.Flight_No;
                        OOPS_M_Order.Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;
                        //OOPS_M_Order.Account_Weight_Fact = OOPS_EntrustmentInfor.Account_Weight_Fact;
                        OOPS_M_Order.Depart_Port = OOPS_EntrustmentInfor.Depart_Port;
                        OOPS_M_Order.End_Port = OOPS_EntrustmentInfor.End_Port;
                        OOPS_M_Order.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                        OOPS_M_Order.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                        //OPS_M_OrderRep.Insert(OOPS_M_Order);
                        WebdbContxt.Entry(OOPS_M_Order).State = EntityState.Added;

                        OPS_EntrustmentInfor OOpsEttInfor = new OPS_EntrustmentInfor();
                        OOpsEttInfor.MBL = MBL;
                        OOpsEttInfor.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No(OOPS_EntrustmentInfor.Flight_Date_Want);
                        OOpsEttInfor.MBLId = OOPS_M_Order.Id;
                        OOpsEttInfor.Flight_No = OOPS_EntrustmentInfor.Flight_No;
                        OOpsEttInfor.Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;
                        //OOpsEttInfor.Account_Weight_Fact = OOPS_EntrustmentInfor.Account_Weight_Fact;
                        OOpsEttInfor.Depart_Port = OOPS_EntrustmentInfor.Depart_Port;
                        OOpsEttInfor.End_Port = OOPS_EntrustmentInfor.End_Port;
                        OOpsEttInfor.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                        OOpsEttInfor.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                        //OPS_EntrustmentInforRep.Insert(OOpsEttInfor);
                        WebdbContxt.Entry(OOpsEttInfor).State = EntityState.Added;

                        WebdbContxt.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ErrMsg = Common.GetExceptionMsg(ex);
                    int ONotificationId = 0;
                    if (ArrNotification == null || !ArrNotification.Any())
                        ONotificationId = 0;
                    else
                    {
                        var ONotification = ArrNotification.Where(x => x.Name == "Sys").Select(x => x.Id).FirstOrDefault();
                        if (ONotification > 0)
                            ONotificationId = ONotification;
                    }
                    Message OMessage = new Message();
                    OMessage.Content = "插入总单结算错误(Bms_Bill_ApService-InsertFT_EntrustmentInfor)：" + ErrMsg;
                    OMessage.CreatedBy = AirOut.Web.Controllers.Utility.CurrentAppUser.UserName;
                    OMessage.CreatedDate = OMessage.NewDate = DateTime.Now;
                    OMessage.Key1 = MBL;
                    OMessage.NotificationId = ONotificationId;
                    OMessage.Type = MessageType.Error.ToString();
                    OMessage.IsSended = false;
                    OMessage.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                    Common.AddMessageToRedis(OMessage, Common.RedisLogMsgType.SQLMessage, Common.RedisKeyMessageLog);
                }
                finally
                {
                    ErrMsg = "未知错误"+Common.GetExceptionMsg(ex);
                    Common.AddMessageToRedis("插入总单结算错误(Bms_Bill_ApService-InsertFT_EntrustmentInfor)：" + ErrMsg, Common.RedisLogMsgType.LocalLog, Common.RedisKeyLocalLog, "Bms_Bill_ApService");
                }
            }
            return ErrMsg;
        }

        /// <summary>
        /// 应付审核
        /// </summary>
        /// <param name="AuditId">应付账单Id</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        public dynamic Audit(List<int> ArrAuditId, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess)
        {
            List<int> ArrBmsBillApId = ArrAuditId;
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                var Bms_Bill_ApRep = _repository;
                if (ArrBmsBillApId == null || ArrBmsBillApId.Any(x => x <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "应付账单-Id 必须大于0！";
                    return ORetAudit;
                }
                else
                {
                    string AuditNo = "";
                    if (!(AuditState >= 0 && AuditState < (int)AirOutEnumType.AuditStatusEnum.AuditSuccess))
                    {
                        AuditNo = SequenceBuilder.NextAuditNo(false);//获取审核 序号
                    }
                    var WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                    var ArrBms_Bill_Ap = Bms_Bill_ApRep.Queryable().Where(x => ArrBmsBillApId.Contains(x.Id) && x.Status == AirOutEnumType.UseStatusEnum.Enable).ToList();
                    if (ArrBms_Bill_Ap.Any(x => x.Sumbmit_Status))
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "数据已提交，无法操作！";
                        return ORetAudit;
                    }
                    foreach (int BmsBillApId in ArrBmsBillApId)
                    {
                        var OBms_Bill_Ap = ArrBms_Bill_Ap.Where(x => x.Id == BmsBillApId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (OBms_Bill_Ap == null || OBms_Bill_Ap.Id <= 0)
                        {
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = "找不到应付账单，请确定数据是否被其他人删除或作废！";
                            return ORetAudit;
                        }
                        else
                        {
                            OBms_Bill_Ap.AuditStatus = ORetAudit.AuditStatus = (AirOutEnumType.AuditStatusEnum)AuditState;
                            if (OBms_Bill_Ap.AuditStatus >= 0 && OBms_Bill_Ap.AuditStatus < AirOutEnumType.AuditStatusEnum.AuditSuccess)
                            {
                                ORetAudit.AuditStatusName = "";
                                OBms_Bill_Ap.AuditNo = "";
                                OBms_Bill_Ap.AuditId = ORetAudit.AuditId = "";
                                OBms_Bill_Ap.AuditDate = ORetAudit.AuditDate = null;
                                OBms_Bill_Ap.AuditName = ORetAudit.AuditName = "";
                            }
                            else
                            {
                                ORetAudit.AuditStatusName = Common.GetEnumDisplay(ORetAudit.AuditStatus);
                                OBms_Bill_Ap.AuditNo = AuditNo;
                                OBms_Bill_Ap.AuditId = ORetAudit.AuditId = AirOut.Web.Controllers.Utility.CurrentAppUser.Id;
                                OBms_Bill_Ap.AuditDate = ORetAudit.AuditDate = DateTime.Now;
                                OBms_Bill_Ap.AuditName = ORetAudit.AuditName = AirOut.Web.Controllers.Utility.CurrentAppUser.UserNameDesc;
                            }

                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry = WebdbContxt.Entry<Bms_Bill_Ap>(OBms_Bill_Ap);
                            OBms_Bill_Ap.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            entry.Property(t => t.AuditNo).IsModified = true; //设置要更新的属性
                            entry.Property(t => t.AuditStatus).IsModified = true; //设置要更新的属性
                            entry.Property(t => t.AuditId).IsModified = true; //设置要更新的属性
                            entry.Property(t => t.AuditDate).IsModified = true; //设置要更新的属性
                            entry.Property(t => t.AuditName).IsModified = true; //设置要更新的属性
                        }
                    }
                    WebdbContxt.SaveChanges();

                    ORetAudit.Success = true;
                    ORetAudit.ErrMsg = "";
                    return ORetAudit;
                }
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "审核出错：<br/>" + errMsg;
                return ORetAudit;
            }
        }
    }
}