using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(CostMoneyMetadata))]
    public partial class CostMoney
    {
    }

    public partial class CostMoneyMetadata
    {
        [Display(Name = "订单", Description = "订单")]
        public Order OOrder { get; set; }

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id", Description = "Id")]
        public int Id { get; set; }

        [Display(Name = "订单", Description = "订单")]
        public int OrderId { get; set; }

        [Display(Name = "供应商名称", Description = "供应商名称")]
        [MaxLength(50)]
        public string SupplierName { get; set; }

        [Display(Name = "服务项目", Description = "服务项目")]
        [MaxLength(50)]
        public string ServiceProject { get; set; }

        [Display(Name = "单价", Description = "单价")]
        public decimal Price { get; set; }

        [Display(Name = "数量", Description = "数量")]
        public decimal Num { get; set; }

        [Display(Name = "金额", Description = "金额")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "请款", Description = "请款")]
        public decimal RequestAmount { get; set; }

        [Display(Name = "余额", Description = "余额")]
        public decimal ExcessAmount { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

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

    public class CostMoneyChangeViewModel
    {
        public IEnumerable<CostMoney> inserted { get; set; }

        public IEnumerable<CostMoney> deleted { get; set; }

        public IEnumerable<CostMoney> updated { get; set; }
    }
}