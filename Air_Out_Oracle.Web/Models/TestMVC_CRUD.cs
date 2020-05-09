using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public partial class TestMVC_CRUD : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "业务编号")]
        [MaxLength(50)]
        public string Dzbh { get; set; }

        [Display(Name = "测试1")]
        [MaxLength(50)]
        public string TestColumn1 { get; set; }

        [Display(Name = "测试2")]
        [MaxLength(50)]
        public string TestColumn2 { get; set; }

        [Display(Name = "测试3")]
        [MaxLength(50)]
        public string TestColumn3 { get; set; }

        [Display(Name = "测试4")]
        [MaxLength(50)]
        public string TestColumn4 { get; set; }

        [Display(Name = "测试5")]
        [MaxLength(50)]
        public string TestColumn5 { get; set; }

        #region ScaffoldColumn

        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

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

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }
}