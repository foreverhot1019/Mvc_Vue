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
    public class FinanceService : IFinanceService
    {
        private readonly IDataTableImportMappingService MappingService;
        private readonly IBms_Bill_ApService Bms_Bill_ApService;
        private readonly IBms_Bill_Ap_DtlService Bms_Bill_Ap_DtlService;
        private readonly IBms_Bill_ArService Bms_Bill_ArService;
        private readonly IBms_Bill_Ar_DtlService Bms_Bill_Ar_DtlService;
        private readonly IOPS_EntrustmentInforService OPS_EntrustmentInforService;

        public FinanceService(IDataTableImportMappingService _mappingservice,
            IBms_Bill_ApService _bms_bill_apservice,
            IBms_Bill_Ap_DtlService _bms_bill_ap_dtlservice,
            IBms_Bill_ArService _bms_bill_arservice,
            IBms_Bill_Ar_DtlService _bms_bill_ar_dtlservice,
            IOPS_EntrustmentInforService _ops_entrustmentinforservice)
        {
            MappingService = _mappingservice;
            Bms_Bill_ApService = _bms_bill_apservice;
            Bms_Bill_Ap_DtlService = _bms_bill_ap_dtlservice;
            Bms_Bill_ArService = _bms_bill_arservice;
            Bms_Bill_Ar_DtlService = _bms_bill_ar_dtlservice;
            OPS_EntrustmentInforService = _ops_entrustmentinforservice;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="datatable"></param>
        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = MappingService.Queryable().Where(x => x.EntitySetName == "Finance").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Finance item = new Finance();

                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type mailreceivertype = item.GetType();
                        PropertyInfo propertyInfo = mailreceivertype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type mailreceivertype = item.GetType();
                        PropertyInfo propertyInfo = mailreceivertype.GetProperty(field.FieldName);
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
                #region 插入数据操作

                #endregion
            }
        }

        /// <summary>
        /// 获取搜索 结果
        /// </summary>
        /// <param name="filterRules">搜索条件</param>
        /// <param name="filterRules">增加搜索条件</param>
        /// <returns></returns>
        public IQueryable<Finance> GetData(string filterRules = "", IEnumerable<filterRule> AddfilterRules = null)
        {
            IEnumerable<filterRule> filters = new List<filterRule>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            if (filters != null && filters.Any())
            {
                filters = filters.Where(x => !string.IsNullOrWhiteSpace(x.value));//去除空的 数据
            }
            //过滤增加查询
            if (AddfilterRules != null && AddfilterRules.Any())
            {
                AddfilterRules = AddfilterRules.Where(x => !filters.Any(n => n.field == x.field && n.value == x.value));
                IEnumerable<filterRule> ArrSame = AddfilterRules.Where(x => filters.Any(n => n.field == x.field));
                filters = filters.Concat(ArrSame);
            }

            var Qbms_bill_ar = Bms_Bill_ArService.Queryable();
            var QBms_Bill_Ar_DtlService = Bms_Bill_Ar_DtlService.Queryable();
            var Qbms_bill_ap = Bms_Bill_ApService.Queryable();
            var QBms_Bill_Ap_DtlService = Bms_Bill_Ap_DtlService.Queryable();
            var QEttInfor = OPS_EntrustmentInforService.Queryable().Where(x => x.Is_TG != true);
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);

            if (filters.Any())
            {
                #region 明细数据搜索

                var hasDtlFilter = false;
                var Qfilters = filters.Where(x => x.field == "Charge_Code" || x.field == "Charge_Desc");
                if (Qfilters.Any())
                {
                    //费用代码
                    Qfilters = filters.Where(x => x.field == "Charge_Code");
                    if (Qfilters.Any())
                    {
                        //if (Qfilters.Count() > 1)
                        //{
                        //    var ArrVal = Qfilters.Select(x => x.value);
                        //    QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => ArrVal.Contains(x.Charge_Code));
                        //    QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => ArrVal.Contains(x.Charge_Code));
                        //}
                        //else
                        //{
                        var val = Qfilters.FirstOrDefault().value;
                        QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => x.Charge_Code == val);
                        QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => x.Charge_Code == val);
                        //}
                        hasDtlFilter = true;
                    }
                    //费用名称
                    Qfilters = filters.Where(x => x.field == "Charge_Desc");
                    if (Qfilters.Any())
                    {
                        //if (Qfilters.Count() > 1)
                        //{
                        //    var ArrVal = Qfilters.Select(x => x.value);
                        //    QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => ArrVal.Contains(x.Charge_Desc));
                        //    QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => ArrVal.Contains(x.Charge_Desc));
                        //}
                        //else
                        //{
                        var val = Qfilters.FirstOrDefault().value;
                        QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => x.Charge_Desc.Contains(val));
                        QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => x.Charge_Desc.Contains(val));
                        //}
                        hasDtlFilter = true;
                    }
                    if (hasDtlFilter)
                    {
                        //搜索条件 转换成 关联字段 在搜索
                        var QBmsBillArDtlService = QBms_Bill_Ar_DtlService.Select(x => x.Bms_Bill_Ar_Id);
                        Qbms_bill_ar = Qbms_bill_ar.Where(x => QBmsBillArDtlService.Contains(x.Id));
                        var QBmsBillApDtlService = QBms_Bill_Ap_DtlService.Select(x => x.Bms_Bill_Ap_Id);
                        Qbms_bill_ap = Qbms_bill_ap.Where(x => QBmsBillApDtlService.Contains(x.Id));
                    }
                }

                #endregion
            }

            #region 应收应付 合起来

            var Qquery = (from n in Qbms_bill_ar
                          select new Finance
                          {
                              Id = n.Id,
                              IsAr = true,//应付账单-true:应付账单,false:应收账单
                              MBL = n.MBL,//总单号
                              IsMBLJS = false,//总单结算-分摊数据时产生
                              FTParentId = 0,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                              Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                              //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                              Dzbh = n.Dzbh,//业务编号
                              Bill_Type = n.Bill_Type,//帐单类型
                              Line_No = n.Line_No,//序号
                              Money_Code = n.Money_Code,//币种
                              Money_CodeNAME = string.Empty,
                              Bill_Account2 = n.Bill_Account2,//实际金额
                              Bill_Account = n.Bill_Account,//理论金额
                              Bill_Amount = n.Bill_Amount,//价
                              Bill_TaxRateType = n.Bill_TaxRateType,//税率类型
                              Bill_TaxRate = n.Bill_TaxRate,//税率
                              Bill_HasTax = n.Bill_HasTax,//含税/不含税
                              Bill_TaxAmount = n.Bill_TaxAmount,//税金
                              Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                              Carriage_Account_Code = string.Empty,//结算方代码
                              Carriage_Account_CodeNAME = string.Empty,//结算方名称
                              Bill_Object_Id = n.Bill_Object_Id,//供方代码
                              Bill_Object_Name = n.Bill_Object_Name,//供方名称
                              Bill_Date = n.Bill_Date,//帐单日期
                              Payway = n.Payway,//支付方式
                              PaywayNAME = string.Empty,
                              Remark = n.Remark,//备注信息
                              AuditNo = n.AuditNo,//审核号
                              AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                              Cancel_Status = n.Cancel_Status,//作废标志
                              Cancel_Date = n.Cancel_Date,//作废日期
                              Sumbmit_Status = n.Sumbmit_Status,//提交标志
                              Sumbmit_No = n.Sumbmit_No,//提交号
                              Sumbmit_Name = n.Sumbmit_Name,//提交人
                              Sumbmit_Id = n.Sumbmit_Id,    //提交人
                              Sumbmit_Date = n.Sumbmit_Date,//提交时间
                              SignIn_Status = false,//签收标志
                              SignIn_No = string.Empty,//签收号
                              SignIn_Date =null,//签收时间
                              SignIn_Name = string.Empty,//签收人
                              Invoice_Status = n.Invoice_Status,//开票标志
                              Invoice_No = n.Invoice_No,//开票号码
                              Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                              Invoice_FeeType = n.Invoice_FeeType,//开票费目
                              Invoice_FeeTypeNAME = string.Empty,//开票费目名称
                              Invoice_TaxRateType = n.Invoice_TaxRateType, //开票税率
                              Invoice_HasTax = n.Invoice_HasTax,//开票是否含税
                              ECCInvoice_Rate = n.ECCInvoice_Rate,//ECC开票汇率
                              ECC_Code = string.Empty,//ECC开票费目
                              Invoice_Id = n.Invoice_Id,//开票人Id
                              Invoice_Name = n.Invoice_Name,//开票人
                              Invoice_Date = n.Invoice_Date,//开票日期
                              Invoice_Remark = n.Invoice_Remark,//开票要求
                              SellAccount_Status = n.SellAccount_Status,//销账标志
                              SellAccount_Name = n.SellAccount_Name,//销账人
                              SellAccount_Date = n.SellAccount_Date,//销账日期
                              Create_Status = n.Create_Status,//产生标志
                              Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                              Flight_Date_Want = null,
                              End_Port = string.Empty,
                              End_PortNAME = string.Empty,
                              Sumbmit_ECCNo = n.Sumbmit_ECCNo,//ECC发票号 
                              SignIn_ECCNo = string.Empty,//采购订单号 
                              ECC_Status = n.ECC_Status,//ECC状态 
                              ECC_StatusMsg = n.ECC_StatusMsg,//ECC状态信息 
                              ECC_InvoiceSendDate = n.ECC_InvoiceSendDate,//ECC发票发送时间
                              ECC_InvoiceRecvDate = n.ECC_InvoiceRecvDate,//ECC发票接收时间
                              OperatingPoint = n.OperatingPoint,//操作点-操作点
                              ADDID = n.ADDID,//新增人
                              ADDWHO = n.ADDWHO,//新增人
                              ADDTS = n.ADDTS,//新增时间
                              EDITWHO = n.EDITWHO,//修改人
                              EDITWHONAME = string.Empty,
                              EDITTS = n.EDITTS,//修改时间
                              EDITID = n.EDITID,//修改人
                              SallerId = null,//销售
                              SallerName = string.Empty,//销售人
                              BillEDITWHO = n.BillEDITWHO,//修改人
                              BillEDITWHONAME = string.Empty,
                              BillEDITTS = n.BillEDITTS,//修改时间
                              BillEDITID = n.BillEDITID,//修改人
                          }).Concat(
                             from n in Qbms_bill_ap
                             select new Finance
                             {
                                 Id = n.Id,
                                 IsAr = false,//应付账单-true:应付账单,false:应收账单
                                 MBL = n.MBL,//总单号
                                 IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                                 FTParentId = n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                 Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                 //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                                 Dzbh = n.Dzbh,//业务编号
                                 Bill_Type = n.Bill_Type,//帐单类型
                                 Line_No = n.Line_No,//序号
                                 Money_Code = n.Money_Code,//币种
                                 Money_CodeNAME = string.Empty,
                                 Bill_Account2 = n.Bill_Account2,//实际金额
                                 Bill_Account = n.Bill_Account,//理论金额
                                 Bill_Amount = n.Bill_Amount,//价
                                 Bill_TaxRateType = n.Bill_TaxRateType,//税率类型
                                 Bill_TaxRate = n.Bill_TaxRate,//税率
                                 Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                 Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                 Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                 Carriage_Account_Code = string.Empty,//结算方代码
                                 Carriage_Account_CodeNAME = string.Empty,//结算方名称
                                 Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                 Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                 Bill_Date = n.Bill_Date,//帐单日期
                                 Payway = n.Payway,//支付方式
                                 PaywayNAME = string.Empty,
                                 Remark = n.Remark,//备注信息
                                 AuditNo = n.AuditNo,//审核号
                                 AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                 Cancel_Status = n.Cancel_Status,//作废标志
                                 Cancel_Date = n.Cancel_Date,//作废日期
                                 Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                 Sumbmit_No = n.Sumbmit_No,//提交号
                                 Sumbmit_Name = n.Sumbmit_Name,//提交人
                                 Sumbmit_Id = n.Sumbmit_Id,    //提交人
                                 Sumbmit_Date = n.Sumbmit_Date,//提交时间
                                 SignIn_Status = n.SignIn_Status,//签收标志
                                 SignIn_No = n.SignIn_No,//签收号
                                 SignIn_Date = n.SignIn_Date,//签收时间
                                 SignIn_Name = n.SignIn_Name,//签收人
                                 Invoice_Status = n.Invoice_Status,//开票标志
                                 Invoice_No = n.Invoice_No,//开票号码
                                 Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                                 Invoice_FeeType = n.Invoice_FeeType,//开票费目
                                 Invoice_FeeTypeNAME = string.Empty,//开票费目名称
                                 Invoice_TaxRateType = n.Invoice_TaxRateType, //开票税率
                                 Invoice_HasTax = n.Invoice_HasTax,//开票是否含税
                                 ECCInvoice_Rate = n.ECCInvoice_Rate,//ECC开票汇率
                                 ECC_Code = string.Empty,//ECC开票费目
                                 Invoice_Id = n.Invoice_Id,//开票人Id
                                 Invoice_Name = n.Invoice_Name,//开票人
                                 Invoice_Date = n.Invoice_Date,//开票日期
                                 Invoice_Remark = n.Invoice_Remark,//开票要求
                                 SellAccount_Status = n.SellAccount_Status,//销账标志
                                 SellAccount_Name = n.SellAccount_Name,//销账人
                                 SellAccount_Date = n.SellAccount_Date,//销账日期
                                 Create_Status = n.Create_Status,//产生标志
                                 Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                 Flight_Date_Want = null,
                                 End_Port = string.Empty,
                                 End_PortNAME = string.Empty,
                                 Sumbmit_ECCNo = string.Empty,//ECC发票号 
                                 SignIn_ECCNo = n.SignIn_ECCNo,//采购订单号 
                                 ECC_Status = n.ECC_Status,//ECC状态 
                                 ECC_StatusMsg = n.ECC_StatusMsg,//ECC状态信息 
                                 ECC_InvoiceSendDate = n.ECC_InvoiceSendDate,//ECC发票发送时间
                                 ECC_InvoiceRecvDate = n.ECC_InvoiceRecvDate,//ECC发票接收时间
                                 OperatingPoint = n.OperatingPoint,//操作点-操作点
                                 ADDID = n.ADDID,//新增人
                                 ADDWHO = n.ADDWHO,//新增人
                                 ADDTS = n.ADDTS,//新增时间
                                 EDITWHO = n.EDITWHO,//修改人
                                 EDITWHONAME = string.Empty,
                                 EDITTS = n.EDITTS,//修改时间
                                 EDITID = n.EDITID,//修改人
                                 SallerId = null,//销售
                                 SallerName = string.Empty,//销售人
                                 BillEDITWHO = n.BillEDITWHO,//修改人
                                 BillEDITWHONAME = string.Empty,
                                 BillEDITTS = n.BillEDITTS,//修改时间
                                 BillEDITID = n.BillEDITID,//修改人
                             }).AsQueryable();

            #endregion;

            #region 查询

            if (filters != null && filters.Any())
            {
                var Qfilters = filters.Where(x => !string.IsNullOrWhiteSpace(x.field));//去除 空条件列

                //搜索 应收账单 null:应收/应付 true：应收 false：应付
                bool? IsAr = null;
                //账单类型
                Qfilters = filters.Where(x => x.field == "Bill_Type");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var ArrFY = val.Split(',').Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
                    if (ArrFY.Any())
                    {
                        List<string> YSFY = new List<string>() { "D/N", "FP" };
                        List<string> YFFY = new List<string>() { "C/N", "FTYF" };
                        var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Bill_Type_Ar");
                        if (QArrBD_DEFDOC_LIST.Any())
                            YSFY = QArrBD_DEFDOC_LIST.Select(x => x.LISTCODE).ToList();
                        QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Bill_Type_Ap");
                        if (QArrBD_DEFDOC_LIST.Any())
                            YFFY = QArrBD_DEFDOC_LIST.Select(x=>x.LISTCODE).ToList();
                        if (ArrFY.Any(x => YSFY.Contains(x)) && !ArrFY.Any(x => YFFY.Contains(x)))
                        {
                            IsAr = true;
                        }
                        if (!ArrFY.Any(x => YSFY.Contains(x)) && ArrFY.Any(x => YFFY.Contains(x)))
                        {
                            IsAr = false;
                        }
                        
                        switch (IsAr)
                        {
                            case true:
                                #region 应收

                                Qquery = (from n in Qbms_bill_ar
                                          select new Finance
                                          {
                                              Id = n.Id,
                                              IsAr = true,//应付账单-true:应付账单,false:应收账单
                                              MBL = n.MBL,//总单号
                                              IsMBLJS = false,//总单结算-分摊数据时产生
                                              FTParentId = 0,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                              Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                              //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                                              Dzbh = n.Dzbh,//业务编号
                                              Bill_Type = n.Bill_Type,//帐单类型
                                              Line_No = n.Line_No,//序号
                                              Money_Code = n.Money_Code,//币种
                                              Money_CodeNAME = string.Empty,
                                              Bill_Account2 = n.Bill_Account2,//实际金额
                                              Bill_Account = n.Bill_Account,//理论金额
                                              Bill_Amount = n.Bill_Amount,//价
                                              Bill_TaxRateType = n.Bill_TaxRateType,//税率类型
                                              Bill_TaxRate = n.Bill_TaxRate,//税率
                                              Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                              Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                              Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                              Carriage_Account_Code = string.Empty,//结算方代码
                                              Carriage_Account_CodeNAME = string.Empty,//结算方名称
                                              Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                              Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                              Bill_Date = n.Bill_Date,//帐单日期
                                              Payway = n.Payway,//支付方式
                                              PaywayNAME = string.Empty,
                                              Remark = n.Remark,//备注信息
                                              AuditNo = n.AuditNo,         //审核号
                                              AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                              Cancel_Status = n.Cancel_Status,//作废标志
                                              Cancel_Date = n.Cancel_Date,//作废日期
                                              Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                              Sumbmit_No = n.Sumbmit_No,//提交号
                                              Sumbmit_Name = n.Sumbmit_Name,    //提交人
                                              Sumbmit_Id = n.Sumbmit_Id,    //提交人
                                              Sumbmit_Date = n.Sumbmit_Date,    //提交时间
                                              SignIn_Status = false,//签收标志
                                              SignIn_No = "",       //签收号
                                              SignIn_Date = null,//签收时间
                                              SignIn_Name = string.Empty,//签收人
                                              Invoice_Status = n.Invoice_Status,//开票标志
                                              Invoice_No = n.Invoice_No,      //开票号码
                                              Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                                              Invoice_FeeType = n.Invoice_FeeType,//开票费目
                                              Invoice_FeeTypeNAME = string.Empty,//开票费目名称
                                              Invoice_TaxRateType = n.Invoice_TaxRateType, //开票税率
                                              Invoice_HasTax = n.Invoice_HasTax,//开票是否含税
                                              ECCInvoice_Rate = n.ECCInvoice_Rate,//ECC开票汇率
                                              ECC_Code = string.Empty,//ECC开票费目
                                              Invoice_Id = n.Invoice_Id,//开票人Id
                                              Invoice_Name = n.Invoice_Name,//开票人
                                              Invoice_Date = n.Invoice_Date,    //开票日期
                                              Invoice_Remark = n.Invoice_Remark,//开票要求
                                              SellAccount_Status = n.SellAccount_Status,//销账标志
                                              SellAccount_Name = n.SellAccount_Name,//销账人
                                              SellAccount_Date = n.SellAccount_Date,//销账日期
                                              Create_Status = n.Create_Status,//产生标志
                                              Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                              Flight_Date_Want = null,//开航日期
                                              End_Port = string.Empty,//目的港
                                              End_PortNAME = string.Empty,
                                              Sumbmit_ECCNo = n.Sumbmit_ECCNo,//ECC发票号 
                                              SignIn_ECCNo = string.Empty,//采购订单号 
                                              ECC_Status = n.ECC_Status,//ECC状态 
                                              ECC_StatusMsg = n.ECC_StatusMsg,//ECC状态信息 
                                              ECC_InvoiceSendDate = n.ECC_InvoiceSendDate,//ECC发票发送时间
                                              ECC_InvoiceRecvDate = n.ECC_InvoiceRecvDate,//ECC发票接收时间
                                              OperatingPoint = n.OperatingPoint,//操作点-操作点
                                              ADDID = n.ADDID,//新增人
                                              ADDWHO = n.ADDWHO,//新增人
                                              ADDTS = n.ADDTS,//新增时间
                                              EDITWHO = n.EDITWHO,//修改人
                                              EDITWHONAME = string.Empty,
                                              EDITTS = n.EDITTS,//修改时间
                                              EDITID = n.EDITID,//修改人
                                              SallerId = null,//销售
                                              SallerName = string.Empty,//销售人
                                              BillEDITWHO = n.BillEDITWHO,//修改人
                                              BillEDITWHONAME = string.Empty,
                                              BillEDITTS = n.BillEDITTS,//修改时间
                                              BillEDITID = n.BillEDITID,//修改人
                                          });
                                IsAr = true;

                                #endregion
                                break;
                            case false:
                                #region 应付

                                Qquery = (from n in Qbms_bill_ap
                                          select new Finance
                                          {
                                              Id = n.Id,
                                              IsAr = false,//应付账单-true:应付账单,false:应收账单
                                              MBL = n.MBL,//总单号
                                              IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                                              FTParentId = 0,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                              Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                              //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                                              Dzbh = n.Dzbh,//业务编号
                                              Bill_Type = n.Bill_Type,//帐单类型
                                              Line_No = n.Line_No,//序号
                                              Money_Code = n.Money_Code,//币种
                                              Money_CodeNAME = string.Empty,
                                              Bill_Account2 = n.Bill_Account2,//实际金额
                                              Bill_Account = n.Bill_Account,//理论金额
                                              Bill_Amount = n.Bill_Amount,//价
                                              Bill_TaxRateType = n.Bill_TaxRateType,//税率类型
                                              Bill_TaxRate = n.Bill_TaxRate,//税率
                                              Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                              Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                              Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                              Carriage_Account_Code = string.Empty,//结算方代码
                                              Carriage_Account_CodeNAME = string.Empty,//结算方名称
                                              Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                              Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                              Bill_Date = n.Bill_Date,//帐单日期
                                              Payway = n.Payway,//支付方式
                                              PaywayNAME = string.Empty,
                                              Remark = n.Remark,//备注信息
                                              AuditNo = n.AuditNo,         //审核号
                                              AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                              Cancel_Status = n.Cancel_Status,//作废标志
                                              Cancel_Date = n.Cancel_Date,//作废日期
                                              Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                              Sumbmit_No = n.Sumbmit_No,//提交号
                                              Sumbmit_Name = n.Sumbmit_Name,    //提交人
                                              Sumbmit_Id = n.Sumbmit_Id,    //提交人
                                              Sumbmit_Date = n.Sumbmit_Date,    //提交时间
                                              SignIn_Status = n.SignIn_Status,//签收标志
                                              SignIn_No = n.SignIn_No,       //签收号
                                              SignIn_Date = n.SignIn_Date,//签收时间
                                              SignIn_Name = n.SignIn_Name,//签收人
                                              Invoice_Status = n.Invoice_Status,//开票标志
                                              Invoice_No = n.Invoice_No,      //开票号码
                                              Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                                              Invoice_FeeType = n.Invoice_FeeType,//开票费目
                                              Invoice_FeeTypeNAME = string.Empty,//开票费目名称
                                              Invoice_TaxRateType = n.Invoice_TaxRateType, //开票税率
                                              Invoice_HasTax = n.Invoice_HasTax,//开票是否含税
                                              ECCInvoice_Rate = n.ECCInvoice_Rate,//ECC开票汇率
                                              ECC_Code = string.Empty,//ECC开票费目
                                              Invoice_Id = n.Invoice_Id,//开票人Id
                                              Invoice_Name = n.Invoice_Name,//开票人
                                              Invoice_Date = n.Invoice_Date,//开票日期
                                              Invoice_Remark = n.Invoice_Remark,//开票要求
                                              SellAccount_Status = n.SellAccount_Status,//销账标志
                                              SellAccount_Name = n.SellAccount_Name,//销账人
                                              SellAccount_Date = n.SellAccount_Date,//销账日期
                                              Create_Status = n.Create_Status,//产生标志
                                              Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                              Flight_Date_Want = null,//开航日期
                                              End_Port = string.Empty,//目的港
                                              End_PortNAME = string.Empty,
                                              Sumbmit_ECCNo = string.Empty,//ECC发票号 
                                              SignIn_ECCNo = n.SignIn_ECCNo,//采购订单号 
                                              ECC_Status = n.ECC_Status,//ECC状态 
                                              ECC_StatusMsg = n.ECC_StatusMsg,//ECC状态信息 
                                              ECC_InvoiceSendDate = n.ECC_InvoiceSendDate,//ECC发票发送时间
                                              ECC_InvoiceRecvDate = n.ECC_InvoiceRecvDate,//ECC发票接收时间
                                              OperatingPoint = n.OperatingPoint,//操作点-操作点
                                              ADDID = n.ADDID,//新增人
                                              ADDWHO = n.ADDWHO,//新增人
                                              ADDTS = n.ADDTS,//新增时间
                                              EDITWHO = n.EDITWHO,//修改人
                                              EDITWHONAME = string.Empty,
                                              EDITTS = n.EDITTS,//修改时间
                                              EDITID = n.EDITID,//修改人
                                              SallerId = null,//销售
                                              SallerName = string.Empty,//销售人
                                              BillEDITWHO = n.BillEDITWHO,//修改人
                                              BillEDITWHONAME = string.Empty,
                                              BillEDITTS = n.BillEDITTS,//修改时间
                                              BillEDITID = n.BillEDITID,//修改人
                                          });
                                IsAr = false;

                                #endregion
                                break;
                        }
                        Qquery = Qquery.Where(x => ArrFY.Contains(x.Bill_Type));
                        Qbms_bill_ar = Qbms_bill_ar.Where(x => ArrFY.Contains(x.Bill_Type));
                        Qbms_bill_ap = Qbms_bill_ap.Where(x => ArrFY.Contains(x.Bill_Type));
                    }
                }

                #region 委托/费用明细 数据搜索 关联 条件合并成 Ops_M_OrdId

                List<string> ArrEttsInfor = new List<string>(){
                        "Entrustment_Name",
                        //"Carriage_Account_Code",
                        "Flight_Date_Want_",
                        "_Flight_Date_Want",
                        "ADDWHO",
                        "End_Port",
                        "Is_DCZ",
                        "Charge_Code",
                        "Charge_Desc",
                        "SallerId",
                    };

                Qfilters = filters.Where(x => ArrEttsInfor.Contains(x.field));
                if (Qfilters.Any())
                {
                    #region  委托数据搜索

                    var hasFilter = false;
                    //委托方
                    Qfilters = filters.Where(x => x.field == "Entrustment_Name");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        QEttInfor = QEttInfor.Where(x => x.Entrustment_Name == val);
                        hasFilter = true;
                    }
                    //结算方-合并到 账单里
                    //Qfilters = filters.Where(x => x.field == "Carriage_Account_Code");
                    //if (Qfilters.Any())
                    //{
                    //    var val = Qfilters.FirstOrDefault().value;
                    //    QEttInfor = QEttInfor.Where(x => x.Carriage_Account_Code == val);
                    //    hasFilter = true;
                    //}
                    //航班日期起
                    Qfilters = filters.Where(x => x.field == "_Flight_Date_Want");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        DateTime? date = Common.ParseStrToDateTime(val);
                        if (date != null)
                        {
                            QEttInfor = QEttInfor.Where(x => x.Flight_Date_Want >= (DateTime)date);
                            hasFilter = true;
                        }
                    }
                    //航班日期讫
                    Qfilters = filters.Where(x => x.field == "Flight_Date_Want_");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        DateTime? date = Common.ParseStrToDateTime(val);
                        if (date != null)
                        {
                            date = ((DateTime)date).AddDays(1);
                            QEttInfor = QEttInfor.Where(x => x.Flight_Date_Want < (DateTime)date);
                            hasFilter = true;
                        }
                    }
                    //接单人
                    Qfilters = filters.Where(x => x.field == "ADDWHO");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        QEttInfor = QEttInfor.Where(x => x.ADDWHO == val);
                        hasFilter = true;
                    }
                    //目的港
                    Qfilters = filters.Where(x => x.field == "End_Port");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        QEttInfor = QEttInfor.Where(x => x.End_Port == val);
                        hasFilter = true;
                    }
                    //代操作
                    Qfilters = filters.Where(x => x.field == "Is_DCZ");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        var boolVal = Common.ChangStrToBool(val);
                        QEttInfor = QEttInfor.Where(x => x.Is_DCZ == boolVal);
                        hasFilter = true;
                    }
                    //销售人
                    Qfilters = filters.Where(x => x.field == "SallerId");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        int SallerId = 0;
                        int.TryParse(val, out SallerId);
                        if (SallerId > 0)
                            QEttInfor = QEttInfor.Where(x => x.SallerId == SallerId);
                        else
                            QEttInfor = QEttInfor.Where(x => x.SallerName.StartsWith(val));
                        hasFilter = true;
                    }

                    #endregion

                    #region 明细数据搜索

                    //var hasFilterDtl = false;
                    //Qfilters = filters.Where(x => x.field == "Charge_Code" || x.field == "Charge_Desc");
                    //if (Qfilters.Any())
                    //{
                    //    //费用代码
                    //    Qfilters = filters.Where(x => x.field == "Charge_Code");
                    //    if (Qfilters.Any())
                    //    {
                    //        var val = Qfilters.FirstOrDefault().value;
                    //        if (IsAr == null)
                    //        {
                    //            QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => x.Charge_Code == val);
                    //            QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => x.Charge_Code == val);
                    //        }
                    //        else
                    //        {
                    //            if ((bool)IsAr)
                    //                QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => x.Charge_Code == val);
                    //            else
                    //                QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => x.Charge_Code == val);
                    //        }
                    //        hasFilterDtl = true;
                    //    }
                    //    //费用名称
                    //    Qfilters = filters.Where(x => x.field == "Charge_Desc");
                    //    if (Qfilters.Any())
                    //    {
                    //        var val = Qfilters.FirstOrDefault().value;
                    //        if (IsAr == null)
                    //        {
                    //            QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => x.Charge_Desc.Contains(val));
                    //            QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => x.Charge_Desc.Contains(val));
                    //        }
                    //        else
                    //        {
                    //            if ((bool)IsAr)
                    //                QBms_Bill_Ar_DtlService = QBms_Bill_Ar_DtlService.Where(x => x.Charge_Desc.Contains(val));
                    //            else
                    //                QBms_Bill_Ap_DtlService = QBms_Bill_Ap_DtlService.Where(x => x.Charge_Desc.Contains(val));
                    //        }
                    //        hasFilterDtl = true;
                    //    }
                    //    if (hasFilterDtl)
                    //    {
                    //        if (hasFilterDtl)
                    //        {
                    //            if (IsAr == null)
                    //            {
                    //                var QBmsBillArDtlService = QBms_Bill_Ar_DtlService.Select(x => (int?)x.Bms_Bill_Ar_Id);
                    //                var QBmsBillApDtlService = QBms_Bill_Ap_DtlService.Select(x => (int?)x.Bms_Bill_Ap_Id);
                    //                var QSearchRestlt = QBmsBillArDtlService.Concat(QBmsBillApDtlService);
                    //                QEttInfor = QEttInfor.Where(x => QSearchRestlt.Contains(x.MBLId));
                    //            }
                    //            else
                    //            {
                    //                if ((bool)IsAr)
                    //                {
                    //                    var QSearchRestlt = QBms_Bill_Ar_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    //                    QEttInfor = QEttInfor.Where(x => QSearchRestlt.Contains(x.MBLId));
                    //                }
                    //                else
                    //                {
                    //                    var QSearchRestlt = QBms_Bill_Ap_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    //                    QEttInfor = QEttInfor.Where(x => QSearchRestlt.Contains(x.MBLId));
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    ////if (hasFilter && hasFilterDtl)
                    ////{
                    ////    var QSearchRestlt = QEttInfor.Select(x => x.MBLId);
                    ////    if (IsAr == null)
                    ////    {
                    ////        var QBmsBillArDtlService = QBms_Bill_Ar_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////        var QBmsBillApDtlService = QBms_Bill_Ap_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////        QSearchRestlt = QSearchRestlt.Concat(QBmsBillArDtlService).Concat(QBmsBillApDtlService);
                    ////    }
                    ////    else
                    ////    {
                    ////        if ((bool)IsAr)
                    ////        {
                    ////            var QBmsBillArDtlService = QBms_Bill_Ar_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////            QSearchRestlt = QSearchRestlt.Concat(QBmsBillArDtlService);
                    ////        }
                    ////        else
                    ////        {
                    ////            var QBmsBillApDtlService = QBms_Bill_Ap_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////            QSearchRestlt = QSearchRestlt.Concat(QBmsBillApDtlService);
                    ////        }
                    ////    }

                    ////    Qquery = Qquery.Where(x => QSearchRestlt.Contains(x.Ops_M_OrdId));
                    ////}
                    ////else
                    ////{
                    ////    if (hasFilter)
                    ////    {
                    ////        var QSearchRestlt = QEttInfor.Select(x => x.MBLId);
                    ////        Qquery = Qquery.Where(x => QSearchRestlt.Contains(x.Ops_M_OrdId));
                    ////    }
                    ////    if (hasFilterDtl)
                    ////    {
                    ////        if (IsAr == null)
                    ////        {
                    ////            var QBmsBillArDtlService = QBms_Bill_Ar_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////            var QBmsBillApDtlService = QBms_Bill_Ap_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////            var QSearchRestlt = QBmsBillArDtlService.Concat(QBmsBillApDtlService);
                    ////            Qquery = Qquery.Where(x => QSearchRestlt.Contains(x.Ops_M_OrdId));
                    ////        }
                    ////        else
                    ////        {
                    ////            if ((bool)IsAr)
                    ////            {
                    ////                var QSearchRestlt = QBms_Bill_Ar_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////                Qquery = Qquery.Where(x => QSearchRestlt.Contains(x.Ops_M_OrdId));
                    ////            }
                    ////            else
                    ////            {
                    ////                var QSearchRestlt = QBms_Bill_Ap_DtlService.Select(x => (int?)x.Ops_M_OrdId);
                    ////                Qquery = Qquery.Where(x => QSearchRestlt.Contains(x.Ops_M_OrdId));
                    ////            }
                    ////        }
                    ////    }
                    ////}

                    #endregion
                }

                #endregion

                #region 搜索条件

                //结算方
                Qfilters = filters.Where(x => x.field == "Carriage_Account_Code");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Bill_Object_Id == val);
                }
                //币别
                Qfilters = filters.Where(x => x.field == "Money_Code");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Money_Code == val);
                }
                //支付方式
                Qfilters = filters.Where(x => x.field == "Payway");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Payway == val);
                }
                //账单日期起
                Qfilters = filters.Where(x => x.field == "_Bill_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    DateTime? data = Common.ParseStrToDateTime(val);
                    if (data != null)
                    {
                        Qquery = Qquery.Where(x => x.Bill_Date >= (DateTime)data);
                    }
                }
                //账单日期起
                Qfilters = filters.Where(x => x.field == "Bill_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    DateTime? date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.Bill_Date < (DateTime)date);
                    }
                }
                //制单人员
                Qfilters = filters.Where(x => x.field == "BillADDWHO");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.ADDWHO == val);
                }
                //提交人
                Qfilters = filters.Where(x => x.field == "Sumbmit_Name");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    //Qquery = Qquery.Where(x => x.Sumbmit_Name == val);
                    Qquery = Qquery.Where(x => x.Sumbmit_Name == val || x.Sumbmit_Id == val);
                }
                Qfilters = filters.Where(x => x.field == "Sumbmit_Id");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Sumbmit_Id == val);
                }
                //提交号
                Qfilters = filters.Where(x => x.field == "Sumbmit_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Sumbmit_No.Contains(val));
                }
                //提交标志
                Qfilters = filters.Where(x => x.field == "Sumbmit_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.Sumbmit_Status == boolVal);
                }
                //提交日期起
                Qfilters = filters.Where(x => x.field == "_Sumbmit_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    Qquery = Qquery.Where(x => x.Sumbmit_Date >= date);
                }
                //提交日期讫
                Qfilters = filters.Where(x => x.field == "Sumbmit_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.Sumbmit_Date <= (DateTime)date);
                    }
                }
                //审核号
                Qfilters = filters.Where(x => x.field == "AuditNo");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.AuditNo.Contains(val));
                }
                //审核标志
                Qfilters = filters.Where(x => x.field == "AuditStatus");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    int IntVal = 0;
                    if (int.TryParse(val, out IntVal))
                        Qquery = Qquery.Where(x => x.AuditStatus == (AirOutEnumType.AuditStatusEnum)IntVal);
                }
                //开票标志
                Qfilters = filters.Where(x => x.field == "Invoice_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.Invoice_Status == boolVal);
                }
                //开票号
                Qfilters = filters.Where(x => x.field == "Invoice_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Invoice_No.Contains(val));
                }
                //开票日期起
                Qfilters = filters.Where(x => x.field == "_Invoice_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        Qquery = Qquery.Where(x => x.Invoice_Date >= (DateTime)date);
                    }
                }
                //开票日期讫
                Qfilters = filters.Where(x => x.field == "Invoice_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.Invoice_Date <= (DateTime)date);
                    }
                }
                //签收标志
                Qfilters = filters.Where(x => x.field == "SignIn_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.SignIn_Status == boolVal);
                }
                //签收号
                Qfilters = filters.Where(x => x.field == "SignIn_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.SignIn_No.Contains(val));
                }
                //销账标志
                Qfilters = filters.Where(x => x.field == "SellAccount_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.SellAccount_Status == boolVal);
                }
                //销账日期起
                Qfilters = filters.Where(x => x.field == "_SellAccount_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        Qquery = Qquery.Where(x => x.SellAccount_Date >= (DateTime)date);
                    }
                }
                //销账日期讫
                Qfilters = filters.Where(x => x.field == "SellAccount_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.SellAccount_Date <= (DateTime)date);
                    }
                }
                //销账人
                Qfilters = filters.Where(x => x.field == "SellAccount_Name");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.SellAccount_Name == val);
                }
                //发票号起
                Qfilters = filters.Where(x => x.field == "_Invoice_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => string.Compare(x.Invoice_No, val) >= 0);
                }
                //发票号讫
                Qfilters = filters.Where(x => x.field == "Invoice_No_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => string.Compare(x.Invoice_No, val) <= 0);
                }
                //作废标志
                Qfilters = filters.Where(x => x.field == "Cancel_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.Cancel_Status == boolVal);
                }
                else
                {
                    //Qquery = Qquery.Where(x => !x.Cancel_Status && x.Status == AirOutEnumType.UseStatusEnum.Enable);
                }
                //业务编号
                Qfilters = filters.Where(x => x.field == "Dzbh");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Dzbh.Contains(val));
                }
                //总单号
                Qfilters = filters.Where(x => x.field == "MBL");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    val = Common.RemoveNotNumber(val);
                    Qquery = Qquery.Where(x => x.MBL.Contains(val));
                }

                #endregion
            }
            else
            {
                Qquery = Qquery.Where(x => !x.Cancel_Status && x.Status == AirOutEnumType.UseStatusEnum.Enable);
            }

            #endregion

            var QqueryResult = (from n in Qquery
                                from ptmp in QEttInfor
                                where n.Ops_M_OrdId == ptmp.MBLId 
                                select new Finance
                                {
                                    Id = n.Id,
                                    IsAr = n.IsAr,//应付账单-true:应付账单,false:应收账单
                                    MBL = n.MBL,//总单号
                                    IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                                    FTParentId = n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                    Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                    Dzbh = n.Dzbh,//业务编号
                                    Bill_Type = n.Bill_Type,//帐单类型
                                    Line_No = n.Line_No,//序号
                                    Money_Code = n.Money_Code,//币种
                                    Money_CodeNAME = string.Empty,
                                    Bill_Account2 = n.Bill_Account2,//实际金额
                                    Bill_Account = n.Bill_Account,//理论金额
                                    Bill_Amount = n.Bill_Amount,//价
                                    Bill_TaxRateType = n.Bill_TaxRateType,//税率类型
                                    Bill_TaxRate = n.Bill_TaxRate,//税率
                                    Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                    Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                    Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                    Carriage_Account_Code = ptmp.Carriage_Account_Code,//结算方代码
                                    Carriage_Account_CodeNAME = string.Empty,//结算方名称
                                    Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                    Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                    Bill_Date = n.Bill_Date,//帐单日期
                                    Payway = n.Payway,//支付方式
                                    PaywayNAME = string.Empty,
                                    Remark = n.Remark,//备注信息
                                    AuditNo = n.AuditNo,         //审核号
                                    AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                    Cancel_Status = n.Cancel_Status,//作废标志
                                    Cancel_Date = n.Cancel_Date,//作废日期
                                    Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                    Sumbmit_No = n.Sumbmit_No,//提交号
                                    Sumbmit_Name = n.Sumbmit_Name,//提交人
                                    Sumbmit_Id = n.Sumbmit_Id,    //提交人
                                    Sumbmit_Date = n.Sumbmit_Date,//提交时间
                                    SignIn_Status = n.SignIn_Status,//签收标志
                                    SignIn_No = n.SignIn_No,//签收号
                                    SignIn_Date = n.SignIn_Date,//签收时间
                                    SignIn_Name = n.SignIn_Name,//签收人
                                    Invoice_Status = n.Invoice_Status,//开票标志
                                    Invoice_No = n.Invoice_No,//开票号码
                                    Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                                    Invoice_FeeType = n.Invoice_FeeType,//开票费目
                                    Invoice_FeeTypeNAME = string.Empty,//开票费目名称
                                    Invoice_TaxRateType = n.Invoice_TaxRateType, //开票税率
                                    Invoice_HasTax = n.Invoice_HasTax,//开票是否含税
                                    ECCInvoice_Rate = n.ECCInvoice_Rate,//ECC开票汇率
                                    ECC_Code = string.Empty,//ECC开票费目
                                    Invoice_Id = n.Invoice_Id,//开票人Id
                                    Invoice_Name = n.Invoice_Name,//开票人
                                    Invoice_Date = n.Invoice_Date,//开票日期
                                    Invoice_Remark = n.Invoice_Remark,//开票要求
                                    SellAccount_Status = n.SellAccount_Status,//销账标志
                                    SellAccount_Name = n.SellAccount_Name,//销账人
                                    SellAccount_Date = n.SellAccount_Date,//销账日期
                                    Create_Status = n.Create_Status,//产生标志
                                    Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                    Flight_Date_Want = ptmp.Flight_Date_Want,//开航日期
                                    End_Port = ptmp.End_Port,//目的港
                                    End_PortNAME = string.Empty,
                                    Sumbmit_ECCNo = n.Sumbmit_ECCNo,//ECC发票号 
                                    SignIn_ECCNo = n.SignIn_ECCNo,//采购订单号 
                                    ECC_Status = n.ECC_Status,//ECC状态 
                                    ECC_StatusMsg = n.ECC_StatusMsg,//ECC状态信息 
                                    ECC_InvoiceSendDate = n.ECC_InvoiceSendDate,//ECC发票发送时间
                                    ECC_InvoiceRecvDate = n.ECC_InvoiceRecvDate,//ECC发票接收时间
                                    OperatingPoint = n.OperatingPoint,//操作点-操作点
                                    ADDID = n.ADDID,//新增人
                                    ADDWHO = n.ADDWHO,//新增人
                                    ADDTS = n.ADDTS,//新增时间
                                    EDITWHO = n.EDITWHO,//修改人
                                    EDITWHONAME = string.Empty,
                                    EDITTS = n.EDITTS,//修改时间
                                    EDITID = n.EDITID,//修改人
                                    SallerId = ptmp.SallerId,//销售
                                    SallerName = ptmp.SallerName,//销售人
                                    BillEDITWHO = n.BillEDITWHO,//修改人
                                    BillEDITWHONAME = string.Empty,
                                    BillEDITTS = n.BillEDITTS,//修改时间
                                    BillEDITID = n.BillEDITID,//修改人
                                }).AsQueryable();
            return QqueryResult;
        }

        /// <summary>
        /// 获取费用明细搜索 结果
        /// </summary>
        /// <param name="ArrFinance">搜索应收/付头数据</param>
        /// <param name="filterRules">搜索条件</param>
        /// <returns></returns>
        public IQueryable<FinanceDtl> GetDtlData(List<Finance> ArrFinance, string filterRules = "")
        {
            IEnumerable<filterRule> filters = new List<filterRule>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            if (filters != null && filters.Any())
            {
                filters = filters.Where(x => !string.IsNullOrWhiteSpace(x.value));//去除空的 数据
            }

            var Qbms_bill_ar = Bms_Bill_ArService.Queryable();
            var Qbms_bill_ar_dtl = Bms_Bill_Ar_DtlService.Queryable();
            var Qbms_bill_ap = Bms_Bill_ApService.Queryable();
            var Qbms_bill_ap_dtl = Bms_Bill_Ap_DtlService.Queryable();

            //搜索 应收账单 null:应收/应付 true：应收 false：应付
            bool? IsAr = null;
            if (ArrFinance.Any(x => x.IsAr) || ArrFinance.Any(x => !x.IsAr))
            {
                if (ArrFinance.Any(x => x.IsAr))
                {
                    IsAr = true;
                    var ArrArId = ArrFinance.Where(x => x.IsAr).Select(x => (int?)x.Id);
                    Qbms_bill_ar_dtl = Qbms_bill_ar_dtl.Where(x => ArrArId.Contains(x.Bms_Bill_Ar_Id));
                }
                if (ArrFinance.Any(x => !x.IsAr))
                {
                    if (IsAr != null && (bool)IsAr)
                        IsAr = null;
                    else
                        IsAr = false;
                    var ArrApId = ArrFinance.Where(x => !x.IsAr).Select(x => (int?)x.Id);
                    Qbms_bill_ap_dtl = Qbms_bill_ap_dtl.Where(x => ArrApId.Contains(x.Bms_Bill_Ap_Id));
                }
            }
            else
            {
                return null;
            }

            if (filters.Any())
            {
                #region 明细数据搜索

                var hasDtlFilter = false;
                var Qfilters = filters.Where(x => x.field == "Charge_Code" || x.field == "Charge_Desc");
                if (Qfilters.Any())
                {
                    //费用代码
                    Qfilters = filters.Where(x => x.field == "Charge_Code");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        Qbms_bill_ar_dtl = Qbms_bill_ar_dtl.Where(x => x.Charge_Code == val);
                        Qbms_bill_ap_dtl = Qbms_bill_ap_dtl.Where(x => x.Charge_Code == val);
                        hasDtlFilter = true;
                    }
                    //费用名称
                    Qfilters = filters.Where(x => x.field == "Charge_Desc");
                    if (Qfilters.Any())
                    {
                        var val = Qfilters.FirstOrDefault().value;
                        Qbms_bill_ar_dtl = Qbms_bill_ar_dtl.Where(x => x.Charge_Desc.Contains(val));
                        Qbms_bill_ap_dtl = Qbms_bill_ap_dtl.Where(x => x.Charge_Desc.Contains(val));
                        hasDtlFilter = true;
                    }
                    if (hasDtlFilter)
                    {
                        //搜索条件 转换成 关联字段 在搜索
                    }
                }

                #endregion
            }
            IQueryable<FinanceDtl> Qquery;
            if (IsAr == null)
            {
                #region 应收应付 合起来

                Qquery = (from x in Qbms_bill_ar_dtl
                          from n in Qbms_bill_ar
                          where x.Bms_Bill_Ar_Id == n.Id
                          select new FinanceDtl
                          {
                              Id = n.Id,
                              IsAr = true,//应付账单-true:应付账单,false:应收账单
                              MBL = n.MBL,//总单号
                              IsMBLJS = false,//总单结算-分摊数据时产生
                              FTParentId = 0,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                              Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                              //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                              Dzbh = n.Dzbh,//业务编号
                              Bill_Type = n.Bill_Type,//帐单类型
                              Line_No = n.Line_No,//序号
                              Line_Id = x.Line_Id,//费用序号
                              Bill_Object_Id = n.Bill_Object_Id,//供方代码
                              Bill_Object_Name = n.Bill_Object_Name,//供方名称
                              Money_Code = n.Money_Code,//币种
                              Money_CodeNAME = string.Empty,
                              Charge_Code = x.Charge_Code,// 费用代码 
                              Charge_Desc = x.Charge_Desc,// 费用名称 
                              Unitprice2 = x.Unitprice2,// 实际单价 
                              Qty = x.Qty,// 数量 
                              Account2 = x.Account2,// 实际金额
                              Bill_HasTax = n.Bill_HasTax,//含税/不含税
                              Bill_TaxRate = n.Bill_TaxRate,//税率
                              Bill_Amount = n.Bill_Amount,//价
                              Bill_TaxAmount = n.Bill_TaxAmount,//税金
                              Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                              Bill_Date = n.Bill_Date,//帐单日期
                              Payway = n.Payway,//支付方式
                              PaywayNAME = string.Empty,
                              Remark = n.Remark,//备注信息
                              AuditNo = n.AuditNo,//审核号
                              AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                              Cancel_Status = n.Cancel_Status,//作废标志
                              Sumbmit_Status = n.Sumbmit_Status,//提交标志
                              Sumbmit_No = n.Sumbmit_No,//提交号
                              Sumbmit_Name = n.Sumbmit_Name,//提交人
                              Sumbmit_Id = n.Sumbmit_Id,//提交人
                              Sumbmit_Date = n.Sumbmit_Date,//提交时间
                              SignIn_Status = false,//签收标志
                              SignIn_No = string.Empty,//签收号
                              Invoice_Status = n.Invoice_Status,//开票标志
                              Invoice_No = n.Invoice_No,//开票号码
                              Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                              Invoice_FeeType = n.Invoice_FeeType,//开票费目
                              Invoice_Id = n.Invoice_Id,//开票人Id
                              Invoice_Name = n.Invoice_Name,//开票人
                              Invoice_Date = n.Invoice_Date,//开票日期
                              SellAccount_Status = n.SellAccount_Status,//销账标志
                              SellAccount_Name = n.SellAccount_Name,//销账人
                              SellAccount_Date = n.SellAccount_Date,//销账日期
                              Create_Status = n.Create_Status,//产生标志
                              Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                              OperatingPoint = n.OperatingPoint,//操作点-操作点
                              ADDID = x.ADDID,//新增人
                              ADDWHO = x.ADDWHO,//新增人
                              ADDTS = x.ADDTS,//新增时间
                              EDITWHO = x.EDITWHO,//修改人
                              EDITWHONAME = string.Empty,
                              EDITTS = x.EDITTS,//修改时间
                              EDITID = x.EDITID,//修改人
                              BillEDITWHO = x.BillEDITWHO,//修改人
                              BillEDITWHONAME = string.Empty,
                              BillEDITTS = x.BillEDITTS,//修改时间
                              BillEDITID = x.BillEDITID,//修改人
                          }).Concat(from x in Qbms_bill_ap_dtl
                                    from n in Qbms_bill_ap
                                    where x.Bms_Bill_Ap_Id == n.Id
                                    select new FinanceDtl
                                    {
                                        Id = n.Id,
                                        IsAr = false,//应付账单-true:应付账单,false:应收账单
                                        MBL = n.MBL,//总单号
                                        IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                                        FTParentId = n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                        Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                        //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                                        Dzbh = n.Dzbh,//业务编号
                                        Bill_Type = n.Bill_Type,//帐单类型
                                        Line_No = n.Line_No,//序号
                                        Line_Id = x.Line_Id,//费用序号
                                        Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                        Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                        Money_Code = n.Money_Code,//币种
                                        Money_CodeNAME = string.Empty,
                                        Charge_Code = x.Charge_Code,// 费用代码 
                                        Charge_Desc = x.Charge_Desc,// 费用名称 
                                        Unitprice2 = x.Unitprice2,// 实际单价 
                                        Qty = x.Qty,// 数量 
                                        Account2 = x.Account2,// 实际金额
                                        Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                        Bill_TaxRate = n.Bill_TaxRate,//税率
                                        Bill_Amount = n.Bill_Amount,//价
                                        Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                        Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                        Bill_Date = n.Bill_Date,//帐单日期
                                        Payway = n.Payway,//支付方式
                                        PaywayNAME = string.Empty,
                                        Remark = n.Remark,//备注信息
                                        AuditNo = n.AuditNo,//审核号
                                        AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                        Cancel_Status = n.Cancel_Status,//作废标志
                                        Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                        Sumbmit_No = n.Sumbmit_No,//提交号
                                        Sumbmit_Name = n.Sumbmit_Name,//提交人
                                        Sumbmit_Id = n.Sumbmit_Id,//提交人
                                        Sumbmit_Date = n.Sumbmit_Date,//提交时间
                                        SignIn_Status = n.SignIn_Status,//签收标志
                                        SignIn_No = n.SignIn_No,//签收号
                                        Invoice_Status = n.Invoice_Status,//开票标志
                                        Invoice_No = n.Invoice_No,//开票号码
                                        Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                                        Invoice_FeeType = n.Invoice_FeeType,//开票费目
                                        Invoice_Id = n.Invoice_Id,//开票人Id
                                        Invoice_Name = n.Invoice_Name,//开票人
                                        Invoice_Date = n.Invoice_Date,//开票日期
                                        SellAccount_Status = n.SellAccount_Status,//销账标志
                                        SellAccount_Name = n.SellAccount_Name,//销账人
                                        SellAccount_Date = n.SellAccount_Date,//销账日期
                                        Create_Status = n.Create_Status,//产生标志
                                        Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                        OperatingPoint = n.OperatingPoint,//操作点-操作点
                                        ADDID = x.ADDID,//新增人
                                        ADDWHO = x.ADDWHO,//新增人
                                        ADDTS = x.ADDTS,//新增时间
                                        EDITWHO = x.EDITWHO,//修改人
                                        EDITWHONAME = string.Empty,
                                        EDITTS = x.EDITTS,//修改时间
                                        EDITID = x.EDITID,//修改人
                                        BillEDITWHO = x.BillEDITWHO,//修改人
                                        BillEDITWHONAME = string.Empty,
                                        BillEDITTS = x.BillEDITTS,//修改时间
                                        BillEDITID = x.BillEDITID,//修改人
                                    }).AsQueryable();

                #endregion;
            }
            else if ((bool)IsAr)
            {
                #region 应收

                Qquery = (from x in Qbms_bill_ar_dtl
                          join ntmp in Qbms_bill_ar on x.Bms_Bill_Ar_Id equals ntmp.Id into n_tmp
                          from n in n_tmp.DefaultIfEmpty()
                          select new FinanceDtl
                          {
                              Id = n.Id,
                              IsAr = true,//应付账单-true:应付账单,false:应收账单
                              MBL = n.MBL,//总单号
                              IsMBLJS = false,//总单结算-分摊数据时产生
                              FTParentId = 0,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                              Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                              //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                              Dzbh = n.Dzbh,//业务编号
                              Bill_Type = n.Bill_Type,//帐单类型
                              Line_No = n.Line_No,//序号
                              Line_Id = x.Line_Id,//费用序号
                              Bill_Object_Id = n.Bill_Object_Id,//供方代码
                              Bill_Object_Name = n.Bill_Object_Name,//供方名称
                              Money_Code = n.Money_Code,//币种
                              Money_CodeNAME = string.Empty,
                              Charge_Code = x.Charge_Code,// 费用代码 
                              Charge_Desc = x.Charge_Desc,// 费用名称 
                              Unitprice2 = x.Unitprice2,// 实际单价 
                              Qty = x.Qty,// 数量 
                              Account2 = x.Account2,// 实际金额
                              Bill_HasTax = n.Bill_HasTax,//含税/不含税
                              Bill_TaxRate = n.Bill_TaxRate,//税率
                              Bill_Amount = n.Bill_Amount,//价
                              Bill_TaxAmount = n.Bill_TaxAmount,//税金
                              Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                              Bill_Date = n.Bill_Date,//帐单日期
                              Payway = n.Payway,//支付方式
                              PaywayNAME = string.Empty,
                              Remark = n.Remark,//备注信息
                              AuditNo = n.AuditNo,//审核号
                              AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                              Cancel_Status = n.Cancel_Status,//作废标志
                              Sumbmit_Status = n.Sumbmit_Status,//提交标志
                              Sumbmit_No = n.Sumbmit_No,//提交号
                              Sumbmit_Name = n.Sumbmit_Name,//提交人
                              Sumbmit_Id = n.Sumbmit_Id,//提交人
                              Sumbmit_Date = n.Sumbmit_Date,//提交时间
                              SignIn_Status = false,//签收标志
                              SignIn_No = string.Empty,//签收号
                              Invoice_Status = n.Invoice_Status,//开票标志
                              Invoice_No = n.Invoice_No,//开票号码
                              Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                              Invoice_FeeType = n.Invoice_FeeType,//开票费目
                              Invoice_Id = n.Invoice_Id,//开票人Id
                              Invoice_Name = n.Invoice_Name,//开票人
                              Invoice_Date = n.Invoice_Date,//开票日期
                              SellAccount_Status = n.SellAccount_Status,//销账标志
                              SellAccount_Name = n.SellAccount_Name,//销账人
                              SellAccount_Date = n.SellAccount_Date,//销账日期
                              Create_Status = n.Create_Status,//产生标志
                              Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                              OperatingPoint = n.OperatingPoint,//操作点-操作点
                              ADDID = x.ADDID,//新增人
                              ADDWHO = x.ADDWHO,//新增人
                              ADDTS = x.ADDTS,//新增时间
                              EDITWHO = x.EDITWHO,//修改人
                              EDITWHONAME = string.Empty,
                              EDITTS = x.EDITTS,//修改时间
                              EDITID = x.EDITID,//修改人
                              BillEDITWHO = x.BillEDITWHO,//修改人
                              BillEDITWHONAME = string.Empty,
                              BillEDITTS = x.BillEDITTS,//修改时间
                              BillEDITID = x.BillEDITID,//修改人
                          });
                IsAr = true;
                #endregion
            }
            else
            {
                #region 应付

                Qquery = (from x in Qbms_bill_ap_dtl
                          join ntmp in Qbms_bill_ap on x.Bms_Bill_Ap_Id equals ntmp.Id into n_tmp
                          from n in n_tmp.DefaultIfEmpty()
                          select new FinanceDtl
                          {
                              Id = n.Id,
                              IsAr = false,//应付账单-true:应付账单,false:应收账单
                              MBL = n.MBL,//总单号
                              IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                              FTParentId = n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                              Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                              //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                              Dzbh = n.Dzbh,//业务编号
                              Bill_Type = n.Bill_Type,//帐单类型
                              Line_No = n.Line_No,//序号
                              Line_Id = x.Line_Id,//费用序号
                              Bill_Object_Id = n.Bill_Object_Id,//供方代码
                              Bill_Object_Name = n.Bill_Object_Name,//供方名称
                              Money_Code = n.Money_Code,//币种
                              Money_CodeNAME = string.Empty,
                              Charge_Code = x.Charge_Code,// 费用代码 
                              Charge_Desc = x.Charge_Desc,// 费用名称 
                              Unitprice2 = x.Unitprice2,// 实际单价 
                              Qty = x.Qty,// 数量 
                              Account2 = x.Account2,// 实际金额
                              Bill_HasTax = n.Bill_HasTax,//含税/不含税
                              Bill_TaxRate = n.Bill_TaxRate,//税率
                              Bill_Amount = n.Bill_Amount,//价
                              Bill_TaxAmount = n.Bill_TaxAmount,//税金
                              Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                              Bill_Date = n.Bill_Date,//帐单日期
                              Payway = n.Payway,//支付方式
                              PaywayNAME = string.Empty,
                              Remark = n.Remark,//备注信息
                              AuditNo = n.AuditNo,//审核号
                              AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                              Cancel_Status = n.Cancel_Status,//作废标志
                              Sumbmit_Status = n.Sumbmit_Status,//提交标志
                              Sumbmit_No = n.Sumbmit_No,//提交号
                              Sumbmit_Name = n.Sumbmit_Name,//提交人
                              Sumbmit_Id = n.Sumbmit_Id,//提交人
                              Sumbmit_Date = n.Sumbmit_Date,//提交时间
                              SignIn_Status = n.SignIn_Status,//签收标志
                              SignIn_No = n.SignIn_No,//签收号
                              Invoice_Status = n.Invoice_Status,//开票标志
                              Invoice_No = n.Invoice_No,//开票号码
                              Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                              Invoice_FeeType = n.Invoice_FeeType,//开票费目
                              Invoice_Id = n.Invoice_Id,//开票人Id
                              Invoice_Name = n.Invoice_Name,//开票人
                              Invoice_Date = n.Invoice_Date,//开票日期
                              SellAccount_Status = n.SellAccount_Status,//销账标志
                              SellAccount_Name = n.SellAccount_Name,//销账人
                              SellAccount_Date = n.SellAccount_Date,//销账日期
                              Create_Status = n.Create_Status,//产生标志
                              Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                              OperatingPoint = n.OperatingPoint,//操作点-操作点
                              ADDID = x.ADDID,//新增人
                              ADDWHO = x.ADDWHO,//新增人
                              ADDTS = x.ADDTS,//新增时间
                              EDITWHO = x.EDITWHO,//修改人
                              EDITWHONAME = string.Empty,
                              EDITTS = x.EDITTS,//修改时间
                              EDITID = x.EDITID,//修改人
                              BillEDITWHO = x.BillEDITWHO,//修改人
                              BillEDITWHONAME = string.Empty,
                              BillEDITTS = x.BillEDITTS,//修改时间
                              BillEDITID = x.BillEDITID,//修改人
                          });
                IsAr = false;
                #endregion
            }

            #region 查询

            if (filters != null && filters.Any())
            {
                var Qfilters = filters.Where(x => !string.IsNullOrWhiteSpace(x.field));//去除 空条件列

                #region 搜索条件

                //账单类型
                Qfilters = filters.Where(x => x.field == "Bill_Type");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Bill_Type == val);
                }

                //账单日期起
                Qfilters = filters.Where(x => x.field == "_Bill_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    DateTime? data = Common.ParseStrToDateTime(val);
                    if (data != null)
                    {
                        Qquery = Qquery.Where(x => x.Bill_Date >= (DateTime)data);
                    }
                }
                //账单日期起
                Qfilters = filters.Where(x => x.field == "Bill_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    DateTime? date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.Bill_Date < (DateTime)date);
                    }
                }
                //制单人员
                Qfilters = filters.Where(x => x.field == "BillADDWHO");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.ADDWHO == val);
                }
                //提交人
                Qfilters = filters.Where(x => x.field == "Sumbmit_Name");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Sumbmit_Name == val || x.Sumbmit_Id == val);
                }
                Qfilters = filters.Where(x => x.field == "Sumbmit_Id");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Sumbmit_Id == val);
                }
                //提交号
                Qfilters = filters.Where(x => x.field == "Sumbmit_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Sumbmit_No.Contains(val));
                }
                //提交标志
                Qfilters = filters.Where(x => x.field == "Sumbmit_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.Sumbmit_Status == boolVal);
                }
                //提交日期起
                Qfilters = filters.Where(x => x.field == "_Sumbmit_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    Qquery = Qquery.Where(x => x.Sumbmit_Date >= date);
                }
                //提交日期讫
                Qfilters = filters.Where(x => x.field == "Sumbmit_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.Sumbmit_Date <= (DateTime)date);
                    }
                }
                //审核号
                Qfilters = filters.Where(x => x.field == "AuditNo");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.AuditNo.Contains(val));
                }
                //审核标志
                Qfilters = filters.Where(x => x.field == "AuditStatus");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    int IntVal = 0;
                    if (int.TryParse(val, out IntVal))
                        Qquery = Qquery.Where(x => x.AuditStatus == (AirOutEnumType.AuditStatusEnum)IntVal);
                }
                //开票标志
                Qfilters = filters.Where(x => x.field == "Invoice_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.Invoice_Status == boolVal);
                }
                //开票号
                Qfilters = filters.Where(x => x.field == "Invoice_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Invoice_No.Contains(val));
                }
                //开票日期起
                Qfilters = filters.Where(x => x.field == "_Invoice_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        Qquery = Qquery.Where(x => x.Invoice_Date >= (DateTime)date);
                    }
                }
                //开票日期讫
                Qfilters = filters.Where(x => x.field == "Invoice_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.Invoice_Date <= (DateTime)date);
                    }
                }
                //签收标志
                Qfilters = filters.Where(x => x.field == "SignIn_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.SignIn_Status == boolVal);
                }
                //签收号
                Qfilters = filters.Where(x => x.field == "SignIn_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.SignIn_No.Contains(val));
                }
                //销账标志
                Qfilters = filters.Where(x => x.field == "SellAccount_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.SellAccount_Status == boolVal);
                }
                //销账日期起
                Qfilters = filters.Where(x => x.field == "_SellAccount_Date");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        Qquery = Qquery.Where(x => x.SellAccount_Date >= (DateTime)date);
                    }
                }
                //销账日期讫
                Qfilters = filters.Where(x => x.field == "SellAccount_Date_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var date = Common.ParseStrToDateTime(val);
                    if (date != null)
                    {
                        date = ((DateTime)date).AddDays(1);
                        Qquery = Qquery.Where(x => x.SellAccount_Date <= (DateTime)date);
                    }
                }
                //销账人
                Qfilters = filters.Where(x => x.field == "SellAccount_Name");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.SellAccount_Name == val);
                }
                //发票号起
                Qfilters = filters.Where(x => x.field == "_Invoice_No");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => string.Compare(x.Invoice_No, val) >= 0);
                }
                //发票号讫
                Qfilters = filters.Where(x => x.field == "Invoice_No_");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => string.Compare(x.Invoice_No, val) <= 0);
                }
                //作废标志
                Qfilters = filters.Where(x => x.field == "Cancel_Status");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    var boolVal = Common.ChangStrToBool(val);
                    Qquery = Qquery.Where(x => x.Cancel_Status == boolVal);
                }
                //业务编号
                Qfilters = filters.Where(x => x.field == "Dzbh");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.Dzbh.Contains(val));
                }
                //总单号
                Qfilters = filters.Where(x => x.field == "MBL");
                if (Qfilters.Any())
                {
                    var val = Qfilters.FirstOrDefault().value;
                    Qquery = Qquery.Where(x => x.MBL.Contains(val));
                }

                #endregion
            }

            #endregion

            var QqueryResult = (from n in Qquery
                                select new FinanceDtl
                                {
                                    Id = n.Id,
                                    IsAr = n.IsAr,//应付账单-true:应付账单,false:应收账单
                                    MBL = n.MBL,//总单号
                                    IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                                    FTParentId = n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                    Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                    //Ops_H_OrdId = n.Ops_H_OrdId,//分单Id
                                    Dzbh = n.Dzbh,//业务编号
                                    Bill_Type = n.Bill_Type,//帐单类型
                                    Line_No = n.Line_No,//序号
                                    Line_Id = n.Line_Id,//费用序号
                                    Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                    Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                    Money_Code = n.Money_Code,//币种
                                    Money_CodeNAME = string.Empty,
                                    Charge_Code = n.Charge_Code,// 费用代码 
                                    Charge_Desc = n.Charge_Desc,// 费用名称 
                                    Unitprice2 = n.Unitprice2,// 实际单价 
                                    Qty = n.Qty,// 数量 
                                    Account2 = n.Account2,// 实际金额
                                    Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                    Bill_TaxRate = n.Bill_TaxRate,//税率
                                    Bill_Amount = n.Bill_Amount,//价
                                    Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                    Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                    Bill_Date = n.Bill_Date,//帐单日期
                                    Payway = n.Payway,//支付方式
                                    PaywayNAME = string.Empty,
                                    Remark = n.Remark,//备注信息
                                    AuditNo = n.AuditNo,//审核号
                                    AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                    Cancel_Status = n.Cancel_Status,//作废标志
                                    Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                    Sumbmit_No = n.Sumbmit_No,//提交号
                                    Sumbmit_Name = n.Sumbmit_Name,//提交人
                                    Sumbmit_Id = n.Sumbmit_Id,//提交人
                                    Sumbmit_Date = n.Sumbmit_Date,//提交时间
                                    SignIn_Status = n.SignIn_Status,//签收标志
                                    SignIn_No = n.SignIn_No,//签收号
                                    Invoice_Status = n.Invoice_Status,//开票标志
                                    Invoice_No = n.Invoice_No,//开票号码
                                    Invoice_MoneyCode = n.Invoice_MoneyCode,//开票币种
                                    Invoice_FeeType = n.Invoice_FeeType,//开票费目
                                    Invoice_Id = n.Invoice_Id,//开票人Id
                                    Invoice_Name = n.Invoice_Name,//开票人
                                    Invoice_Date = n.Invoice_Date,//开票日期
                                    SellAccount_Status = n.SellAccount_Status,//销账标志
                                    SellAccount_Name = n.SellAccount_Name,//销账人
                                    SellAccount_Date = n.SellAccount_Date,//销账日期
                                    Create_Status = n.Create_Status,//产生标志
                                    Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                    OperatingPoint = n.OperatingPoint,//操作点-操作点
                                    ADDID = n.ADDID,//新增人
                                    ADDWHO = n.ADDWHO,//新增人
                                    ADDTS = n.ADDTS,//新增时间
                                    EDITWHO = n.EDITWHO,//修改人
                                    EDITWHONAME = string.Empty,
                                    EDITTS = n.EDITTS,//修改时间
                                    EDITID = n.EDITID,//修改人
                                    BillEDITWHO = n.BillEDITWHO,//修改人
                                    BillEDITWHONAME = string.Empty,
                                    BillEDITTS = n.BillEDITTS,//修改时间
                                    BillEDITID = n.BillEDITID,//修改人
                                }).AsQueryable();
            return QqueryResult;
        }

        /// <summary>
        /// 获取应收应付 金额（按币种）
        /// </summary>
        /// <param name="ArrFinance"></param>
        /// <returns></returns>
        public IEnumerable<Bill_AccountTotalByMoney_Code> GetArApBillAccountByFlight_Date(List<Finance> ArrFinance)
        {
            try
            {
                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return null;
                }
                //汇率
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);//unitOfWork.Repository<Rate>().Queryable();
                var QResult = ArrFinance;
                if (QResult.Any(x => x == null || x.Flight_Date_Want == null))
                {
                    return null;
                }

                #region 根据航班日期 读取 汇率

                var ArrFlight_Date_Want = QResult.Where(x => x.Flight_Date_Want != null);
                var NewArrFlight_Date_Want = ArrFlight_Date_Want.Select(x => new { Year = ((DateTime)x.Flight_Date_Want).Year, Month = ((DateTime)x.Flight_Date_Want).Month }).Distinct();
                RateRep = RateRep.Where(x => x.ForeignCurrCode=="CNY" && x.Status == AirOutEnumType.UseStatusEnum.Enable);
                var ArrRare = RateRep.Where(x => NewArrFlight_Date_Want.Contains(new { Year = x.Year, Month = x.Month }));

                #endregion

                var ArGroup = from p in ArrFlight_Date_Want
                              group p by new { IsAr = p.IsAr, Money_Code = p.Money_Code, Year = ((DateTime)p.Flight_Date_Want).Year, Month = ((DateTime)p.Flight_Date_Want).Month } into g
                              select new
                              {
                                  IsAr = g.Key.IsAr,
                                  Money_Code = g.Key.Money_Code,
                                  Year = g.Key.Year,
                                  Month = g.Key.Month,
                                  Bill_Account2 = g.Sum(x => x.Bill_Account2)
                              };
                var NewResult = from p in ArGroup
                                join r in ArrRare on new { Money_Code = p.Money_Code, Year = p.Year, Month = p.Month } equals new { Money_Code = r.LocalCurrCode, Year = r.Year, Month = r.Month } into t_mp
                                from tmp in t_mp.DefaultIfEmpty()
                                select new
                                {
                                    p.IsAr,
                                    p.Money_Code,
                                    p.Year,
                                    p.Month,
                                    p.Bill_Account2,
                                    NewBill_Account2 = tmp == null ? p.Bill_Account2 : (p.Bill_Account2 * (p.IsAr ? tmp.RecRate : tmp.PayRate))
                                };
                var ArApTotal = from p in NewResult
                                group p by new { p.IsAr, p.Money_Code } into g
                                select new Bill_AccountTotalByMoney_Code
                                {
                                    IsAr = g.Key.IsAr,
                                    Money_Code = g.Key.Money_Code,
                                    Bill_Account2Total = g.Sum(x => x.Bill_Account2),
                                    NewBill_Account2Total = g.Sum(x => x.NewBill_Account2)
                                };

                return ArApTotal;
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, "Finace", true, true);
                return null;
            }
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="filterRules"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Stream ExportExcel(string filterRules = "", string sort = "ID", string order = "asc")
        {
            var QResult = this.GetData(filterRules);
            var QResultData = QResult.OrderBy(sort, order).ToList();

            #region 获取 缓存 数据 并关联

            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);//客商
            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);//枚举
            var ArrPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);//港口
            var ArrPARA_CURR = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);//币种

            var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "PayWay");

            var Q_Result = (from n in QResultData
                            join a in ArrAppUser on n.EDITWHO equals a.UserName into a_tmp
                            from atmp in a_tmp.DefaultIfEmpty()
                            join b in QArrBD_DEFDOC_LIST on n.Payway equals b.LISTNAME into b_tmp
                            from btmp in b_tmp.DefaultIfEmpty()
                            join c in ArrPARA_AirPort on n.End_Port equals c.PortCode into c_tmp
                            from ctmp in c_tmp.DefaultIfEmpty()
                            join d in ArrPARA_CURR on n.Money_Code equals d.CURR_CODE into d_tmp
                            from dtmp in d_tmp.DefaultIfEmpty()
                            join e in ArrCusBusInfo on n.Carriage_Account_Code equals e.EnterpriseId into e_tmp
                            from etmp in e_tmp.DefaultIfEmpty()
                            select new Finance
                            {
                                Id = n.Id,
                                IsAr = n.IsAr,//应付账单-true:应付账单,false:应收账单
                                MBL = n.MBL,//总单号
                                IsMBLJS = n.IsMBLJS,//总单结算-分摊数据时产生
                                FTParentId = n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                                Ops_M_OrdId = n.Ops_M_OrdId,//总单Id
                                Dzbh = n.Dzbh,//业务编号
                                Bill_Type = n.Bill_Type,//帐单类型
                                Line_No = n.Line_No,//序号
                                Money_Code = n.Money_Code,//币种
                                Money_CodeNAME = dtmp == null ? string.Empty : dtmp.CURR_Name,
                                Bill_Account2 = n.Bill_Account2,//实际金额
                                Bill_Account = n.Bill_Account,//理论金额
                                Bill_Amount = n.Bill_Amount,//价
                                Bill_TaxRateType = n.Bill_TaxRateType,//税率类型
                                Bill_TaxRate = n.Bill_TaxRate,//税率
                                Bill_HasTax = n.Bill_HasTax,//含税/不含税
                                Bill_TaxAmount = n.Bill_TaxAmount,//税金
                                Bill_AmountTaxTotal = n.Bill_AmountTaxTotal,//价税合计
                                Carriage_Account_Code = n.Carriage_Account_Code,
                                Carriage_Account_CodeNAME = etmp == null ? string.Empty : etmp.CHNName,
                                Bill_Object_Id = n.Bill_Object_Id,//供方代码
                                Bill_Object_Name = n.Bill_Object_Name,//供方名称
                                Bill_Date = n.Bill_Date,//帐单日期
                                Payway = n.Payway,//支付方式
                                PaywayNAME = btmp == null ? string.Empty : btmp.LISTNAME,
                                Remark = n.Remark,//备注信息
                                AuditNo = n.AuditNo,         //审核号
                                AuditStatus = n.AuditStatus,//审批状态-0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                Cancel_Status = n.Cancel_Status,//作废标志
                                Sumbmit_Status = n.Sumbmit_Status,//提交标志
                                Sumbmit_No = n.Sumbmit_No,//提交号
                                Sumbmit_Name = n.Sumbmit_Name,//提交人
                                Sumbmit_Date = n.Sumbmit_Date,//提交时间
                                SignIn_Status = n.SignIn_Status,//签收标志
                                SignIn_No = n.SignIn_No,//签收号
                                Invoice_Status = n.Invoice_Status,//开票标志
                                Invoice_No = n.Invoice_No,//开票号码
                                Invoice_Date = n.Invoice_Date,//开票日期
                                SellAccount_Status = n.SellAccount_Status,//销账标志
                                SellAccount_Name = n.SellAccount_Name,//销账人
                                SellAccount_Date = n.SellAccount_Date,//销账日期
                                Create_Status = n.Create_Status,//产生标志
                                Status = n.Status,//使用状态-状态-0:草稿,1:启用,-1:停用
                                Flight_Date_Want = n.Flight_Date_Want,//开航日期
                                End_Port = n.End_Port,//目的港
                                End_PortNAME = ctmp == null ? string.Empty : ctmp.PortName,
                                OperatingPoint = n.OperatingPoint,//操作点-操作点
                                ADDID = n.ADDID,//新增人
                                ADDWHO = n.ADDWHO,//新增人
                                ADDTS = n.ADDTS,//新增时间
                                EDITWHO = n.EDITWHO,//修改人
                                EDITWHONAME = atmp == null ? string.Empty : atmp.UserNameDesc,
                                EDITTS = n.EDITTS,//修改时间
                                EDITID = n.EDITID//修改人
                            }).ToList();

            #endregion

            return ExcelHelper.ExportExcel(typeof(Finance), Q_Result);
        }
    }
}