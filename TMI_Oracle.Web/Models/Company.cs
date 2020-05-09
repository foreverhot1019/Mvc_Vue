using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    //公司
    public partial class Company : Entity
    {
        public Company()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "企业编号")]
        [MaxLength(50)]
        [Required]
        [Index("IX_Company_Key", IsUnique = true, Order = 1)]
        [Vue_PagePropty(SearchShow = true)]
        public string CIQID { get; set; }

        [Display(Name = "企业名称")]
        [MaxLength(100)]
        [Required]
        [Vue_PagePropty(SearchShow = true)]
        public string Name { get; set; }

        [Display(Name = "企业简称")]
        [MaxLength(50)]
        public string SimpleName { get; set; }

        [Display(Name = "企业英文名称")]
        [MaxLength(100)]
        public string Eng_Name { get; set; }

        [Display(Name = "地址")]
        [MaxLength(300)]
        public string Address { get; set; }

        [Display(Name = "市")]
        [MaxLength(20)]
        public string City { get; set; }

        [Display(Name = "省份")]
        [MaxLength(20)]
        public string Province { get; set; }

        [Display(Name = "注册日期")]
        public DateTime? RegisterDate { get; set; }

        [Display(Name = "Logo")]
        [MaxLength(500)]
        public string Logo { get; set; }

        [Display(Name = "合同生效日")]
        public DateTime? ContractStart { get; set; }

        [Display(Name = "合同截止日")]
        public DateTime? ContractEnd { get; set; }

        [Display(Name = "授权金额")]
        public decimal? AuthorizeAmount { get; set; }

        [Display(Name = "授权币制")]
        [MaxLength(50)]
        public string Currency { get; set; }

        [Display(Name = "对账日期")]
        [MaxLength(50)]
        public string CheckBillDate { get; set; }

        [Display(Name = "付款日期")]
        [MaxLength(50)]
        public string PayPalDate { get; set; }

        [Display(Name = "发票类型")]
        [MaxLength(50)]
        public string InvoiceType { get; set; }

        [Display(Name = "备注")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "使用状态")]
        [Index("IX_Company_Status", IsUnique = false, Order = 1)]
        public EnumType.UseStatusEnum Status { get; set; }

        [Index("IX_Company_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [Vue_PagePropty(ListShow = false, FormShow = false,SearchShow=false)]
        public int OperatingPoint { get; set; }

        #region ScaffoldColumn

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        [Vue_PagePropty(FormShow=false, Editable = false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        [Vue_PagePropty(SearchShow=true, Editable = false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        [Vue_PagePropty(SearchShow = true, Editable = false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        [Vue_PagePropty(SearchShow = true, Editable = false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        [Vue_PagePropty(SearchShow = true, Editable = false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        [Vue_PagePropty(FormShow = false, Editable = false)]
        public string EDITID { get; set; }

        #endregion

    }
}