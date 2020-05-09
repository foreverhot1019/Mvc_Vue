













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PARA_AirPortMetadata))]
    public partial class PARA_AirPort
    {
    }

    public partial class PARA_AirPortMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "口岸代码")]

		public string PortCode{ get; set; }

        [Display(Name = "中文名称")]

		public string PortName{ get; set; }

        [Display(Name = "英文名称")]

		public string PortNameEng{ get; set; }

        [Display(Name = "口岸性质")]

		public string PortType{ get; set; }

        [Display(Name = "口岸描述")]
        public string Description { get; set; }

        [Display(Name = "国家代码")]

		public string CountryCode{ get; set; }

        [Display(Name = "使用状态")]

		public bool Status{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

	}

	public class PARA_AirPortChangeViewModel
    {
        public IEnumerable<PARA_AirPort> inserted { get; set; }

        public IEnumerable<PARA_AirPort> deleted { get; set; }

        public IEnumerable<PARA_AirPort> updated { get; set; }
    }
}
