using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(NotificationMetadata))]
    public partial class Notification
    {
    }

    public partial class NotificationMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "描述")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Display(Name = "类型")]
        [MaxLength(30)]
        public string Type { get; set; }

        [Display(Name = "发送方")]
        [MaxLength(50)]
        public string Sender { get; set; }

        [Display(Name = "接收方")]
        [MaxLength(100)]
        public string Receiver { get; set; }

        [Display(Name = "计划")]
        [MaxLength(50)]
        public string Schedule { get; set; }

        [Required(ErrorMessage = "Please enter : 是否禁用")]
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
        public Message Messages { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "CreatedBy")]
        public string CreatedBy { get; set; }

        [Display(Name = "ModifiedBy")]
        public string ModifiedBy { get; set; }

    }

	public class NotificationChangeViewModel
    {
        public IEnumerable<Notification> inserted { get; set; }

        public IEnumerable<Notification> deleted { get; set; }

        public IEnumerable<Notification> updated { get; set; }
    }

}
