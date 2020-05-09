using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(CusBusInfoMetadata))]
    public partial class CusBusInfo
    {
    }

    public partial class CusBusInfoMetadata
    {
        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "业务伙伴代码")]
        public string EnterpriseId { get; set; }

        [Display(Name = "业务伙伴简称")]
        public string EnterpriseShortName { get; set; }

        [Display(Name = "集团代码")]
        public string EnterpriseGroupCode { get; set; }

        [Display(Name = "总公司编码")]
        public string TopEnterpriseCode { get; set; }

        [Display(Name = "海关编码")]
        public string CIQID { get; set; }

        [Display(Name = "商检编码")]
        public string CHECKID { get; set; }

        //[Display(Name = "关区编号")]
        //public string CustomsId { get; set; }

        [Display(Name = "关区代码")]
        public string CustomsCode { get; set; }

        [Display(Name = "业务伙伴中文名称")]
        public string CHNName { get; set; }

        [Display(Name = "业务伙伴英文名称")]
        public string EngName { get; set; }

        [Display(Name = "中文地址")]
        public string AddressCHN { get; set; }

        [Display(Name = "英文地址")]
        public string AddressEng { get; set; }

        [Display(Name = "网址")]
        public string WebSite { get; set; }

        //[Display(Name = "行业类型编号")]
        //public string TradeTypeId { get; set; }

        [Display(Name = "行业类型代码")]
        public string TradeTypeCode { get; set; }

        //[Display(Name = "区域编号")]
        //public string AreaId { get; set; }

        [Display(Name = "区域代码")]
        public string AreaCode { get; set; }

        //[Display(Name = "国家编号")]
        //public string CountryId { get; set; }

        [Display(Name = "国家代码")]
        public string CountryCode { get; set; }

        //[Display(Name = "企业性质编号")]
        //public string CopoKindId { get; set; }

        [Display(Name = "企业性质代码")]
        public string CopoKindCode { get; set; }

        [Display(Name = "企业法人")]
        public string CorpartiPerson { get; set; }

        [Display(Name = "注册资金")]
        public string ResteredCapital { get; set; }

        [Display(Name = "是否内部公司")]
        public bool IsInternalCompany { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "状态")]
        public string Status { get; set; }

        [Display(Name = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人")]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "修改人")]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间")]
        public DateTime EDITTS { get; set; }

        [Display(Name = "业务伙伴全称")]
        public string EnterpriseName { get; set; }

        [Display(Name = "开户币种")]
        public string Currency { get; set; }

        [Display(Name = "集团")]
        public string EnterpriseGroup { get; set; }

    }

    public class CusBusInfoChangeViewModel
    {
        public IEnumerable<CusBusInfo> inserted { get; set; }

        public IEnumerable<CusBusInfo> deleted { get; set; }

        public IEnumerable<CusBusInfo> updated { get; set; }
    }
}
