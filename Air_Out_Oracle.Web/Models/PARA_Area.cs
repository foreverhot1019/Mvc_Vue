using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //海关参数查询--国内地区
    public partial class PARA_Area : Entity
    {
        public PARA_Area()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "国家代码", Description = "国家代码")]
        [Required, Index("IX_PARA_Area", IsUnique = true, Order = 1)]
        [StringLength(10)]
        public string Country_CO { get; set; }

        [Display(Name = "地区代码", Description = "地区代码")]
        [Required, Index("IX_PARA_Area", IsUnique = true, Order = 2)]
        [StringLength(10)]
        public string AreaCode { get; set; }

        [Display(Name = "地区名称", Description = "地区名称")]
        [StringLength(100)]
        public string AreaName { get; set; }

        [Display(Name = "使用状态")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

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

    }
}
