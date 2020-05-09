













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(PARA_CountryMetadata))]
    public partial class PARA_Country
    {
    }

    public partial class PARA_CountryMetadata
    {

		[Required(ErrorMessage = "Please enter:COUNTRY_CO")]

        [Display(Name = "国别（地区）代码")]

		public string COUNTRY_CO{ get; set; }

        [Display(Name = "国别英文简称")]

		public string COUNTRY_EN{ get; set; }

        [Display(Name = "国别（地区）名称")]

		public string COUNTRY_NA{ get; set; }

		[Display(Name="EXAM_MARK")]

		public string EXAM_MARK{ get; set; }

		[Display(Name="HIGH_LOW")]

		public string HIGH_LOW{ get; set; }

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

	public class PARA_CountryChangeViewModel
    {
        public IEnumerable<PARA_Country> inserted { get; set; }

        public IEnumerable<PARA_Country> deleted { get; set; }

        public IEnumerable<PARA_Country> updated { get; set; }
    }
}
