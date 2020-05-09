using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //成交条款
    public partial class DealArticle : Entity
    {
        public DealArticle()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "成交条款代码", Description = "")]
        [Required]
        [MaxLength(50)]
        [Index("IX_DealArticle_Key", IsUnique = true, Order = 1)]
        public string DealArticleCode { get; set; }

        [Display(Name = "成交条款名称", Description = "")]
        [Required]
        [MaxLength(50)]
        public string DealArticleName { get; set; }

        [Display(Name = "运输条款", Description = "")]
        [MaxLength(50)]
        public string TransArticle { get; set; }

        [Display(Name = "成交条款描述")]
        [StringLength(500)]
        public string Description { get; set; }

        #region 新增字段 20181121

        [Display(Name = "付款方式", Description = "付款方式")]
        [MaxLength(50)]
        public string Pay_ModeCode { get; set; }

        [Display(Name = "运费P/C", Description = "运费P/C")]
        [MaxLength(1)]
        public string Carriage { get; set; }

        [Display(Name = "运费英文", Description = "运费英文")]
        [MaxLength(50)]
        public string CarriageEns { get; set; }

        [Display(Name = "杂费P/C", Description = "杂费P/C")]
        [MaxLength(1)]
        public string Incidental_Expenses { get; set; }

        [Display(Name = "杂费英文", Description = "杂费英文")]
        [MaxLength(50)]
        public string Incidental_ExpensesEns { get; set; }

        #endregion 

        [Index("IX_DealArticle_Point", IsUnique = false, Order = 2)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Index("IX_DealArticle_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

    }

}