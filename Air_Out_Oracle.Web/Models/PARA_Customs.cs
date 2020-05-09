using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //海关参数查询--海关 关区代码
    public partial class PARA_Customs : Entity
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "进出口岸代码", Description = "进出口岸代码")]
        public string Customs_Code { get; set; }

        [StringLength(20)]
        [Display(Name = "进出口岸名称", Description = "进出口岸名称")]
        public string Customs_Name { get; set; }

        [StringLength(100)]
        [Display(Name = "拼音简称", Description = "拼音简称")]
        public string PinYinSimpleName { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
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
