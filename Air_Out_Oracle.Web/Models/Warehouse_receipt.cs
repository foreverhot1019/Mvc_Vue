using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    /// <summary>
    /// 仓库接单主表
    /// </summary>
    public partial class Warehouse_receipt :Entity
    {
        public Warehouse_receipt()
        {
            ArrWarehouse_Cargo_Size = new List<Warehouse_Cargo_Size>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "仓库编号", Description = "仓库编号")]
        [Required]
        [MaxLength(20)]
        public string Warehouse_Id { get; set; }

        [Display(Name = "进仓编号", Description = "进仓编号")]
        [Required]
        [MaxLength(20)]
        public string Entry_Id { get; set; }

        [Display(Name = "仓库", Description = "仓库")]
        [MaxLength(20)]
        public string Warehouse_Code { get; set; }

        [Display(Name = "实际件数", Description = "实际件数")]
        public decimal? Pieces_CK { get; set; }

        [Display(Name = "实际毛重", Description = "实际毛重")]
        public decimal? Weight_CK { get; set; }

        [Display(Name = "实际体积", Description = "实际体积")]
        public decimal? Volume_CK { get; set; }

        [Display(Name = "包装", Description = "包装")]
        [MaxLength(20)]
        public string Packing { get; set; }

        [Display(Name = "计费重量", Description = "计费重量")]
        public decimal? CHARGE_WEIGHT_CK { get; set; }

        [Display(Name = "泡重", Description = "泡重")]
        public decimal? Bulk_Weight_CK { get; set; }

        [Display(Name = "破损", Description = "破损")]
        public bool Damaged_CK { get; set; }

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

        [Display(Name = "关封", Description = "关封")]
        public bool Is_GF { get; set; }

        [Display(Name = "关封备注", Description = "关封备注")]
        [MaxLength(500)]
        public string Closure_Remark { get; set; }

        [Display(Name = "清关", Description = "清关")]
        public bool Is_QG { get; set; }

        [Display(Name = "仓库备注", Description = "仓库备注")]
        [MaxLength(500)]
        public string Warehouse_Remark { get; set; }

        [Display(Name = "委托方", Description = "委托方")]
        [MaxLength(50)]
        public string Consign_Code_CK { get; set; }

        [Display(Name = "主单ID", Description = "主单ID")]
        public int? MBLId { get; set; }

        [Display(Name = "总单号", Description = "总单号")]
        [MaxLength(20)]
        public string MBL { get; set; }

        [Display(Name = "业务编号", Description = "业务编号(借用‘分单号’)")]
        [MaxLength(20)]
        public string HBL { get; set; }

        [Display(Name = "航班日期", Description = "航班日期")]
        public DateTime? Flight_Date_Want { get; set; }

        [Display(Name = "航班号", Description = "航班号")]
        [MaxLength(20)]
        public string FLIGHT_No { get; set; }

        [Display(Name = "目的港", Description = "目的港")]
        [MaxLength(50)]
        public string End_Port { get; set; }

        [Display(Name = "入库日期", Description = "入库日期")]
        [ScaffoldColumn(true)]
        public DateTime? In_Date { get; set; }

        [Display(Name = "入库时间", Description = "入库时间")]
        [MaxLength(10)]
        public string In_Time { get; set; }

        [Display(Name = "出库日期", Description = "出库日期")]
        public DateTime? Out_Date { get; set; }

        [Display(Name = "出库时间", Description = "出库时间")]
        [MaxLength(10)]
        public string Out_Time { get; set; }

        [Display(Name = "中文品名", Description = "中文品名")]
        [MaxLength(500)]
        public string CH_Name_CK { get; set; }

        [Display(Name = "客户提回", Description = "客户提回")]
        public bool Is_CustomerReturn { get; set; }

        [Display(Name = "我司提货", Description = "我司提货")]
        public bool Is_MyReturn { get; set; }

        [Display(Name = "卡车号", Description = "卡车号")]
        [MaxLength(20)]
        public string Truck_Id { get; set; }

        [Display(Name = "送货司机", Description = "送货司机")]
        [MaxLength(20)]
        public string Driver { get; set; }

        [Display(Name = "破损上传", Description = "破损上传")]
        public bool Is_DamageUpload { get; set; }

        [Display(Name = "交货上传", Description = "交货上传")]
        public bool Is_DeliveryUpload { get; set; }

        [Display(Name = "进仓上传", Description = "进仓上传")]
        public bool Is_EntryUpload { get; set; }

        [Display(Name = "是否绑定", Description = "是否绑定")]
        public bool Is_Binding { get; set; }

        [Display(Name = "明细", Description = "明细")]
        public virtual List<Warehouse_Cargo_Size> ArrWarehouse_Cargo_Size { get; set; }

        #region 基本字段

        [Index("IX_WH_Recpt_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [Index("IX_WH_Recpt_OP", IsUnique = false, Order = 1)]
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