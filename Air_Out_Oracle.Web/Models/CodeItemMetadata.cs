using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(CodeItemMetadata))]
    public partial class CodeItem
    {
    }

    public partial class CodeItemMetadata
    {
        [Display(Name = "基础代码")]
        public BaseCode BaseCode { get; set; }

        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter : 值")]
        [Display(Name = "值")]
        [MaxLength(30)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Please enter : 显示")]
        [Display(Name = "显示")]
        [MaxLength(50)]
        public string Text { get; set; }

        [Display(Name = "描述")]
        [MaxLength(50)]
        public string Description { get; set; }

        [Display(Name = "基础代码")]
        public int BaseCodeId { get; set; }

    }




	public class CodeItemChangeViewModel
    {
        public IEnumerable<CodeItem> inserted { get; set; }
        public IEnumerable<CodeItem> deleted { get; set; }
        public IEnumerable<CodeItem> updated { get; set; }
    }

}
