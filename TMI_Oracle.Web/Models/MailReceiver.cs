using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace TMI.Web.Models
{
    public partial class MailReceiver : Entity
    {
        [Key]
        [Display(Name = "行号", Description = "行号")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [Display(Name = "错误类型", Description = "错误类型")]
        [StringLength(50)]
        [Index("IX_MailReceiver", IsUnique = true, Order = 1)]
        public string ErrType { get; set; }

        [Required]
        [Display(Name = "错误方法", Description = "错误方法")]
        [StringLength(50)]
        [Index("IX_MailReceiver", IsUnique = true, Order = 2)]
        public string ErrMethod { get; set; }

        [Required]
        [Display(Name = "接收人邮件", Description = "接收人邮件")]
        [StringLength(50),EmailAddress(ErrorMessage="邮件格式不正确")]
        [Index("IX_MailReceiver", IsUnique = true, Order = 3)]
        public string RecMailAddress { get; set; }

        [Display(Name = "抄送人邮件", Description = "抄送人邮件(多个以,分割)")]
        [StringLength(500)]
        public string CCMailAddress { get; set; }

        [Display(Name = "建立人ID", Description = "建立人ID")]
        [StringLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "建立者", Description = "建立者")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "建立日期", Description = "建立日期")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改者", Description = "修改者")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期", Description = "修改日期")]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人ID", Description = "修改人ID")]
        [StringLength(50)]
        public string EDITID { get; set; }

    }
}