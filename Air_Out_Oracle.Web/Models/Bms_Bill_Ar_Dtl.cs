using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //应收帐单明细
    public partial class Bms_Bill_Ar_Dtl : Entity
    {
        public Bms_Bill_Ar_Dtl()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "总单Id")]
        public int Ops_M_OrdId { get; set; }

        [Display(Name = "分单Id")]
        public int Ops_H_OrdId { get; set; }

        [Display(Name = "应收帐单Id", Description = "")]
        public int? Bms_Bill_Ar_Id { get; set; }

        [ForeignKey("Bms_Bill_Ar_Id")]
        [Display(Name = "应收帐单", Description = "")]
        public virtual Bms_Bill_Ar OBms_Bill_Ar { get; set; }

        [Index("UQ_BmsBillArDtl", IsUnique = true, Order = 1)]
        [Display(Name = "业务编号", Description = "")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Index("UQ_BmsBillArDtl", IsUnique = true, Order = 2)]
        [Display(Name = "序号")]
        public int Line_No { get; set; }

        [Index("UQ_BmsBillArDtl", IsUnique = true, Order = 3)]
        [Display(Name = "明细序号")]
        public int Line_Id { get; set; }

        [Index("IX_ArDtl_CHCode", IsUnique = false, Order = 1)]
        [Display(Name = "费用代码", Description = "")]
        [Required,MaxLength(50)]
        public string Charge_Code { get; set; }

        [Display(Name = "费用名称", Description = "")]
        [MaxLength(100)]
        public string Charge_Desc { get; set; }

        [Display(Name = "理论单价", Description = "")]
        public decimal Unitprice { get; set; }

        [Display(Name = "实际单价", Description = "")]
        public decimal Unitprice2 { get; set; }

        [Display(Name = "数量", Description = "")]
        public decimal Qty { get; set; }

        [Display(Name = "理论金额", Description = "")]
        public decimal Account { get; set; }

        [Display(Name = "实际金额", Description = "")]
        public decimal Account2 { get; set; }

        [Display(Name = "币种", Description = "")]
        [MaxLength(50)]
        public string Money_Code { get; set; }

        [Display(Name = "摘要", Description = "")]
        [MaxLength(100)]
        public string Summary { get; set; }

        #region 对账

        [Display(Name = "对帐人Id", Description = "")]
        [MaxLength(50)]
        public string Collate_Id { get; set; }

        [Display(Name = "对帐人", Description = "")]
        [MaxLength(50)]
        public string Collate_Name { get; set; }

        [Display(Name = "对帐日期", Description = "")]
        public DateTime? Collate_Date { get; set; }

        [Display(Name = "对帐标志", Description = "")]
        public int Collate_Status { get; set; }

        [Display(Name = "对账号码", Description = "")]
        [MaxLength(50)]
        public string Collate_No { get; set; }

        [Display(Name = "操作人员", Description = "")]
        [MaxLength(50)]
        public string User_Id { get; set; }

        [Display(Name = "操作人名", Description = "")]
        [MaxLength(50)]
        public string User_Name { get; set; }

        #endregion

        #region 表头标志

        [Display(Name = "审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "审核号")]
        [MaxLength(50)]
        public string AuditNo { get; set; }

        [Display(Name = "开票标志")]
        public bool Invoice_Status { get; set; }

        [Display(Name = "开票号码")]
        [MaxLength(50)]
        public string Invoice_No { get; set; }

        [Display(Name = "提交标志")]
        public bool Sumbmit_Status { get; set; }

        [Display(Name = "提交号")]
        [MaxLength(50)]
        public string Sumbmit_No { get; set; }

        [Display(Name = "销账标志")]
        public bool SellAccount_Status { get; set; }

        [Display(Name = "作废标志")]
        public bool Cancel_Status { get; set; }

        #endregion
        
        #region 2018-11-05 新增

        [Display(Name = "价")]
        public decimal Bill_Amount { get; set; }

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

        [Display(Name = "产生标志")]
        public AirOutEnumType.Bms_BillCreate_Status Create_Status { get; set; }

        [Index("BmsBillArDtl_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        #region ScaffoldColumn

        [Index("BmsBillArDtl_OP", IsUnique = false, Order = 1)]
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