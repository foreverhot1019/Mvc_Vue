using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    //Please Register DbSet in DbContext.cs
    //public DbSet<RoleMenu> RoleMenus { get; set; }
    //public Entity.DbSet<RoleMenu> RoleMenus { get; set; }
    public partial class RoleMenu : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(20), Display(Name = "角色", Description = "角色", Order = 1)]
        [Index("IX_RoleMenu", 1, IsUnique = true)]
        public string RoleName { get; set; }

        [Required, StringLength(50), Display(Name = "角色Id", Description = "角色Id", Order = 2)]
        [Index("IX_RoleMenu", 2, IsUnique = true)]
        public string RoleId { get; set; }

        [Required, Display(Name = "菜单Id", Description = "菜单Id", Order = 3)]
        [Index("IX_RoleMenu", 3, IsUnique = true)]
        public int MenuId { get; set; }

        [ForeignKey("MenuId"), Display(Name = "菜单", Description = "菜单")]
        public MenuItem MenuItem { get; set; }

        [Required, Display(Name = "启用", Description = "启用", Order = 4)]
        public bool IsEnabled { get; set; }

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

    public class RoleMenuChangeViewModel
    {
        public IEnumerable<RoleMenu> inserted { get; set; }

        public IEnumerable<RoleMenu> deleted { get; set; }

        public IEnumerable<RoleMenu> updated { get; set; }
    }
}