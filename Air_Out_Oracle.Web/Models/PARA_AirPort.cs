using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //海关参数查询--港口（Port）
    public partial class PARA_AirPort : Entity
    {
        public PARA_AirPort()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_PARA_AirPort", IsUnique = true, Order = 0)]
        [Required, StringLength(20)]
        [Display(Name = "口岸代码", Description = "口岸代码")]
        public string PortCode { get; set; }

        [StringLength(20)]
        [Display(Name = "中文名称", Description = "中文名称")]
        public string PortName { get; set; }

        [StringLength(50)]
        [Display(Name = "英文名称", Description = "英文名称")]
        public string PortNameEng { get; set; }

        [StringLength(50)]
        [Display(Name = "口岸性质", Description = "口岸性质")]
        public string PortType { get; set; }

        [StringLength(20)]
        [Display(Name = "国家代码", Description = "国家代码")]
        public string CountryCode { get; set; }

        [StringLength(50)]
        [Display(Name = "太平港口名称", Description = "太平港口名称")]
        public string PeacePortName { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? EDITTS { get; set; }
    }
}
