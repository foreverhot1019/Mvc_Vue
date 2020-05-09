using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(Bms_Bill_Ap_DtlMetadata))]
    public partial class Bms_Bill_Ap_Dtl
    {
    }

    public partial class Bms_Bill_Ap_DtlMetadata
    {
        [Display(Name="应付帐单",Description="应付帐单")]
        public Bms_Bill_Ap OBms_Bill_Ap{ get; set; }

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name="Id",Description="Id")]
        public int Id{ get; set; }

        [Display(Name="应付帐单Id",Description="应付帐单Id")]
        public int? Bms_Bill_Ap_Id { get; set; }

        [Display(Name="业务编号",Description="业务编号")]
        [MaxLength(50)]
        public string Dzbh{ get; set; }

        [Display(Name="序号",Description="序号")]
        public int Line_No{ get; set; }

        [Display(Name="明细序号",Description="明细序号")]
        public int Line_Id{ get; set; }

        [Display(Name="费用代码",Description="费用代码")]
        [MaxLength(50)]
        public string Charge_Code{ get; set; }

        [Display(Name="费用名称",Description="费用名称")]
        [MaxLength(50)]
        public string Charge_Desc{ get; set; }

        [Display(Name="理论单价",Description="理论单价")]
        public decimal Unitprice{ get; set; }

        [Display(Name="实际单价",Description="实际单价")]
        public decimal Unitprice2{ get; set; }

        [Display(Name="数量",Description="数量")]
        public decimal Qty{ get; set; }

        [Display(Name="理论金额",Description="理论金额")]
        public decimal Account{ get; set; }

        [Display(Name="实际金额",Description="实际金额")]
        public decimal Account2{ get; set; }

        [Display(Name="币种",Description="币种")]
        [MaxLength(50)]
        public string Money_Code{ get; set; }

        [Display(Name="摘要",Description="摘要")]
        [MaxLength(50)]
        public string Summary{ get; set; }

        [Display(Name="使用状态",Description="使用状态")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name="操作点",Description="操作点")]
        public int OperatingPoint{ get; set; }

        [Display(Name="新增人",Description="新增人")]
        [MaxLength(50)]
        public string ADDID{ get; set; }

        [Display(Name="新增人",Description="新增人")]
        [MaxLength(20)]
        public string ADDWHO{ get; set; }

        [Display(Name="新增时间",Description="新增时间")]
        public DateTime? ADDTS { get; set; }

        [Display(Name="修改人",Description="修改人")]
        [MaxLength(20)]
        public string EDITWHO{ get; set; }

        [Display(Name="修改时间",Description="修改时间")]
        public DateTime? EDITTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]
        [MaxLength(50)]
        public string EDITID{ get; set; }

	}

	public class Bms_Bill_Ap_DtlChangeViewModel
    {
        public IEnumerable<Bms_Bill_Ap_Dtl> inserted { get; set; }

        public IEnumerable<Bms_Bill_Ap_Dtl> deleted { get; set; }

        public IEnumerable<Bms_Bill_Ap_Dtl> updated { get; set; }
    }
}
