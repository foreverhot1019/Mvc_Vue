













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PARA_TablesMetadata))]
    public partial class PARA_Tables
    {
    }

    public partial class PARA_TablesMetadata
    {

        [Required(ErrorMessage = "Please enter : ID")]

        [Display(Name = "ID")]

        public int ID { get; set; }


        [Display(Name = "海关参数表名")]

        public string PARA_tbName { get; set; }


        [Display(Name = "海关参数代码")]

        public string PARA_Code { get; set; }


        [Display(Name = "海关参数名称")]

        public string PARA_Name { get; set; }


        [Display(Name = "海关参数代码列名")]

        public string PARA_CodeColumn { get; set; }


        [Display(Name = "海关参数名称列名")]

        public string PARA_NameColumn { get; set; }

  

    }




	public class PARA_TablesChangeViewModel
    {
        public IEnumerable<PARA_Tables> inserted { get; set; }
        public IEnumerable<PARA_Tables> deleted { get; set; }
        public IEnumerable<PARA_Tables> updated { get; set; }
    }

}
