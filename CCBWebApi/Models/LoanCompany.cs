using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class LoanCompany
    {
        public LoanCompany()
        {
            Status = EnumType.StatusEnum.Enable;
            Id = default(Guid);//默认 00000000000000000000
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }//供应商名称

        [Display(Name = "唯一码", Description = "")]
        [Required, MaxLength(50)]
        [Index("IX_LoanCompany_KeyCode", IsUnique = true, Order = 1)]
        public string KeyCode { get; set; }

        [Display(Name = "交易流水号", Description = "日期YYYYMMDD+交易码+6位序号")]
        [MaxLength(50)]
        public string Trans_Id { get; set; }

        [Display(Name = "交易码", Description = "FLD1001")]
        [MaxLength(50)]
        public string Trans_Code { get; set; }

        [Display(Name = "合作平台编号")]
        [Required, MaxLength(50)]
        public string CoPlf_ID { get; set; }//合作平台编号

        [Display(Name = "统一社会信用代码")]
        [Required, MaxLength(50)]
        public string Unn_Soc_Cr_Cd { get; set; }//统一社会信用代码

        [Display(Name = "供应商名称")]
        [Required, MaxLength(100)]
        public string Splr_Nm { get; set; }//供应商名称

        [Display(Name = "付款方名称")]
        [Required, MaxLength(50)]
        public string Pyr_Nm { get; set; }//付款方名称

        [Display(Name = "总额度值")]
        public decimal TLmt_Val { get; set; }//总额度值

        [Display(Name = "申请贷款金额")]
        public decimal LoanApl_Amt { get; set; }//申请贷款金额

        [Display(Name = "交易到期日期")]
        public DateTime Txn_ExDat { get; set; }//交易到期日期

        [Display(Name = "回款账号")]
        [Required, MaxLength(50)]
        public string Rfnd_AccNo { get; set; }//回款账号

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
}