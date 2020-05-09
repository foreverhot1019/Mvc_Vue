using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AirOut.Web;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(Bms_Bill_ApMetadata))]
    public partial class Bms_Bill_Ap
    {
    }

    public partial class Bms_Bill_ApMetadata
    {
        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name="Id",Description="Id")]
        public int Id{ get; set; }

        [Display(Name = "总单号")]
        [Required, MaxLength(50)]
        public string MBL { get; set; }

        [Display(Name = "总单结算", Description = "分摊数据时产生")]
        public bool IsMBLJS { get; set; }

        [Display(Name = "分摊主数据", Description = "分摊数据时，记录有哪个数据 分摊下来的")]
        public int FTParentId { get; set; }

        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        [Display(Name = "分单Id")]
        public int Ops_H_OrdId { get; set; }

        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Display(Name = "序号")]
        public int Line_No { get; set; }

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
        [Range(0, int.MaxValue)]
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

        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

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

        [Display(Name = "代收代付状态")]
        public int Dsdf_Status { get; set; }

        [Display(Name = "摘要")]
        [MaxLength(100)]
        public string Summary { get; set; }

        [Display(Name = "产生标志")]
        public AirOutEnumType.Bms_BillCreate_Status Create_Status { get; set; }

        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

	}

	public class Bms_Bill_ApChangeViewModel
    {
        public IEnumerable<Bms_Bill_Ap> inserted { get; set; }

        public IEnumerable<Bms_Bill_Ap> deleted { get; set; }

        public IEnumerable<Bms_Bill_Ap> updated { get; set; }
    }
}
