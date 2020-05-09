using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(PARA_CURRMetadata))]
    public partial class PARA_CURR
    {
    }

    public partial class PARA_CURRMetadata
    {

		[Required(ErrorMessage = "Please enter:ID")]

		[Display(Name="ID")]

		public int ID{ get; set; }

        [Display(Name = "币制代码")]

		public string CURR_CODE{ get; set; }

        [Display(Name = "币制名称")]

		public string CURR_Name{ get; set; }

        [Display(Name = "币制英文简称")]

		public string CURR_NameEng{ get; set; }

        [Display(Name = "货币代码")]

		public string Money_CODE{ get; set; }

        [Display(Name = "描述")]

		public string Description{ get; set; }

        [Display(Name = "使用状态")]

		public int Status{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

	}

	public class PARA_CURRChangeViewModel
    {
        public IEnumerable<PARA_CURR> inserted { get; set; }

        public IEnumerable<PARA_CURR> deleted { get; set; }

        public IEnumerable<PARA_CURR> updated { get; set; }
    }
}
