using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace TMI.Web.Models
{
    //国家代码
    public partial class PARA_Country : Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Index("IX_P_CountryNOKey", IsUnique = true, Order = 1)]
        [StringLength(3), Display(Name = "数字代码", Description = "国别（地区）代码")]
        public string COUNTRY_NO { get; set; }

        [Index("IX_P_CountryCOKey", IsUnique = true, Order = 1)]
        [StringLength(3), Display(Name = "代码", Description = "国别（地区）代码")]
        public string COUNTRY_CO { get; set; }

        [StringLength(50,MinimumLength=0), Display(Name = "国别英文简称", Description = "国别英文简称")]
        public string COUNTRY_EN { get; set; }

        [StringLength(50), Display(Name = "国别（地区）名称", Description = "国别（地区）名称")]
        public string COUNTRY_NA { get; set; }

        [StringLength(1)]
        public string EXAM_MARK { get; set; }

        [StringLength(1)]
        public string HIGH_LOW { get; set; }

        [Display(Name = "使用状态")]
        public bool Status { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }
    }
}
