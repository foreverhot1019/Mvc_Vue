using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class EnumType
    {
        /// <summary>
        /// 使用状态
        /// </summary>
        public enum StatusEnum
        {
            [Display(Name = "草稿", Description = "草稿")]
            Draft = 0,
            [Display(Name = "启用", Description = "启用")]
            Enable = 1,
            [Display(Name = "停用", Description = "停用")]
            Disable = -1
        }

        /// <summary>
        /// 审批状态
        /// </summary>
        public enum AuditStatusEnum
        {
            [Display(Name = "草稿", Description = "草稿")]
            draft = 0,
            [Display(Name = "审批中", Description = "审批中")]
            Auditing = 1,
            [Display(Name = "审批通过", Description = "审批通过")]
            AuditSuccess = 2,
            [Display(Name = "审批拒绝", Description = "审批拒绝")]
            AuditFail = -1
        }

        /// <summary>
        /// 日志类型
        /// SQLMessage 存到数据库
        /// LocalLog 存到本地
        /// </summary>
        public enum RedisLogMsgType
        {
            LocalLog = 1,
            SQLMessage = 2
        }

        /// <summary>
        /// Log4Net 信息 类型
        /// level（级别）：标识这条日志信息的重要级别None>Fatal>ERROR>WARN>DEBUG>INFO>ALL，设定一个
        /// </summary>
        public enum Log4NetMsgType
        {
            [Display(Name = "None", Description = "未标记")]
            None,
            [Display(Name = "Fatal", Description = "严重错误")]
            Fatal = 1,
            [Display(Name = "Error", Description = "错误")]
            Error = 2,
            [Display(Name = "Warn", Description = "警告")]
            Warn = 3,
            [Display(Name = "Debug", Description = "调试")]
            Debug = 4,
            [Display(Name = "Info", Description = "信息")]
            Info = 5,
            [Display(Name = "All", Description = "所有信息")]
            All
        };

    }
}