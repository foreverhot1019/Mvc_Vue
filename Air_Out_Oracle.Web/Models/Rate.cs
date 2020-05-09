using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public partial class Rate : Entity
    {
        public Rate()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "本币", Order = 0, Description = "本币")]
        [MaxLength(20)]
        public string LocalCurrency { get; set; }

        [Display(Name = "币别", Order = 0, Description = "币别")]
        [Required,MaxLength(20)]
        [Index("IX_RateKey", IsUnique = true, Order = 1)]
        public string LocalCurrCode { get; set; }

        [Display(Name = "外币", Order = 0, Description = "外币")]
        [MaxLength(20)]
        public string ForeignCurrency { get; set; }

        [Display(Name = "兑换币别", Order = 0, Description = "兑换币别")]
        [Required, MaxLength(20)]
        [Index("IX_RateKey", IsUnique = true, Order = 2)]
        public string ForeignCurrCode { get; set; }

        [Display(Name = "年度", Order = 0, Description = "年度")]
        [Index("IX_RateKey", IsUnique = true, Order = 3)]
        public int Year { get; set; }

        [Display(Name = "月份", Order = 0, Description = "月份")]
        [Index("IX_RateKey", IsUnique = true, Order = 4)]
        public int Month { get; set; }

        [Display(Name = "应收款汇率", Order = 0, Description = "应收款汇率")]
        public decimal RecRate { get; set; }

        [Display(Name = "应付款汇率", Order = 0, Description = "应付款汇率")]
        public decimal PayRate { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Index("IX_Rate_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
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
    }
}