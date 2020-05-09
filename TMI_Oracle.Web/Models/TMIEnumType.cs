using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    public static class EnumType
    {
        /// <summary>
        /// 使用状态
        /// </summary>
        public enum UseStatusEnum
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
        /// 计算运算符
        /// </summary>
        public enum UseCalcSignEnum
        {
            [Display(Name = ">", Description = ">")]
            DaYu = 1,
            [Display(Name = ">=", Description = ">=")]
            DaYuDengYu = 2,
            [Display(Name = "<", Description = "<")]
            XiaoYu = 3,
            [Display(Name = "<=", Description = "<=")]
            XiaoYuDengYu = 4,
            [Display(Name = "=", Description = "=")]
            DengYu = 5,
            [Display(Name = "!=", Description = "!=")]
            BuDengYu = 6
        }

        /// <summary>
        /// 联系方式
        /// </summary>
        public enum ContactType
        {
            [Display(Name = "手机", Description = "")]
            手机 = 1,
            [Display(Name = "QQ", Description = "")]
            QQ = 2,
            [Display(Name = "微信", Description = "")]
            微信 = 3,
        }

        /// <summary>
        /// 客户等级
        /// </summary>
        public enum CustomerLevel
        {
            [Display(Name = "一级", Description = "")]
            一级 = 1,
            [Display(Name = "二级", Description = "")]
            二级 = 2,
            [Display(Name = "三级", Description = "")]
            三级 = 3,
            [Display(Name = "四级", Description = "")]
            四级 = 4,
            [Display(Name = "五级", Description = "")]
            五级 = 5,
            [Display(Name = "六级", Description = "")]
            六级 = 6,
        }

        /// <summary>
        /// 客户来源
        /// </summary>
        public enum CustomerSource
        {
            [Display(Name = "开发", Description = "")]
            开发 = 1,
            [Display(Name = "上门", Description = "")]
            上门 = 2,
        }

        /// <summary>
        /// 客户类型
        /// </summary>
        public enum CustomerType
        {
            [Display(Name = "散客", Description = "")]
            散客 = 1,
            [Display(Name = "团客", Description = "")]
            团客 = 2,
        }

        /// <summary>
        /// 活跃状态
        /// </summary>
        public enum ActiveStatus
        {
            [Display(Name = "正常", Description = "")]
            正常 = 1,
            [Display(Name = "活跃", Description = "")]
            活跃 = 2,
            [Display(Name = "潜水", Description = "")]
            潜水 = 3,
        }

        /// <summary>
        /// 证件类型
        /// </summary>
        public enum IdCardType
        {
            [Display(Name = "身份证", Description = "身份证")]
            S = 1,
            [Display(Name = "护照", Description = "护照")]
            H = 2,
            [Display(Name = "港澳通行证", Description = "港澳通行证")]
            G = 3,
            [Display(Name = "台湾通行证", Description = "台湾通行证")]
            T = 4,
            [Display(Name = "台胞证", Description = "台胞证")]
            B = 5
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public enum OrderStatusEnum
        {
            [Display(Name = "草稿", Description = "草稿")]
            Draft = 0,
            [Display(Name = "确认", Description = "确认")]
            OK = 1,
            [Display(Name = "未成交", Description = "未成交")]
            Cancle = -1
        }

        /// <summary>
        /// 旅游类型
        /// </summary>
        public enum TravleTypeEnum
        {
            [Display(Name = "出境", Description = "出境")]
            Foreign = 1,
            [Display(Name = "短线", Description = "短线")]
            ShortLine = 2,
            [Display(Name = "长线", Description = "长线")]
            LongLine = 3
        }

        /// <summary>
        /// 旅游订单类型
        /// </summary>
        public enum TravleOrdTypeEnum
        {
            [Display(Name = "旅游", Description = "旅游")]
            Travle = 1,
            [Display(Name = "机票", Description = "机票")]
            AirLineTicket = 2,
        }

        /// <summary>
        /// 机票订单类型
        /// </summary>
        public enum AirTicketOrdTypeEnum
        {
            [Display(Name = "企业客户", Description = "企业客户")]
            Company = 1,
            [Display(Name = "散客", Description = "散客")]
            Individual= 2,
            [Display(Name = "专案", Description = "专案")]
            SpecialCase = 3,
        }

        /// <summary>
        /// 票别类型
        /// </summary>
        public enum TicketTypeEnum
        {
            [Display(Name = "国际", Description = "国际")]
            International = 1,
            [Display(Name = "国内", Description = "国内")]
            Domestic = 2,
        }

        /// <summary>
        /// 航班类型
        /// </summary>
        public enum AirLineTypeEnum
        {
            [Display(Name = "去程", Description = "去程")]
            Deaprture = 1,
            [Display(Name = "回程", Description = "回程")]
            Return = 2,
            [Display(Name = "联程", Description = "联程")]
            Transit = 3,
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public enum PayType
        {
            [Display(Name = "支付宝", Description = "支付宝")]
            AliPay = 1,
            [Display(Name = "微信", Description = "微信")]
            WeChat = 2,
            [Display(Name = "银行卡", Description = "银行卡")]
            BankCard = 3,
            [Display(Name = "现金", Description = "现金")]
            Cash = 4,
            [Display(Name = "银联", Description = "银联")]
            UnionPay = 5,
        }

        #region Message 使用
        
        /// <summary>
        /// 消息类型
        /// </summary>
        public enum MessageType
        {
            Information,
            Message,
            Error,
            Alert,
            Warning
        }

        /// <summary>
        /// 通知类型
        /// </summary>
        public enum NotificationType
        {
            Email,
            GSM,
            Phone,
            Message
        }

        /// <summary>
        /// 通知标志
        /// </summary>
        public enum NotificationTag
        {
            Sys,//系统
            Servive//服务
        }

        #endregion
    }
}