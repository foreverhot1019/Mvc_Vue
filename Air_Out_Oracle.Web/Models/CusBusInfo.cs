using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //客商信息主表
    public partial class CusBusInfo : Entity
    {
        public CusBusInfo()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
            EnterpriseGroupCode = "-";
            TopEnterpriseCode = "-";
            EnterpriseGroup = "-";
            SallerName = "-";//销售
            //EnterpriseName = "-";
            //EnterpriseShortName = "-";
            //EnterpriseType = "-";
            //AddressCHN = "-";
            //AreaCode = "-";
            //ECCAreaCode = "-";
            //CountryCode = "-";
            //ResteredCapital = "-";
            //TaxPayerType = "-";
            //UnifiedSocialCreditCode = "-";
            //InvoiceCountryCode = "-";
            //InvoiceAddress = "-";
            //BankName = "-";
            //BankAccount = "-";
            //Currency = "-";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "业务伙伴代码")]
        [Required(ErrorMessage = "业务伙伴代码不能为空！")]
        [Index("IX_CusBusInfor_Key", IsUnique = true, Order = 1)]
        [MaxLength(20)]
        [System.Web.Mvc.Remote("GetIsUse", "CusBusInfos", AdditionalFields = "Id", HttpMethod = "post", ErrorMessage = "业务伙伴代码已存在")]
        public string EnterpriseId { get; set; }

        [Display(Name = "业务伙伴全称")]
        [Required, MaxLength(500)]
        public string EnterpriseName { get; set; }

        [Display(Name = "业务伙伴简称")]
        [Required, MaxLength(50)]
        public string EnterpriseShortName { get; set; }

        [Display(Name = "客户分类")]
        [Required, MaxLength(50)]
        public string EnterpriseType { get; set; }

        [Display(Name = "业务伙伴中文名称")]
        [MaxLength(200)]
        public string CHNName { get; set; }

        [Display(Name = "业务伙伴英文名称")]
        [MaxLength(500)]
        public string EngName { get; set; }

        [Display(Name = "集团名称")]
        [MaxLength(50)]
        public string EnterpriseGroup { get; set; }

        [Display(Name = "集团代码")]
        [MaxLength(50)]
        public string EnterpriseGroupCode { get; set; }

        [Display(Name = "总公司编码")]
        [MaxLength(50)]
        public string TopEnterpriseCode { get; set; }

        [Display(Name = "海关编码")]
        [MaxLength(50)]
        public string CIQID { get; set; }

        [Display(Name = "商检编码")]
        [MaxLength(50)]
        public string CHECKID { get; set; }

        //[Display(Name = "关区编号")]
        //[MaxLength(50)]
        //public string CustomsId { get; set; }

        [Display(Name = "关区代码")]
        [MaxLength(50)]
        public string CustomsCode { get; set; }

        [Display(Name = "中文地址")]
        [Required]
        [MaxLength(1000)]
        public string AddressCHN { get; set; }

        [Display(Name = "英文地址")]
        [MaxLength(1000)]
        public string AddressEng { get; set; }

        [Display(Name = "网址")]
        [MaxLength(200)]
        public string WebSite { get; set; }

        //[Display(Name = "行业类型编号")]
        //[MaxLength(100)]
        //public string TradeTypeId { get; set; }

        [Display(Name = "行业类型代码")]
        [MaxLength(50)]
        public string TradeTypeCode { get; set; }

        //[Display(Name = "区域编号")]
        //[MaxLength(100)]
        //public string AreaId { get; set; }

        [Display(Name = "区域代码")]
        [Required]
        [MaxLength(50)]
        public string AreaCode { get; set; }

        [Display(Name = "ECC区域代码")]
        [Required]
        [MaxLength(50)]
        public string ECCAreaCode { get; set; }

        //[Display(Name = "国家编号")]
        //[MaxLength(50)]
        //public string CountryId { get; set; }

        [Display(Name = "国家代码")]
        [Required]
        [MaxLength(50)]
        public string CountryCode { get; set; }

        //[Display(Name = "企业性质编号")]
        //[MaxLength(50)]
        //public string CopoKindId { get; set; }

        [Display(Name = "企业性质代码")]
        [MaxLength(50)]
        public string CopoKindCode { get; set; }

        [Display(Name = "企业法人")]
        [MaxLength(50)]
        public string CorpartiPerson { get; set; }

        [Display(Name = "注册资金")]
        [Required, MaxLength(50)]
        public string ResteredCapital { get; set; }

        [Display(Name = "是否内部公司(1-是,0-否)")]
        public bool IsInternalCompany { get; set; }

        [Display(Name = "描述")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态", Description = "状态( 0:草稿,1:启用,-1:停用)")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        #region 后加

        [Display(Name = "联系人")]
        [MaxLength(50)]
        public string LinkerMan { get; set; }

        [Display(Name = "联系电话", Description = "总机")]
        [MaxLength(100)]
        public string Telephone { get; set; }

        [Display(Name = "传真")]
        [MaxLength(100)]
        public string Fax { get; set; }

        [Display(Name = "纳税人性质")]
        [Required, MaxLength(50)]
        public string TaxPayerType { get; set; }

        [Display(Name = "统一社会信用代码")]
        [Required, MaxLength(50)]
        public string UnifiedSocialCreditCode { get; set; }

        [Display(Name = "开票国家代码", Description = "开票国家代码")]
        [Required]
        [MaxLength(50)]
        public string InvoiceCountryCode { get; set; }

        [Display(Name = "开票地址", Description = "开票地址")]
        [Required]
        [MaxLength(100)]
        public string InvoiceAddress { get; set; }

        [Display(Name = "银行名称", Description = "")]
        [Required]
        [MaxLength(100)]
        public string BankName { get; set; }

        [Display(Name = "银行账号", Description = "")]
        [Required]
        [MaxLength(100)]
        public string BankAccount { get; set; }

        [Display(Name = "开户币种", Description = "")]
        [Required]
        [MaxLength(50)]
        public string Currency { get; set; }

        #endregion

        #region 销售 2019-03-14新增 

        [Display(Name = "销售人", Description = "")]
        [Required]
        [MaxLength(50)]
        public string SallerName { get; set; }

        [Display(Name = "销售", Description = "")]
        public int? SallerId { get; set; }

        [Display(Name = "销售", Description = "")]
        [ForeignKey("SallerId")]
        public virtual Saller OSaller { get; set; }

        #endregion

        #region 交货地点 2019-03-29新增

        [Display(Name = "交货地点", Description = "交货地点")]
        [MaxLength(20)]
        public string Delivery_Point { get; set; }

        #endregion

        #region 不使用 基架 生成字段
        
        [Index("IX_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }
}