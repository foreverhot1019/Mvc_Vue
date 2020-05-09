using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //应付帐单
    public partial class Bms_Bill_Ap : Entity
    {
        public Bms_Bill_Ap()
        {
            AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
            Status = AirOutEnumType.UseStatusEnum.Enable;
            MBL = "-";
            ArrBms_Bill_Ap_Dtl = new List<Bms_Bill_Ap_Dtl>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_ApMBL", IsUnique = false, Order = 1)]
        [Display(Name = "总单号")]
        [Required,MaxLength(50)]
        public string MBL { get; set; }

        [Index("IX_ApIsMBLJS", IsUnique = false, Order = 1)]
        [Display(Name = "总单结算",Description="分摊数据时产生")]
        public bool IsMBLJS { get; set; }

        [Display(Name = "分摊主数据", Description = "分摊数据时，记录有哪个数据 分摊下来的")]
        public int FTParentId { get; set; }

        [Index("IX_ApOpsMId", IsUnique = false, Order = 1)]
        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        [Display(Name = "分单Id")]
        public int Ops_H_OrdId { get; set; }

        [Index("UQ_BmsBillAp", IsUnique = true, Order = 1)]
        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Index("UQ_BmsBillAp", IsUnique = true, Order = 2)]
        [Display(Name = "序号")]
        public int Line_No { get; set; }

        [Index("IX_ApBill_Type", IsUnique = false, Order = 1)]
        [Display(Name = "帐单类型")]
        [MaxLength(20)]
        public string Bill_Type { get; set; }

        [Display(Name = "理论金额")]
        public decimal Bill_Account { get; set; }
        
        #region 2018-11-05 新增
        
        [Display(Name = "价")]
        public decimal Bill_Amount { get; set; }

        [Display(Name = "税率类型")]
        [MaxLength(50)]
        public string Bill_TaxRateType { get; set; }

        [Display(Name = "税率")]
        [Range(0,int.MaxValue)]
        public decimal Bill_TaxRate { get; set; }

        [Display(Name = "含税/不含税")]
        public bool Bill_HasTax { get; set; }

        [Display(Name = "税金")]
        public decimal Bill_TaxAmount { get; set; }

        [Display(Name = "价税合计")]
        public decimal Bill_AmountTaxTotal { get; set; }
        
        #endregion

        [Display(Name = "实际金额")]
        public decimal Bill_Account2 { get; set; }

        [Display(Name = "币种")]
        [MaxLength(20)]
        public string Money_Code { get; set; }

        [Display(Name = "供方代码")]
        [MaxLength(50)]
        public string Bill_Object_Id { get; set; }

        [Display(Name = "供方名称")]
        [MaxLength(300)]
        public string Bill_Object_Name { get; set; }

        [Display(Name = "支付方式")]
        [MaxLength(20)]
        public string Payway { get; set; }

        [Display(Name = "备注信息")]
        [MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "帐单日期")]
        public DateTime? Bill_Date { get; set; }

        [Display(Name = "原始币别")]
        [MaxLength(20)]
        public string Org_Money_Code { get; set; }

        [Display(Name = "原实际金额")]
        public decimal Org_Bill_Account2 { get; set; }
       
        [Display(Name = "分摊前金额")]
        public decimal Nowent_Acc { get; set; }

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

        [Index("IX_ApCAStatus", IsUnique = false, Order = 2)]
        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Index("IX_ApCAStatus", IsUnique = false, Order = 1)]
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

        [Index("IX_ApSmbtStatus", IsUnique = false, Order = 1)]
        [Display(Name = "提交标志")]
        public bool Sumbmit_Status { get; set; }

        [Display(Name = "提交号")]
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

        //[Index("IX_ApSignStatus", IsUnique = false, Order = 1)]
        [Display(Name = "签收标志")]
        public bool SignIn_Status { get; set; }

        [Display(Name = "签收号")]
        [MaxLength(50)]
        public string SignIn_No { get; set; }

        [Display(Name = "签收人Id")]
        [MaxLength(50)]
        public string SignIn_Id { get; set; }

        [Display(Name = "签收人")]
        [MaxLength(20)]
        public string SignIn_Name { get; set; }

        [Display(Name = "签收日期")]
        public DateTime? SignIn_Date { get; set; }

        //[Index("IX_ApInvcStatus", IsUnique = false, Order = 1)]
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

        //[Index("IX_ApSllaStatus", IsUnique = false, Order = 1)]
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

        #region ECC
        
        [Display(Name = "采购订单号", Description = "ECC 回填")]
        [MaxLength(50)]
        public string SignIn_ECCNo { get; set; }

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
        public DateTime? ECC_InvoiceRecvDate { get; set; }

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

        [Display(Name = "代收代付状态")]
        public int Dsdf_Status { get; set; }

        [Display(Name = "摘要")]
        [MaxLength(100)]
        public string Summary { get; set; }

        [Display(Name = "产生标志")]
        public AirOutEnumType.Bms_BillCreate_Status Create_Status { get; set; }

        [Index("BmsBillAp_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "应付帐单明细", Description = "")]
        public virtual List<Bms_Bill_Ap_Dtl> ArrBms_Bill_Ap_Dtl { get; set; }

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

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion

    }
}