using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(MenuActionMetadata))]
    public partial class MenuAction
    {
    }

    public partial class MenuActionMetadata
    {
        [Required(ErrorMessage = "Please enter : 主键")]
        [Display(Name = "主键")]
        public int Id { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "代码")]
        public string Code { get; set; }

        [Display(Name = "排序")]
        public string Sort { get; set; }

        [Display(Name = "显示")]
        public bool IsEnabled { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "创建人")]
        public string CreatedUserId { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedDateTime { get; set; }

        [Display(Name = "编辑人")]
        public string LastEditUserId { get; set; }

        [Display(Name = "编辑时间")]
        public DateTime LastEditDateTime { get; set; }

    }

    public class MenuActionChangeViewModel
    {
        public IEnumerable<MenuAction> inserted { get; set; }

        public IEnumerable<MenuAction> deleted { get; set; }

        public IEnumerable<MenuAction> updated { get; set; }
    }

}
