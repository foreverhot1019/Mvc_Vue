using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //应收帐单
    public partial class Bms_Bill_Ar : Entity
    {
        public Bms_Bill_Ar()
        {
            AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
            Status = AirOutEnumType.UseStatusEnum.Enable;
            MBL = "-";
            ArrBms_Bill_Ar_Dtl = new List<Bms_Bill_Ar_Dtl>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_ArMBL", IsUnique = false, Order = 1)]
        [Display(Name = "总单号")]
        [Required, MaxLength(50)]
        public string MBL { get; set; }

        [Index("IX_ArOpsMId", IsUnique = false, Order = 1)]
        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        [Display(Name = "分单Id")]
        public int Ops_H_OrdId { get; set; }

        [Index("UQ_BmsBillAr", IsUnique = true, Order = 1)]
        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Index("UQ_BmsBillAr", IsUnique = true, Order = 2)]
        [Display(Name = "序号")]
        public int Line_No { get; set; }

        [Index("IX_ArBill_Type", IsUnique = false, Order = 1)]
        [Display(Name = "帐单类型")]
        [MaxLength(50)]
        public string Bill_Type { get; set; }

        #region 2018-11-05 新增

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

        #endregion

        [Display(Name = "理论金额")]
        public decimal Bill_Account { get; set; }

        [Display(Name = "实际金额")]
        public decimal Bill_Account2 { get; set; }

        [Display(Name = "币种")]
        [MaxLength(50)]
        public string Money_Code { get; set; }

        [Display(Name = "客户代码")]
        [MaxLength(50)]
        public string Bill_Object_Id { get; set; }

        [Display(Name = "客户名称")]
        [MaxLength(300)]
        public string Bill_Object_Name { get; set; }

        [Display(Name = "支付方式")]
        [MaxLength(50)]
        public string Payway { get; set; }

        [Display(Name = "备注信息")]
        [MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "帐单日期")]
        public DateTime? Bill_Date { get; set; }

        [Display(Name = "原始币别")]
        [MaxLength(50)]
        public string Org_Money_Code { get; set; }

        [Display(Name = "原实际金额")]
        public decimal Org_Bill_Account2 { get; set; }

        [Display(Name = "审核号")]
        [MaxLength(50)]
        public string AuditNo { get; set; }

        [Display(Name = "审核人Id")]
        [MaxLength(50)]
        public string AuditId { get; set; }

        [Display(Name = "审核人")]
        [MaxLength(20)]
        public string AuditName { get; set; }

        [Display(Name = "审核日期")]
        public DateTime? AuditDate { get; set; }

        [Index("IX_ArCAStatus", IsUnique = false, Order = 2)]
        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Index("IX_ArCAStatus", IsUnique = false, Order = 1)]
        [Display(Name = "作废标志")]
        public bool Cancel_Status { get; set; }

        [Display(Name = "作废人Id")]
        [MaxLength(50)]
        public string Cancel_Id { get; set; }

        [Display(Name = "作废人")]
        [MaxLength(20)]
        public string Cancel_Name { get; set; }

        [Display(Name = "作废日期")]
        public DateTime? Cancel_Date { get; set; }

        #region 2018-11-05 新增

        [Index("IX_ArSmbtStatus", IsUnique = false, Order = 1)]
        [Display(Name = "提交标志")]
        public bool Sumbmit_Status { get; set; }

        [Display(Name = "发票号")]
        [MaxLength(50)]
        public string Sumbmit_No { get; set; }

        [Display(Name = "原始提交号")]
        [MaxLength(50)]
        public string Sumbmit_No_Org { get; set; }

        [Display(Name = "提交人Id")]
        [MaxLength(50)]
        public string Sumbmit_Id { get; set; }

        [Display(Name = "提交人")]
        [MaxLength(20)]
        public string Sumbmit_Name { get; set; }

        [Display(Name = "提交日期")]
        public DateTime? Sumbmit_Date { get; set; }

        //[Index("IX_ArSignStatus", IsUnique = false, Order = 1)]
        //[Display(Name = "签收标志")]
        //public bool SignIn_Status { get; set; }

        //[Display(Name = "签收号")]
        //[MaxLength(50)]
        //public string SignIn_No { get; set; }

        //[Display(Name = "签收人Id")]
        //[MaxLength(50)]
        //public string SignIn_Id { get; set; }

        //[Display(Name = "签收人")]
        //[MaxLength(20)]
        //public string SignIn_Name { get; set; }

        //[Display(Name = "签收日期")]
        //public DateTime? SignIn_Date { get; set; }

        //[Index("IX_ArInvcStatus", IsUnique = false, Order = 1)]
        [Display(Name = "开票标志")]
        public bool Invoice_Status { get; set; }

        [Display(Name = "开票号码")]
        [MaxLength(50)]
        public string Invoice_No { get; set; }

        [Display(Name = "开票名称", Description = "")]
        [MaxLength(100)]
        public string Invoice_Desc { get; set; }

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

        //[Index("IX_ArSllaStatus", IsUnique = false, Order = 1)]
        [Display(Name = "销账标志")]
        public bool SellAccount_Status { get; set; }

        [Display(Name = "销账人Id")]
        [MaxLength(50)]
        public string SellAccount_Id { get; set; }

        [Display(Name = "销账人")]
        [MaxLength(20)]
        public string SellAccount_Name { get; set; }

        [Display(Name = "销账日期")]
        public DateTime? SellAccount_Date { get; set; }

        #endregion

        #region 暂时不用
        
        [Display(Name = "代收代付状态")]
        public int Dsdf_Status { get; set; }

        [Display(Name = "代开帐单记录号")]
        [MaxLength(50)]
        public string Dk_Bill_Id { get; set; }

        [Display(Name = "代开业务编号")]
        [MaxLength(50)]
        public string Dk_Operation_Id { get; set; }

        [Display(Name = "分摊前金额")]
        public decimal Nowent_Acc { get; set; }

        #endregion

        #region ECC

        [Display(Name = "ECC发票号", Description = "ECC 回填")]
        [MaxLength(50)]
        public string Sumbmit_ECCNo { get; set; }

        [Display(Name = "开票币种")]
        [MaxLength(50)]
        public string Invoice_MoneyCode { get; set; }

        [Display(Name = "开票费目")]
        [MaxLength(100)]
        public string Invoice_FeeType { get; set; }

        [Display(Name = "开票税率")]
        [MaxLength(50)]
        public string Invoice_TaxRateType { get; set; }

        [Display(Name = "含税/不含税")]
        public bool Invoice_HasTax { get; set; }

        [Display(Name = "ECC开票汇率")]
        public decimal? ECCInvoice_Rate { get; set; }

        [Display(Name = "ECC状态", Description = "")]
        public bool? ECC_Status { get; set; }

        [Display(Name = "ECC状态信息", Description = "")]
        [MaxLength(500)]
        public string ECC_StatusMsg { get; set; }

        [Display(Name = "ECC发票发送时间", Description = "")]
        public DateTime? ECC_InvoiceSendDate { get; set; }

        [Display(Name = "ECC发票接收时间", Description = "")]
        public DateTime? ECC_InvoiceRecvDate{ get; set; }

        #endregion

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

        #endregion

        [Display(Name = "摘要")]
        [MaxLength(100)]
        public string Summary { get; set; }

        [Display(Name = "产生标志")]
        public AirOutEnumType.Bms_BillCreate_Status Create_Status { get; set; }

        [Index("BmsBillAr_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "应收帐单明细", Description = "")]
        public virtual List<Bms_Bill_Ar_Dtl> ArrBms_Bill_Ar_Dtl { get; set; }

        #region ScaffoldColumn

        [Index("BmsBillAr_OP", IsUnique = false, Order = 1)]
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

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion

    }

    /// <summary>
    /// 应收/付 视图
    /// </summary>
    public class BmsBillAp_ArView
    {
        ////（原来）主单 挂应收 委托挂应付
        ////委托信息
        //public OPS_EntrustmentInfor OOPS_EntrustmentInfor { get; set; }
        ////分单信息
        //public OPS_H_Order OOPS_H_Order { get; set; }
        ////主单信息
        //public OPS_M_Order OOPS_M_Order { get; set; }
        ////报关报检
        //public CustomsInspection OCustomsInspection { get; set; }

        [Display(Name = "应收账单", Description = "")]
        public bool IsBmsBillAr { get; set; }

        [Display(Name = "总单Key", Description = "")]
        public int OPS_M_Id { get; set; }

        [Display(Name = "分单Key", Description = "")]
        public int OPS_H_Id { get; set; }

        [Display(Name = "业务编号", Description = "分单和委托 对应字段")]
        [Required]
        public string Operation_Id { get; set; }

        [Display(Name = "总运单号", Description = "")]
        public string MBL { get; set; }

        [Display(Name = "分单号", Description = "")]
        public string HBL { get; set; }

        [Display(Name = "航班日期", Description = "")]
        public DateTime? Flight_Date_Want { get; set; }

        [Display(Name = "结算方", Description = "")]
        public string Settle_Code { get; set; }

        [Display(Name = "结算方", Description = "结算方名称")]
        public string Settle_CodeNAME { get; set; }

        [Display(Name = "委托方", Description = "")]
        public string Customer_Code { get; set; }

        [Display(Name = "委托方", Description = "委托方名称")]
        public string Customer_CodeNAME { get; set; }

        [Display(Name = "分单结算重量", Description = "")]
        public decimal? Settle_Weight { get; set; }

        [Display(Name = "件数", Description = "")]
        public decimal? Num { get; set; }

        [Display(Name = "体积", Description = "")]
        public decimal? Volume { get; set; }

        [Display(Name = "付款方式", Description = "")]
        public string Pay_Mode { get; set; }

        [Display(Name = "报关单套数", Description = "")]
        [Range(0, 9999)]
        public int BG_Num { get; set; }

        [Display(Name = "报关单张数", Description = "")]
        [Range(0, 9999)]
        public int BG_PageNum { get; set; }

        [Display(Name = "航空公司", Description = "")]
        public string Airways_Code { get; set; }

        [Display(Name = "航空公司", Description = "航空公司名称")]
        public string Airways_CodeNAME { get; set; }

        [Display(Name = "国外代理", Description = "")]
        public string Foreign_Proxy { get; set; }

        [Display(Name = "国外代理", Description = "国外代理名称")]
        public string Foreign_ProxyNAME { get; set; }

        [Display(Name = "目的港", Description = "")]
        public string DestinationPort { get; set; }

        [Display(Name = "总单结算重量", Description = "")]
        public decimal? Weight { get; set; }

        [Display(Name = "作废标志", Description = "")]
        public bool Cancellation { get; set; }

        [Display(Name = "航班号", Description = "")]
        public string Flight_No { get; set; }

        [Display(Name = "审核号")]
        [MaxLength(50)]
        public string AuditNo { get; set; }

        [Display(Name = "审核人Id")]
        public string AuditId { get; set; }

        [Display(Name = "审核人")]
        public string AuditName { get; set; }

        [Display(Name = "审核日期")]
        public DateTime? AuditDate { get; set; }

        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "审批状态")]
        public string AuditStatusName { get; set; }

        [Display(Name = "结算备注")]
        public string Remark { get; set; }

        ////应收帐单
        //public Bms_Bill_Ar OBms_Bill_Ar { get; set; }

        ////应收帐单明细
        //public Bms_Bill_Ar_Dtl ArrBms_Bill_Ar_Dtl { get; set; }
    }

    /// <summary>
    /// 弹出框 添加编辑 应收/付 账单 
    /// </summary>
    public class BmsBillAp_ArPopupView
    {
        [Display(Name = "是否应收账单")]
        public bool? IsBms_Bill_Ar { get; set; }

        ////新增表头
        //public bool AddHead { get; set; }
        ////编辑表体
        //public bool EditHead { get; set; }

        [Display(Name = "主单Id")]
        [Range(0, int.MaxValue)]
        public int OPS_M_OrdId { get; set; }

        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Display(Name = "总单号")]
        [MaxLength(50)]
        public string MBL { get; set; }

        [Display(Name = "总单结算")]
        public bool IsMBLJS { get; set; }

        [Display(Name = "表头")]
        public Ar_ApHead Ar_ApHead { get; set; }

        [Display(Name = "表体")]
        public Ar_ApDetl Ar_ApDetl { get; set; }

        [Display(Name = "表体")]
        public IEnumerable<Ar_ApDetl> ArrAr_ApDtl { get; set; }
    }

    /// <summary>
    /// 编辑 应收/付 账单 表头
    /// </summary>
    public class Ar_ApHead
    {
        #region 表头

        [Display(Name = "账单Id")]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [Display(Name = "帐单类型")]
        [MaxLength(50)]
        public string Bill_Type { get; set; }

        [Display(Name = "币种")]
        [MaxLength(50)]
        public string Money_Code { get; set; }

        [Display(Name = "支付方式")]
        [MaxLength(50)]
        public string Payway { get; set; }

        [Display(Name = "客户代码")]
        [MaxLength(50)]
        public string Bill_Object_Id { get; set; }

        [Display(Name = "原始币别")]
        [MaxLength(50)]
        public string Org_Money_Code { get; set; }

        [Display(Name = "备注信息")]
        [MaxLength(100)]
        public string Remark { get; set; }

        [Display(Name = "产生标志")]
        public int Create_Status { get; set; }

        [Display(Name = "客户名称")]
        [MaxLength(100)]
        public string Bill_Object_Name { get; set; }

        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "税率类型")]
        [MaxLength(50)]
        public string Bill_TaxRateType { get; set; }

        [Display(Name = "税率")]
        [Range(0, int.MaxValue)]
        public decimal Bill_TaxRate { get; set; }

        [Display(Name = "含税/不含税")]
        public bool Bill_HasTax { get; set; }

        #endregion
    }

    /// <summary>
    /// 编辑 应收/付 账单 表体
    /// </summary>
    public class Ar_ApDetl
    {
        #region 表体

        [Display(Name = "账单明细表头Id")]
        [Range(0, int.MaxValue)]
        public int DtlHeadId { get; set; }

        [Display(Name = "账单明细Id")]
        [Range(0, int.MaxValue)]
        public int DtlId { get; set; }

        [Display(Name = "费用代码", Description = "")]
        [Required, MaxLength(50)]
        public string Charge_Code { get; set; }

        [Display(Name = "费用名称", Description = "")]
        [MaxLength(100)]
        public string Charge_Desc { get; set; }

        [Display(Name = "实际单价", Description = "")]
        public decimal Unitprice2 { get; set; }

        [Display(Name = "数量", Description = "")]
        public decimal Qty { get; set; }

        [Display(Name = "实际金额", Description = "")]
        public decimal Account2 { get; set; }

        [Display(Name = "摘要", Description = "")]
        [MaxLength(100)]
        public string Summary { get; set; }

        #endregion
    }

    /// <summary>
    /// 税金编辑
    /// </summary>
    public class Ar_ApTaxRateEditView
    {
        [Display(Name = "是否应收账单")]
        public bool? IsBms_Bill_Ar { get; set; }

        [Display(Name = "应收/付Id")]
        [Range(0, int.MaxValue)]
        public int Bms_Bill_ArAp_Id { get; set; }

        [Display(Name = "实际金额")]
        public decimal Bill_Account2 { get; set; }

        [Display(Name = "价")]
        public decimal Bill_Amount { get; set; }

        [Display(Name = "税金")]
        public decimal Bill_TaxAmount { get; set; }

        [Display(Name = "税金", Description = "税金整数位")]
        public int Bill_TaxAmount_Profix { get; set; }

        [Display(Name = "税金",Description="税金小数位")]
        [Range(0,99)]
        public int Bill_TaxAmount_Precision { get; set; }

        [Display(Name = "价税合计")]
        public decimal Bill_AmountTaxTotal { get; set; }
    }

}