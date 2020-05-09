using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace TMI.Web.Models
{
    //操作点仓库
    public partial class OperatePointList : Entity
    {
        [Display(Name = "主键")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "操作点ID")]
        [Required]
        public int? OperatePointID { get; set; }

        [Display(Name = "操作点代码")]
        [Required, StringLength(50)]
        public string OperatePointCode { get; set; }

        [Display(Name = "操作点")]
        [ForeignKey("OperatePointID")]
        public OperatePoint OperatePoint { get; set; }

        [Display(Name = "仓库代码")]
        [Required, StringLength(50)]
        [Index("IX_OperatePointList", IsUnique = true, Order = 1)]
        public string CompanyCode { get; set; }

        [Display(Name = "仓库名称")]
        [Required, StringLength(100)]
        public string CompanyName { get; set; }

        [Display(Name = "是否启用")]
        public bool IsEnabled { get; set; }

        [Display(Name = "添加时间", Description = "添加时间")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "添加人ID", Description = "添加人ID")]
        [StringLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "添加人", Description = "添加人")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改人ID", Description = "修改人ID")]
        [StringLength(50)]
        public string EDITID { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? EDITTS { get; set; }

    }
}