using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //旅游订单
    public partial class Order : Entity
    {
        public Order()
        {
            Contact = "-";
            AdvsyOrdNo = "-";
            ArrOrderCustomer = new HashSet<OrderCustomer>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "订单编号")]
        [MaxLength(100)]
        [Index("IX_Order_OrdNo", IsUnique = true, Order = 1)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string OrderNo { get; set; }

        [Display(Name = "咨询单")]
        public int? AdvisoryOrderId { get; set; }

        [Display(Name = "咨询单")]
        [ForeignKey("AdvisoryOrderId")]
        public virtual AdvisoryOrder OAdvisoryOrder { get; set; }

        [Display(Name = "咨询单编号")]
        [Required, MaxLength(50)]
        [Index("IX_Order_AdvsyOrdNo", IsUnique = false, Order = 1)]
        public string AdvsyOrdNo { get; set; }

        [Display(Name = "订单客户")]
        public ICollection<OrderCustomer> ArrOrderCustomer { get; set; }

        [Display(Name = "旅游订单类型")]
        public EnumType.TravleOrdTypeEnum TravleOrdType { get; set; }

        [Display(Name = "旅游类型")]
        public EnumType.TravleTypeEnum TravleType { get; set; }

        [Display(Name = "出游人数")]
        public int TravlePersonNum { get; set; }

        [Display(Name = "单价")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "总价")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "出团日期")]
        public DateTime STravleDate { get; set; }

        [Display(Name = "回程日期")]
        public DateTime? ETravleDate { get; set; }

        [Display(Name = "线路编号")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 2)]
        public string RouteNo { get; set; }

        [Display(Name = "线路名称")]
        [Required, MaxLength(50)]
        public string RouteName { get; set; }

        [Display(Name = "销售")]
        [MaxLength(50)]
        public string Saller { get; set; }

        [Display(Name = "客服")]
        [MaxLength(50)]
        public string OP { get; set; }

        [Display(Name = "预计支付日期")]
        public DateTime ForeCastPayDate { get; set; }

        [Display(Name = "供应商编号")]
        [MaxLength(50)]
        public string SupplyierNo { get; set; }

        [Display(Name = "供应商名称")]
        [MaxLength(50)]
        public string SupplierName { get; set; }

        [Display(Name = "联系方式")]
        [Required, MaxLength(100)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 6)]
        public string Contact { get; set; }

        [Display(Name = "支付方式方式")]
        public EnumType.PayType PayType { get; set; }

        [Display(Name = "线路图")]
        [MaxLength(50)]
        public string RoutePhoto { get; set; }

        [Display(Name = "客户诉求")]
        [MaxLength(2000)]
        public string CustomerRequire { get; set; }

        [Display(Name = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "状态")]
        public EnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "审核状态")]
        public EnumType.AuditStatusEnum AuditStatus { get; set; }

        #region 订单详情

        [Display(Name = "成年人")]
        public int AdultNum { get; set; }

        [Display(Name = "实际成年人")]
        public int AdultActualNum { get; set; }

        [Display(Name = "成年人售价")]
        public decimal? AdultPrice { get; set; }

        [Display(Name = "大童")]
        public int BoyNum { get; set; }

        [Display(Name = "实际大童")]
        public int BoyActualNum { get; set; }

        [Display(Name = "大童售价")]
        public decimal? BoyPrice { get; set; }

        [Display(Name = "儿童")]
        public int ChildNum { get; set; }

        [Display(Name = "实际儿童")]
        public int ChildActualNum { get; set; }

        [Display(Name = "儿童售价")]
        public decimal? ChildPrice { get; set; }
        
        [Display(Name = "售价补差")]
        public decimal? PriceRepair{ get; set; }

        [Display(Name = "售价补差备注")]
        [MaxLength(500)]
        public string PriceRepairRemark { get; set; }

        [Display(Name = "合计")]
        [MaxLength(500)]
        public string TotalRemark { get; set; }
        
        #endregion

        #region ScaffoldColumn

        [Index("IX_Order_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
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

    //订单客户 OrderCustomer
    //预算成本 CostMoney
    //实际成本 ActualMoney
    //财务请款 FinanceMoney

}