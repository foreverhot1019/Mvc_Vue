using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //咨询单
    public partial class AdvisoryOrder:Entity
    {
        public AdvisoryOrder()
        {
            ArrSupplyierAskPrice = new HashSet<SupplyierAskPrice>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "咨询单号")]
        [MaxLength(100)]
        [Index("IX_AdvsyOrd_OrdNo", IsUnique = true, Order = 1)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string OrderNo{ get; set; }

        [Display(Name = "旅游订单类型")]
        public EnumType.TravleOrdTypeEnum TravleOrdType { get; set; }

        [Display(Name = "旅游类型")]
        public EnumType.TravleTypeEnum TravleType { get; set; }

        [Display(Name = "出游人数")]
        public int? TravlePersonNum { get; set; }

        [Display(Name = "单价")]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "总价")]
        public decimal? TotalPrice { get; set; }

        [Display(Name = "线路编号")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 2)]
        public string RouteNo { get; set; }

        [Display(Name = "线路名称")]
        [Required,MaxLength(50)]
        public string RouteName { get; set; }
             
        [Display(Name = "线路图")]
        [MaxLength(50)]
        public string RoutePhoto { get; set; }

        [Display(Name = "客户诉求")]
        [MaxLength(2000)]
        public string CustomerRequire { get; set; }

        [Display(Name = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "使用状态")]
        [Index("IX_AdvsyOrd_OrdStatus", IsUnique = false, Order = 3)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public EnumType.OrderStatusEnum Status { get; set; }

        [Display(Name = "供应商询价")]
        public ICollection<SupplyierAskPrice> ArrSupplyierAskPrice { get; set; }

        #region 客户

        [Display(Name="客户",Description="")]
        public int? CustomerId { get; set; }

        [Display(Name = "客户", Description = "")]
        public virtual Customer OCustomer { get; set; }

        [Display(Name = "客户姓名")]
        [Required,MaxLength(1000)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 4)]
        public string CustomerName { get; set; }

        [Display(Name = "联系方式")]
        [Required]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 5)]
        public EnumType.ContactType ContactType { get; set; }

        [Display(Name = "联系方式")]
        [Required, MaxLength(100)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 6)]
        public string Contact { get; set; }

        [Display(Name = "销售")]
        [MaxLength(50)]
        public string Saller { get; set; }

        [Display(Name = "客服")]
        [MaxLength(50)]
        public string OP { get; set; }

        [Display(Name = "客户类型")]
        public EnumType.CustomerType CustomerType { get; set; }

        [Display(Name = "客户来源")]
        public EnumType.CustomerSource CustomerSource { get; set; }

        [Display(Name = "活跃状态")]
        public EnumType.ActiveStatus ActiveStatus { get; set; }

        [Display(Name = "商机等级")]
        public EnumType.CustomerLevel CustomerLevel { get; set; }

        [Display(Name = "公司名称")]
        [MaxLength(50)]
        public string ComponyName { get; set; }

        [Display(Name = "公司")]
        public int? ComponyId { get; set; }

        [Display(Name = "公司")]
        [ForeignKey("ComponyId")]
        public virtual Company OCompany { get; set; }
        
        #endregion

        #region ScaffoldColumn

        [Index("IX_AdvsyOrd_OP", IsUnique = false, Order = 1)]
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
}