using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(OPS_M_OrderMetadata))]
    public partial class OPS_M_Order
    {
    }

    public partial class OPS_M_OrderMetadata
    {
        #region

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "总单号")]
        public string MBL { get; set; }

        [Display(Name = "航空公司")]
        public string Airways_Code { get; set; }

        [Display(Name = "国外代理")]

        public string FWD_Code { get; set; }

        [Display(Name = "主单发货人")]

        public string Shipper_M { get; set; }

        [Display(Name = "主单收货人")]

        public string Consignee_M { get; set; }

        [Display(Name = "主单通知人")]

        public string Notify_Part_M { get; set; }

        [Display(Name = "起运港")]

        public string Depart_Port { get; set; }

        [Display(Name = "目的港")]

        public string End_Port { get; set; }

        [Display(Name = "航班号")]

        public string Flight_No { get; set; }

        [Display(Name = "航班日期")]

        public DateTime Flight_Date_Want { get; set; }

        [Display(Name = "币种")]

        public string Currency_M { get; set; }

        [Display(Name = "成交条款")]

        public string Bragainon_Article_M { get; set; }

        [Display(Name = "付款方式")]

        public string Pay_Mode_M { get; set; }

        [Display(Name = "运费P/C")]

        public string Carriage_M { get; set; }

        [Display(Name = "杂费P/C")]

        public string Incidental_Expenses_M { get; set; }

        [Display(Name = "申明价值（运输）")]

        public string Declare_Value_Trans_M { get; set; }

        [Display(Name = "申明价值（海关）")]

        public string Declare_Value_Ciq_M { get; set; }

        [Display(Name = "保险额度")]

        public string Risk_M { get; set; }

        [Display(Name = "主单唛头")]

        public string Marks_M { get; set; }

        [Display(Name = "主单英文货名")]

        public string EN_Name_M { get; set; }

        [Display(Name = "Hand_Info_M")]

        public string Hand_Info_M { get; set; }

        [Display(Name = "Signature_Agent_M")]

        public string Signature_Agent_M { get; set; }

        [Display(Name = "Rate_Class_M")]

        public string Rate_Class_M { get; set; }

        [Display(Name = "空运费单价")]

        public decimal Air_Frae { get; set; }

        [Display(Name = "AWC")]

        public decimal AWC { get; set; }

        [Display(Name = "件数")]

        public decimal Pieces_M { get; set; }

        [Display(Name = "毛重")]

        public decimal Weight_M { get; set; }

        [Display(Name = "计费重量")]

        public decimal Volume_M { get; set; }

        [Display(Name = "计费重量")]

        public decimal Charge_Weight_M { get; set; }

        [Display(Name = "约价方案")]

        public string Price_Article { get; set; }

        [Display(Name = "CCC")]

        public string CCC { get; set; }

        [Display(Name = "附档")]

        public string File_M { get; set; }

        [Display(Name = "状态")]

        public int Status { get; set; }


        [Display(Name = "总单结算状态")]
        public bool OPS_BMS_Status { get; set; }

        [Display(Name = "操作点")]

        public int OperatingPoint { get; set; }

        [Display(Name = "新增人")]

        public string ADDID { get; set; }

        [Display(Name = "新增人")]

        public string ADDWHO { get; set; }

        [Display(Name = "新增时间")]

        public DateTime ADDTS { get; set; }

        [Display(Name = "修改人")]

        public string EDITWHO { get; set; }

        [Display(Name = "修改时间")]

        public DateTime EDITTS { get; set; }

        [Display(Name = "修改人")]

        public string EDITID { get; set; }
        #endregion

        #region 需求修改后的新增字段

        [Display(Name = "分单信息")]
        public ICollection<OPS_H_Order> OPS_H_Orders { get; set; }

        [Display(Name = "委托方信息")]
        public ICollection<OPS_EntrustmentInfor> OPS_EntrustmentInfors { get; set; }

        #endregion

        #region 20180913 add

        [Display(Name = "发送主单", Description = "发送主单")]
        public bool SendOut_ZD { get; set; }

        [Display(Name = "发送人", Description = "发送人")]
        public string SendOut_WHO { get; set; }

        [Display(Name = "发送时间", Description = "发送时间")]
        public DateTime? SendOut_TS { get; set; }

        [Display(Name = "发送人", Description = "发送人")]
        public string SendOut_ID { get; set; }

        #endregion

    }

    public class OPS_M_OrderChangeViewModel
    {
        public IEnumerable<OPS_M_Order> inserted { get; set; }

        public IEnumerable<OPS_M_Order> deleted { get; set; }

        public IEnumerable<OPS_M_Order> updated { get; set; }
    }
}
