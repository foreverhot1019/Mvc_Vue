using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace TMI.Web.Models
{
    public partial class BD_DEFDOC : Entity
    {
        public BD_DEFDOC()
        {
            BD_DEFDOC_LIST = new HashSet<BD_DEFDOC_LIST>();
        }

        [Key]
        [Display(Name = "行号", Description = "行号")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "枚举代码", Description = "枚举代码")]
        [Required, Index("IX_BD_DEFDOC", IsUnique = true)]
        [MaxLength(20)]
        public string DOCCODE { get; set; }

        [Display(Name = "枚举名称", Description = "枚举名称")]
        [Required]
        [MaxLength(20)]
        public string DOCNAME { get; set; }

        [Display(Name = "说明", Description = "说明")]
        [MaxLength(50)]
        public string REMARK { get; set; }

        [Display(Name = "状态", Description = "状态（1/0）")]
        [Required]
        [MaxLength(10)]
        [Vue_PagePropty(IsForeignKey = true, ForeignKeyGetListUrl = "/Home/GetPagerEnum?EnumName=UseStatusEnum")]
        public string STATUS { get; set; }

        [Display(Name = "建立者", Description = "建立者")]
        [MaxLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "建立日期", Description = "建立日期")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改者", Description = "修改者")]
        [MaxLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期", Description = "修改日期")]
        public DateTime? EDITTS { get; set; }

        [Index("IX_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "枚举信息", Description = "枚举信息")]
        public virtual ICollection<BD_DEFDOC_LIST> BD_DEFDOC_LIST { get; set; }
    }
}