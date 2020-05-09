namespace TMI.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Repository.Pattern.Ef6;

    //海关参数查询--币制
    public partial class PARA_CURR : Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, Index("IX_PARA_CURR", IsUnique = true, Order = 0)]
        [StringLength(3)]
        [Display(Name = "币制代码", Description = "币制代码")]
        public string CURR_CODE { get; set; }

        [StringLength(10)]
        [Display(Name = "币制名称", Description = "币制名称")]
        public string CURR_Name { get; set; }

        [StringLength(20)]
        [Display(Name = "币制英文简称", Description = "币制英文简称")]
        public string CURR_NameEng { get; set; }

        [StringLength(10)]
        [Display(Name = "货币代码", Description = "货币代码")]
        public string Money_CODE { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        public EnumType.UseStatusEnum Status { get; set; }

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
