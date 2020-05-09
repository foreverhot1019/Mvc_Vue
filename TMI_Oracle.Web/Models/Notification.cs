using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    public partial class Notification:Entity
    {
        public Notification()
        {
            Messages = new HashSet<Message>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_Notification", IsUnique = true)]
        [Display(Name = "名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "描述")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Display(Name = "类型")]
        [MaxLength(30)]
        public string Type { get; set; }

        [Required]
        [Display(Name = "发送方")]
        [MaxLength(50)]
        public string Sender { get; set; }

        [Required]
        [Display(Name = "接收方")]
        [MaxLength(100)]
        public string Receiver { get; set; }

        [Display(Name = "计划")]
        [MaxLength(50)]
        public string Schedule { get; set; }

        [Display(Name = "是否禁用")]
        public bool Disabled { get; set; }

        [Display(Name = "登录用户")]
        [MaxLength(30)]
        public string AuthUser { get; set; }

        [Display(Name = "登录密码")]
        [MaxLength(30)]
        public string AuthPassword { get; set; }

        [Display(Name = "主机")]
        [MaxLength(30)]
        public string Host { get; set; }

        [Display(Name = "消息")]
        public virtual ICollection<Message> Messages { get; set; }

        #region IAuditable Members

        [Display(Name = "创建时间")]
        [ScaffoldColumn(false)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? ModifiedDate { get; set; }

        [MaxLength(20)]
        [Display(Name = "创建人")]
        [ScaffoldColumn(false)]
        public string CreatedBy { get; set; }

        [MaxLength(20)]
        [Display(Name = "最后修改人")]
        [ScaffoldColumn(false)]
        public string ModifiedBy { get; set; }

        #endregion

    }
}