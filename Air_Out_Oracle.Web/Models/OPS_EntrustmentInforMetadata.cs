













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(OPS_EntrustmentInforMetadata))]
    public partial class OPS_EntrustmentInfor
    {
    }

    public partial class OPS_EntrustmentInforMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }


        [Display(Name = "主单ID")]
        public int MBLId { get; set; }

        [Display(Name = "主单")]
        public virtual OPS_M_Order OPS_M_Order { get; set; }

        [Display(Name = "发货方")]

		public string Consign_Code{ get; set; }

        [Display(Name = "关区")]

		public string Custom_Code{ get; set; }

        [Display(Name = "区域")]

		public string Area_Code{ get; set; }

        [Display(Name = "委托方")]

		public string Entrustment_Name{ get; set; }

        [Display(Name = "委托方编号")]

		public string Entrustment_Code{ get; set; }

        [Display(Name = "国外代理")]

		public string FWD_Code{ get; set; }

        [Display(Name = "收货方")]

		public string Consignee_Code{ get; set; }

        [Display(Name = "运费结算方")]

		public string Carriage_Account_Code{ get; set; }

        [Display(Name = "杂费结算方")]

		public string Incidental_Account_Code{ get; set; }

        [Display(Name = "起运港")]

		public string Depart_Port{ get; set; }

        [Display(Name = "中转港")]

		public string Transfer_Port{ get; set; }

        [Display(Name = "目的港")]

		public string End_Port{ get; set; }

        [Display(Name = "分单发货人")]

		public string Shipper_H{ get; set; }

        [Display(Name = "分单收货人")]

		public string Consignee_H{ get; set; }

        [Display(Name = "分单通知人")]

		public string Notify_Part_H{ get; set; }

        [Display(Name = "主单发货人")]

		public string Shipper_M{ get; set; }

        [Display(Name = "主单收货人")]

		public string Consignee_M{ get; set; }

        [Display(Name = "主单通知人")]

		public string Notify_Part_M{ get; set; }

        [Display(Name = "托书/件数")]

		public decimal Pieces_TS{ get; set; }

        [Display(Name = "托书/毛重")]

		public decimal Weight_TS{ get; set; }

        [Display(Name = "收款分单/件数")]

		public decimal Pieces_SK{ get; set; }

        [Display(Name = "收款分单/SLAC")]

		public decimal Slac_SK{ get; set; }

        [Display(Name = "收款分单/毛重")]

		public decimal Weight_SK{ get; set; }

        [Display(Name = "订舱主单/件数")]

		public decimal Pieces_DC{ get; set; }

        [Display(Name = "订舱主单/SLAC")]

		public decimal Slac_DC{ get; set; }

        [Display(Name = "订舱主单/毛重")]

		public decimal Weight_DC{ get; set; }

        [Display(Name = "进仓实际/件数")]

		public decimal Pieces_Fact{ get; set; }

        [Display(Name = "进仓实际/毛重")]

		public decimal Weight_Fact{ get; set; }

        [Display(Name = "是否靠级")]

		public bool IS_MoorLevel{ get; set; }

        [Display(Name = "靠级")]

		public string MoorLevel{ get; set; }

        [Display(Name = "托书/体积")]

		public decimal Volume_TS{ get; set; }

        [Display(Name = "托书/计费重量")]

		public decimal Charge_Weight_TS{ get; set; }

        [Display(Name = "托书/泡重KG")]

		public decimal Bulk_Weight_TS{ get; set; }

        [Display(Name = "收款分单/体积")]

		public decimal Volume_SK{ get; set; }

        [Display(Name = "收款分单/计费重量")]

		public decimal Charge_Weight_SK{ get; set; }

        [Display(Name = "收款分单/泡重KG")]

		public decimal Bulk_Weight_SK{ get; set; }

        [Display(Name = "收款分单/分泡比%")]

		public decimal Bulk_Percent_SK{ get; set; }

        [Display(Name = "收款分单/结算重量")]

		public decimal Account_Weight_SK{ get; set; }

        [Display(Name = "订舱主单/体积")]

		public decimal Volume_DC{ get; set; }

        [Display(Name = "订舱主单/计费重量")]

		public decimal Charge_Weight_DC{ get; set; }

        [Display(Name = "订舱主单/泡重KG")]

		public decimal Bulk_Weight_DC{ get; set; }

        [Display(Name = "订舱主单/分泡比%")]

		public decimal Bulk_Percent_DC{ get; set; }

        [Display(Name = "订舱主单/结算重量")]

		public decimal Account_Weight_DC{ get; set; }

        [Display(Name = "进仓实际/体积")]

		public decimal Volume_Fact{ get; set; }

        [Display(Name = "进仓实际/计费重量")]

		public decimal Charge_Weight_Fact{ get; set; }

        [Display(Name = "进仓实际/泡重KG")]

		public decimal Bulk_Weight_Fact{ get; set; }

        [Display(Name = "进仓实际/分泡比%")]

		public decimal Bulk_Percent_Fact{ get; set; }

        [Display(Name = "进仓实际/结算重量")]

		public decimal Account_Weight_Fact{ get; set; }

        [Display(Name = "分单唛头")]

		public string Marks_H{ get; set; }

        [Display(Name = "分单英文货名")]

		public string EN_Name_H{ get; set; }

        [Display(Name = "KSF")]
        public bool Is_Book_Flat { get; set; }

        [Display(Name = "订舱方")]

		public string Book_Flat_Code{ get; set; }

        [Display(Name = "航空公司")]

		public string Airways_Code{ get; set; }

        [Display(Name = "航班号/EDT")]

        public string Flight_No { get; set; }

        [Display(Name = "总单号")]

		public string MBL{ get; set; }

        [Display(Name = "分单号")]

		public string HBL{ get; set; }

        [Display(Name = "航班日期")]

		public DateTime Flight_Date_Want{ get; set; }

        [Display(Name = "订舱要求")]

		public string Book_Remark{ get; set; }

        [Display(Name = "交货地点")]

		public string Delivery_Point{ get; set; }

        [Display(Name = "仓库")]

		public string Warehouse_Code{ get; set; }

        [Display(Name = "口岸入库日期")]

		public DateTime RK_Date{ get; set; }

        [Display(Name = "口岸出库日期")]

		public DateTime CK_Date{ get; set; }

        [Display(Name = "中文货名")]

		public string CH_Name{ get; set; }

		[Display(Name="AMS")]

		public decimal AMS{ get; set; }


        #region 20180913 add

        [Display(Name = "货到齐", Description = "货到齐")]
        public bool Is_HDQ { get; set; }

        [Display(Name = "是否报关", Description = "是否报关")]
        public bool Is_BG { get; set; }

        [Display(Name = "打印标签", Description = "是否打印标签")]
        public bool Is_BQ { get; set; }

        [Display(Name = "是否出库", Description = "是否出库")]
        public bool Is_OutGoing { get; set; }

        #endregion 

        [Display(Name = "状态")]

        public int Status { get; set; }

        [Display(Name = "备注")]

		public string Remark{ get; set; }

        [Display(Name = "操作点")]

		public int OperatingPoint{ get; set; }

        [Display(Name = "新增人")]

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

	public class OPS_EntrustmentInforChangeViewModel
    {
        public IEnumerable<OPS_EntrustmentInfor> inserted { get; set; }

        public IEnumerable<OPS_EntrustmentInfor> deleted { get; set; }

        public IEnumerable<OPS_EntrustmentInfor> updated { get; set; }
    }
}
