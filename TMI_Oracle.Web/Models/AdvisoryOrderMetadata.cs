using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(AdvisoryOrderMetadata))]
    public partial class AdvisoryOrder
    {
    }

    public partial class AdvisoryOrderMetadata
    {
        [Display(Name = "公司", Description = "公司")]
        public Company OCompany { get; set; }

        [Display(Name = "客户", Description = "客户")]
        public Customer OCustomer { get; set; }

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id", Description = "Id")]
        public int Id { get; set; }

        [Display(Name = "咨询单号", Description = "咨询单号")]
        [MaxLength(100)]
        public string OrderNo { get; set; }

        [Display(Name = "旅游类型", Description = "旅游类型")]
        public EnumType.TravleTypeEnum TravleType { get; set; }

        [Display(Name = "线路编号", Description = "线路编号")]
        [MaxLength(50)]
        public string RouteNo { get; set; }

        //[Display(Name = "出团日期", Description = "出团日期")]
        //public DateTime STravleDate { get; set; }

        //[Display(Name = "回程日期", Description = "回程日期")]
        //public DateTime? ETravleDate { get; set; }

        [Display(Name = "出游人数", Description = "出游人数")]
        public int TravlePersonNum { get; set; }

        [Display(Name = "单价", Description = "单价")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "总价", Description = "总价")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "线路图", Description = "线路图")]
        [MaxLength(50)]
        public string RoutePhoto { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "使用状态", Description = "使用状态")]
        public EnumType.OrderStatusEnum Status { get; set; }

        [Display(Name = "客户", Description = "客户")]
        public int CustomerId { get; set; }

        [Display(Name = "客户姓名", Description = "客户姓名")]
        [MaxLength(1000)]
        public string CustomerName { get; set; }

        [Display(Name = "联系方式", Description = "联系方式")]
        public EnumType.ContactType ContactType { get; set; }

        [Display(Name = "联系方式", Description = "联系方式")]
        [MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "销售", Description = "销售")]
        [MaxLength(50)]
        public string Saller { get; set; }

        [Display(Name = "客服", Description = "OP")]
        [MaxLength(50)]
        public string OP { get; set; }

        [Display(Name = "客户类型", Description = "客户类型")]
        public EnumType.CustomerType CustomerType { get; set; }

        [Display(Name = "客户来源", Description = "客户来源")]
        public EnumType.CustomerSource CustomerSource { get; set; }

        [Display(Name = "活跃状态", Description = "活跃状态")]
        public EnumType.ActiveStatus ActiveStatus { get; set; }

        [Display(Name = "商机等级", Description = "商机等级")]
        public EnumType.CustomerLevel CustomerLevel { get; set; }

        [Display(Name = "公司名称", Description = "公司名称")]
        [MaxLength(50)]
        public string ComponyName { get; set; }

        [Display(Name = "公司", Description = "公司")]
        public int ComponyId { get; set; }

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

    public class AdvisoryOrderChangeViewModel
    {
        public IEnumerable<AdvisoryOrder> inserted { get; set; }

        public IEnumerable<AdvisoryOrder> deleted { get; set; }

        public IEnumerable<AdvisoryOrder> updated { get; set; }
    }
}
