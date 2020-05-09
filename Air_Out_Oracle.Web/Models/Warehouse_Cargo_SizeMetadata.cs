













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(Warehouse_Cargo_SizeMetadata))]
    public partial class Warehouse_Cargo_Size
    {
    }

    public partial class Warehouse_Cargo_SizeMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="仓库接单Id",Description="仓库接单Id")]

        public int Warehouse_Receipt_Id{ get; set; }

        [Display(Name="进仓编号",Description="进仓编号")]

        [MaxLength(20)]

        public string Entry_Id{ get; set; }

        [Display(Name="长",Description="长")]

        public decimal CM_Length{ get; set; }

        [Display(Name="宽",Description="宽")]

        public decimal CM_Width{ get; set; }

        [Display(Name="高",Description="高")]

        public decimal CM_Height{ get; set; }

        [Display(Name="件",Description="件")]

        public decimal CM_Piece{ get; set; }

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

	public class Warehouse_Cargo_SizeChangeViewModel
    {
        public IEnumerable<Warehouse_Cargo_Size> inserted { get; set; }

        public IEnumerable<Warehouse_Cargo_Size> deleted { get; set; }

        public IEnumerable<Warehouse_Cargo_Size> updated { get; set; }
    }
}
