using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace TMI.Web.Models
{
    public partial class BD_DEFDOC_LIST : Entity
    {
        [Key]
        [Display(Name = "主键", Description = "主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "枚举ID", Description = "枚举ID")]
        [Required, Index("IX_BD_DEFDOC_LIST", IsUnique = true, Order = 0)]
        [Column(Order = 0)]
        public int DOCID { get; set; }

        [Display(Name = "枚举Code", Description = "枚举Code")]
        [Required, Index("IX_BD_DEFDOC_LIST", IsUnique = true, Order = 1)]
        [Index("IX_BD_DEFDOC_LIST_A", IsUnique = false, Order = 0)]
        [Column(Order = 1), StringLength(50)]
        [Vue_PagePropty(FormShow=false)]
        public string DOCCODE { get; set; }

        [Display(Name = "代码", Description = "明细代码")]
        [Required, Index("IX_BD_DEFDOC_LIST", IsUnique = true, Order = 2)]
        [Index("IX_BD_DEFDOC_LIST_A", IsUnique = false, Order = 1)]
        [Column(Order = 2)]
        [StringLength(50)]
        public string LISTCODE { get; set; }

        [Display(Name = "名称", Description = "名称")]
        [Required]
        [StringLength(50)]
        public string LISTNAME { get; set; }

        [Display(Name = "代码全称", Description = "代码全称")]
        [StringLength(50)]
        public string ListFullName { get; set; }

        [Display(Name = "英文全称", Description = "英文全称")]
        [StringLength(50)]
        public string ENAME { get; set; }

        [Display(Name = "说明", Description = "说明")]
        [StringLength(50)]
        public string REMARK { get; set; }

        [Display(Name = "状态", Description = "状态（1/0）")]
        [Required]
        [StringLength(10)]
        [Vue_PagePropty(IsForeignKey = true, ForeignKeyGetListUrl = "/Home/GetPagerEnum?EnumName=UseStatusEnum")]
        public string STATUS { get; set; }

        [Display(Name = "顺序", Description = "顺序")]
        public int SortNum { get; set; }

        [Display(Name = "关联类别", Description = "关联类别")]
        [StringLength(50)]
        public string R_CODE { get; set; }

        [Display(Name = "建立者", Description = "建立者")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "建立日期", Description = "建立日期")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改者", Description = "修改者")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期", Description = "修改日期")]
        public DateTime? EDITTS { get; set; }

        [ForeignKey("DOCID")]
        [Display(Name = "枚举", Description = "枚举")]
        public virtual BD_DEFDOC BD_DEFDOC { get; set; }

        [Index("IX_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }
    }
}