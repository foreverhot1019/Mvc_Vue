





using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(AirLineMetadata))]
    public partial class AirLine
    {
    }

    public partial class AirLineMetadata
    {

        [Display(Name="机票订单",Description="机票订单")]

        public AirTicketOrder OAirTicket{ get; set; }

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="机票订单",Description="机票订单")]

        public int AirTicketOrderId{ get; set; }

        [Display(Name="航班类型",Description="航班类型")]

        public TMI.Web.Models.EnumType.AirLineTypeEnum AirLineType{ get; set; }

        [Display(Name="航空公司",Description="航空公司")]

        [MaxLength(50)]

        public string AirCompany{ get; set; }

        [Display(Name="航班号",Description="航班号")]

        [MaxLength(50)]

        public string Flight_No{ get; set; }

        [Display(Name="城市对",Description="城市对")]

        [MaxLength(50)]

        public string City{ get; set; }

        [Display(Name="仓位",Description="仓位")]

        [MaxLength(50)]

        public string Position{ get; set; }

        [Display(Name="航班日期",Description="航班日期")]

        public DateTime Flight_Date_Want{ get; set; }

        [Display(Name="起飞时间",Description="起飞时间")]

        public DateTime TakeOffTime{ get; set; }

        [Display(Name="到达时间",Description="到达时间")]

        public DateTime ArrivalTime{ get; set; }

        [Display(Name="票价",Description="票价")]

        public decimal TicketPrice{ get; set; }

        [Display(Name="税金",Description="税金")]

        public decimal BillTaxAmount{ get; set; }

        [Display(Name="成本",Description="成本")]

        public decimal CostMoney{ get; set; }

        [Display(Name="售价",Description="售价")]

        public decimal SellPrice{ get; set; }

        [Display(Name="机票号",Description="机票号")]

        [MaxLength(50)]

        public string TicketNum{ get; set; }

        [Display(Name="利润",Description="利润")]

        public decimal Profit{ get; set; }

        [Display(Name="保险",Description="保险")]

        public decimal Insurance{ get; set; }

        [Display(Name="改签",Description="改签")]

        public bool Is_Endorse{ get; set; }

        [Display(Name="改签日期",Description="改签日期")]

        public DateTime EndorseDate{ get; set; }

        [Display(Name="改签人",Description="改签人")]

        [MaxLength(50)]

        public string EndorseWho{ get; set; }

        [Display(Name="退票",Description="退票")]

        public bool Is_ReturnTicket{ get; set; }

        [Display(Name="退票日期",Description="退票日期")]

        public DateTime ReturnTicketDate{ get; set; }

        [Display(Name="退票人",Description="退票人")]

        [MaxLength(50)]

        public string ReturnTicketWho{ get; set; }

        [Display(Name="作废",Description="作废")]

        public bool Is_Cancel{ get; set; }

        [Display(Name="作废日期",Description="作废日期")]

        public DateTime CancelDate{ get; set; }

        [Display(Name="作废人",Description="作废人")]

        [MaxLength(50)]

        public string CancelWho{ get; set; }

        [Display(Name="备注",Description="备注")]

        [MaxLength(500)]

        public string Remark{ get; set; }

        [Display(Name="操作点",Description="操作点")]

        public int OperatingPoint{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(50)]

        public string ADDID{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(20)]

        public string ADDWHO{ get; set; }

        [Display(Name="新增时间",Description="新增时间")]

        public DateTime ADDTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(20)]

        public string EDITWHO{ get; set; }

        [Display(Name="修改时间",Description="修改时间")]

        public DateTime EDITTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(50)]

        public string EDITID{ get; set; }

	}

	public class AirLineChangeViewModel
    {
        public IEnumerable<AirLine> inserted { get; set; }

        public IEnumerable<AirLine> deleted { get; set; }

        public IEnumerable<AirLine> updated { get; set; }
    }
}
