using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(BD_DEFDOC_LISTMetadata))]
    public partial class BD_DEFDOC_LIST
    {
    }

    public class BD_DEFDOC_LISTMetadata
    {
        [Display(Name = "枚举代码")]
        public BD_DEFDOC BD_DEFDOC { get; set; }

        [Required(ErrorMessage = "Please enter : ID")]
        [Display(Name = "行号")]
        public int ID { get; set; }

        [Display(Name = "枚举ID")]
        public int DOCID { get; set; }

        [Display(Name = "代码")]
        public string LISTCODE { get; set; }

        [Display(Name = "名称")]
        public string LISTNAME { get; set; }

        [Display(Name = "说明")]
        public string REMARK { get; set; }

        [Display(Name = "状态")]
        public string STATUS { get; set; }

        [Display(Name = "建立者")]
        public string ADDWHO { get; set; }

        [Display(Name = "建立日期")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "修改者")]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期")]
        public DateTime EDITTS { get; set; }

        [Display(Name = "关联类别")]
        public string R_CODE { get; set; }

        [Display(Name = "英文全称")]
        public string ENAME { get; set; }

    }

    public class BD_DEFDOC_LISTChangeViewModel
    {
        public IEnumerable<BD_DEFDOC_LIST> inserted { get; set; }
        public IEnumerable<BD_DEFDOC_LIST> deleted { get; set; }
        public IEnumerable<BD_DEFDOC_LIST> updated { get; set; }
    }
}