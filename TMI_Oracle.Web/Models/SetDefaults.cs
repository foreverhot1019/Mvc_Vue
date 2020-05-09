using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    public partial class SetDefaults
    {
        public SetDefaults()
        {
        }

        [Required]
        [Display(Name = "表名", Description = "默认值表名（英文）", Order = 0)]
        public string TableName { get; set; }

        [Required]
        [Display(Name = "表名", Description = "默认值表名", Order = 1)]
        public string TableNameChs { get; set; }

        [Required]
        [Display(Name = "字段", Description = "默认值字段（英文）", Order = 2)]
        public string ColumnName { get; set; }

        [Required]
        [Display(Name = "字段", Description = "默认值字段", Order = 3)]
        public string ColumnNameChs { get; set; }

        [Required]
        [Display(Name = "默认值", Description = "默认值", Order = 4)]
        public string DefaultValue { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "数据类型", Description = "数据类型（int,decimal,bool..等）", Order = 5)]
        public string DataType { get; set; }
    }
}