using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //实际成本
    public partial class ActualMoney : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "订单")]
        public int? OrderId { get; set; }

        [Display(Name = "订单")]
        [ForeignKey("OrderId")]
        public virtual Order OOrder { get; set; }

        [Display(Name = "供应商名称")]
        [MaxLength(50)]
        public string SupplierName { get; set; }

        [Display(Name = "服务项目")]
        [MaxLength(50)]
        [Vue_PagePropty(IsForeignKey = true, ForeignKeyGetListUrl = "/BD_DEFDOC_LISTS/GetPager_DEFDOC_DICT_FromCache?DOCCODE=ServiceProject")]
        public string ServiceProject { get; set; }

        [Display(Name = "单价")]
        public decimal Price { get; set; }

        [Display(Name = "数量")]
        public decimal Num { get; set; }

        [Display(Name = "金额")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "请款")]
        public decimal? RequestAmount { get; set; }

        [Display(Name = "余额")]
        public decimal? ExcessAmount { get; set; }

        [Display(Name = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        #region ScaffoldColumn

        [Index("IX_CostMoney_OP", IsUnique = false, Order = 1)]
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