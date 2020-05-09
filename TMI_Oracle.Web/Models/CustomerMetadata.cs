using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using TMI.Web.Models;

namespace TMI.Web.Models
{
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
    }

    public partial class CustomerMetadata
    {
        [Display(Name = "公司", Description = "公司")]
        public Company OCompany { get; set; }

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id", Description = "Id")]
        public int Id { get; set; }

        [Display(Name = "中文名", Description = "中文名")]
        [MaxLength(50)]
        public string NameChs { get; set; }

        [Display(Name = "中文名", Description = "中文名")]
        [MaxLength(100)]
        public string NamePinYin { get; set; }

        [Display(Name = "英文名", Description = "英文名")]
        [MaxLength(100)]
        public string NameEng { get; set; }

        [Display(Name = "性别", Description = "性别")]
        public bool Sex { get; set; }

        [Display(Name = "出生年月", Description = "出生年月")]
        public DateTime Birthday { get; set; }

        [Display(Name = "航空公司会员卡", Description = "航空公司会员卡")]
        [MaxLength(100)]
        public string AirLineMember { get; set; }

        [Display(Name = "联系方式", Description = "联系方式")]
        public EnumType.ContactType ContactType { get; set; }

        [Display(Name = "联系方式", Description = "联系方式")]
        [MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "活跃状态", Description = "活跃状态")]
        public EnumType.ActiveStatus ActiveStatus { get; set; }

        [Display(Name = "销售", Description = "销售")]
        [MaxLength(50)]
        public string Saller { get; set; }

        [Display(Name = "OP", Description = "OP")]
        [MaxLength(50)]
        public string OP { get; set; }

        [Display(Name = "等级", Description = "等级")]
        public EnumType.CustomerLevel CustomerLevel { get; set; }

        [Display(Name = "客户来源", Description = "客户来源")]
        public EnumType.CustomerSource CustomerSource { get; set; }

        [Display(Name = "客户类型", Description = "客户类型")]
        public EnumType.CustomerType CustomerType { get; set; }

        [Display(Name = "公司名称", Description = "公司名称")]
        [MaxLength(50)]
        public string ComponyName { get; set; }

        [Display(Name = "公司", Description = "公司")]
        public int ComponyId { get; set; }

        [Display(Name = "身份证", Description = "身份证")]
        [MaxLength(50)]
        public string IdCard { get; set; }

        [Display(Name = "身份证有效期起", Description = "身份证有效期起")]
        public DateTime IdCardLimit_S { get; set; }

        [Display(Name = "身份证有效期讫", Description = "身份证有效期讫")]
        public DateTime IdCardLimit_E { get; set; }

        [Display(Name = "身份证正面照", Description = "身份证正面照")]
        [MaxLength(100)]
        public string IdCardPhoto_A { get; set; }

        [Display(Name = "身份证反面照", Description = "身份证反面照")]
        [MaxLength(100)]
        public string IdCardPhoto_B { get; set; }

        [Display(Name = "护照", Description = "护照")]
        [MaxLength(50)]
        public string Passpord { get; set; }

        [Display(Name = "护照有效期起", Description = "护照有效期起")]
        public DateTime PasspordLimit_S { get; set; }

        [Display(Name = "护照有效期讫", Description = "护照有效期讫")]
        public DateTime PasspordLimit_E { get; set; }

        [Display(Name = "护照正面照", Description = "护照正面照")]
        [MaxLength(100)]
        public string PasspordPhoto_A { get; set; }

        [Display(Name = "护照反面照", Description = "护照反面照")]
        [MaxLength(100)]
        public string PasspordPhoto_B { get; set; }

        [Display(Name = "港澳通行证", Description = "港澳通行证")]
        [MaxLength(50)]
        public string HK_MacauPass { get; set; }

        [Display(Name = "港澳通行证有效期起", Description = "港澳通行证有效期起")]
        public DateTime HK_MacauPassLimit_S { get; set; }

        [Display(Name = "港澳通行证有效期讫", Description = "港澳通行证有效期讫")]
        public DateTime HK_MacauPassLimit_E { get; set; }

        [Display(Name = "港澳通行证正面照", Description = "港澳通行证正面照")]
        [MaxLength(100)]
        public string HK_MacauPassPhoto_A { get; set; }

        [Display(Name = "港澳通行证反面照", Description = "港澳通行证反面照")]
        [MaxLength(100)]
        public string HK_MacauPassPhoto_B { get; set; }

        [Display(Name = "台湾通行证", Description = "台湾通行证")]
        [MaxLength(50)]
        public string TaiwanPass { get; set; }

        [Display(Name = "台湾通行证有效期起", Description = "台湾通行证有效期起")]
        public DateTime TaiwanPassLimit_S { get; set; }

        [Display(Name = "台湾通行证有效期讫", Description = "台湾通行证有效期讫")]
        public DateTime TaiwanPassLimit_E { get; set; }

        [Display(Name = "台湾通行证正面照", Description = "台湾通行证正面照")]
        [MaxLength(100)]
        public string TaiwanPassPhoto_A { get; set; }

        [Display(Name = "台湾通行证反面照", Description = "台湾通行证反面照")]
        [MaxLength(100)]
        public string TaiwanPassPhoto_B { get; set; }

        [Display(Name = "台胞证", Description = "台胞证")]
        [MaxLength(50)]
        public string TWIdCard { get; set; }

        [Display(Name = "台胞证有效期起", Description = "台胞证有效期起")]
        public DateTime TWIdCardLimit_S { get; set; }

        [Display(Name = "台胞证有效期讫", Description = "台胞证有效期讫")]
        public DateTime TWIdCardLimit_E { get; set; }

        [Display(Name = "台胞证正面照", Description = "台胞证正面照")]
        [MaxLength(100)]
        public string TWIdCardPhoto_A { get; set; }

        [Display(Name = "台胞证反面照", Description = "台胞证反面照")]
        [MaxLength(100)]
        public string TWIdCardPhoto_B { get; set; }

        [Display(Name = "回乡证", Description = "回乡证")]
        [MaxLength(50)]
        public string HometownPass { get; set; }

        [Display(Name = "回乡证有效期起", Description = "回乡证有效期起")]
        public DateTime HometownPassLimit_S { get; set; }

        [Display(Name = "回乡证有效期讫", Description = "回乡证有效期讫")]
        public DateTime HometownPassLimit_E { get; set; }

        [Display(Name = "回乡证正面照", Description = "回乡证正面照")]
        [MaxLength(100)]
        public string HometownPassPhoto_A { get; set; }

        [Display(Name = "回乡证反面照", Description = "回乡证反面照")]
        [MaxLength(100)]
        public string HometownPassPhoto_B { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "使用状态", Description = "使用状态")]
        public EnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        public string EDITID { get; set; }

    }

    public class CustomerChangeViewModel
    {
        public IEnumerable<Customer> inserted { get; set; }

        public IEnumerable<Customer> deleted { get; set; }

        public IEnumerable<Customer> updated { get; set; }
    }
}