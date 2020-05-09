using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public enum MessageType
    {
        Information,
        Message,
        Error,
        Alert,
        Warning
    }

    public enum NotificationType
    {
        Email,
        GSM,
        Phone
    }

    public enum NotificationTag
    {
        Sys,//系统
        WebService,//服务
        AsyncGetByRedis,//异步读取Redis数据
        AsyncWriteLogByRedis,//异步写日志
        SendECCService,//异步发送ECC
        CRMService,//CRM接收服务
        RecvECCService,//ECC接收服务
    }

    public partial class Message : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; }

        public string Key1 { get; set; }

        public string Key2 { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime NewDate { get; set; }

        public bool IsSended { get; set; }

        public DateTime? SendDate { get; set; }

        public int NotificationId { get; set; }

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