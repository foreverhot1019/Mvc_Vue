using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //总单信息表
    public partial class OPS_M_Order : Entity
    {
        public OPS_M_Order()
        {
            OPS_H_Orders = new HashSet<OPS_H_Order>();
            OPS_EntrustmentInfors = new HashSet<OPS_EntrustmentInfor>();
            Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
        }

        #region 主单信息

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_OPSMOrdMBL", IsUnique = false, Order = 1)]
        [Display(Name = "总单号", Description = "总单号")]
        [MaxLength(20)]
        public string MBL { get; set; }

        [Display(Name = "航空公司", Description = "航空公司")]
        [MaxLength(20)]
        public string Airways_Code { get; set; }

        [Display(Name = "国外代理", Description = "国外代理")]
        [MaxLength(20)]
        public string FWD_Code { get; set; }

        [Display(Name = "主单发货人", Description = "主单发货人")]
        [MaxLength(2000)]
        public string Shipper_M { get; set; }

        [Display(Name = "主单收货人", Description = "主单收货人")]
        [MaxLength(2000)]
        public string Consignee_M { get; set; }

        [Display(Name = "主单通知人", Description = "主单通知人(同时收货人)")]
        [MaxLength(2000)]
        public string Notify_Part_M { get; set; }

        [Display(Name = "起运港", Description = "起运港")]
        [MaxLength(50)]
        public string Depart_Port { get; set; }

        [Display(Name = "目的港", Description = "目的港")]
        [MaxLength(50)]
        public string End_Port { get; set; }

        [Display(Name = "航班号", Description = "航班号")]
        [MaxLength(20)]
        public string Flight_No { get; set; }

        [Display(Name = "航班日期", Description = "航班日期")]
        public DateTime? Flight_Date_Want { get; set; }

        [Display(Name = "币种", Description = "币种")]
        [MaxLength(20)]
        public string Currency_M { get; set; }

        [Display(Name = "成交条款", Description = "成交条款")]
        [MaxLength(20)]
        public string Bragainon_Article_M { get; set; }

        [Display(Name = "付款方式", Description = "付款方式")]
        [MaxLength(20)]
        public string Pay_Mode_M { get; set; }

        [Display(Name = "运费P/C", Description = "运费P/C")]
        [MaxLength(1)]
        public string Carriage_M { get; set; }

        [Display(Name = "杂费P/C", Description = "杂费P/C")]
        [MaxLength(1)]
        public string Incidental_Expenses_M { get; set; }

        [Display(Name = "申明价值（运输）", Description = "申明价值（运输）")]
        [MaxLength(20)]
        public string Declare_Value_Trans_M { get; set; }

        [Display(Name = "申明价值（海关）", Description = "申明价值（海关）")]
        [MaxLength(20)]
        public string Declare_Value_Ciq_M { get; set; }

        [Display(Name = "保险额度", Description = "保险额度")]
        [MaxLength(20)]
        public string Risk_M { get; set; }

        [Display(Name = "主单唛头", Description = "主单唛头")]
        [MaxLength(1000)]
        public string Marks_M { get; set; }

        [Display(Name = "主单英文货名", Description = "主单英文货名")]
        [MaxLength(500)]
        public string EN_Name_M { get; set; }

        [Display(Name = "Handing Information", Description = "Handing Information")]
        [MaxLength(1000)]
        public string Hand_Info_M { get; set; }

        [Display(Name = "Signature of Shipper or his Agent", Description = "Signature of Shipper or his Agent")]
        [MaxLength(1000)]
        public string Signature_Agent_M { get; set; }

        [Display(Name = "Rate Class", Description = "Rate Class")]
        [MaxLength(1)]
        public string Rate_Class_M { get; set; }

        [Display(Name = "空运费单价", Description = "空运费单价")]
        public decimal? Air_Frae { get; set; }

        [Display(Name = "AWC", Description = "AWC")]
        public decimal? AWC { get; set; }

        [Display(Name = "件数", Description = "件数")]
        public decimal? Pieces_M { get; set; }

        [Display(Name = "毛重", Description = "毛重")]
        public decimal? Weight_M { get; set; }

        [Display(Name = "体积", Description = "体积")]
        public decimal? Volume_M { get; set; }

        [Display(Name = "计费重量", Description = "计费重量")]
        public decimal? Charge_Weight_M { get; set; }

        [Display(Name = "约价方案", Description = "约价方案")]
        [MaxLength(50)]
        public string Price_Article { get; set; }

        [Display(Name = "CCC", Description = "CCC")]
        [MaxLength(50)]
        public string CCC { get; set; }

        [Display(Name = "附档", Description = "附档")]
        [MaxLength(1000)]
        public string File_M { get; set; }

        [Index("IX_OPSMOrd_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Display(Name = "总单结算状态", Description = "总单结算状态")]
        public bool OPS_BMS_Status { get; set; }

        #endregion

        #region 需求修改后的新增字段

        [Display(Name = "分单明细", Description = "分单明细")]
        public virtual ICollection<OPS_H_Order> OPS_H_Orders { get; set; }

        [Display(Name = "委托类型明细", Description = "委托类型明细")]
        public virtual ICollection<OPS_EntrustmentInfor> OPS_EntrustmentInfors { get; set; }

        #endregion

        #region 20180913 add

        [Display(Name = "发送主单", Description = "发送主单")]
        public bool? SendOut_ZD { get; set; }

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

        //[Display(Name = "备注", Description = "备注")]
        //[MaxLength(500)]
        //public string Remark { get; set; }

        #region 基本字段

        [Index("IX_OPSMOrd_OP", IsUnique = false, Order = 1)]
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