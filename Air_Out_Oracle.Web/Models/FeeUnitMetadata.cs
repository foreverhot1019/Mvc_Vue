using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(FeeUnitMetadata))]
    public partial class FeeUnit
    {
    }

    public partial class FeeUnitMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "计费单位名称")]

		public string FeeUnitName{ get; set; }

        [Display(Name = "备注")]

		public string Remark{ get; set; }

        [Display(Name = "描述")]

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

	public class FeeUnitChangeViewModel
    {
        public IEnumerable<FeeUnit> inserted { get; set; }

        public IEnumerable<FeeUnit> deleted { get; set; }

        public IEnumerable<FeeUnit> updated { get; set; }
    }
}
