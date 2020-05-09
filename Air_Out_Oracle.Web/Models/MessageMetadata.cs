using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(MessageMetadata))]
    public partial class Message
    {
    }

    public partial class MessageMetadata
    {
        [Display(Name = "通知")]
        public Notification Notification { get; set; }

        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter : 主题")]
        [Display(Name = "主题")]
        [MaxLength(100)]
        public string Subject { get; set; }

        [Display(Name = "业务单号1")]
        [MaxLength(100)]
        public string Key1 { get; set; }

        [Display(Name = "业务单号2")]
        [MaxLength(100)]
        public string Key2 { get; set; }

        [Required(ErrorMessage = "Please enter : 内容")]
        [Display(Name = "内容")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Please enter : 类型")]
        [Display(Name = "类型")]
        [MaxLength(20)]
        public string Type { get; set; }

        [Display(Name = "创建日期")]
        public DateTime NewDate { get; set; }

        [Display(Name = "是否已发送")]
        public bool IsSended { get; set; }

        [Display(Name = "发送日期")]
        public DateTime SendDate { get; set; }

        [Display(Name = "通知")]
        public int NotificationId { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "CreatedBy")]
        public string CreatedBy { get; set; }

        [Display(Name = "ModifiedBy")]
        public string ModifiedBy { get; set; }

    }




	public class MessageChangeViewModel
    {
        public IEnumerable<Message> inserted { get; set; }
        public IEnumerable<Message> deleted { get; set; }
        public IEnumerable<Message> updated { get; set; }
    }

}
