













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PARA_CustomsMetadata))]
    public partial class PARA_Customs
    {
    }

    public partial class PARA_CustomsMetadata
    {

		[Required(ErrorMessage = "Please enter:Customs_Code")]

		[Display(Name="Customs_Code")]

		public string Customs_Code{ get; set; }

		[Display(Name="Customs_Name")]

		public string Customs_Name{ get; set; }

		[Display(Name="PinYinSimpleName")]

		public string PinYinSimpleName{ get; set; }

		[Display(Name="Description")]

		public string Description{ get; set; }

		[Display(Name="Status")]

		public bool Status{ get; set; }

		[Display(Name="ADDWHO")]

		public string ADDWHO{ get; set; }

		[Display(Name="ADDTS")]

		public DateTime ADDTS{ get; set; }

		[Display(Name="EDITWHO")]

		public string EDITWHO{ get; set; }

		[Display(Name="EDITTS")]

		public DateTime EDITTS{ get; set; }

	}

	public class PARA_CustomsChangeViewModel
    {
        public IEnumerable<PARA_Customs> inserted { get; set; }

        public IEnumerable<PARA_Customs> deleted { get; set; }

        public IEnumerable<PARA_Customs> updated { get; set; }
    }
}
