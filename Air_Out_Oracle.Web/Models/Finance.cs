using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public class Finance
    {
        public Finance()
        {
            AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
            Status = AirOutEnumType.UseStatusEnum.Enable;
            MBL = "-";
        }

        public int Id { get; set; }

        [Display(Name = "应付账单", Description = "true:应付账单,false:应收账单")]
        public bool IsAr { get; set; }

        [Display(Name = "总单号")]
        [Required, MaxLength(50)]
        public string MBL { get; set; }

        [Display(Name = "总单结算", Description = "分摊数据时产生")]
        public bool IsMBLJS { get; set; }

        [Display(Name = "分摊主数据", Description = "分摊数据时，记录有哪个数据 分摊下来的")]
        public int FTParentId { get; set; }

        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        //[Display(Name = "分单Id")]
        //public int Ops_H_OrdId { get; set; }

        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Display(Name = "帐单类型")]
        [MaxLength(20)]
        public string Bill_Type { get; set; }

        [Display(Name = "序号")]
        public int Line_No { get; set; }

        [Display(Name = "币种")]
        [MaxLength(20)]
        public string Money_Code { get; set; }
        
        [Display(Name = "币种名称")]
        [MaxLength(100)]
        public string Money_CodeNAME { get; set; }

        [Display(Name = "实际金额")]
        public decimal Bill_Account2 { get; set; }

        [Display(Name = "理论金额")]
        public decimal Bill_Account { get; set; }

        [Display(Name = "价")]
        public decimal Bill_Amount { get; set; }

        [Display(Name = "税率类型")]
        [MaxLength(50)]
        public string Bill_TaxRateType { get; set; }

        [Display(Name = "税率")]
        [Range(0, int.MaxValue)]
        public decimal Bill_TaxRate { get; set; }

        [Display(Name = "含税/不含税")]
        public bool Bill_HasTax { get; set; }

        [Display(Name = "税金")]
        public decimal Bill_TaxAmount { get; set; }

        [Display(Name = "价税合计")]
        public decimal Bill_AmountTaxTotal { get; set; }

        [Display(Name = "结算方代码")]
        [MaxLength(50)]
        public string Carriage_Account_Code { get; set; }

        [Display(Name = "结算方名称")]
        [MaxLength(100)]
        public string Carriage_Account_CodeNAME { get; set; }

        [Display(Name = "供方代码")]
        [MaxLength(50)]
        public string Bill_Object_Id { get; set; }

        [Display(Name = "供方名称")]
        [MaxLength(100)]
        public string Bill_Object_Name { get; set; }

        [Display(Name = "帐单日期")]
        public DateTime? Bill_Date { get; set; }

        [Display(Name = "支付方式")]
        [MaxLength(20)]
        public string Payway { get; set; }

        [Display(Name = "支付方式名称")]
        [MaxLength(100)]
        public string PaywayNAME { get; set; }

        [Display(Name = "备注信息")]
        [MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "审核号")]
        [MaxLength(50)]
        public string AuditNo { get; set; }

        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "作废标志")]
        public bool Cancel_Status { get; set; }

        [Display(Name = "作废日期")]
        public DateTime? Cancel_Date { get; set; }

        [Display(Name = "提交标志")]
        public bool Sumbmit_Status { get; set; }

        [Display(Name = "提交号")]
        [MaxLength(50)]
        public string Sumbmit_No { get; set; }

        [Display(Name = "提交人")]
        [MaxLength(50)]
        public string Sumbmit_Name { get; set; }

        [Display(Name = "提交人")]
        [MaxLength(50)]
        public string Sumbmit_Id { get; set; }

        [Display(Name = "提交时间")]
        public DateTime? Sumbmit_Date { get; set; }

        [Display(Name = "签收标志")]
        public bool SignIn_Status { get; set; }

        [Display(Name = "签收号")]
        [MaxLength(50)]
        public string SignIn_No { get; set; }

        [Display(Name = "签收时间")]
        public DateTime? SignIn_Date { get; set; }

        [Display(Name = "签收人")]
        [MaxLength(50)]
        public string SignIn_Name { get; set; }

        [Display(Name = "开票标志")]
        public bool Invoice_Status { get; set; }

        [Display(Name = "开票号码")]
        [MaxLength(50)]
        public string Invoice_No { get; set; }

        [Display(Name = "开票币种")]
        [MaxLength(50)]
        public string Invoice_MoneyCode { get; set; }

        [Display(Name = "开票汇率")]
        public decimal? ECCInvoice_Rate{ get; set; }

        [Display(Name = "开票费目")]
        [MaxLength(100)]
        public string Invoice_FeeType { get; set; }

        [Display(Name = "开票费目名称")]
        [MaxLength(100)]
        public string Invoice_FeeTypeNAME { get; set; }

        [Display(Name = "开票税率")]
        [MaxLength(50)]
        public string Invoice_TaxRateType { get; set; }

        [Display(Name = "开票是否含税")]
        public bool Invoice_HasTax { get; set; }

        [Display(Name = "ECC开票费目")]
        [MaxLength(50)]
        public string ECC_Code { get; set; }

        [Display(Name = "开票人Id")]
        [MaxLength(50)]
        public string Invoice_Id { get; set; }

        [Display(Name = "开票人")]
        [MaxLength(20)]
        public string Invoice_Name { get; set; }

        [Display(Name = "开票日期")]
        public DateTime? Invoice_Date { get; set; }

        [Display(Name = "开票要求")]
        [MaxLength(200)]
        public string Invoice_Remark { get; set; }

        [Display(Name = "销账标志")]
        public bool SellAccount_Status { get; set; }

        [Display(Name = "销账人")]
        [MaxLength(20)]
        public string SellAccount_Name { get; set; }

        [Display(Name = "销账日期")]
        public DateTime? SellAccount_Date { get; set; }

        [Display(Name = "产生标志")]
        public AirOutEnumType.Bms_BillCreate_Status Create_Status { get; set; }

        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "开航日期")]
        public DateTime? Flight_Date_Want { get; set; }

        [Display(Name = "目的港")]
        public string End_Port { get; set; }

        [Display(Name = "目的港名称")]
        [MaxLength(100)]
        public string End_PortNAME { get; set; }

        #region ECC

        [Display(Name = "ECC发票号", Description = "ECC 回填")]
        [StringLength(50)]
        public string Sumbmit_ECCNo { get; set; }

        [Display(Name = "采购订单号", Description = "ECC 回填")]
        [MaxLength(50)]
        public string SignIn_ECCNo { get; set; }

        [Display(Name = "ECC状态", Description = "")]
        public bool? ECC_Status { get; set; }

        [Display(Name = "ECC状态信息", Description = "")]
        [MaxLength(500)]
        public string ECC_StatusMsg { get; set; }

        [Display(Name = "ECC发票发送时间", Description = "")]
        public DateTime? ECC_InvoiceSendDate { get; set; }

        [Display(Name = "ECC发票接收时间", Description = "")]
        public DateTime? ECC_InvoiceRecvDate { get; set; }

        #endregion
        
        #region 销售 2019-03-14新增

        [Display(Name = "销售人", Description = "")]
        [Required]
        [MaxLength(50)]
        public string SallerName { get; set; }

        [Display(Name = "销售", Description = "")]
        public int? SallerId { get; set; }

        #endregion

        #region 2019-3-26 新增

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string BillEDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? BillEDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        public string BillEDITID { get; set; }

        [Display(Name = "修改人名称")]
        [MaxLength(100)]
        public string BillEDITWHONAME { get; set; }

        #endregion

        #region ScaffoldColumn

        [Index("BmsBillAp_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改人名称")]
        [MaxLength(100)]
        public string EDITWHONAME { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion

    }

    public class FinanceChangeViewModel
    {
        public IEnumerable<Finance> inserted { get; set; }

        public IEnumerable<Finance> deleted { get; set; }

        public IEnumerable<Finance> updated { get; set; }
    }

    public class FinanceDtl
    {
        public FinanceDtl()
        {
            AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
            Status = AirOutEnumType.UseStatusEnum.Enable;
            MBL = "-";
        }

        public int Id { get; set; }

        [Display(Name = "应付账单", Description = "true:应付账单,false:应收账单")]
        public bool IsAr { get; set; }

        [Display(Name = "总单号")]
        [Required, MaxLength(50)]
        public string MBL { get; set; }

        [Display(Name = "总单结算", Description = "分摊数据时产生")]
        public bool IsMBLJS { get; set; }

        [Display(Name = "分摊主数据", Description = "分摊数据时，记录有哪个数据 分摊下来的")]
        public int FTParentId { get; set; }

        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        //[Display(Name = "分单Id")]
        //public int Ops_H_OrdId { get; set; }

        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Display(Name = "帐单类型")]
        [MaxLength(20)]
        public string Bill_Type { get; set; }

        [Display(Name = "序号")]
        public int Line_No { get; set; }

        [Display(Name = "费用序号")]
        public int Line_Id { get; set; }

        [Display(Name = "结算方")]
        [MaxLength(50)]
        public string Bill_Object_Id { get; set; }

        [Display(Name = "结算方名称")]
        [MaxLength(100)]
        public string Bill_Object_Name { get; set; }

        [Display(Name = "币种")]
        [MaxLength(20)]
        public string Money_Code { get; set; }

        [Display(Name = "币种名称")]
        [MaxLength(100)]
        public string Money_CodeNAME { get; set; }

        [Display(Name = "费用代码")]
        [MaxLength(20)]
        public string Charge_Code { get; set; }

        [Display(Name = "费用名称")]
        [MaxLength(20)]
        public string Charge_Desc { get; set; }

        [Display(Name = "实际单价")]
        public decimal Unitprice2 { get; set; }

        [Display(Name = "数量")]
        public decimal Qty { get; set; }

        [Display(Name = "实际金额")]
        public decimal Account2 { get; set; }

        [Display(Name = "含税/不含税")]
        public bool Bill_HasTax { get; set; }

        [Display(Name = "税率")]
        [Range(0, int.MaxValue)]
        public decimal Bill_TaxRate { get; set; }

        [Display(Name = "价")]
        public decimal Bill_Amount { get; set; }

        [Display(Name = "税金")]
        public decimal Bill_TaxAmount { get; set; }

        [Display(Name = "价税合计")]
        public decimal Bill_AmountTaxTotal { get; set; }

        [Display(Name = "帐单日期")]
        public DateTime? Bill_Date { get; set; }

        [Display(Name = "支付方式")]
        [MaxLength(20)]
        public string Payway { get; set; }

        [Display(Name = "支付方式名称")]
        [MaxLength(100)]
        public string PaywayNAME { get; set; }

        [Display(Name = "备注信息")]
        [MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "审核号")]
        [MaxLength(50)]
        public string AuditNo { get; set; }

        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "作废标志")]
        public bool Cancel_Status { get; set; }

        [Display(Name = "提交标志")]
        public bool Sumbmit_Status { get; set; }

        [Display(Name = "提交号")]
        [MaxLength(50)]
        public string Sumbmit_No { get; set; }

        [Display(Name = "提交人")]
        [MaxLength(50)]
        public string Sumbmit_Name { get; set; }

        [Display(Name = "提交人")]
        [MaxLength(50)]
        public string Sumbmit_Id { get; set; }

        [Display(Name = "提交时间")]
        public DateTime? Sumbmit_Date { get; set; }

        [Display(Name = "签收标志")]
        public bool SignIn_Status { get; set; }

        [Display(Name = "签收号")]
        [MaxLength(50)]
        public string SignIn_No { get; set; }

        [Display(Name = "开票标志")]
        public bool Invoice_Status { get; set; }

        [Display(Name = "开票号码")]
        [MaxLength(50)]
        public string Invoice_No { get; set; }

        [Display(Name = "开票币种")]
        [MaxLength(50)]
        public string Invoice_MoneyCode { get; set; }

        [Display(Name = "开票费目")]
        [MaxLength(100)]
        public string Invoice_FeeType { get; set; }

        [Display(Name = "开票人Id")]
        [MaxLength(50)]
        public string Invoice_Id { get; set; }

        [Display(Name = "开票人")]
        [MaxLength(20)]
        public string Invoice_Name { get; set; }

        [Display(Name = "开票日期")]
        public DateTime? Invoice_Date { get; set; }

        [Display(Name = "销账标志")]
        public bool SellAccount_Status { get; set; }

        [Display(Name = "销账人")]
        [MaxLength(20)]
        public string SellAccount_Name { get; set; }

        [Display(Name = "销账日期")]
        public DateTime? SellAccount_Date { get; set; }

        [Display(Name = "产生标志")]
        public AirOutEnumType.Bms_BillCreate_Status Create_Status { get; set; }

        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        #region 2019-3-26 新增

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string BillEDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? BillEDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        public string BillEDITID { get; set; }

        [Display(Name = "修改人名称")]
        [MaxLength(100)]
        public string BillEDITWHONAME { get; set; }

        #endregion

        #region ScaffoldColumn

        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改人名称")]
        [MaxLength(100)]
        public string EDITWHONAME { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }

    public class Bill_AccountTotalByMoney_Code
    {
        [Display(Name = "应付账单", Description = "true:应付账单,false:应收账单")]
        public bool IsAr { get; set; }

        [Display(Name = "币种")]
        [MaxLength(20)]
        public string Money_Code { get; set; }

        [Display(Name = "理论金额汇总")]
        public decimal Bill_Account2Total { get; set; }

        [Display(Name = "理论金额转换CNY汇总")]
        public decimal NewBill_Account2Total { get; set; }
    }

    /// <summary>
    /// 增值税
    /// </summary>
    public class ApValAddedTax
    {
        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        [Display(Name = "总单号")]
        [Required, MaxLength(50)]
        public string MBL { get; set; }

        [Display(Name = "提交号")]
        [MaxLength(50)]
        public string Sumbmit_No { get; set; }

        [Display(Name = "币种")]
        [MaxLength(20)]
        public string Money_Code { get; set; }

        [Display(Name = "金额", Description = "提交号下所有账单的实际金额汇总，加币种")]
        public decimal Bill_Account2 { get; set; }

        [Display(Name = "结算方代码", Description = "供方代码")]
        [MaxLength(50)]
        public string Bill_Object_Id { get; set; }

        [Display(Name = "结算方", Description = "供方名称")]
        [MaxLength(100)]
        public string Bill_Object_Name { get; set; }

        [Display(Name="识别号",Description = "统一社会信用代码")]
        [Required, MaxLength(50)]
        public string UnifiedSocialCreditCode { get; set; }

        [Display(Name = "开票国家代码", Description = "开票国家代码")]
        [Required]
        [MaxLength(50)]
        public string InvoiceCountryCode { get; set; }

        [Display(Name = "地址", Description = "开票地址")]
        [Required]
        [MaxLength(100)]
        public string InvoiceAddress { get; set; }

        [Display(Name = "开户行", Description = "银行名称")]
        [Required]
        [MaxLength(100)]
        public string BankName { get; set; }

        [Display(Name = "账号", Description = "银行账号")]
        [Required]
        [MaxLength(100)]
        public string BankAccount { get; set; }

        [Display(Name = "价")]
        public decimal Bill_Amount { get; set; }

        [Display(Name = "税金")]
        public decimal Bill_TaxAmount { get; set; }

        [Display(Name = "价税合计")]
        public decimal Bill_AmountTaxTotal { get; set; }

        [Display(Name = "备注", Description = "默认：空运出口/总单号/KSF号（委托编号）/开航日期/目的港（三字代码）")]
        [MaxLength(200)]
        public string Remark { get; set; }

    }
}