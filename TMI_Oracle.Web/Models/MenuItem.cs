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
    //public DbSet<MenuItem> MenuItems { get; set; }
    //public Entity.DbSet<MenuItem> MenuItems { get; set; }
    public partial class MenuItem : Entity
    {
        public MenuItem()
        {
            SubMenus = new HashSet<MenuItem>();
        }

        [Key]
        [Vue_PagePropty(Editable = false, SearchShow = false, ListShow = true, FormShow = false)]
        public int Id { get; set; }

        [Display(Name = "菜单名称", Description = "菜单名称")]
        [StringLength(20), Required(ErrorMessage = "Please enter : 菜单名称"), Index("IX_MenuTitle", 1, IsUnique = false)]
        [Vue_PagePropty(Editable = false, SearchShow = true,SearchOrder = 1)]
        public string Title { get; set; }

        [Display(Name = "菜单描述", Description = "菜单描述")]
        [StringLength(100)]
        public string Description { get; set; }

        [Display(Name = "排序代码", Description = "菜单排序代码（0100开始）")]
        [StringLength(20), Required(ErrorMessage = "Please enter : 代码"), Index("IX_MenuCode", 1, IsUnique = true)]
        [Vue_PagePropty(SearchOrder = 2,SearchShow=true)]
        public string Code { get; set; }

        [Display(Name = "菜单Url", Description = "菜单Url")]
        [StringLength(100), Required(ErrorMessage = "Please enter : Url"), Index("IX_MenuUrl", 1, IsUnique = false)]
        [Vue_PagePropty(SearchOrder = 3, SearchShow = true)]
        public string Url { get; set; }

        [Display(Name = "是否启用", Description = "是否启用")]
        public bool IsEnabled { get; set; }

        [Display(Name = "控制器", Description = "控制器")]
        [MaxLength(50)]
        public string Controller { get; set; }

        [Display(Name = "页面", Description = "页面")]
        [MaxLength(100)]
        public string Action { get; set; }

        [Display(Name = "菜单图标", Description = "菜单图标")]
        [StringLength(50)]
        public string IconCls { get; set; }

        [Display(Name = "子菜单", Description = "子菜单")]
        public ICollection<MenuItem> SubMenus { get; set; }

        [Vue_PagePropty(SearchOrder = 4, SearchShow = true)]
        [Display(Name = "上级菜单", Description = "上级菜单")]
        public int? ParentId { get; set; }

        [Display(Name = "上级菜单", Description = "上级菜单")]
        [ForeignKey("ParentId")]
        public MenuItem Parent { get; set; }

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
}