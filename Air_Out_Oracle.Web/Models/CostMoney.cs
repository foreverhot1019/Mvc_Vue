using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //成本
    public partial class CostMoney : Entity
    {
        public CostMoney()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
            AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "记录号", Description = "C201805040001:C+年月日+4位流水码")]
        [Required,MaxLength(50)]
        [Index("IX_CostS_Key", IsUnique = true, Order = 1)]
        public string SerialNo { get; set; }

        [Display(Name = "结算方", Description = "")]
        [Required, MaxLength(50)]
        public string SettleAccount { get; set; }

        [Display(Name = "费用代码", Description = "")]
        [Required, MaxLength(50)]
        public string FeeCode { get; set; }

        [Display(Name = "费用名称", Description = "")]
        [Required, MaxLength(50)]
        public string FeeName { get; set; }

        [Display(Name = "KSF", Description = "KSF")]
        public bool? Is_Book_Flat { get; set; }

        [Display(Name = "起始地", Description = "")]
        [MaxLength(50)]
        public string StartPlace { get; set; }

        [Display(Name = "中转地", Description = "")]
        [MaxLength(50)]
        public string TransitPlace { get; set; }

        [Display(Name = "目的地", Description = "")]
        [MaxLength(50)]
        public string EndPlace { get; set; }

        [Display(Name = "航空公司", Description = "")]
        [MaxLength(50)]
        public string AirLineCo { get; set; }

        [Display(Name = "航班号", Description = "")]
        [MaxLength(50)]
        public string AirLineNo { get; set; }

        [Display(Name = "订舱方", Description = "")]
        [MaxLength(50)]
        public string WHBuyer { get; set; }

        [Display(Name = "代操作", Description = "")]
        [MaxLength(50)]
        public string ProxyOperator { get; set; }

        [Display(Name = "成交条款", Description = "")]
        [MaxLength(50)]
        public string DealWithArticle { get; set; }

        [Display(Name = "BSA", Description = "")]
        [MaxLength(50)]
        public string BSA { get; set; }

        [Display(Name = "报关方式", Description = "")]
        [MaxLength(50)]
        public string CustomsType { get; set; }

        [Display(Name = "商检标志", Description = "")]
        [MaxLength(50)]
        public string InspectMark { get; set; }

        [Display(Name = "取单标志", Description = "")]
        [MaxLength(50)]
        public string GetOrdMark { get; set; }

        [Display(Name = "靠级", Description = "")]
        [MaxLength(50)]
        public string MoorLevel { get; set; }

        [Display(Name = "计算单位", Description = "")]
        [Required, MaxLength(50)]
        public string BillingUnit { get; set; }

        [Display(Name = "单价", Description = "")]
        public decimal Price { get; set; }

        [Display(Name = "币别", Description = "")]
        [Required, MaxLength(50)]
        public string CurrencyCode { get; set; }

        [Display(Name = "计费条件值1", Description = "")]
        public decimal FeeConditionVal1 { get; set; }

        [Display(Name = "计费条件运算符1", Description = "")]
        [MaxLength(50)]
        public string CalcSign1 { get; set; }

        [Display(Name = "计费单位", Description = "")]
        [MaxLength(50)]
        public string FeeCondition { get; set; }

        [Display(Name = "计费条件运算符2", Description = "")]
        [MaxLength(50)]
        public string CalcSign2 { get; set; }

        [Display(Name = "计费条件值2", Description = "")]
        public decimal FeeConditionVal2 { get; set; }

        [Display(Name = "计算公式", Description = "")]
        [Required, MaxLength(50)]
        public string CalcFormula { get; set; }

        [Display(Name = "费用MIN", Description = "")]
        public decimal? FeeMin { get; set; }

        [Display(Name = "费用MAX", Description = "")]
        public decimal? FeeMax { get; set; }

        [Display(Name = "审批状态", Description = "")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "开始日期", Description = "")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "结束日期", Description = "")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "交货点", Description = "")]
        [MaxLength(50)]
        public string DeliveryPoint { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Index("IX_CostS_Point", IsUnique = false, Order = 2)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Index("IX_CostS_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

    }
}