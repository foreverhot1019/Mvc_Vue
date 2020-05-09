using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //机票订单
    public partial class AirTicketOrder : Entity
    {
        public AirTicketOrder()
        {
            ArrPlanePerson = new HashSet<PlanePerson>();
            ArrAirLine = new HashSet<AirLine>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "订单编号")]
        [MaxLength(100)]
        [Index("IX_AirTicketOrdNo", IsUnique = true, Order = 1)]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 1)]
        public string AirTicketNo { get; set; }

        [Display(Name = "企业编号")]
        [MaxLength(50)]
        [Required]
        //[Vue_PagePropty(SearchShow = true, SearchOrder = 2)]
        public string CompanyCIQID { get; set; }

        [Display(Name = "企业名称")]
        [MaxLength(100)]
        [Required]
        //[Vue_PagePropty(SearchShow = true, SearchOrder = 3)]
        public string CompanyName { get; set; }

        [Display(Name = "销售")]
        [MaxLength(50)]
        public string Saller { get; set; }

        [Display(Name = "机票订单类型")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 4)]
        public EnumType.AirTicketOrdTypeEnum AirTicketOrdType { get; set; }

        [Display(Name = "票别")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 5)]
        public EnumType.TicketTypeEnum TicketType { get; set; }

        [Display(Name = "乘机人")]
        [Vue_PagePropty(SearchShow = true, SearchOrder = 6)]

        [MaxLength(50)]
        public string PlanePerson { get; set; }

        [Display(Name = "乘机人信息")]
        public ICollection<PlanePerson> ArrPlanePerson { get; set; }

        [Display(Name = "航班信息")]
        public ICollection<AirLine> ArrAirLine { get; set; }

        [Display(Name = "PNR")]
        public string PNR { get; set; }

        [Display(Name = "出团人数")]
        public int? TravlePersonNum { get; set; }

        [Display(Name = "预计支付日期")]
        public DateTime ExpectPaymentDate { get; set; }

        [Display(Name = "供应商")]
        [MaxLength(50)]
        public string SupplierName { get; set; }

        [Display(Name = "原订单编号")]
        [MaxLength(50)]
        public string AirTicketNo_Org { get; set; }

        [Display(Name = "状态")]
        public EnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "审核状态")]
        public EnumType.AuditStatusEnum AuditStatus { get; set; }

        #region ScaffoldColumn

        [Index("IX_AirTicketOrd_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }
}