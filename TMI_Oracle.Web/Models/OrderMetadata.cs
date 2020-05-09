using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(OrderMetadata))]
    public partial class Order
    {
    }

    public partial class OrderMetadata
    {
        [Display(Name = "订单客户", Description = "订单客户")]
        public OrderCustomer ArrOrderCustomer { get; set; }

        [Display(Name = "咨询单", Description = "咨询单")]
        public AdvisoryOrder OAdvisoryOrder { get; set; }

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id", Description = "Id")]
        public int Id { get; set; }

        [Display(Name = "订单编号", Description = "订单编号")]
        [MaxLength(100)]
        public string OrderNo { get; set; }

        [Display(Name = "咨询单", Description = "咨询单")]
        public int AdvisoryOrderId { get; set; }

        [Display(Name = "咨询单编号", Description = "咨询单编号")]
        [MaxLength(50)]
        public string AdvsyOrdNo { get; set; }

        [Display(Name = "旅游订单类型", Description = "旅游订单类型")]
        public EnumType.TravleOrdTypeEnum TravleOrdType { get; set; }

        [Display(Name = "旅游类型", Description = "旅游类型")]
        public EnumType.TravleTypeEnum TravleType { get; set; }

        [Display(Name = "出团日期", Description = "出团日期")]
        public DateTime STravleDate { get; set; }

        [Display(Name = "回程日期", Description = "回程日期")]
        public DateTime ETravleDate { get; set; }

        [Display(Name = "线路编号", Description = "线路编号")]
        [MaxLength(50)]
        public string RouteNo { get; set; }

        [Display(Name = "线路名称", Description = "线路名称")]
        [MaxLength(50)]
        public string RouteName { get; set; }

        [Display(Name = "销售", Description = "销售")]
        [MaxLength(50)]
        public string Saller { get; set; }

        [Display(Name = "客服", Description = "客服")]
        [MaxLength(50)]
        public string OP { get; set; }

        [Display(Name = "预计支付日期", Description = "预计支付日期")]
        public DateTime ForeCastPayDate { get; set; }

        [Display(Name = "供应商编号", Description = "供应商编号")]
        [MaxLength(50)]
        public string SupplyierNo { get; set; }

        [Display(Name = "供应商名称", Description = "供应商名称")]
        [MaxLength(50)]
        public string SupplierName { get; set; }

        [Display(Name = "联系方式", Description = "联系方式")]
        [MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "支付方式方式", Description = "支付方式方式")]
        public EnumType.PayType PayType { get; set; }

        [Display(Name = "线路图", Description = "线路图")]
        [MaxLength(50)]
        public string RoutePhoto { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "状态", Description = "状态")]
        public EnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "审核状态", Description = "审核状态")]
        public EnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "成年人", Description = "成年人")]
        public int AdultNum { get; set; }

        [Display(Name = "实际成年人", Description = "实际成年人")]
        public int AdultActualNum { get; set; }

        [Display(Name = "成年人售价", Description = "成年人售价")]
        public decimal AdultPrice { get; set; }

        [Display(Name = "大童", Description = "大童")]
        public int BoyNum { get; set; }

        [Display(Name = "实际大童", Description = "实际大童")]
        public int BoyActualNum { get; set; }

        [Display(Name = "大童售价", Description = "大童售价")]
        public decimal BoyPrice { get; set; }

        [Display(Name = "儿童", Description = "儿童")]
        public int ChildNum { get; set; }

        [Display(Name = "实际儿童", Description = "实际儿童")]
        public int ChildActualNum { get; set; }

        [Display(Name = "儿童售价", Description = "儿童售价")]
        public decimal ChildPrice { get; set; }

        [Display(Name = "售价补差", Description = "售价补差")]
        public decimal PriceRepair { get; set; }

        [Display(Name = "售价补差备注", Description = "售价补差备注")]
        [MaxLength(500)]
        public string PriceRepairRemark { get; set; }

        [Display(Name = "合计", Description = "合计")]
        [MaxLength(500)]
        public string TotalRemark { get; set; }

        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        public string EDITID { get; set; }

    }

    /// <summary>
    /// 旅游订单
    /// </summary>
    public class OrderView
    {
        public Order Order { get; set; }

        [Display(Name = "订单客户", Description = "订单客户 OrderCustomer")]
        public IEnumerable<OrderCustomerView> ArrOrderCustomer { get; set; }

        [Display(Name = "预算成本", Description = "预算成本 CostMoney")]
        public IEnumerable<CostMoney> ArrCostMoney { get; set; }

        [Display(Name = "预算成本", Description = "实际成本 ActualMoney")]
        public IEnumerable<ActualMoney> ArrActualMoney { get; set; }

        [Display(Name = "预算成本", Description = "财务请款 FinanceMoney")]
        public IEnumerable<FinanceMoney> ArrFinanceMoney { get; set; }
    }

    //需要删除的数据
    public class OCAFdeltRows{
        public IEnumerable<int> DelOrdCustomer { get; set; }
        public IEnumerable<int> DelCostMoney { get; set; }
        public IEnumerable<int> DelActualMoney { get; set; }
        public IEnumerable<int> DelFinanceMoney { get; set; }
    }

    public class OrderChangeViewModel
    {
        public IEnumerable<OrderView> inserted { get; set; }

        public IEnumerable<OrderView> deleted { get; set; }

        public IEnumerable<OrderView> updated { get; set; }

        [Display(Name = "需要删除的数据", Description = "")]
        public OCAFdeltRows OOCAFdeltRows { get; set; }
    }
}