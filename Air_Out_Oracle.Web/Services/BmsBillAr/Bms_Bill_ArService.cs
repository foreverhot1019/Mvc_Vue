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
    public class Bms_Bill_ArService : Service<Bms_Bill_Ar>, IBms_Bill_ArService
    {
        private readonly IRepositoryAsync<Bms_Bill_Ar> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public Bms_Bill_ArService(IRepositoryAsync<Bms_Bill_Ar> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Bms_Bill_Ar").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Bms_Bill_Ar item = new Bms_Bill_Ar();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type bms_bill_artype = item.GetType();
                        PropertyInfo propertyInfo = bms_bill_artype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type bms_bill_artype = item.GetType();
                        PropertyInfo propertyInfo = bms_bill_artype.GetProperty(field.FieldName);
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
            var bms_bill_ar = this.Query(new Bms_Bill_ArQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = bms_bill_ar.Select(n => new
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
                Dk_Operation_Id = n.Dk_Operation_Id,
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

            return ExcelHelper.ExportExcel(typeof(Bms_Bill_Ar), datarows);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(Bms_Bill_Ar OBms_Bill_Ar)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            if (!string.IsNullOrEmpty(OBms_Bill_Ar.Bill_TaxRateType))
            {
                var OBill_TaxRateType = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Bill_TaxRateType" && x.LISTCODE == OBms_Bill_Ar.Bill_TaxRateType).FirstOrDefault();
                ODynamic.Bill_TaxRateTypeNAME = OBill_TaxRateType == null ? "" : OBill_TaxRateType.LISTNAME + (string.IsNullOrEmpty(OBill_TaxRateType.ENAME) ? "" : "-" + OBill_TaxRateType.ENAME);
            }
            return ODynamic;
        }

        /// <summary>
        /// 计算 价，税，价税合计
        /// </summary>
        /// <param name="Bill_HasTax">是否含税</param>
        /// <param name="Bill_TaxRate">税率</param>
        /// <param name="Bill_Account2">实际金额</param>
        /// <returns></returns>
        public dynamic CalcTaxRate(bool Bill_HasTax, decimal Bill_TaxRate, decimal Bill_Account2)
        {
            dynamic Odynamic = new System.Dynamic.ExpandoObject();
            Odynamic.Success = true;
            try
            {
                if (Bill_HasTax)
                {
                    //价（实际金额 / 1+税率）
                    Odynamic.Bill_Amount = Math.Round(Bill_Account2 / (1 + Bill_TaxRate), 2);
                    //税金 （实际金额 - 价）
                    Odynamic.Bill_TaxAmount = Bill_Account2 - Odynamic.Bill_Amount;
                    //价税合计 (实际金额)
                    Odynamic.Bill_AmountTaxTotal = Bill_Account2;
                }
                else
                {
                    //价（实际金额）
                    Odynamic.Bill_Amount = Bill_Account2;
                    //税金 （实际金额 * 税率）
                    Odynamic.Bill_TaxAmount = Bill_Account2 * Bill_TaxRate;
                    //价税合计 (价+税金)
                    Odynamic.Bill_AmountTaxTotal = Bill_Account2 + Odynamic.Bill_TaxAmount;
                }
            }
            catch
            {
                Odynamic.Success = false;
                //价（实际金额）
                Odynamic.Bill_Amount = 0.00;
                //税金 （实际金额 * 税率）
                Odynamic.Bill_TaxAmount = 0.00;
                //价税合计 (价+税金)
                Odynamic.Bill_AmountTaxTotal = 0.00;
            }
            return Odynamic;
        }

        /// <summary>
        /// 应收审核
        /// </summary>
        /// <param name="AuditId">应收账单Id</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        public dynamic Audit(List<int> ArrAuditId, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess)
        {
            List<int> ArrBmsBillArId = ArrAuditId;
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                var Bms_Bill_ArRep = _repository;
                if (ArrBmsBillArId == null || ArrBmsBillArId.Any(x => x <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "应收账单-Id 必须大于0！";
                    return ORetAudit;
                }
                else
                {
                    string AuditNo = "";
                    if (!(AuditState >= 0 && AuditState < (int)AirOutEnumType.AuditStatusEnum.AuditSuccess))
                    {
                        AuditNo = SequenceBuilder.NextAuditNo(true);//获取审核 序号
                    }
                    var WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                    var ArrBms_Bill_Ar = Bms_Bill_ArRep.Queryable().Where(x => ArrBmsBillArId.Contains(x.Id) && x.Status == AirOutEnumType.UseStatusEnum.Enable).ToList();
                    if (ArrBms_Bill_Ar.Any(x => x.Sumbmit_Status))
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "数据已提交，无法操作！";
                        return ORetAudit;
                    }
                    foreach (int BmsBillArId in ArrBmsBillArId)
                    {
                        var OBms_Bill_Ar = ArrBms_Bill_Ar.Where(x => x.Id == BmsBillArId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (OBms_Bill_Ar == null || OBms_Bill_Ar.Id <= 0)
                        {
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = "找不到应收账单，请确定数据是否被其他人删除或作废！";
                            return ORetAudit;
                        }
                        else
                        {
                            OBms_Bill_Ar.AuditStatus = ORetAudit.AuditStatus = (AirOutEnumType.AuditStatusEnum)AuditState;
                            if (OBms_Bill_Ar.AuditStatus >= 0 && OBms_Bill_Ar.AuditStatus < AirOutEnumType.AuditStatusEnum.AuditSuccess)
                            {
                                OBms_Bill_Ar.AuditNo = "";
                                ORetAudit.AuditStatusName = "";
                                OBms_Bill_Ar.AuditId = ORetAudit.AuditId = "";
                                OBms_Bill_Ar.AuditDate = ORetAudit.AuditDate = null;
                                OBms_Bill_Ar.AuditName = ORetAudit.AuditName = "";
                            }
                            else
                            {
                                ORetAudit.AuditStatusName = Common.GetEnumDisplay(ORetAudit.AuditStatus);
                                OBms_Bill_Ar.AuditNo = AuditNo;
                                OBms_Bill_Ar.AuditId = ORetAudit.AuditId = AirOut.Web.Controllers.Utility.CurrentAppUser.Id;
                                OBms_Bill_Ar.AuditDate = ORetAudit.AuditDate = DateTime.Now;
                                OBms_Bill_Ar.AuditName = ORetAudit.AuditName = AirOut.Web.Controllers.Utility.CurrentAppUser.UserNameDesc;
                            }

                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(OBms_Bill_Ar);
                            OBms_Bill_Ar.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
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