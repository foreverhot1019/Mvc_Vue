
     
 
 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;
using AirOut.Web.Models;
using AirOut.Web.Repositories;

namespace AirOut.Web.Services
{
    public interface IRoleMenuService:IService<RoleMenu>
    {

                  IEnumerable<RoleMenu> GetByMenuId(int  menuid);

                  IEnumerable<RoleMenu> GetByRoleName(string roleName);
                  void Authorize(RoleMenusView[] list);

                  IEnumerable<MenuItem> RenderMenus(string[] roleNames);
      
 
	}
}