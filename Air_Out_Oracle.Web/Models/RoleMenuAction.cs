using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public partial class RoleMenuAction : Entity
    {
        public RoleMenuAction()
        {
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(50), Display(Name = "角色", Description = "角色")]
        [Required(ErrorMessage = "Please enter : 角色")]
        [Index("IX_RoleMenuAction", 1, IsUnique = true)]
        public string RoleId { get; set; }

        [Display(Name = "菜单", Description = "菜单")]
        [Required(ErrorMessage = "Please enter : 菜单")]
        [Index("IX_RoleMenuAction", 2, IsUnique = true)]
        public int? MenuId { get; set; }

        [Display(Name = "动作", Description = "动作")]
        [Required(ErrorMessage = "Please enter : 动作")]
        [Index("IX_RoleMenuAction", 3, IsUnique = true)]
        public int? ActionId { get; set; }

        //[Display(Name = "角色", Description = "角色")]
        //[ForeignKey("RoleId")]
        //public ApplicationRole ApplicationRole { get; set; }

        [Display(Name = "菜单", Description = "菜单")]
        [ForeignKey("MenuId")]
        public MenuItem MenuItem { get; set; }

        [Display(Name = "动作", Description = "动作")]
        [ForeignKey("ActionId")]
        public MenuAction MenuAction { get; set; }

        #region

        [ScaffoldColumn(false)]
        [Display(Name = "新增用户", Description = "新增用户")]
        [StringLength(50)]
        public string CreatedUserId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "新增时间", Description = "新增时间")]
        public DateTime? CreatedDateTime { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "最后修改用户", Description = "最后修改用户")]
        [StringLength(50)]
        public string LastEditUserId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "最后修改时间", Description = "最后修改时间")]
        public DateTime? LastEditDateTime { get; set; }

        #endregion
    }

	public class RoleMenuActionChangeViewModel
    {
        public IEnumerable<RoleMenuAction> inserted { get; set; }

        public IEnumerable<RoleMenuAction> deleted { get; set; }

        public IEnumerable<RoleMenuAction> updated { get; set; }
    }
}