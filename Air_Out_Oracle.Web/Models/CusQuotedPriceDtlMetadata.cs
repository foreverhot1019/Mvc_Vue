













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(CusQuotedPriceDtlMetadata))]
    public partial class CusQuotedPriceDtl
    {
    }

    public partial class CusQuotedPriceDtlMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="客户报价号",Description="客户报价号")]

        [MaxLength(50)]

        public string CusQPSerialNo{ get; set; }

        [Display(Name="客户报价Key",Description="客户报价Key")]

        [Range(0, 2147483647)]

        public int CusQPId{ get; set; }

        [Display(Name="报价号",Description="报价号")]

        [MaxLength(50)]

        public string QPSerialNo{ get; set; }

        [Display(Name="报价Key",Description="报价Key")]

        [Range(0, 2147483647)]

        public int QPId{ get; set; }

        [Display(Name="费用代码",Description="费用代码")]

        [MaxLength(50)]

        public string FeeCode{ get; set; }

        [Display(Name="费用名称",Description="费用名称")]

        [MaxLength(50)]

        public string FeeName{ get; set; }

        [Display(Name="起始地",Description="起始地")]

        [MaxLength(50)]

        public string StartPlace{ get; set; }

        [Display(Name="中转地",Description="中转地")]

        [MaxLength(50)]

        public string TransitPlace{ get; set; }

        [Display(Name="目的地",Description="目的地")]

        [MaxLength(50)]

        public string EndPlace{ get; set; }

        [Display(Name="航空公司",Description="航空公司")]

        [MaxLength(50)]

        public string AirLineCo{ get; set; }

        [Display(Name="航班号",Description="航班号")]

        [MaxLength(50)]

        public string AirLineNo{ get; set; }

        [Display(Name="订舱方",Description="订舱方")]

        [MaxLength(50)]

        public string WHBuyer{ get; set; }

        [Display(Name="代操作",Description="代操作")]

        public bool ProxyOperator{ get; set; }

        [Display(Name="成交条款",Description="成交条款")]

        [MaxLength(50)]

        public string DealWithArticle{ get; set; }

        [Display(Name="BSA",Description="BSA")]

        public bool BSA{ get; set; }

        [Display(Name="报关方式",Description="报关方式")]

        [MaxLength(50)]

        public string CustomsType{ get; set; }

        [Display(Name="商检标志",Description="商检标志")]

        public bool InspectMark{ get; set; }

        [Display(Name="取单标志",Description="取单标志")]

        public bool GetOrdMark{ get; set; }

        [Display(Name="靠级",Description="靠级")]

        [MaxLength(50)]

        public string MoorLevel{ get; set; }

        [Display(Name="计费单位",Description="计费单位")]

        [MaxLength(50)]

        public string BillingUnit{ get; set; }

        [Display(Name="单价",Description="单价")]

        public decimal Price{ get; set; }

        [Display(Name="币别",Description="币别")]

        [MaxLength(50)]

        public string CurrencyCode{ get; set; }

        [Display(Name="计费条件值1",Description="计费条件值1")]

        public decimal FeeConditionVal1{ get; set; }

        [Display(Name="计费条件运算符1",Description="计费条件运算符1")]

        [MaxLength(50)]

        public string CalcSign1{ get; set; }

        [Display(Name="计费条件",Description="计费条件")]

        [MaxLength(50)]

        public string FeeCondition{ get; set; }

        [Display(Name="计费条件运算符2",Description="计费条件运算符2")]

        [MaxLength(50)]

        public string CalcSign2{ get; set; }

        [Display(Name="计费条件值2",Description="计费条件值2")]

        public decimal FeeConditionVal2{ get; set; }

        [Display(Name="计算公式",Description="计算公式")]

        [MaxLength(50)]

        public string CalcFormula{ get; set; }

        [Display(Name="费用MIN",Description="费用MIN")]

        public decimal FeeMin{ get; set; }

        [Display(Name="费用MAX",Description="费用MAX")]

        public decimal FeeMax{ get; set; }

        [Display(Name="开始日期",Description="开始日期")]

        public DateTime StartDate{ get; set; }

        [Display(Name="结束日期",Description="结束日期")]

        public DateTime EndDate{ get; set; }

        [Display(Name="操作点",Description="操作点")]

        public int OperatingPoint{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(50)]

        public string ADDID{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(20)]

        public string ADDWHO{ get; set; }

        [Display(Name="新增时间",Description="新增时间")]

        public DateTime ADDTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(20)]

        public string EDITWHO{ get; set; }

        [Display(Name="修改时间",Description="修改时间")]

        public DateTime EDITTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(50)]

        public string EDITID{ get; set; }

	}

	public class CusQuotedPriceDtlChangeViewModel
    {
        public IEnumerable<CusQuotedPriceDtl> inserted { get; set; }

        public IEnumerable<CusQuotedPriceDtl> deleted { get; set; }

        public IEnumerable<CusQuotedPriceDtl> updated { get; set; }
    }
}
