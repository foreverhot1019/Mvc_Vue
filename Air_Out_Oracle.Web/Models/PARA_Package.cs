using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //包装代码
    public partial class PARA_Package:Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "代码")]
        [Required, StringLength(50)]
        [Index("IX_P_Package_Key", IsUnique = true, Order = 1)]
        public string PackageCode { get; set; }

        [Display(Name = "名称")]
        [StringLength(50)]
        public string PackageName { get; set; }

        [Display(Name = "是否木质")]
        public bool IsWood { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "是否使用")]
        public bool Status { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [ScaffoldColumn(true)]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [ScaffoldColumn(true)]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

    }
}