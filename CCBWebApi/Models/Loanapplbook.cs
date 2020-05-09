using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class Loanapplbook
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "统一社会信用代码", Description = "")]
        [Required, MaxLength(50)]
        [Index("IX_Loanapp_USCC", IsUnique = false, Order = 1)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string Unn_Soc_Cr_Cd { get; set; }

        [Display(Name = "签约日期", Description = "")]
        [Required]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public DateTime AcctBeginDate { get; set; }

        [Display(Name = "借款客户名称", Description = "")]
        [Required, MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string CustName { get; set; }

        [Display(Name = "签约额度", Description = "")]
        [Required]
        public decimal AR_Lmt { get; set; }

        [Display(Name = "额度到期日期", Description = "")]
        [Required]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public DateTime Lmt_ExDat { get; set; }

        [Display(Name = "贷款余额", Description = "")]
        [Required]
        public decimal LoanBal { get; set; }

        [Display(Name = "回款账号", Description = "")]
        [Required,MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string Rfnd_AccNo { get; set; }

        [Display(Name = "上传日期", Description = "")]
        public DateTime UploadDate { get; set; }

        #region ScaffoldColumn

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string AddUser { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime AddDate { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string LastEditUser { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? LastEditDate { get; set; }

        #endregion
    }
}