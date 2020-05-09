using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;
using Newtonsoft.Json;

namespace AirOut.Web.Models
{
    //每日汇率
    public partial class DailyRate : Entity
    {
        //{"LocalCurrency": "人民币", "ForeignCurrency": "丹麦克朗", "PriceType": "钞/汇卖价", "BankName": "中信银行", "Price": "104.8200"}
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "本币", Order = 0, Description = "本币")]
        [MaxLength(50)]
        public string LocalCurrency { get; set; }

        [Display(Name = "本币代码", Order = 0, Description = "本币代码")]
        [MaxLength(50)]
        public string LocalCurrCode { get; set; }

        [Display(Name = "外币", Order = 0, Description = "外币")]
        [MaxLength(50)]
        public string ForeignCurrency { get; set; }

        [Display(Name = "外币代码", Order = 0, Description = "外币代码")]
        [MaxLength(50)]
        public string ForeignCurrCode { get; set; }

        //[中间价,钞买价,汇买价,钞/汇卖价]
        [Display(Name = "钞/汇卖价", Order = 0, Description = "钞/汇卖价")]
        [MaxLength(50)]
        public string PriceType { get; set; }

        [Display(Name = "汇率银行", Order = 0, Description = "汇率银行")]
        [MaxLength(50)]
        public string BankName { get; set; }

        [Display(Name = "汇率", Order = 0, Description = "汇率")]
        public decimal Price { get; set; }

        [Display(Name = "爬取时间", Order = 0, Description = "爬取时间")]
        public DateTime? ScrapyDate { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        public bool Status { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

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

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }
    }

    public class CurrencyCode
    {
        /// <summary>
        /// 币种名称
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// 币种代码
        /// </summary>
        public string code { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class DailyRateJson
    {
        [Display(Name = "本币", Order = 0, Description = "本币")]
        public string LocalCurrency { get; set; }

        [Display(Name = "本币代码", Order = 0, Description = "本币代码")]
        public string LocalCurrCode { get; set; }

        [Display(Name = "外币", Order = 0, Description = "外币")]
        public string ForeignCurrency { get; set; }

        [Display(Name = "外币代码", Order = 0, Description = "外币代码")]
        public string ForeignCurrCode { get; set; }

        //[中间价,钞买价,汇买价,钞/汇卖价]
        [Display(Name = "钞/汇卖价", Order = 0, Description = "钞/汇卖价")]
        public string PriceType { get; set; }

        [Display(Name = "汇率银行", Order = 0, Description = "汇率银行")]
        public string BankName { get; set; }

        [JsonIgnore]
        [Display(Name = "汇率", Order = 0, Description = "汇率")]
        public decimal? Price1
        {
            get
            {
                decimal _Price1;
                if (decimal.TryParse(Price, out _Price1))
                    return _Price1;
                else
                    return null;
            }
        }

        [JsonProperty(PropertyName = "Price")]
        public string Price { get; set; }

        [JsonIgnore]
        [Display(Name = "爬取时间", Order = 0, Description = "爬取时间")]
        public DateTime? ScrapyDate { get; set; }
    }
}