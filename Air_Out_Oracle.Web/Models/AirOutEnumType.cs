using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public static class AirOutEnumType
    {
        /// <summary>
        /// 使用状态
        /// </summary>
        public enum UseStatusEnum
        {
            [Display(Name = "草稿", Description = "草稿")] 
            draft = 0,
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
        /// 状态
        /// </summary>
        public enum UseStatusIsOrNoEnum
        {
            [Display(Name = "否", Description = "否")]
            draft = 0,
            [Display(Name = "是", Description = "是")]
            Enable = 1
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
        /// 图片类型
        /// </summary>
        public enum PictureTypeEnum
        {
            [Display(Name = "破损", Description = "破损")]
            Damaged = 1,
            [Display(Name = "交货", Description = "交货")]
            Dampness = 2,
            [Display(Name = "进仓", Description = "进仓")]
            Entry = 3,
            [Display(Name = "附档文件", Description = "附档文件")]
            Fileupload = 4,
            [Display(Name = "总单附档文件", Description = "总单附档文件")]
            Fileupload_MBL = 5,
            [Display(Name = "分单附档文件", Description = "分单附档文件")]
            Fileupload_HBL = 6
        }

        /// <summary>
        /// 账单 产生标志
        /// </summary>
        public enum Bms_BillCreate_Status
        {
            [Display(Name = "自动", Description = "自动产生")]
            AutoSet = 1,
            [Display(Name = "手动", Description = "手动产生")]
            HandSet = 2
        }
    }
}