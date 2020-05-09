using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //行业类型
    public partial class TradeType : Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "代码")]
        [MaxLength(50)]
        [Index("IX_TradeTypeKey", IsUnique = true)]
        public string Code { get; set; }

        [Display(Name = "名称")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "描述")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        [Index("IX_TradeTypeStatus", IsUnique = false)]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }
    
    }
}