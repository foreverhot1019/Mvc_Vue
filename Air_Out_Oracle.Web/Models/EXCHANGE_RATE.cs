using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

//固定汇率
namespace AirOut.Web.Models
{
    public partial class EXCHANGE_RATE : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "币别", Description = "本币")]
        [Required,StringLength(20)]
        public string CURR_Code { get; set; }

        [Display(Name = "兑换币别", Description = "外币")]
        [Required, StringLength(20)]
        public string ExCURR_Code { get; set; }

        [Required]
        [Display(Name = "年度", Description = "年度")]
        [Range(1900,9999)]
        public int Year { get; set; }

        [Required]
        [Display(Name = "月份", Description = "月份")]
        [Range(1, 12)]
        public int Month{ get; set; }

        [Required]
        [Display(Name = "应收款汇率", Description = "应收款汇率")]
        public decimal RecRate { get; set; }

        [Required]
        [Display(Name = "应付款汇率", Description = "应付款汇率")]
        public decimal PayRate { get; set; }

        [Display(Name = "使用状态")]
        public bool Status { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [StringLength(100)]
        public string Remark { get; set; }

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