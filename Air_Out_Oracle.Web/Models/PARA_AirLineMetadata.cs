













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PARA_AirLineMetadata))]
    public partial class PARA_AirLine
    {
    }

    public partial class PARA_AirLineMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "航班代码")]

		public string AirCode{ get; set; }

        [Display(Name = "航线")]

		public string AirLine{ get; set; }

        [Display(Name = "航空公司")]

		public string AirCompany{ get; set; }

        [Display(Name = "起始地")]

		public string StarStation{ get; set; }

        [Display(Name = "中转地")]

		public string TransferStation{ get; set; }

        [Display(Name = "目的地")]

		public string EndStation{ get; set; }

        [Display(Name = "航期")]

		public DateTime AirDate{ get; set; }

        [Display(Name = "航班时间")]
        public string AirTime { get; set; }

        [Display(Name = "描述")]

		public string Description{ get; set; }

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

	public class PARA_AirLineChangeViewModel
    {
        public IEnumerable<PARA_AirLine> inserted { get; set; }

        public IEnumerable<PARA_AirLine> deleted { get; set; }

        public IEnumerable<PARA_AirLine> updated { get; set; }
    }
}
