using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(CompanyMetadata))]
    public partial class Company
    {
    }

    public partial class CompanyMetadata
    {
        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name="Id",Description="Id")]
        public int Id{ get; set; }

        [Display(Name="企业编号",Description="企业编号")]
        [MaxLength(50)]
        public string CIQID{ get; set; }

        [Display(Name="企业名称",Description="企业名称")]
        [MaxLength(100)]
        public string Name{ get; set; }

        [Display(Name="企业简称",Description="企业简称")]
        [MaxLength(50)]
        public string SimpleName{ get; set; }

        [Display(Name="企业英文名称",Description="企业英文名称")]
        [MaxLength(100)]
        public string Eng_Name{ get; set; }

        [Display(Name="地址",Description="地址")]
        [MaxLength(300)]
        public string Address{ get; set; }

        [Display(Name="市",Description="市")]
        [MaxLength(20)]
        public string City{ get; set; }

        [Display(Name="省份",Description="省份")]
        [MaxLength(20)]
        public string Province{ get; set; }

        [Display(Name="注册日期",Description="注册日期")]
        public DateTime RegisterDate{ get; set; }

        [Display(Name="Logo",Description="Logo")]
        [MaxLength(500)]
        public string Logo{ get; set; }

        [Display(Name="合同生效日",Description="合同生效日")]
        public DateTime ContractStart{ get; set; }

        [Display(Name="合同截止日",Description="合同截止日")]
        public DateTime ContractEnd{ get; set; }

        [Display(Name="授权金额",Description="授权金额")]
        public decimal AuthorizeAmount{ get; set; }

        [Display(Name="授权币制",Description="授权币制")]
        [MaxLength(50)]
        public string Currency{ get; set; }

        [Display(Name="对账日期",Description="对账日期")]
        [MaxLength(50)]
        public string CheckBillDate{ get; set; }

        [Display(Name="付款日期",Description="付款日期")]
        [MaxLength(50)]
        public string PayPalDate{ get; set; }

        [Display(Name="发票类型",Description="发票类型")]
        [MaxLength(50)]
        public string InvoiceType{ get; set; }

        [Display(Name="备注",Description="备注")]
        [MaxLength(1000)]
        public string Remark{ get; set; }

        [Display(Name="使用状态",Description="使用状态")]
        public EnumType.UseStatusEnum Status{ get; set; }

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

	public class CompanyChangeViewModel
    {
        public IEnumerable<Company> inserted { get; set; }

        public IEnumerable<Company> deleted { get; set; }

        public IEnumerable<Company> updated { get; set; }
    }
}
