using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public class OrderDetailProductViewModel
    {
        [Display(Name = "表头ID")]
        public int OID { get; set; }

        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "关联主键")]
        [StringLength(50)]
        public string RELATIONID { get; set; }

        [Display(Name = "被合并标记")]
        [StringLength(50)]
        public string MERGEFLAG { get; set; }

        [Display(Name = "关务流水号")]
        public string CustomsflowNo { get; set; }

        [Display(Name = "预关务流水号")]
        public string YucustomsflowNo { get; set; }

        [Display(Name = "商品序号")]
        public decimal? GoodsNum { get; set; }

        [Display(Name = "项号")]
        public string ItemNo { get; set; }

        [Display(Name = "征免")]
        public string Exemption { get; set; }

        [Display(Name = "商品编码")]
        public string GoodsCode { get; set; }

        [Display(Name = "海关监管条件")]
        [StringLength(50)]
        public string CustomsSuperCond { get; set; }

        [Display(Name = "商检监管条件")]
        [StringLength(50)]
        public string MerchantSuperCond { get; set; }

        [Display(Name = "品名")]
        public string ProductName { get; set; }

        [Display(Name = "规格型号")]
        public string SpecifModel { get; set; }

        [Display(Name = "申报数量")]
        public decimal? ReportNum { get; set; }

        [Display(Name = "币制")]
        public string Currency { get; set; }

        [Display(Name = "申报单位")]
        public string ReportUnit { get; set; }

        //[Display(Name = "申报单位名称")]
        //public string ReportUnitNAME { get; set; }

        [Display(Name = "单价")]
        public decimal? PriceUnit { get; set; }

        [Display(Name = "总价")]
        public decimal? Total { get; set; }

        [Display(Name = "原产国")]
        public string MaterialCountry { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string MaterialCountryNAME { get; set; }

        [Display(Name = "最终目的国（地区）")]
        public string DEST_Country { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string DEST_CountryNAME { get; set; }

        [Display(Name = "第一数量")]
        public decimal? FirstNum { get; set; }

        [Display(Name = "第一单位")]
        public string FirstUnit { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string FirstUnitNAME { get; set; }

        [Display(Name = "第二数量")]
        public decimal? SecondNum { get; set; }

        [Display(Name = "第二单位")]
        public string SecondUnit { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string SecondUnitNAME { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string SpecialRelaConf { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string PriceEffectConf { get; set; }

        //[Display(Name = "DDDDDDDDDDDDDD")]
        //public string UseFeeConf { get; set; }

        [Display(Name = "一二线标记")]
        public string Lineflag { get; set; }

        [Display(Name = "订单明细Ids")]
        public string DetailIds { get; set; }


    }
}