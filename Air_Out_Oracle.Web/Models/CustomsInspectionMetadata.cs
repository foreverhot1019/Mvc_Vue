using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(CustomsInspectionMetadata))]
    public partial class CustomsInspection
    {
    }

    public partial class CustomsInspectionMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="业务编号",Description="业务编号")]

        [MaxLength(50)]

        public string Operation_ID{ get; set; }

        [Display(Name="航班",Description="航班")]

        [MaxLength(50)]

        public string Flight_NO{ get; set; }

        [Display(Name="航班日期",Description="航班日期")]

        public DateTime Flight_Date_Want{ get; set; }

        [Display(Name="总单号",Description="总单号")]

        [MaxLength(20)]

        public string MBL{ get; set; }

        [Display(Name="委托方",Description="委托方")]

        [MaxLength(100)]

        public string Consign_Code_CK{ get; set; }

        [Display(Name="订舱方",Description="订舱方")]

        [MaxLength(50)]

        public string Book_Flat_Code{ get; set; }

        [Display(Name="报关方式",Description="报关方式")]

        [MaxLength(50)]

        public string Customs_Declaration{ get; set; }

        [Display(Name="票数",Description="票数")]

        public decimal Num_BG{ get; set; }

        [Display(Name="备注",Description="备注")]

        [MaxLength(500)]

        public string Remarks_BG{ get; set; }

        [Display(Name="报关方",Description="报关方")]

        [MaxLength(50)]

        public string Customs_Broker_BG{ get; set; }

        [Display(Name="报关日期",Description="报关日期")]

        public DateTime Customs_Date_BG{ get; set; }

        [Display(Name="托书件数",Description="托书件数")]

        public decimal Pieces_TS{ get; set; }

        [Display(Name="托书毛重",Description="托书毛重")]

        public decimal Weight_TS{ get; set; }

        [Display(Name="托书体积",Description="托书体积")]

        public decimal Volume_TS{ get; set; }

        [Display(Name="进仓件数",Description="进仓件数")]

        public decimal Pieces_Fact{ get; set; }

        [Display(Name="进仓毛重",Description="进仓毛重")]

        public decimal Weight_Fact{ get; set; }

        [Display(Name="进仓体积",Description="进仓体积")]

        public decimal Volume_Fact{ get; set; }

        [Display(Name="报关件数",Description="报关件数")]

        public decimal Pieces_BG{ get; set; }

        [Display(Name="报关毛重",Description="报关毛重")]

        public decimal Weight_BG{ get; set; }

        [Display(Name="报关体积",Description="报关体积")]

        public decimal Volume_BG{ get; set; }

        [Display(Name="海关查检",Description="海关查检")]

        public bool IS_Checked_BG{ get; set; }

        [Display(Name="海关查检日期",Description="海关查检日期")]

        public DateTime Check_QTY{ get; set; }

        [Display(Name="海关查检次数",Description="海关查检次数")]

        public decimal Check_Date{ get; set; }

        [Display(Name="附档文件上传",Description="附档文件上传")]

        [MaxLength(200)]

        public string Fileupload{ get; set; }

        [Display(Name="使用状态",Description="使用状态")]

        public AirOutEnumType.UseStatusIsOrNoEnum Status{ get; set; }

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

	public class CustomsInspectionChangeViewModel
    {
        public IEnumerable<CustomsInspection> inserted { get; set; }

        public IEnumerable<CustomsInspection> deleted { get; set; }

        public IEnumerable<CustomsInspection> updated { get; set; }
    }
}
