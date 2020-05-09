
     
 
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using TMI.Web.Models;
using TMI.Web.Repositories;

namespace TMI.Web.Services
{
    public interface IRoleMenuService:IService<RoleMenu>
    {

                  IEnumerable<RoleMenu> GetByMenuId(int  menuid);

                  IEnumerable<RoleMenu> GetByRoleName(string roleName);
                  void Authorize(RoleMenusView[] list);

                  IEnumerable<MenuItem> RenderMenus(string[] roleNames);
      
 
	}
}