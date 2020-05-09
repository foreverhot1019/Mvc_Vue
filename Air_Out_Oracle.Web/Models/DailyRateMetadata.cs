using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(DailyRateMetadata))]
    public partial class DailyRate
    {
    }

    public partial class DailyRateMetadata
    {
        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "本币")]
        public string LocalCurrency { get; set; }

        [Display(Name = "本币代码")]
        public string LocalCurrCode { get; set; }

        [Display(Name = "外币")]
        public string ForeignCurrency { get; set; }

        [Display(Name = "外币代码")]
        public string ForeignCurrCode { get; set; }

        [Display(Name = "钞/汇卖价")]
        public string PriceType { get; set; }

        [Display(Name = "汇率银行")]
        public string BankName { get; set; }

        [Display(Name = "汇率")]
        public decimal Price { get; set; }

        [Display(Name = "爬取时间")]
        public DateTime ScrapyDate { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        public bool Status { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? EDITTS { get; set; } 

    }

    public class DailyRateChangeViewModel
    {
        public IEnumerable<DailyRate> inserted { get; set; }

        public IEnumerable<DailyRate> deleted { get; set; }

        public IEnumerable<DailyRate> updated { get; set; }
    }
}
