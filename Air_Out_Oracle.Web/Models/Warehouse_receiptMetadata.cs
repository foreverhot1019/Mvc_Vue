













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(Warehouse_receiptMetadata))]
    public partial class Warehouse_receipt
    {
    }

    public partial class Warehouse_receiptMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="仓库编号",Description="仓库编号")]

        [MaxLength(20)]

        public string Warehouse_Id{ get; set; }

        [Display(Name="进仓编号",Description="进仓编号")]

        [MaxLength(20)]

        public string Entry_Id{ get; set; }

        [Display(Name="仓库",Description="仓库")]

        [MaxLength(20)]

        public string Warehouse_Code{ get; set; }

        [Display(Name="实际件数",Description="实际件数")]

        public decimal Pieces_CK{ get; set; }

        [Display(Name="实际毛重",Description="实际毛重")]

        public decimal Weight_CK{ get; set; }

        [Display(Name="实际体积",Description="实际体积")]

        public decimal Volume_CK{ get; set; }

        [Display(Name="包装",Description="包装")]
        public string Packing{ get; set; }

        [Display(Name = "计费重量", Description = "计费重量")]
        public decimal? CHARGE_WEIGHT_CK { get; set; }

        [Display(Name="泡重",Description="泡重")]

        public decimal Bulk_Weight_CK{ get; set; }

        [Display(Name="破损",Description="破损")]

        public bool Damaged_CK{ get; set; }

        [Display(Name = "破损件数", Description = "破损件数")]
        public decimal? Damaged_Num { get; set; }

        [Display(Name = "受潮", Description = "受潮")]
        public bool Dampness_CK { get; set; }

        [Display(Name = "受潮件数", Description = "受潮件数")]
        public decimal? Dampness_Num { get; set; }

        [Display(Name = "变形", Description = "变形")]
        public bool Deformation { get; set; }

        [Display(Name = "变形件数", Description = "变形件数")]
        public decimal? Deformation_Num { get; set; }

        [Display(Name = "随货文件", Description = "随货文件")]

        public bool Is_GF{ get; set; }

        [Display(Name="备注",Description="备注")]

        [MaxLength(500)]

        public string Closure_Remark{ get; set; }

        [Display(Name="一体化",Description="一体化")]

        public bool Is_QG{ get; set; }

        [Display(Name="仓库备注",Description="仓库备注")]

        [MaxLength(500)]

        public string Warehouse_Remark{ get; set; }

        [Display(Name="委托方",Description="委托方")]

        [MaxLength(50)]

        public string Consign_Code_CK{ get; set; }

        [Display(Name = "主单ID")]
        public int MBLId { get; set; }

        [Display(Name="总单号",Description="总单号")]

        [MaxLength(20)]

        public string MBL{ get; set; }

        [Display(Name = "业务编号", Description = "业务编号(借用‘分单号’)")]

        [MaxLength(20)]

        public string HBL{ get; set; }

        [Display(Name="航班日期",Description="航班日期")]

        public DateTime Flight_Date_Want{ get; set; }

        [Display(Name="航班号",Description="航班号")]

        [MaxLength(20)]

        public string FLIGHT_No{ get; set; }

        [Display(Name="目的港",Description="目的港")]

        [MaxLength(50)]

        public string End_Port{ get; set; }

        [Display(Name="入库日期",Description="入库日期")]

        public DateTime In_Date{ get; set; }

        [Display(Name = "入库时间", Description = "入库时间")]
        [MaxLength(10)]
        public string In_Time { get; set; }

        [Display(Name="出库日期",Description="出库日期")]

        public DateTime Out_Date{ get; set; }

        [Display(Name = "出库时间", Description = "出库时间")]
        [MaxLength(10)]
        public string Out_Time { get; set; }

        [Display(Name="中文品名",Description="中文品名")]

        [MaxLength(500)]

        public string CH_Name_CK{ get; set; }

        [Display(Name = "客户提回", Description = "客户提回")]

        public bool Is_CustomerReturn{ get; set; }

        [Display(Name = "我司提货", Description = "我司提货")]

        public bool Is_MyReturn{ get; set; }

        [Display(Name="卡车号",Description="卡车号")]

        [MaxLength(20)]

        public string Truck_Id{ get; set; }

        [Display(Name="送货司机",Description="送货司机")]

        [MaxLength(20)]

        public string Driver{ get; set; }

        [Display(Name="破损上传",Description="破损上传")]

        public bool Is_DamageUpload{ get; set; }

        [Display(Name="交货上传",Description="交货上传")]

        public bool Is_DeliveryUpload{ get; set; }

        [Display(Name="进仓上传",Description="进仓上传")]

        public bool Is_EntryUpload{ get; set; }

        [Display(Name = "是否绑定", Description = "是否绑定")]
        public bool Is_Binding { get; set; }

        [Display(Name="使用状态",Description="使用状态")]

        public int Status{ get; set; }

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

	public class Warehouse_receiptChangeViewModel
    {
        public IEnumerable<Warehouse_receipt> inserted { get; set; }

        public IEnumerable<Warehouse_receipt> deleted { get; set; }

        public IEnumerable<Warehouse_receipt> updated { get; set; }
    }
}
