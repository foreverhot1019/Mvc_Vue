using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //分单信息表
    public partial class OPS_H_Order : Entity
    {
        public OPS_H_Order()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
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

        [Display(Name = "业务编号", Description = "业务编号")]
        [MaxLength(20)]
        public string Operation_Id { get; set; }

        [Display(Name = "分单发货人", Description = "分单发货人")]
        [MaxLength(2000)]
        public string Shipper_H { get; set; }

        [Display(Name = "分单收货人", Description = "分单收货人")]
        [MaxLength(2000)]
        public string Consignee_H { get; set; }

        [Display(Name = "分单通知人", Description = "分单通知人(同时收货人)")]
        [MaxLength(2000)]
        public string Notify_Part_H { get; set; }

        [Display(Name = "币种", Description = "币种")]
        [MaxLength(20)]
        public string Currency_H { get; set; }

        [Display(Name = "成交条款", Description = "成交条款")]
        [MaxLength(20)]
        public string Bragainon_Article_H { get; set; }

        [Display(Name = "付款方式", Description = "付款方式")]
        [MaxLength(20)]
        public string Pay_Mode_H { get; set; }

        [Display(Name = "运费P/C", Description = "运费P/C")]
        [MaxLength(1)]
        public string Carriage_H { get; set; }

        [Display(Name = "杂费P/C", Description = "杂费P/C")]
        [MaxLength(1)]
        public string Incidental_Expenses_H { get; set; }

        [Display(Name = "申明价值（运输）", Description = "申明价值（运输）")]
        [MaxLength(20)]
        public string Declare_Value_Trans_H { get; set; }

        [Display(Name = "申明价值（海关）", Description = "申明价值（海关）")]
        [MaxLength(20)]
        public string Declare_Value_Ciq_H { get; set; }

        [Display(Name = "保险额度", Description = "保险额度")]
        [MaxLength(20)]
        public string Risk_H { get; set; }

        [Display(Name = "唛头", Description = "唛头")]
        [MaxLength(2000)]
        public string Marks_H { get; set; }

        [Display(Name = "英文货名", Description = "英文货名")]
        [MaxLength(2000)]
        public string EN_Name_H { get; set; }

        [Display(Name = "件数", Description = "件数")]
        public decimal? Pieces_H { get; set; }

        [Display(Name = "毛重", Description = "毛重")]
        public decimal? Weight_H { get; set; }

        [Display(Name = "体积", Description = "体积")]
        public decimal? Volume_H { get; set; }

        [Display(Name = "计费重量", Description = "计费重量")]
        public decimal? Charge_Weight_H { get; set; }

        #region

        [Display(Name = "总单号", Description = "总单号")]
        [MaxLength(20)]
        public string MBL { get; set; }

        [Display(Name = "分单号", Description = "分单号")]
        [MaxLength(20)]
        public string HBL { get; set; }

        [Display(Name = "自营", Description = "自营")]
        public bool Is_Self { get; set; }

        [Display(Name = "接单类别", Description = "接单类别")]
        [MaxLength(20)]
        public string Ty_Type { get; set; }

        [Display(Name = "拼箱码", Description = "拼箱码")]
        [MaxLength(20)]
        public string Lot_No { get; set; }

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

        [Index("IX_OPSHOrd_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        //[Display(Name = "备注", Description = "备注")]
        //[MaxLength(500)]
        //public string Remark { get; set; }

        [Display(Name = "批次号", Description = "批次号")]
        [MaxLength(20)]
        public string Batch_Num { get; set; }

        #endregion

        #region 20180913 add

        [Display(Name = "发送分单", Description = "发送分单")]
        public bool? SendOut_FD { get; set; }

        [Display(Name = "发送人", Description = "发送人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string SendOut_WHO { get; set; }

        [Display(Name = "发送时间", Description = "发送时间")]
        [ScaffoldColumn(false)]
        public DateTime? SendOut_TS { get; set; }

        [Display(Name = "发送人", Description = "发送人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string SendOut_ID { get; set; }

        #endregion 

        #region 基本字段

        [Index("IX_OPSHOrd_OP", IsUnique = false, Order = 1)]
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