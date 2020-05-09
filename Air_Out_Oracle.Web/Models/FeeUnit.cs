using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //计费单位
    public partial class FeeUnit : Entity
    {
        public FeeUnit()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "计费单位名称", Description = "")]
        [Required]
        [MaxLength(50)]
        [Index("IX_FeeUnit_Key", IsUnique = true, Order = 1)]
        public string FeeUnitName { get; set; }

        [Display(Name = "备注", Description = "")]
        [MaxLength(50)]
        public string Remark { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Index("IX_FeeUnit_Point", IsUnique = false, Order = 2)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Index("IX_FeeUnit_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

    }
}