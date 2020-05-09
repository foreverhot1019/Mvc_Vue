using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //委托信息表
    public partial class OPS_EntrustmentInfor : Entity
    {
        public OPS_EntrustmentInfor()
        {
            Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
            SallerName = "-";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        #region 需求修改后的新增字段

        [Display(Name = "主单ID", Description = "主单ID")]
        public int? MBLId { get; set; }

        [ForeignKey("MBLId")]
        [Display(Name = "主单", Description = "主单")]
        public virtual OPS_M_Order OPS_M_Order { get; set; }

        #endregion

        [Index("IX_OPEttInforMBL", IsUnique = false, Order = 1)]
        [Display(Name = "总单号", Description = "总单号")]
        [MaxLength(20)]
        public string MBL { get; set; }

        [Display(Name = "分单号", Description = "分单号")]
        [MaxLength(20)]
        public string HBL { get; set; }

        [Display(Name = "业务编号", Description = "业务编号")]
        [MaxLength(20)]
        public string Operation_Id { get; set; }

        #region 委托信息一

        [Display(Name = "发货方", Description = "发货方")]
        [MaxLength(50)]
        public string Consign_Code { get; set; }

        [Display(Name = "关区", Description = "关区")]
        [MaxLength(50)]
        public string Custom_Code { get; set; }

        [Display(Name = "区域", Description = "区域")]
        [MaxLength(50)]
        public string Area_Code { get; set; }

        [Display(Name = "委托方", Description = "委托方")]
        [MaxLength(100)]
        public string Entrustment_Name { get; set; }

        [Display(Name = "委托方编号", Description = "委托方编号")]
        [MaxLength(100)]
        public string Entrustment_Code { get; set; }

        [Display(Name = "国外代理", Description = "国外代理")]
        [MaxLength(50)]
        public string FWD_Code { get; set; }

        [Display(Name = "收货方", Description = "收货方")]
        [MaxLength(50)]
        public string Consignee_Code { get; set; }

        [Display(Name = "运费结算方", Description = "运费结算方")]
        [MaxLength(50)]
        public string Carriage_Account_Code { get; set; }

        [Display(Name = "杂费结算方", Description = "杂费结算方")]
        [MaxLength(50)]
        public string Incidental_Account_Code { get; set; }

        [Display(Name = "起运港", Description = "起运港")]
        [MaxLength(50)]
        public string Depart_Port { get; set; }

        [Display(Name = "中转港", Description = "中转港")]
        [MaxLength(50)]
        public string Transfer_Port { get; set; }

        [Display(Name = "目的港", Description = "目的港")]
        [MaxLength(50)]
        public string End_Port { get; set; }

        [Display(Name = "分单发货人", Description = "分单发货人")]
        [MaxLength(2000)]
        public string Shipper_H { get; set; }

        [Display(Name = "分单收货人", Description = "分单收货人")]
        [MaxLength(2000)]
        public string Consignee_H { get; set; }

        [Display(Name = "分单通知人", Description = "分单通知人(同时通知人)")]
        [MaxLength(2000)]
        public string Notify_Part_H { get; set; }

        [Display(Name = "主单发货人", Description = "主单发货人")]
        [MaxLength(2000)]
        public string Shipper_M { get; set; }

        [Display(Name = "主单收货人", Description = "主单收货人")]
        [MaxLength(2000)]
        public string Consignee_M { get; set; }

        [Display(Name = "主单通知人", Description = "主单通知人(同时通知人)")]
        [MaxLength(2000)]
        public string Notify_Part_M { get; set; }

        #endregion

        #region 委托信息二

        [Display(Name = "是否靠级", Description = "是否靠级(0:否,1:是)")]
        public bool IS_MoorLevel { get; set; }

        [Display(Name = "靠级", Description = "靠级")]
        [MaxLength(20)]
        public string MoorLevel { get; set; }

        [Display(Name = "托书/件数", Description = "托书/件数")]
        public decimal? Pieces_TS { get; set; }

        [Display(Name = "托书/毛重", Description = "托书/毛重")]
        public decimal? Weight_TS { get; set; }

        [Display(Name = "托书/体积", Description = "托书/体积")]
        public decimal? Volume_TS { get; set; }

        [Display(Name = "托书/计费重量", Description = "托书/计费重量")]
        public decimal? Charge_Weight_TS { get; set; }

        [Display(Name = "托书/泡重KG", Description = "托书/泡重KG")]
        public decimal? Bulk_Weight_TS { get; set; }

        [Display(Name = "收款分单/件数", Description = "收款分单/件数")]
        public decimal? Pieces_SK { get; set; }

        [Display(Name = "收款分单/毛重", Description = "收款分单/毛重")]
        public decimal? Weight_SK { get; set; }

        [Display(Name = "收款分单/体积", Description = "收款分单/体积")]
        public decimal? Volume_SK { get; set; }

        [Display(Name = "收款分单/SLAC", Description = "收款分单/SLAC")]
        public decimal? Slac_SK { get; set; }

        [Display(Name = "收款分单/计费重量", Description = "收款分单/计费重量")]
        public decimal? Charge_Weight_SK { get; set; }

        [Display(Name = "收款分单/泡重KG", Description = "收款分单/泡重KG")]
        public decimal? Bulk_Weight_SK { get; set; }

        [Display(Name = "收款分单/分泡比%", Description = "收款分单/分泡比%")]
        public decimal? Bulk_Percent_SK { get; set; }

        [Display(Name = "收款分单/结算重量", Description = "收款分单/结算重量")]
        public decimal? Account_Weight_SK { get; set; }

        [Display(Name = "订舱主单/件数", Description = "订舱主单/件数")]
        public decimal? Pieces_DC { get; set; }

        [Display(Name = "订舱主单/SLAC", Description = "订舱主单/SLAC")]
        public decimal? Slac_DC { get; set; }

        [Display(Name = "订舱主单/毛重", Description = "订舱主单/毛重")]
        public decimal? Weight_DC { get; set; }

        [Display(Name = "订舱主单/体积", Description = "订舱主单/体积")]
        public decimal? Volume_DC { get; set; }

        [Display(Name = "订舱主单/计费重量", Description = "订舱主单/计费重量")]
        public decimal? Charge_Weight_DC { get; set; }

        [Display(Name = "订舱主单/泡重KG", Description = "订舱主单/泡重KG")]
        public decimal? Bulk_Weight_DC { get; set; }

        [Display(Name = "订舱主单/分泡比%", Description = "订舱主单/分泡比%")]
        public decimal? Bulk_Percent_DC { get; set; }

        [Display(Name = "订舱主单/结算重量", Description = "订舱主单/结算重量")]
        public decimal? Account_Weight_DC { get; set; }

        [Display(Name = "进仓实际/件数", Description = "进仓实际/件数")]
        public decimal? Pieces_Fact { get; set; }

        [Display(Name = "进仓实际/毛重", Description = "进仓实际/毛重")]
        public decimal? Weight_Fact { get; set; }

        [Display(Name = "进仓实际/体积", Description = "进仓实际/体积")]
        public decimal? Volume_Fact { get; set; }

        //[Display(Name = "进仓实际/SLAC", Description = "进仓实际/SLAC")]
        //public decimal? Slac_DC { get; set; }

        [Display(Name = "进仓实际/计费重量", Description = "进仓实际/计费重量")]
        public decimal? Charge_Weight_Fact { get; set; }

        [Display(Name = "进仓实际/泡重KG", Description = "进仓实际/泡重KG")]
        public decimal? Bulk_Weight_Fact { get; set; }

        [Display(Name = "进仓实际/分泡比%", Description = "进仓实际/分泡比%")]
        public decimal? Bulk_Percent_Fact { get; set; }

        [Display(Name = "进仓实际/结算重量", Description = "进仓实际/结算重量")]
        public decimal? Account_Weight_Fact { get; set; }

        [Display(Name = "分单唛头", Description = "分单唛头")]
        [MaxLength(1000)]
        public string Marks_H { get; set; }

        [Display(Name = "分单英文货名", Description = "分单英文货名")]
        [MaxLength(500)]
        public string EN_Name_H { get; set; }

        [Display(Name = "KSF", Description = "KSF")]
        public bool? Is_Book_Flat { get; set; }

        [Display(Name = "订舱方", Description = "订舱方")]
        [MaxLength(20)]
        public string Book_Flat_Code { get; set; }

        [Display(Name = "航空公司", Description = "航空公司")]
        [MaxLength(20)]
        public string Airways_Code { get; set; }

        [Display(Name = "航班号/EDT", Description = "航班号/EDT")]
        [MaxLength(20)]
        public string Flight_No { get; set; }

        [Index("IX_OPMEttInforFgtDate", IsUnique = false, Order = 1)]
        [Display(Name = "航班日期", Description = "航班日期")]
        public DateTime? Flight_Date_Want { get; set; }

        [Display(Name = "订舱要求", Description = "订舱要求")]
        [MaxLength(500)]
        public string Book_Remark { get; set; }

        [Display(Name = "交货地点", Description = "交货地点")]
        [MaxLength(20)]
        public string Delivery_Point { get; set; }

        [Display(Name = "仓库", Description = "仓库")]
        [MaxLength(20)]
        public string Warehouse_Code { get; set; }

        [Display(Name = "口岸入库日期", Description = "口岸入库日期")]
        [ScaffoldColumn(true)]
        public DateTime? RK_Date { get; set; }

        [Display(Name = "口岸出库日期", Description = "口岸出库日期")]
        public DateTime? CK_Date { get; set; }

        [Display(Name = "中文货名", Description = "中文货名")]
        [MaxLength(500)]
        public string CH_Name { get; set; }

        [Display(Name = "AMS", Description = "AMS")]
        public decimal? AMS { get; set; }

        [Display(Name = "拼箱码", Description = "拼箱码")]
        [MaxLength(20)]
        public string Lot_No { get; set; }

        [Display(Name = "批次号", Description = "批次号")]
        [MaxLength(20)]
        public string Batch_Num { get; set; }

        #endregion

        [Display(Name = "自营", Description = "自营")]
        public bool Is_Self { get; set; }

        [Display(Name = "接单类别", Description = "接单类别")]
        [MaxLength(20)]
        public string Ty_Type { get; set; }

        [Display(Name = "分担运费", Description = "分担运费")]
        [MaxLength(20)]
        public string Hbl_Feight { get; set; }

        [Display(Name = "消磁", Description = "消磁")]
        public bool Is_XC { get; set; }

        [Display(Name = "BSA", Description = "BSA")]
        public bool Is_BAS { get; set; }

        [Display(Name = "代操作", Description = "代操作")]
        public bool Is_DCZ { get; set; }

        [Display(Name = "重板", Description = "重板")]
        public bool Is_ZB { get; set; }

        [Display(Name = "退关", Description = "退关")]
        public bool Is_TG { get; set; }

        [Display(Name = "创建点", Description = "创建点")]
        public int ADDPoint { get; set; }

        [Display(Name = "修改点", Description = "修改点")]
        public int EDITPoint { get; set; }

        #region 20180913 add

        [Display(Name = "货到齐", Description = "货到齐")]
        public bool? Is_HDQ { get; set; }

        [Display(Name = "是否报关", Description = "是否报关")]
        public bool? Is_BG { get; set; }

        [Display(Name = "打印标签", Description = "是否打印标签")]
        public bool? Is_BQ { get; set; }

        [Display(Name = "是否出库", Description = "是否出库")]
        public bool? Is_OutGoing { get; set; }

        #endregion

        #region 销售 2019-03-14新增

        [Display(Name = "销售人", Description = "")]
        [Required]
        [MaxLength(50)]
        [Index("IX_OpsEntt_SallerName", IsUnique = false)]
        public string SallerName { get; set; }

        [Display(Name = "销售", Description = "")]
        [Index("IX_OpsEntt_SallerId", IsUnique = false)]
        public int? SallerId { get; set; }

        [Display(Name = "销售", Description = "")]
        [ForeignKey("SallerId")]
        public virtual Saller OSaller { get; set; }

        #endregion

        #region 基本字段

        [Index("IX_OPSEntInfo_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(2000)]
        public string Remark { get; set; }

        [Index("IX_OPSEntInfo_OP", IsUnique = false, Order = 1)]
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

        #endregion

    }
}