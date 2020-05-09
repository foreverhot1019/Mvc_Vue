using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(FeeTypeMetadata))]
    public partial class FeeType
    {
    }

    public partial class FeeTypeMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "费用项目代码")]

		public string FeeCode{ get; set; }

        [Display(Name = "费用项目名称")]

		public string FeeName{ get; set; }

        [Display(Name = "英文名称")]

		public string FeeEName{ get; set; }

        [Display(Name = "报检费用")]

		public string InspectionFee{ get; set; }

        [Display(Name = "报关费用")]

		public string CustomsFee{ get; set; }

        [Display(Name = "计算点时间")]

		public string MathDate{ get; set; }

        [Display(Name = "费用项目描述")]

		public string Description{ get; set; }

        [Display(Name = "使用状态")]

        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "操作点")]

		public int OperatingPoint{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

	}

	public class FeeTypeChangeViewModel
    {
        public IEnumerable<FeeType> inserted { get; set; }

        public IEnumerable<FeeType> deleted { get; set; }

        public IEnumerable<FeeType> updated { get; set; }
    }
}
