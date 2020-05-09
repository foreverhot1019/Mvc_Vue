













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PARA_AreaMetadata))]
    public partial class PARA_Area
    {
    }

    public partial class PARA_AreaMetadata
    {

		[Required(ErrorMessage = "Please enter:ID")]

		[Display(Name="ID")]

		public int ID{ get; set; }

        [Display(Name = "国内地区代码")]

		public string AreaCode{ get; set; }

        [Display(Name = "国内地区名称")]

		public string AreaName{ get; set; }

	}

	public class PARA_AreaChangeViewModel
    {
        public IEnumerable<PARA_Area> inserted { get; set; }

        public IEnumerable<PARA_Area> deleted { get; set; }

        public IEnumerable<PARA_Area> updated { get; set; }
    }
}
