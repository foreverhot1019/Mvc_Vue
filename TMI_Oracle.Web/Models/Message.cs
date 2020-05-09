using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    public partial class Message : Entity
    {
        [Key]
        [Display(Name = "主键", Description = "")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "主题", Description = "")]
        public string Subject { get; set; }

        [Display(Name = "关键字1", Description = "")]
        public string Key1 { get; set; }

        [Display(Name = "关键字2", Description = "")]
        public string Key2 { get; set; }

        [Required]
        [Display(Name = "内容", Description = "")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "类型",Description="")]
        public string Type { get; set; }

        [Display(Name = "新增时间", Description = "")]
        public DateTime NewDate { get; set; }

        [Display(Name = "已发送", Description = "")]
        public bool IsSended { get; set; }

        [Display(Name = "发送时间", Description = "")]
        public DateTime? SendDate { get; set; }

        [Display(Name = "通知", Description = "")]
        public int NotificationId { get; set; }

        [Display(Name = "通知", Description = "")]
        [ForeignKey("NotificationId")]
        public virtual Notification Notification { get; set; }

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