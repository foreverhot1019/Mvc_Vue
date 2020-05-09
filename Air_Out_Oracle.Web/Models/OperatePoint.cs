using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //操作点
    public partial class OperatePoint : Entity
    {
        public OperatePoint()
        {
            OperatePointLists = new HashSet<OperatePointList>();
        }

        [Display(Name="主键")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "代码")]
        [Required, StringLength(50)]
        [Index("IX_OperatePoint",IsUnique=true,Order=1)]
        public string OperatePointCode { get; set; }

        [Display(Name = "名称")]
        [Required, StringLength(100)]
        public string OperatePointName { get; set; }

        [Display(Name = "描述")]
        [StringLength(100)]
        public string Description { get; set; }

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

        [Display(Name = "操作仓库", Description = "操作仓库")]
        public ICollection<OperatePointList> OperatePointLists { get; set; }
    }
}