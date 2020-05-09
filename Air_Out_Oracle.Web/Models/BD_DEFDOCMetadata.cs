using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(BD_DEFDOCMetadata))]
    public partial class BD_DEFDOC
    {
    }


    public partial class BD_DEFDOCMetadata
    {
        [Display(Name = "枚举信息")]
        public BD_DEFDOC_LIST BD_DEFDOC_LIST { get; set; }

        [Required(ErrorMessage = "Please enter : ID")]
        [Display(Name = "主键")]
        public int ID { get; set; }

        [Display(Name = "枚举代码")]
        public string DOCCODE { get; set; }

        [Display(Name = "枚举名称")]
        public string DOCNAME { get; set; }

        [Display(Name = "说明")]
        public string REMARK { get; set; }

        [Display(Name = "建立者")]
        public string ADDWHO { get; set; }

        [Display(Name = "建立日期")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "修改者")]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期")]
        public DateTime EDITTS { get; set; }

    }

    public class BD_DEFDOCChangeViewModel
    {
        public IEnumerable<BD_DEFDOC> inserted { get; set; }
        public IEnumerable<BD_DEFDOC> deleted { get; set; }
        public IEnumerable<BD_DEFDOC> updated { get; set; }
    }
}