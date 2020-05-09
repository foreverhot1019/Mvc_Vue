using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //客户报价
    public partial class CustomerQuotedPrice : Entity
    {
        public CustomerQuotedPrice()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
            AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "记录号", Description = "CQ201805040001:CQ+年月日+4位流水码")]
        [Required, MaxLength(50)]
        [Index("IX_CusQuotedP_Key", IsUnique = true, Order = 1)]
        public string SerialNo { get; set; }

        [Display(Name = "业务类型", Description = "")]
        [MaxLength(50)]
        public string BusinessType { get; set; }

        [Display(Name = "客户代码", Description = "")]
        [MaxLength(50)]
        public string CustomerCode { get; set; }

        [Display(Name = "本仓标志", Description = "")]
        [MaxLength(50)]
        public string LocalWHMark { get; set; }

        [Display(Name = "目的港", Description = "")]
        [MaxLength(50)]
        public string EndPortCODE { get; set; }

        [Display(Name = "起始地", Description = "")]
        [MaxLength(50)]
        public string StartPlace { get; set; }

        [Display(Name = "中转地", Description = "")]
        [MaxLength(50)]
        public string TransitPlace { get; set; }

        [Display(Name = "目的地", Description = "")]
        [MaxLength(50)]
        public string EndPlace { get; set; }

        [Display(Name = "代理", Description = "")]
        [MaxLength(50)]
        public string ProxyOperator { get; set; }

        [Display(Name = "自定义名称", Description = "")]
        [MaxLength(50)]
        public string CusDefinition { get; set; }

        [Display(Name = "收货人", Description = "")]
        [MaxLength(50)]
        public string Receiver { get; set; }

        [Display(Name = "发货人", Description = "")]
        [MaxLength(50)]
        public string Shipper { get; set; }

        [Display(Name = "联系人", Description = "")]
        [MaxLength(50)]
        public string Contact { get; set; }

        [Display(Name = "报价政策", Description = "")]
        [MaxLength(50)]
        public string QuotedPricePolicy { get; set; }

        [Display(Name = "销售员", Description = "")]
        [MaxLength(50)]
        public string Seller { get; set; }

        [Display(Name = "开始日期", Description = "")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "停用日期", Description = "")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "审批状态", Description = "")]
        public AirOutEnumType.AuditStatusEnum AuditStatus { get; set; }

        [Index("IX_CusQtdPic_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Index("IX_CusQtdPic_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }
    }
}