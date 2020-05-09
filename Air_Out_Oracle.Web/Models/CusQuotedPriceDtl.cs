using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //客户报价费用明细
    public partial class CusQuotedPriceDtl : Entity
    {
        public CusQuotedPriceDtl()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Display(Name = "客户报价明细号", Description = "")]
        //[Required, MaxLength(50)]
        //public string CusQPDtlSerialNo { get; set; }

        [Display(Name = "客户报价号", Description = "来源于客户报价主表，CQ201805040001:CQ+年月日+4位流水码")]
        [Required, MaxLength(50)]
        [Index("IX_CQPriceNo", IsUnique = false, Order = 1)]
        public string CusQPSerialNo { get; set; }

        [Display(Name = "客户报价Key", Description = "来源于客户报价主表，CQ201805040001:CQ+年月日+4位流水码")]
        [Range(0, int.MaxValue)]//设置 最小值为0
        [Index("IX_CQPriceId", IsUnique = false, Order = 1)]
        public int CusQPId{ get; set; }

        [Display(Name = "报价号", Description = "来源于报价主表，Q201805040001:Q+年月日+4位流水码")]
        [Required, MaxLength(50)]
        [Index("IX_QPriceNo", IsUnique = false, Order = 1)]
        public string QPSerialNo { get; set; }

        [Display(Name = "报价Key", Description = "来源于报价主表，Q201805040001:Q+年月日+4位流水码")]
        [Range(0,int.MaxValue)]//设置 最小值为0
        [Index("IX_QPriceId", IsUnique = false, Order = 1)]
        public int QPId { get; set; }

        #region 报价单数据（报价主表）

        [Display(Name = "费用代码", Description = "")]
        [Required, MaxLength(50)]
        public string FeeCode { get; set; }

        [Display(Name = "费用名称", Description = "")]
        [Required, MaxLength(50)]
        public string FeeName { get; set; }

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
        public bool ProxyOperator { get; set; }

        [Display(Name = "成交条款", Description = "")]
        [MaxLength(50)]
        public string DealWithArticle { get; set; }

        [Display(Name = "BSA", Description = "")]
        public bool BSA { get; set; }

        [Display(Name = "报关方式", Description = "")]
        [MaxLength(50)]
        public string CustomsType { get; set; }

        [Display(Name = "商检标志", Description = "")]
        public bool InspectMark { get; set; }

        [Display(Name = "取单标志", Description = "")]
        public bool GetOrdMark { get; set; }

        [Display(Name = "靠级", Description = "")]
        [MaxLength(50)]
        public string MoorLevel { get; set; }

        [Display(Name = "计费单位", Description = "")]
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

        [Display(Name = "计费条件", Description = "")]
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

        [Display(Name = "开始日期", Description = "")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "结束日期", Description = "")]
        public DateTime? EndDate { get; set; }

        #endregion

        #region ScaffoldColumn
        
        [Index("IX_CQPriceDtl_OP", IsUnique = false, Order = 1)]
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