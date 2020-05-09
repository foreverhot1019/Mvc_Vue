using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(ContactsMetadata))]
    public partial class Contacts
    {
    }

    public partial class ContactsMetadata
    {
        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name="Id",Description="Id")]
        public int Id{ get; set; }

        [Display(Name="客商代码（业务伙伴代码）",Description="客商代码（业务伙伴代码）")]
        [MaxLength(50)]
        public string CusBusInfoId{ get; set; }

        [Display(Name="名称",Description="名称")]
        [MaxLength(200)]
        public string CompanyName{ get; set; }

        [Display(Name="代码",Description="代码")]
        [MaxLength(50)]
        public string CompanyCode{ get; set; }

        [Display(Name="地址",Description="地址")]
        [MaxLength(500)]
        public string CoAddress{ get; set; }

        [Display(Name="区域",Description="区域")]
        [MaxLength(50)]
        public string CoArea{ get; set; }

        [Display(Name="国家",Description="国家")]
        [MaxLength(50)]
        public string CoCountry{ get; set; }

        [Display(Name="联系电话",Description="联系电话")]
        [MaxLength(100)]
        public string Contact{ get; set; }

        [Display(Name="联系地址信息",Description="联系地址信息")]
        [MaxLength(2000)]
        public string ContactInfo{ get; set; }

        [Display(Name="使用状态",Description="使用状态")]
        public int Status{ get; set; }

        [Display(Name="备注",Description="备注")]
        [MaxLength(100)]
        public string Remark{ get; set; }

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

	public class ContactsChangeViewModel
    {
        public IEnumerable<Contacts> inserted { get; set; }

        public IEnumerable<Contacts> deleted { get; set; }

        public IEnumerable<Contacts> updated { get; set; }
    }
}