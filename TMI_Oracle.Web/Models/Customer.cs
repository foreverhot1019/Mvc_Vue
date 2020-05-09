using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    public partial class Customer : Entity
    {
        public Customer()
        {
            Status = EnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "编号")]
        [Required,MaxLength(50)]
        [Index("Ix_Customer_No", IsUnique = true, Order = 1)]
        [Vue_PagePropty(SearchShow=true,Editable=false)]
        public string CustomerNo { get; set; }

        [Display(Name = "中文名")]
        [MaxLength(50)]
        [Required]
        [Index("Ix_Customer_NameChs", IsUnique = false, Order = 1)]
        [Vue_PagePropty(SearchShow=true,SearchOrder=1)]
        public string NameChs { get; set; }

        [Display(Name = "中文拼音")]
        [MaxLength(100)]
        [Required]
        [Index("Ix_Customer_NameChs", IsUnique = false, Order = 2)]
        public string NamePinYin { get; set; }

        [Display(Name = "英文名")]
        [MaxLength(100)]
        [Required]
        [Index("Ix_Customer_NameChs", IsUnique = false, Order = 3)]
        public string NameEng { get; set; }

        [Display(Name = "性别")]
        public bool? Sex { get; set; }

        [Display(Name = "出生年月")]
        public DateTime Birthday { get; set; }

        [Display(Name = "出生地")]
        [MaxLength(50)]
        public string BornCity { get; set; }

        [Display(Name = "签发地")]
        [MaxLength(50)]
        public string CheckCity { get; set; }

        /// <summary>
        /// 证件类型
        /// 身份证S = 1,护照H = 2,港澳通行证G = 3,台湾通行证T = 4,台胞证B = 5
        /// </summary>
        [Display(Name = "证件类型", Description = "多个/分割")]
        [MaxLength(50)]
        public string IdCardType { get; set; }

        [Display(Name = "航空公司会员卡")]
        [MaxLength(100)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 2)]
        public string AirLineMember { get; set; }

        [Display(Name = "联系方式")]
        public EnumType.ContactType ContactType { get; set; }

        [Display(Name = "联系方式")]
        [MaxLength(100)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 3)]
        public string Contact { get; set; }

        [Display(Name = "活跃状态")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 4)]
        public EnumType.ActiveStatus ActiveStatus { get; set; }

        [Display(Name = "销售")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 5)]
        public string Saller { get; set; }

        [Display(Name = "客服")]
        [MaxLength(50)]
        public string OP { get; set; }

        [Display(Name = "等级")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 6)]
        public EnumType.CustomerLevel CustomerLevel { get; set; }

        [Display(Name = "客户来源")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 7)]
        public EnumType.CustomerSource CustomerSource { get; set; }

        [Display(Name = "客户类型")]
        public EnumType.CustomerType CustomerType { get; set; }

        [Display(Name = "公司名称")]
        [MaxLength(50)]
        public string ComponyName { get; set; }

        [Display(Name = "公司")]
        public int? ComponyId { get; set; }

        [Display(Name = "公司")]
        [ForeignKey("ComponyId")]
        public virtual Company OCompany { get; set; }

        [Display(Name = "身份证")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 8)]
        public string IdCard { get; set; }

        [Display(Name = "身份证有效期起")]
        public DateTime? IdCardLimit_S { get; set; }

        [Display(Name = "身份证有效期讫")]
        public DateTime? IdCardLimit_E { get; set; }

        [Display(Name = "身份证正面照")]
        [MaxLength(100)]
        public string IdCardPhoto_A { get; set; }

        [Display(Name = "身份证反面照")]
        [MaxLength(100)]
        public string IdCardPhoto_B { get; set; }

        [Display(Name = "护照")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 9)]
        public string Passpord { get; set; }

        [Display(Name = "护照有效期起")]
        public DateTime? PasspordLimit_S { get; set; }

        [Display(Name = "护照有效期讫")]
        public DateTime? PasspordLimit_E { get; set; }

        [Display(Name = "护照正面照")]
        [MaxLength(100)]
        public string PasspordPhoto_A { get; set; }

        [Display(Name = "护照反面照")]
        [MaxLength(100)]
        public string PasspordPhoto_B { get; set; }

        [Display(Name = "港澳通行证")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 10)]
        public string HK_MacauPass { get; set; }

        [Display(Name = "港澳通行证有效期起")]
        public DateTime? HK_MacauPassLimit_S { get; set; }

        [Display(Name = "港澳通行证有效期讫")]
        public DateTime? HK_MacauPassLimit_E { get; set; }

        [Display(Name = "港澳通行证正面照")]
        [MaxLength(100)]
        public string HK_MacauPassPhoto_A { get; set; }

        [Display(Name = "港澳通行证反面照")]
        [MaxLength(100)]
        public string HK_MacauPassPhoto_B { get; set; }

        [Display(Name = "台湾通行证")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 11)]
        public string TaiwanPass { get; set; }

        [Display(Name = "台湾通行证有效期起")]
        public DateTime? TaiwanPassLimit_S { get; set; }

        [Display(Name = "台湾通行证有效期讫")]
        public DateTime? TaiwanPassLimit_E { get; set; }

        [Display(Name = "台湾通行证正面照")]
        [MaxLength(100)]
        public string TaiwanPassPhoto_A { get; set; }

        [Display(Name = "台湾通行证反面照")]
        [MaxLength(100)]
        public string TaiwanPassPhoto_B { get; set; }

        [Display(Name = "台胞证")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 12)]
        public string TWIdCard { get; set; }

        [Display(Name = "台胞证有效期起")]
        public DateTime? TWIdCardLimit_S { get; set; }

        [Display(Name = "台胞证有效期讫")]
        public DateTime? TWIdCardLimit_E { get; set; }

        [Display(Name = "台胞证正面照")]
        [MaxLength(100)]
        public string TWIdCardPhoto_A { get; set; }

        [Display(Name = "台胞证反面照")]
        [MaxLength(100)]
        public string TWIdCardPhoto_B { get; set; }

        [Display(Name = "回乡证")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 13)]
        public string HometownPass { get; set; }

        [Display(Name = "回乡证有效期起")]
        public DateTime? HometownPassLimit_S { get; set; }

        [Display(Name = "回乡证有效期讫")]
        public DateTime? HometownPassLimit_E { get; set; }

        [Display(Name = "回乡证正面照")]
        [MaxLength(100)]
        public string HometownPassPhoto_A { get; set; }

        [Display(Name = "回乡证反面照")]
        [MaxLength(100)]
        public string HometownPassPhoto_B { get; set; }

        [Display(Name = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "使用状态")]
        [Index("IX_Customer_Status", IsUnique = false, Order = 1)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 14)]
        public EnumType.UseStatusEnum Status { get; set; }

        [Index("IX_Customer_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        #region ScaffoldColumn

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }
}