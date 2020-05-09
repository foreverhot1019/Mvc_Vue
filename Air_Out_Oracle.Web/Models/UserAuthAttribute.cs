using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirOut.Web.Extensions;
using System.Web.Routing;

namespace AirOut.Web.Models
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        //1. 当Controller和Action都没有被赋予权限的时候，则默认所有人都可以进入。
        //2. 当Controller有权限而Action没有的时候，所有的Action都取决于其Controller的权限。
        //3. 当Controller没有有权限而Action有的时候，取决于Action的权限。
        //4. 当Controller和Action都有权限的时候，取决于Action的权限。

        /// <summary>
        /// 自定义属性
        /// </summary>
        public string UserActions { get; set; }

        //代码顺序为：OnAuthorization-->AuthorizeCore-->HandleUnauthorizedRequest 
        //如果AuthorizeCore返回false时，才会走HandleUnauthorizedRequest 方法，并且Request.StausCode会返回401，401错误又对应了Web.config中的
        //<authentication mode="Forms">
        //<forms loginUrl="~/" timeout="2880" />
        //</authentication>
        //所有，AuthorizeCore==false 时，会跳转到 web.config 中定义的  loginUrl="~/"

        //在OnAuthorization验证是否有权限
        private bool isAllowed = true;

        /// <summary>
        /// 在过程请求授权时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文，它封装用于 System.Web.Mvc.AuthorizeAttribute 的信息。</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //获取 当前请求 Controller中的 Action 设置的 可访问 权限名称
            string RoleNames = string.IsNullOrEmpty(this.Roles) ? base.Users : this.Roles;
            //获取 当前请求 Controller中的 Action 设置的 可访问 用户名称
            string UserNames = string.IsNullOrEmpty(this.Users) ? base.Users : this.Users;
            //获取当前 请求的 Controller名称
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //获取当前 请求的 Controller中的Action名称
            string actionName = filterContext.ActionDescriptor.ActionName;
            //获取当前 请求的 Action类型 是 Get 还是 POST
            string RequestType = filterContext.HttpContext.Request.RequestType;
            //判断请求是否是Ajax请求
            bool IsAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();

            Microsoft.Owin.Security.IAuthenticationManager IAuthenManager = filterContext.HttpContext.GetOwinContext().Authentication;
            
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            #region 验证 filterContext是否有效

            if (filterContext==null || filterContext.HttpContext == null)
            {
                var MsgStr = "未登录，无法访问：" + controllerName;
                RedictUrl("Home", "Index", IsAjaxRequest, filterContext, MsgStr);
                return;
            }
            else
            {
                var MsgStr = "未登录，无法访问：" + controllerName;
                var OwinContext = filterContext.HttpContext.GetOwinContext();
                if (OwinContext == null)
                {
                    RedictUrl("Home", "Index", IsAjaxRequest, filterContext, MsgStr);
                    return;
                }
            }

            #endregion

            var OHttpContext = HttpContext.Current;

            //ajax请求无需验证权限----------------------------------------------------------------------------------------
            if (IsAjaxRequest)
            {
                if (OHttpContext.Session["LoginUser"] != null)
                    return;
            }

            //登陆用户
            ApplicationUser AppUser = null;
            //缓存操作点
            List<OperatePoint> ArrOperatePoints = new List<OperatePoint>();
            //缓存用户操作点和关联表
            List<UserOperatePointLink> ArrUserOperatePointLink = new List<UserOperatePointLink>();
            //缓存菜单
            List<MenuItem> ArrMenuItem = new List<MenuItem>();
            //缓存菜单动作
            List<MenuAction> ArrMenuActions = new List<MenuAction>();
            //用户操作点
            List<OperatePoint> ArrUserOperatePoints = new List<OperatePoint>();
            //用户角色
            List<ApplicationRole> ArrAppUserRoles = new List<ApplicationRole>();
            //角色菜单动作
            List<RoleMenuAction> ArrUserRoleMenuActions = new List<RoleMenuAction>();

            if (IAuthenManager.User != null)
            {
                if (IAuthenManager.User.Identity.IsAuthenticated)
                {
                    #region 一个 controll对应 多个controll的权限

                    Dictionary<string, List<string>> dictOnetoManyControllerName = new Dictionary<string, List<string>>();
                    lock (Common.lockCachedictOnetoManyControllerName)
                    {
                        dictOnetoManyControllerName = CacheHelper.Get_SetOnetoManyCntrlerName() as Dictionary<string, List<string>>;
                    }

                    #endregion

                    if (OHttpContext.Session["LoginUser"] != null)
                        AppUser = (ApplicationUser)OHttpContext.Session["LoginUser"];
                    IEnumerable<ApplicationUser> Users;
                    if (AppUser == null)
                    {
                        List<ApplicationUser> ArrUsers = (List<ApplicationUser>)CacheHelper.Get_SetCache(Common.CacheNameS.ApplicationUser);
                        Users = ArrUsers.Where(x => x.UserName == IAuthenManager.User.Identity.Name).ToList();
                        if (!Users.Any())
                        {
                            ApplicationDbContext AppDb = new ApplicationDbContext();
                            Users = AppDb.Users.Where(x => x.UserName == IAuthenManager.User.Identity.Name).ToList();
                        }
                        if (Users.Any())
                        {
                            AppUser = Users.FirstOrDefault();
                            OHttpContext.Session["LoginUser"] = AppUser;
                        }
                    }
                    else
                    {
                        Users = new List<ApplicationUser> { AppUser };
                    }
                    //ajax请求无需验证权限----------------------------------------------------------------------------------------
                    if (IsAjaxRequest)
                    {
                        return;
                    }

                    if (Users.Any())
                    {
                        ApplicationDbContext AppDbContext = new ApplicationDbContext();
                        WebdbContext dbContext = new WebdbContext();

                        #region 验证用户是否选择了操作点

                        #region 缓存操作点

                        //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                        lock (Common.lockCacheOperatePoints)
                        {
                            ArrOperatePoints = CacheHelper.Get_SetCache(Common.CacheNameS.OperatePoint) as List<OperatePoint>;

                            ArrUserOperatePointLink = (List<UserOperatePointLink>)CacheHelper.Get_SetCache(Common.CacheNameS.UserOperatePointLink);
                        }

                        #endregion

                        if (AppUser.UserName.ToLower() == "admin" || ArrAppUserRoles.Any(x => x.Name.Contains("超级管理员")))
                        {
                            if (string.IsNullOrEmpty(AppUser.UserOperatPoint))
                            {
                                //OHttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] = ArrOperatePoints;
                                OHttpContext.Session["LoginUserOperatePoint_s"] = ArrOperatePoints;
                            }
                            else
                            {
                                if (OHttpContext.Session["LoginUserOperatePoint"] == null)//&& HttpContext.Current.Request.Cookies[AppUser.Id] == null
                                {
                                    int ID = Convert.ToInt16(AppUser.UserOperatPoint);
                                    var WhereArrOperatePoints = ArrOperatePoints.Where(x => x.ID == ID).ToList();
                                    if (WhereArrOperatePoints.Any())
                                    {
                                        OHttpContext.Session["LoginUserOperatePoint"] = WhereArrOperatePoints;
                                        string OperatePointStr = Newtonsoft.Json.JsonConvert.SerializeObject(WhereArrOperatePoints); //WhereArrOperatePoints.FirstOrDefault().ID.ToString();
                                        //HttpContext.Request.Cookies[UserId].Value = OperatePointStr;
                                        System.Web.HttpCookie newcookie = new HttpCookie(AppUser.Id);//"CURRENT-OPSITE-" + AppUser.UserName);
                                        newcookie.Value = OperatePointStr;
                                        newcookie.Expires = DateTime.Now.AddDays(1);
                                        HttpContext.Current.Response.Cookies.Add(newcookie);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(AppUser.UserOperatPoint))
                            {
                                if (OHttpContext.Session["LoginUserOperatePoint"] == null || HttpContext.Current.Request.Cookies[AppUser.Id] == null)//"CURRENT-OPSITE-" + AppUser.UserName] == null)
                                {
                                    var IQUserOptPont = from p in ArrOperatePoints
                                                        from x in ArrUserOperatePointLink
                                                        where x.UserId == AppUser.Id &&
                                                        x.OperateOpintId == p.ID
                                                        select p;
                                    ArrUserOperatePoints = IQUserOptPont.ToList();
                                    if (ArrUserOperatePoints.Any())
                                    {
                                        OHttpContext.Session["LoginUserOperatePoint_s"] = ArrUserOperatePoints;
                                        //只有 一个操作点时无需选择
                                        if (ArrUserOperatePoints.Count == 1)
                                        {
                                            var WhereArrOperatePoints = ArrUserOperatePoints.Take(1).ToList();
                                            OHttpContext.Session["LoginUserOperatePoint"] = WhereArrOperatePoints;
                                            string OperatePointStr = WhereArrOperatePoints.FirstOrDefault().ID.ToString();//Newtonsoft.Json.JsonConvert.SerializeObject(WhereArrOperatePoints);
                                            //HttpContext.Request.Cookies[UserId].Value = OperatePointStr;
                                            System.Web.HttpCookie newcookie = new HttpCookie(AppUser.Id);//"CURRENT-OPSITE-" + AppUser.UserName);
                                            newcookie.Value = OperatePointStr;
                                            newcookie.Expires = DateTime.Now.AddDays(1);
                                            HttpContext.Current.Response.Cookies.Add(newcookie);
                                        }
                                    }
                                }
                                else
                                {
                                    if (OHttpContext.Session["LoginUserOperatePoint"] != null)
                                    {
                                        //int ID = Convert.ToInt16(AppUser.UserOperatPoint);
                                        var WhereArrOperatePoints = OHttpContext.Session["LoginUserOperatePoint"] as List<OperatePoint>;
                                        if (WhereArrOperatePoints.Any())
                                        {
                                            string OperatePointStr = WhereArrOperatePoints.FirstOrDefault().ID.ToString();//Newtonsoft.Json.JsonConvert.SerializeObject(WhereArrOperatePoints);
                                            //HttpContext.Request.Cookies[UserId].Value = OperatePointStr;
                                            System.Web.HttpCookie newcookie = new HttpCookie(AppUser.Id);//"CURRENT-OPSITE-" + AppUser.UserName);
                                            newcookie.Value = OperatePointStr;
                                            newcookie.Expires = DateTime.Now.AddDays(1);
                                            HttpContext.Current.Response.Cookies.Add(newcookie);
                                        }
                                    }
                                    else if (HttpContext.Current.Request.Cookies[AppUser.Id] == null)//"CURRENT-OPSITE-" + AppUser.UserName] != null)
                                    {
                                        var CookieVal = HttpContext.Current.Request.Cookies[AppUser.Id].Value;//"CURRENT-OPSITE-" + AppUser.UserName].Value;
                                        int CookieInt = 0;
                                        int.TryParse(CookieVal, out  CookieInt);
                                        var WhereArrOperatePoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OperatePoint>>(CookieVal); //ArrOperatePoints.Where(x => x.ID == CookieInt).ToList();
                                        if (WhereArrOperatePoints.Any())
                                        {
                                            OHttpContext.Session["LoginUserOperatePoint"] = WhereArrOperatePoints;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (OHttpContext.Session["LoginUserOperatePoint"] == null && HttpContext.Current.Request.Cookies[AppUser.Id] == null)//"CURRENT-OPSITE-" + AppUser.UserName] == null)
                                {
                                    int ID = Convert.ToInt32(AppUser.UserOperatPoint);

                                    var WhereArrOperatePoints = ArrOperatePoints.Where(x => x.ID == ID).ToList();
                                    if (WhereArrOperatePoints.Any())
                                    {
                                        OHttpContext.Session["LoginUserOperatePoint"] = WhereArrOperatePoints;
                                        string OperatePointStr = Newtonsoft.Json.JsonConvert.SerializeObject(WhereArrOperatePoints);//WhereArrOperatePoints.FirstOrDefault().ID.ToString();
                                        //HttpContext.Request.Cookies[UserId].Value = OperatePointStr;
                                        System.Web.HttpCookie newcookie = new HttpCookie(AppUser.Id);//"CURRENT-OPSITE-" + AppUser.UserName);
                                        newcookie.Value = OperatePointStr;
                                        newcookie.Expires = DateTime.Now.AddDays(1);
                                        HttpContext.Current.Response.Cookies.Add(newcookie);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 获取用户权限和 显示的菜单

                        if (OHttpContext.Session["LoginUserRoles"] == null)
                        {
                            //var IdentityAppUserRoles = ArrIdentityRole.Where(x => x.Users.Any(t => t.UserId == AppUser.Id)).ToList();
                            var IdentityAppUserRoles = AppDbContext.Roles.Where(x => x.Users.Any(t => t.UserId == AppUser.Id)).ToList();
                            if (IdentityAppUserRoles.Any())
                            {
                                foreach (var item in IdentityAppUserRoles)
                                {
                                    ApplicationRole AppRole = new ApplicationRole();
                                    AppRole.Id = item.Id;
                                    AppRole.Name = item.Name;
                                    ArrAppUserRoles.Add(AppRole);
                                }
                            }
                            OHttpContext.Session["LoginUserRoles"] = ArrAppUserRoles;
                        }
                        else
                        {
                            ArrAppUserRoles = OHttpContext.Session["LoginUserRoles"] as List<ApplicationRole>;
                            if (!ArrAppUserRoles.Any())
                            {
                                //var IdentityAppUserRoles = ArrIdentityRole.Where(x => x.Users.Any(t => t.UserId == AppUser.Id)).ToList();
                                var IdentityAppUserRoles = AppDbContext.Roles.Where(x => x.Users.Any(t => t.UserId == AppUser.Id)).ToList();
                                if (IdentityAppUserRoles.Any())
                                {
                                    foreach (var item in IdentityAppUserRoles)
                                    {
                                        ApplicationRole AppRole = new ApplicationRole();
                                        AppRole.Id = item.Id;
                                        AppRole.Name = item.Name;
                                        ArrAppUserRoles.Add(AppRole);
                                    }
                                }
                                OHttpContext.Session["LoginUserRoles"] = ArrAppUserRoles;
                            }
                        }

                        #region 用户角色菜单动作

                        //List<RoleMenuAction> UserRoleMenuActions = new List<RoleMenuAction>();
                        if (ArrAppUserRoles.Any())
                        {
                            if (OHttpContext.Session["LoginUserRoleMenuActions"] == null)
                            {
                                var AppUserRoleIds = ArrAppUserRoles.Select(n => n.Id).ToList();
                                ArrUserRoleMenuActions = dbContext.RoleMenuAction.AsQueryable().Where(x => AppUserRoleIds.Contains(x.RoleId)).ToList();
                                OHttpContext.Session["LoginUserRoleMenuActions"] = ArrUserRoleMenuActions;
                            }
                            else
                            {
                                ArrUserRoleMenuActions = OHttpContext.Session["LoginUserRoleMenuActions"] as List<RoleMenuAction>;
                                if (!ArrUserRoleMenuActions.Any())
                                {
                                    var AppUserRoleIds = ArrAppUserRoles.Select(n => n.Id).ToList();
                                    ArrUserRoleMenuActions = dbContext.RoleMenuAction.AsQueryable().Where(x => AppUserRoleIds.Contains(x.RoleId)).ToList();
                                    OHttpContext.Session["LoginUserRoleMenuActions"] = ArrUserRoleMenuActions;
                                }
                            }
                        }

                        #endregion

                        #region 缓存菜单

                        //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                        lock (Common.lockCacheMenuItem)
                        {
                            ArrMenuItem = CacheHelper.Get_SetCache(Common.CacheNameS.MenuItem) as List<MenuItem>;
                        }

                        #endregion

                        #region 缓存菜单动作

                        //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                        lock (Common.lockCacheMenuAction)
                        {
                            ArrMenuActions = CacheHelper.Get_SetCache(Common.CacheNameS.MenuAction) as List<MenuAction>;
                        }

                        #endregion

                        if (OHttpContext.Session["Login_MenuItem"] == null)
                        {
                            List<MenuItem> ListMenuItem = ArrMenuItem;// OHttpContext.Cache.Get(Common.GeCacheEnumByName("MenuItem").ToString()) as List<MenuItem>;
                            ListMenuItem = ListMenuItem.ToList();//Where(x => x.IsEnabled == true)
                            var ListRoleNames = ArrAppUserRoles.Select(x => x.Name).ToList();
                            if (ListRoleNames.Any())
                            {
                                if (!string.IsNullOrEmpty(AppUser.UserName))
                                {
                                    if (AppUser.UserName.ToLower() == "admin" || ListRoleNames.Contains("超级管理员"))
                                    {
                                        OHttpContext.Session["Login_MenuItem"] = ListMenuItem;
                                    }
                                    else
                                    {
                                        var MenuId_s = dbContext.RoleMenu.Where(x => ListRoleNames.Contains(x.RoleName)).Select(x => x.MenuId);
                                        var OutPutMenuItem = ListMenuItem.Where(x => MenuId_s.Contains(x.Id)).OrderBy(x => x.Code).ToList();

                                        #region 判断 Parent=null 或 0 的 Menu是否都加载

                                        var ParentMenuId = from p in OutPutMenuItem
                                                           where p.ParentId != null && p.ParentId > 0
                                                           group p by p.ParentId into g
                                                           select g.Key;
                                        var ParentMenu = OutPutMenuItem.Where(p => p.ParentId == null || p.ParentId == 0).ToList();
                                        if (ParentMenuId.Count() != ParentMenu.Count())
                                        {
                                            var MenuId_slinq = dbContext.RoleMenu.AsNoTracking().Where(x => ListRoleNames.Contains(x.RoleName)).Select(x => x.MenuId);
                                            var Menu_slinq = dbContext.MenuItem.AsNoTracking().Where(x => MenuId_slinq.Contains(x.Id) && x.ParentId != null && x.ParentId > 0).Select(x => x.ParentId);
                                            var NotInParentMenu = dbContext.MenuItem.AsNoTracking().Where(p => (p.ParentId == null || p.ParentId == 0) &&
                                                Menu_slinq.Contains(p.Id)).OrderBy(x => x.Code).ToList();
                                            foreach (var item in NotInParentMenu)
                                            {
                                                if (!ParentMenu.Select(x => x.Id).Contains(item.Id))
                                                {
                                                    OutPutMenuItem.Add(item);
                                                }
                                            }
                                            OutPutMenuItem = OutPutMenuItem.OrderBy(x => x.Code).ToList();
                                        }
                                        OHttpContext.Session["Login_MenuItem"] = OutPutMenuItem;

                                        #endregion
                                    }
                                }
                                else
                                    OHttpContext.Session["Login_MenuItem"] = new List<MenuItem>();// ListMenuItem;
                            }
                            else
                            {
                                if (AppUser.UserName.ToLower() == "admin" || ListRoleNames.Contains("超级管理员"))
                                {
                                    OHttpContext.Session["Login_MenuItem"] = ListMenuItem;
                                }
                            }
                        }
                        #endregion

                        //没有操作点 退出
                        if (AirOut.Web.Controllers.Utility.CurrentUserOperatePoint == null || !AirOut.Web.Controllers.Utility.CurrentUserOperatePoint.Any())
                        {
                            IAuthenManager.SignOut();
                            //Common.ExitReLogin();
                            ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                        }
                    }
                    else
                    {
                        IAuthenManager.SignOut();
                        //Common.ExitReLogin();
                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                    }

                    #region 验证 是否有权限

                    //超级管理员 不需要验证 权限
                    if (Users.Any(x => x.UserName.ToLower() == "admin") || ArrAppUserRoles.Any(x => x.Name == "超级管理员"))
                        return;

                    #region 无需验证权限的页面配置

                    Dictionary<string, string> NoQxValid = new Dictionary<string, string>();
                    lock (Common.lockCacheNoQxValid)
                    {
                        NoQxValid = CacheHelper.Get_SetCache(Common.CacheNameS.NoQxValid) as Dictionary<string, string>;
                    }

                    #endregion

                    //无需验证权限的页面
                    if (NoQxValid.Any(x => x.Key == actionName.ToLower() && x.Value == controllerName.ToLower()))
                        return;

                    if (HttpContext.Current.Session["Login_MenuItem"] != null)
                    {
                        List<MenuItem> ViewMenu = HttpContext.Current.Session["Login_MenuItem"] as List<MenuItem>;
                        var WhereDictOnetoManyControllerName = dictOnetoManyControllerName.Where(x => x.Value.Any(n => n.ToLower() == controllerName.ToLower()));
                        List<MenuItem> MinSubMenus = new List<MenuItem>();
                        if (WhereDictOnetoManyControllerName.Any())
                        {
                            var Keys = WhereDictOnetoManyControllerName.Select(x => x.Key.ToLower());
                            MinSubMenus = ViewMenu.Where(x => !x.SubMenus.Any() && Keys.Contains(x.Controller.ToLower())).ToList();
                        }
                        else
                            MinSubMenus = ViewMenu.Where(x => !x.SubMenus.Any() && x.Url.ToLower().StartsWith("/" + controllerName.ToLower())).ToList();
                        var ss = ViewMenu.Select(x => x.Url).ToList();
                        if (MinSubMenus.Any())
                        {
                            IEnumerable<MenuAction> WhereAction;
                            List<int?> ActionIds = new List<int?>();
                            switch (actionName.ToLower())
                            {
                                #region

                                case "create":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "create");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                case "edit":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "edit");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                case "index":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "view");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                case "delt":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "delete");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                case "delete":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "delete");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                case "export":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "export");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                case "import":
                                    WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == actionName.ToLower());
                                    if (!WhereAction.Any())
                                        WhereAction = ArrMenuActions.Where(x => x.Code.ToLower() == "import");
                                    if (WhereAction.Any())
                                        ActionIds = WhereAction.Select(x => (int?)x.Id).ToList();

                                    if (!ArrUserRoleMenuActions.Any(x => ArrAppUserRoles.Select(n => n.Id).Contains(x.RoleId) &&
                                        MinSubMenus.Select(t => (int?)t.Id).Contains(x.MenuId) && ActionIds.Contains(x.ActionId)))
                                    {
                                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                                    }
                                    break;
                                default:
                                    break;

                                #endregion
                            }
                        }
                        else
                        {
                            ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                        }
                    }
                    else
                    {
                        ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext);
                    }

                    #endregion
                }
                else
                {
                    var MsgStr = "未登录，无法访问：" + controllerName;
                    ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext, MsgStr);
                }
            }
            else
            {
                var MsgStr = "未登录，无法访问：" + controllerName;
                ReturnToHome(controllerName, actionName, IsAjaxRequest, ArrAppUserRoles, filterContext, MsgStr);
            }
        }

        /// <summary>
        /// 确定是否获得访问核心框架的授权。
        /// </summary>
        /// <param name="httpContext">HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>
        /// <returns>如果获得访问授权，则为 true；否则为 false。</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!isAllowed)
            {
                httpContext.Response.StatusCode = 401;//无权限状态码  
            }

            return isAllowed;
        }

        /// <summary>
        /// 处理授权失败的 HTTP 请求。
        /// </summary>
        /// <param name="filterContext">封装用于 System.Web.Mvc.AuthorizeAttribute 的信息。filterContext 对象包括控制器、HTTP 上下文、请求上下文、操作结果和路由数据。</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 401)
            {
                filterContext.Result = new RedirectResult("/");
            }

            //int i = filterContext.HttpContext.Response.StatusCode;//false返回200
            //// filterContext.Result = new ViewResult { ViewName = View };
            //if (filterContext.HttpContext.Response.StatusCode == 403)
            //{
            //    filterContext.Result = new EmptyResult();//此方法导致整个页面空白
            //    filterContext.HttpContext.Response.Write("<Script lanuage='javascript'>alert('您没有此操作权限')</Script>");
            //    //filterContext.HttpContext.Response.End();//此方法导致整个页面空白
            //}
        }

        /// <summary>
        /// 没有权限返回到Home页
        /// </summary>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="ArrAppUserRole">用户权限</param>
        /// <param name="filterContext"></param>
        protected void ReturnToHome(string controllerName, string actionName, bool IsAjaxRequest, List<ApplicationRole> ArrAppUserRole, AuthorizationContext filterContext, string MsgStr = "")
        {
            RedictUrl(controllerName, actionName, IsAjaxRequest, filterContext, MsgStr);
        }

        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="filterContext"></param>
        private void RedictUrl(string controllerName, string actionName, bool IsAjaxRequest, AuthorizationContext filterContext,string MsgStr ="")
        {
            if (string.IsNullOrWhiteSpace(MsgStr))
                MsgStr = "您没有权限访问：" + controllerName;

            if (IsAjaxRequest)
            {
                //filterContext.HttpContext.Response.Clear();
                //filterContext.HttpContext.Response.Write("{\"Success\":\"false\",\"ErrMsg\":\"" + "您没有权限" + controllerName + "\"}");
                //filterContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.Write("{\"Success\":false,\"success\":false,\"ErrMsg\":\"" + MsgStr + "\"}");
                filterContext.HttpContext.Response.End();
            }
            else
            {
                if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.User == null || filterContext.HttpContext.User.Identity == null || !filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    if (controllerName.ToLower() != "account" && actionName.ToLower() != "login")
                    {
                        string returnUrlStr = HttpContext.Current.Response.RedirectLocation == null ? "/Home/Index" : HttpContext.Current.Response.RedirectLocation;
                        //filterContext.Result = new EmptyResult();
                        //HttpContext.Current.Response.RedirectToRoute("Default", new { controller = "Account", action = "Login", returnUrl = returnUrlStr });

                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Account", action = "Login", returnUrl = returnUrlStr })
                        );
                    }
                }
                else
                {
                    string returnUrlStr = HttpContext.Current.Response.RedirectLocation == null ? "/Home/Index" : HttpContext.Current.Response.RedirectLocation;
                    //filterContext.Result = new EmptyResult();
                    //HttpContext.Current.Response.RedirectToRoute("Default", new { controller = "Home", action = "NoQXErr", returnUrl = returnUrlStr, ViewMsg = "您没有权限访问：" + controllerName });

                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "NoQXErr", returnUrl = returnUrlStr, ViewMsg = MsgStr })
                    );
                }
            }
        }
    }
}