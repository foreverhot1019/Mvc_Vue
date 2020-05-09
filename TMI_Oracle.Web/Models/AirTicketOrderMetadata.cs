using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(AirTicketOrderMetadata))]
    public partial class AirTicketOrder
    {
    }

    public partial class AirTicketOrderMetadata
    {

        [Display(Name="航班信息",Description="航班信息")]

        public AirLine ArrAirLine{ get; set; }

        [Display(Name="乘机人信息",Description="乘机人信息")]

        public PlanePerson ArrPlanePerson{ get; set; }

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="订单编号",Description="订单编号")]

        [MaxLength(100)]

        public string AirTicketNo{ get; set; }

        [Display(Name="企业编号",Description="企业编号")]

        [MaxLength(50)]

        public string CompanyCIQID{ get; set; }

        [Display(Name="企业名称",Description="企业名称")]

        [MaxLength(100)]

        public string CompanyName{ get; set; }

        [Display(Name="销售",Description="销售")]

        [MaxLength(50)]

        public string Saller{ get; set; }

        [Display(Name="机票订单类型",Description="机票订单类型")]

        public EnumType.AirTicketOrdTypeEnum AirTicketOrdType { get; set; }

        [Display(Name="票别",Description="票别")]

        public EnumType.TicketTypeEnum TicketType { get; set; }

        [Display(Name="乘机人",Description="乘机人")]

        [MaxLength(50)]

        public string PlanePerson{ get; set; }

        [Display(Name="PNR",Description="PNR")]

        [MaxLength(50)]

        public string PNR{ get; set; }

        [Display(Name="出团人数",Description="出团人数")]

        public int TravlePersonNum{ get; set; }

        [Display(Name="预计支付日期",Description="预计支付日期")]

        public DateTime ExpectPaymentDate{ get; set; }

        [Display(Name="供应商",Description="供应商")]

        [MaxLength(50)]

        public string SupplierName{ get; set; }

        [Display(Name="原订单编号",Description="原订单编号")]

        [MaxLength(50)]

        public string AirTicketNo_Org{ get; set; }

        [Display(Name="状态",Description="状态")]

        public EnumType.UseStatusEnum Status { get; set; }

        [Display(Name="审核状态",Description="审核状态")]

        public EnumType.AuditStatusEnum AuditStatus { get; set; }

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

    public class AirTicketOrderView
    {
        public AirTicketOrder AirTicketOrder { get; set; }

        [Display(Name = "乘机人信息", Description = "乘机人信息 ArrPlanePerson")]
        public IEnumerable<PlanePerson> ArrPlanePerson { get; set; }

        [Display(Name = "航班信息", Description = "航班信息 ArrAirLine")]
        public IEnumerable<AirLine> ArrAirLine { get; set; }

    }

    //需要删除的数据
    public class ATPAdeltRows
    {
        public IEnumerable<int> DelPlanePerson { get; set; }
        public IEnumerable<int> DelAirLine { get; set; }
    }


	public class AirTicketOrderChangeViewModel
    {
        public IEnumerable<AirTicketOrderView> inserted { get; set; }

        public IEnumerable<AirTicketOrderView> deleted { get; set; }

        public IEnumerable<AirTicketOrderView> updated { get; set; }

        public ATPAdeltRows ATPAdeltRows { get; set; }

    }
}
