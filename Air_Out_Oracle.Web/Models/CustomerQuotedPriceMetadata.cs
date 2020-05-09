using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(CustomerQuotedPriceMetadata))]
    public partial class CustomerQuotedPrice
    {
    }

    public partial class CustomerQuotedPriceMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "记录号")]

		public string SerialNo{ get; set; }

        [Display(Name = "业务类型")]

		public string BusinessType{ get; set; }

        [Display(Name = "客户代码")]

		public string CustomerCode{ get; set; }

        [Display(Name = "本仓标志")]

		public string LocalWHMark{ get; set; }

        [Display(Name = "起始地")]

		public string StartPlace{ get; set; }

        [Display(Name = "中转地")]

		public string TransitPlace{ get; set; }

        [Display(Name = "目的地")]

		public string EndPlace{ get; set; }

        [Display(Name = "代理")]

		public string ProxyOperator{ get; set; }

        [Display(Name = "自定义名称")]

		public string CusDefinition{ get; set; }

        [Display(Name = "收货人")]

		public string Receiver{ get; set; }

        [Display(Name = "发货人")]

		public string Shipper{ get; set; }

        [Display(Name = "联系人")]

		public string Contact{ get; set; }

        [Display(Name = "报价政策")]

		public string QuotedPricePolicy{ get; set; }

        [Display(Name = "销售员")]

		public string Seller{ get; set; }

        [Display(Name = "开始日期")]

		public DateTime StartDate{ get; set; }

        [Display(Name = "停用日期")]

		public DateTime EndDate{ get; set; }

        [Display(Name = "描述")]

		public string Description{ get; set; }

        [Display(Name = "审批状态")]

        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Display(Name = "使用状态")]

        public AirOutEnumType.UseStatusEnum Status { get; set; }

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

	public class CustomerQuotedPriceChangeViewModel
    {
        public IEnumerable<CustomerQuotedPrice> inserted { get; set; }

        public IEnumerable<CustomerQuotedPrice> deleted { get; set; }

        public IEnumerable<CustomerQuotedPrice> updated { get; set; }
    }
}
