using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //供应商询价记录表
    public class SupplyierAskPrice : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "供应商名称")]
        [MaxLength(50)]
        public string SupplierName { get; set; }

        [Display(Name = "线路名称")]
        [MaxLength(50)]
        public string RouteName { get; set; }

        [Display(Name = "服务项目")]
        [MaxLength(50)]
        [Vue_PagePropty(IsForeignKey=true, ForeignKeyGetListUrl = "/BD_DEFDOC_LISTS/GetPager_DEFDOC_DICT_FromCache?DOCCODE=ServiceProject")]
        public string ServiceProject { get; set; }

        [Display(Name = "单价")]
        [Range(0.1,double.MaxValue)]
        public decimal Price { get; set; }

        [Display(Name = "标记")]
        public bool Target { get; set; }

        [Display(Name = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "咨询单")]
        public int? AdvisoryOrderId { get; set; }

        [Display(Name = "咨询单")]
        [ForeignKey("AdvisoryOrderId")]
        public virtual AdvisoryOrder OAdvisoryOrder { get; set; }

        #region ScaffoldColumn

        [Index("IX_SupyAskPrc_OP", IsUnique = false, Order = 1)]
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