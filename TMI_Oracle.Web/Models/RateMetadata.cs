using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(RateMetadata))]
    public partial class Rate
    {
    }

    public partial class RateMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "本币")]

		public string LocalCurrency{ get; set; }

        [Display(Name = "币别")]

		public string LocalCurrCode{ get; set; }

        [Display(Name = "外币")]
		public string ForeignCurrency{ get; set; }

        [Display(Name = "兑换币别")]
		public string ForeignCurrCode{ get; set; }

        [Display(Name = "年度")]
		public int Year{ get; set; }

        [Display(Name = "月份")]
		public int Month{ get; set; }

        [Display(Name = "应收款汇率")]
		public decimal RecRate{ get; set; }

        [Display(Name = "应付款汇率")]

		public decimal PayRate{ get; set; }

        [Display(Name = "描述")]

		public string Description{ get; set; }

        [Display(Name = "使用状态")]

		public int Status{ get; set; }

        [Display(Name = "操作点")]

		public int OperatingPoint{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

	}

	public class RateChangeViewModel
    {
        public IEnumerable<Rate> inserted { get; set; }

        public IEnumerable<Rate> deleted { get; set; }

        public IEnumerable<Rate> updated { get; set; }
    }
}
