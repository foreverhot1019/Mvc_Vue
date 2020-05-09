using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(OPS_H_OrderMetadata))]
    public partial class OPS_H_Order
    {
    }

    public partial class OPS_H_OrderMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

        [Display(Name = "Id")]

		public int Id{ get; set; }

        [Display(Name = "主单ID")]
        public int MBLId { get; set; }

        [Display(Name = "主单")]
        public virtual OPS_M_Order OPS_M_Order { get; set; }

        [Display(Name = "分单发货人")]

		public string Shipper_H{ get; set; }

        [Display(Name = "分单收货人")]

		public string Consignee_H{ get; set; }

        [Display(Name = "分单通知人")]

		public string Notify_Part_H{ get; set; }

        [Display(Name = "币种")]

		public string Currency_H{ get; set; }

        [Display(Name = "成交条款")]

		public string Bragainon_Article_H{ get; set; }

        [Display(Name = "付款方式")]

		public string Pay_Mode_H{ get; set; }

        [Display(Name = "运费P/C")]

		public string Carriage_H{ get; set; }

        [Display(Name = "杂费P/C")]

		public string Incidental_Expenses_H{ get; set; }

        [Display(Name = "申明价值（运输）")]

		public string Declare_Value_Trans_H{ get; set; }

        [Display(Name = "申明价值（海关）")]

		public string Declare_Value_Ciq_H{ get; set; }

        [Display(Name = "保险额度")]

		public string Risk_H{ get; set; }

        [Display(Name = "唛头")]

		public string Marks_H{ get; set; }

        [Display(Name = "英文货名")]

		public string EN_Name_H{ get; set; }

        [Display(Name = "件数")]

		public decimal Pieces_H{ get; set; }

        [Display(Name = "毛重")]

		public decimal Weight_H{ get; set; }

        [Display(Name = "体积")]

		public decimal Volume_H{ get; set; }

        [Display(Name = "计费重量")]

		public decimal Charge_Weight_H{ get; set; }

        [Display(Name = "分单号")]

		public string HBL{ get; set; }

        [Display(Name = "业务编号")]

		public string Operation_Id{ get; set; }

        [Display(Name = "自营")]

		public bool Is_Self{ get; set; }

        [Display(Name = "接单类别")]

		public string Ty_Type{ get; set; }

        [Display(Name = "拼箱码")]

		public string Lot_No{ get; set; }

        [Display(Name = "分担运费")]

		public string Hbl_Feight{ get; set; }

        [Display(Name = "消磁")]

		public bool Is_XC{ get; set; }

        [Display(Name = "BSA")]

		public bool Is_BAS{ get; set; }

        [Display(Name = "代操作")]

		public bool Is_DCZ{ get; set; }

        [Display(Name = "重板")]

		public bool Is_ZB{ get; set; }

        [Display(Name = "创建点")]

		public int ADDPoint{ get; set; }

        [Display(Name = "修改点")]

		public int EDITPoint{ get; set; }

        #region 20180913 add

        [Display(Name = "发送分单", Description = "发送分单")]
        public bool SendOut_FD { get; set; }

        [Display(Name = "发送人", Description = "发送人")]
        public string SendOut_WHO { get; set; }

        [Display(Name = "发送时间", Description = "发送时间")]
        public DateTime? SendOut_TS { get; set; }

        [Display(Name = "发送人", Description = "发送人")]
        public string SendOut_ID { get; set; }

        #endregion 

        [Display(Name = "状态")]

		public int Status{ get; set; }


        [Display(Name = "批次号")]

        public string Batch_Num { get; set; }

        [Display(Name = "操作点")]

		public int OperatingPoint{ get; set; }

        [Display(Name = "新增人ID")]

		public string ADDID{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITID{ get; set; }

	}

	public class OPS_H_OrderChangeViewModel
    {
        public IEnumerable<OPS_H_Order> inserted { get; set; }

        public IEnumerable<OPS_H_Order> deleted { get; set; }

        public IEnumerable<OPS_H_Order> updated { get; set; }
    }
}
