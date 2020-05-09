using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //航班信息
    public partial class AirLine:Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "机票订单")]
        public int? AirTicketOrderId { get; set; }

        [Display(Name = "机票订单")]
        [ForeignKey("AirTicketOrderId")]
        public virtual AirTicketOrder OAirTicket { get; set; }

        [Display(Name = "航班类型")]
        public EnumType.AirLineTypeEnum AirLineType { get; set; }

        [Display(Name = "航空公司")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true)]
        public string AirCompany { get; set; }

        [Display(Name = "航班号")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true)]
        public string Flight_No { get; set; }

        [Display(Name = "城市对")]
        [MaxLength(50)]
        public string City { get; set; }

        [Display(Name = "仓位")]
        [MaxLength(50)]
        public string Position { get; set; }

        [Display(Name = "航班日期")]
        public DateTime Flight_Date_Want { get; set; }

        [Display(Name = "起飞时间")]
        public DateTime TakeOffTime { get; set; }

        [Display(Name = "到达时间")]
        public DateTime ArrivalTime { get; set; }

        [Display(Name = "票价")]
        public decimal? TicketPrice { get; set; }

        [Display(Name = "税金")]
        public decimal? BillTaxAmount { get; set; }

        [Display(Name = "成本")]
        public decimal? CostMoney { get; set; }

        [Display(Name = "售价")]
        public decimal? SellPrice { get; set; }

        [Display(Name = "机票号")]
        [MaxLength(50)]
        [Vue_PagePropty(SearchShow = true)]
        public string  TicketNum{ get; set; }

        [Display(Name = "利润")]
        public decimal? Profit { get; set; }

        [Display(Name = "保险")]
        public decimal? Insurance { get; set; }

        [Display(Name = "改签")]
        public bool? Is_Endorse { get; set; }

        [Display(Name = "改签日期")]
        public DateTime? EndorseDate { get; set; }

        [Display(Name = "改签人")]
        [MaxLength(50)]
        public string EndorseWho { get; set; }

        [Display(Name = "退票")]
        public bool? Is_ReturnTicket { get; set; }

        [Display(Name = "退票日期")]
        public DateTime? ReturnTicketDate { get; set; }

        [Display(Name = "退票人")]
        [MaxLength(50)]
        public string ReturnTicketWho { get; set; }

        [Display(Name = "作废")]
        public bool? Is_Cancel { get; set; }

        [Display(Name = "作废日期")]
        public DateTime? CancelDate { get; set; }

        [Display(Name = "作废人")]
        [MaxLength(50)]
        public string CancelWho { get; set; }

        [Display(Name = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        #region ScaffoldColumn

        [Index("IX_AirLine_OP", IsUnique = false, Order = 1)]
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