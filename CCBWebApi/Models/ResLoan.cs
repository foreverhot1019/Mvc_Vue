using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class ResLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "交易流水号", Description = "日期YYYYMMDD+交易码+6位序号")]
        [Required,MaxLength(50)]
        [Index("IX_ResLoan_Key", IsUnique = true, Order = 1)] 
        [Vue_PagePropty(SearchShow=true,SearchOrder=1)]
        public string Trans_Id { get; set; }

        [Display(Name = "交易码", Description = "FLD1002")]
        [Required, MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string Trans_Code { get; set; }

        //[Display(Name = "返回码", Description = "000000：代表成功，其他代表失败")]
        //[Required, MaxLength(50)]
        //public string return_code{ get; set; }

        //[Display(Name = "返回描述", Description = "")]
        //[MaxLength(500)]
        //public string return_msg { get; set; }

        [Display(Name = "统一社会信用代码", Description = "")]
        [Required, MaxLength(50)]
        [Index("IX_ResLoan_USCC", IsUnique = false, Order = 1)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string Unn_Soc_Cr_Cd { get; set; }

        [Display(Name = "签约客户名称", Description = "")]
        [Required, MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string Sgn_Cst_Nm { get; set; }

        [Display(Name = "合作平台编号", Description = "")]
        [MaxLength(50)]
        public string CoPlf_ID { get; set; }

        [Display(Name = "签约日期", Description = "")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public DateTime Sign_Dt { get; set; }

        [Display(Name = "合约额度", Description = "")]
        public decimal AR_Lmt { get; set; }

        [Display(Name = "额度到期日期", Description = "")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public DateTime? Lmt_ExDat { get; set; }

        [Display(Name = "回款账号", Description = "")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string Rfnd_AccNo { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [Display(Name = "使用状态", Description = "使用状态")]
        public EnumType.StatusEnum Status { get; set; }

        [Display(Name = "审批状态", Description = "审批状态")]
        public EnumType.AuditStatusEnum AuditStatus { get; set; }

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

    public class ResLoanViewModel
    {
        public IEnumerable<ResLoan> inserted { get; set; }

        public IEnumerable<ResLoan> deleted { get; set; }

        public IEnumerable<ResLoan> updated { get; set; }
    }

}