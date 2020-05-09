using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TMI.Web.Models;
using Repository.Pattern.UnitOfWork;
using TMI.Web.Extensions;
using System.Collections.Generic;
using TMI.Web.Services;

namespace TMI.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //数据库实例
        WebdbContext dbContext = new WebdbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IOperatePointService _opService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public AccountController()
        {
        }

        public AccountController(IUnitOfWorkAsync unitOfWork, IOperatePointService opService)
        {
            //ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager,ApplicationRoleManager roleManager,
            //UserManager = userManager;
            //SignInManager = signInManager;
            //RoleManager = roleManager;
            _unitOfWork = unitOfWork;
            this._opService = opService;
        }

        /// <summary>
        /// 用户管理
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// 登录管理
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// 角色管理
        /// </summary>
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            //return RedirectToAction("Login?returnUrl=" + returnUrl);
            return RedirectToLocal("/Account/Login?returnUrl=" + returnUrl);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //清除已登录的信息
            //HttpContext.Application.Clear();
            //HttpContext.Session.Clear();
            //HttpContext.Session.Abandon();
            //AuthenticationManager.SignOut();

            //ViewBag.OpList = this._opService.Query().Select(x => new { Id = x.ID, Name = x.OperatePointName }).ToList();
            ViewBag.OpList = ((List<OperatePoint>)CacheHelper.Get_SetCache(Common.CacheNameS.OperatePoint)).
                Select(x => new
                {
                    Id = x.ID,
                    Name = x.OperatePointName
                }).ToList();
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        //根据选择的
        [HttpPost]
        [AllowAnonymous]
        public ActionResult changeOp(string userName)
        {
            //根据用户名找出对应的用户信息
            ApplicationUser AppUser = UserManager.FindByName(userName.Trim());
            if (AppUser != null)
            {
                //操作点数据
                IEnumerable<OperatePoint> OperatePointsQuery = (List<OperatePoint>)CacheHelper.Get_SetCache(Common.CacheNameS.OperatePoint);
                //用户角色
                var ArrApplicationRole = RoleManager.Roles.Where(x => x.Users.Any(t => t.UserId == AppUser.Id)).ToList();

                //判断是否特殊身份（超级管理员，mvp管理员）,是则返回全部操作点
                //if (uaerRoles.Contains("67a61842-2293-4f91-a613-d117241a9270") || uaerRoles.Contains("f00dd34d-9e40-4a14-a9e9-31c956461af7"))//超级管理员 或者 MVP管理员
                if (userName.ToLower() == "admin" || ArrApplicationRole.Any(x => x.Name.EndsWith("管理员")))
                {
                    var return_data = OperatePointsQuery.AsQueryable().Select(x => new { x.ID, x.OperatePointName }).Distinct().ToList();
                    var pagelist = new { success = true, rows = return_data };
                    return Json(pagelist, JsonRequestBehavior.AllowGet);
                }
                string userId = AppUser.Id;
                IEnumerable<UserOperatePointLink> UserOperatePointLinkQuery = (List<UserOperatePointLink>)CacheHelper.Get_SetCache(Common.CacheNameS.UserOperatePointLink);

                var last_data = UserOperatePointLinkQuery.Where(x => x.UserId == userId).Select(x => x.OperateOpintId).ToList();
                var lastData = OperatePointsQuery.Where(x => last_data.Contains(x.ID)).Select(x => new { x.ID, x.OperatePointName }).Distinct().ToList();

                var pagelist1 = new { success = true, rows = lastData };
                return Json(pagelist1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var pagelist = new { success = false };
                return Json(pagelist, JsonRequestBehavior.AllowGet);
            }
        }

        //插入基本数据
        public void InitData()
        {
            ApplicationRole[] ArrApplicationRole = new ApplicationRole[]{
                new ApplicationRole { Id=Guid.NewGuid().ToString(), Name="管理员" },
                new ApplicationRole { Id=Guid.NewGuid().ToString(), Name = "普通用户" }
            };
            foreach (var itemRole in ArrApplicationRole)
                RoleManager.Create(itemRole);
            

            var PswHashStr = "AO3NFrIjahNzm/cSbKQ49qkrue4mKj0dHp2onLObgloIUM4LAdgv4wiYe/bqPjlzlg==";
            ApplicationUser[] ArrAppUser = new ApplicationUser[]{
              //new ApplicationUser { Id=Guid.NewGuid().ToString(), UserName = "admin", Email = "admin@feiliks.com", UserNameDesc = "超级管理员", PasswordHash = PswHashStr },
              new ApplicationUser { Id=Guid.NewGuid().ToString(), UserName = "User1", Email = "User1@feiliks.cn", UserNameDesc = "用户1", PasswordHash =  PswHashStr},
              new ApplicationUser { Id=Guid.NewGuid().ToString(), UserName = "User2", Email = "User2@feiliks.cn", UserNameDesc = "用户2", PasswordHash = PswHashStr }
            };
            foreach (var itemUser in ArrAppUser) {
                itemUser.PasswordHash = UserManager.PasswordHasher.HashPassword($"{itemUser.UserName}123");
                UserManager.Create(itemUser);
            }

            UserManager.AddToRole(ArrAppUser[0].Id, ArrApplicationRole[0].Name);
            UserManager.AddToRole(ArrAppUser[1].Id, ArrApplicationRole[1].Name);

            //-------------------------------------------------------------------------------------------------

            Notification[] ArrNotification = new Notification[] { 
                new Notification{ //Id=1, 
                    Name = "Sys", Description="系统", Sender="Sys@feili.com", Receiver="Sys@feili.com", Disabled = false},
                new Notification{ //Id=2, 
                    Name = "WebService", Description="服务", Sender="WebService@feili.com", Receiver="WebService@feili.com", Disabled = false}
            };
            _unitOfWork.Repository<Notification>().InsertRange(ArrNotification);

            OperatePoint[] ArrOperatePoint = new OperatePoint[] { 
                new OperatePoint{ //ID=1, 
                    OperatePointCode="Op_Code1", OperatePointName="操作点1",IsEnabled=true},
                new OperatePoint{ //ID=2, 
                    OperatePointCode="Op_Code2", OperatePointName="操作点2",IsEnabled=true}
            };
            _unitOfWork.Repository<OperatePoint>().InsertRange(ArrOperatePoint);
            _unitOfWork.SaveChanges();

            OperatePointList[] ArrOperatePointList = new OperatePointList[] { 
                new OperatePointList{ //ID=1, 
                    OperatePointID=ArrOperatePoint[0].ID, OperatePointCode=ArrOperatePoint[0].OperatePointCode, CompanyCode="CP_Code1", CompanyName="公司1", IsEnabled=true},
                new OperatePointList{ //ID=2, 
                    OperatePointID=ArrOperatePoint[0].ID, OperatePointCode=ArrOperatePoint[0].OperatePointCode, CompanyCode="CP_Code2", CompanyName="公司2", IsEnabled=true},
                new OperatePointList{ //ID=3, 
                    OperatePointID=ArrOperatePoint[0].ID, OperatePointCode=ArrOperatePoint[0].OperatePointCode, CompanyCode="CP_Code3", CompanyName="公司3", IsEnabled=true},
                new OperatePointList{ //ID=4, 
                    OperatePointID=ArrOperatePoint[1].ID, OperatePointCode=ArrOperatePoint[0].OperatePointCode, CompanyCode="CP_Code4", CompanyName="公司4", IsEnabled=true},
                new OperatePointList{ //ID=5, 
                    OperatePointID=ArrOperatePoint[1].ID, OperatePointCode=ArrOperatePoint[0].OperatePointCode, CompanyCode="CP_Code5", CompanyName="公司5", IsEnabled=true},
                new OperatePointList{ //ID=6, 
                    OperatePointID=ArrOperatePoint[1].ID, OperatePointCode=ArrOperatePoint[0].OperatePointCode, CompanyCode="CP_Code6", CompanyName="公司6", IsEnabled=true}
            };
            _unitOfWork.Repository<OperatePointList>().InsertRange(ArrOperatePointList);
            _unitOfWork.SaveChanges();

            MenuAction[] ArrMenuAction = new MenuAction[] { 
                new MenuAction{ //Id=1, 
                    Name="创建", Code="Create",Sort="1",IsEnabled=true},
                new MenuAction{ //Id=2, 
                    Name="编辑", Code="Edit",Sort="2",IsEnabled=true},
                new MenuAction{ //Id=3, 
                    Name="删除", Code="Delete",Sort="3",IsEnabled=true},
                new MenuAction{ //Id=4, 
                    Name="查看", Code="View",Sort="4",IsEnabled=true},
                new MenuAction{ //Id=5, 
                    Name="导入", Code="Import",Sort="5",IsEnabled=true},
                new MenuAction{ //Id=6, 
                    Name="导出", Code="Export",Sort="6",IsEnabled=true},
                new MenuAction{ //Id=7, 
                    Name="审批", Code="Audit",Sort="7",IsEnabled=true}
            };
            _unitOfWork.Repository<MenuAction>().InsertRange(ArrMenuAction);

            MenuItem PMenuItem = new MenuItem { //Id = 1, 
                Title = "系统管理", Code = "000100", Url = "#", IsEnabled = true, Controller = "#", Action = "#", IconCls = "fa fa-gear", ParentId = null };
            _unitOfWork.Repository<MenuItem>().Insert(PMenuItem);
            _unitOfWork.SaveChanges();

            MenuItem[] ArrMenuItem = new MenuItem[]{
                new MenuItem{//Id=2, 
                    Title="用户管理", Code="000101", Url="/AccountManage/Index",IsEnabled=true, Controller="AccountManage", Action="Index", IconCls="fa fa-fw fa-user", ParentId=PMenuItem.Id},
                new MenuItem{//Id=3, 
                    Title="授权管理", Code="000102", Url="/Management/Index",IsEnabled=true, Controller="Management", Action="Index", IconCls="fa fa-users", ParentId=PMenuItem.Id},
                new MenuItem{//Id=4, 
                    Title="菜单维护", Code="000103", Url="/MenuItems/Index",IsEnabled=true, Controller="MenuItems", Action="Index", IconCls="fa fa-bookmark", ParentId=PMenuItem.Id},
                new MenuItem{//Id=5, 
                    Title="菜单授权", Code="000106", Url="/RoleMenus/Index",IsEnabled=true, Controller="RoleMenus", Action="Index", IconCls="fa fa-sitemap", ParentId=PMenuItem.Id},
                new MenuItem{//Id=6, 
                    Title="基础代码管理", Code="000105", Url="/CodeItems/Index",IsEnabled=true, Controller="CodeItems", Action="Index", IconCls="fa fa-file-code-o", ParentId=PMenuItem.Id},
                new MenuItem{//Id=7, 
                    Title="Excel导入配置", Code="000107", Url="/DataTableImportMappings/Index",IsEnabled=true, Controller="DataTableImportMappings", Action="Index", IconCls="fa fa-file-excel-o", ParentId=PMenuItem.Id},
                new MenuItem{//Id=8, 
                    Title="消息通知&提醒", Code="000108", Url="/Notifications/Index",IsEnabled=true, Controller="Notifications", Action="Index", IconCls="fa fa-bell-o", ParentId=PMenuItem.Id},
                new MenuItem{//Id=9, 
                    Title="菜单动作", Code="000104", Url="/MenuActions/Index",IsEnabled=true, Controller="MenuActions", Action="Index", IconCls="fa fa-anchor", ParentId=PMenuItem.Id},
                new MenuItem{//Id=10, 
                    Title="操作点管理", Code="000109", Url="/OperatePoints/Index",IsEnabled=true, Controller="OperatePoints", Action="Index", IconCls="fa fa-circle", ParentId=PMenuItem.Id}
            };
            _unitOfWork.Repository<MenuItem>().InsertRange(ArrMenuItem);
            _unitOfWork.SaveChanges();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [CustomerValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ApplicationDbContext AppDbContext = new ApplicationDbContext();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //清除已登录的信息
            //HttpContext.Application.Clear();
            //HttpContext.Session.Clear();
            //HttpContext.Session.Abandon();
            //SignInManager.Dispose();

            //根据用户名获取 用户
            var OAppUser = UserManager.FindByNameAsync(model.UserName.Trim());//AppDbContext.Users.Where(x => x.UserName == model.UserName.Trim()).FirstOrDefault();

            ApplicationUser AppUser = await OAppUser;
            if (AppUser == null)
            {
                ModelState.AddModelError("", "登录失败,用户名或密码不正确");
                return View(model);
            }
            string LoginName = AppUser == null ? "" : AppUser.UserName;
            string userId = AppUser == null ? Guid.NewGuid().ToString() : AppUser.Id;
            int opId = Convert.ToInt32(model.OP);
            //List<ApplicationRole> AppRoles = RoleManager.Roles.Where(x => AppUser.Roles.Select(n => n.RoleId).Contains(x.Id)).ToList();

            string PasswordStr = AppUser == null ? "" : AppUser.PasswordHash;
            //明文密码加密成 HashPassword
            string Password_Str = SignInManager.UserManager.PasswordHasher.HashPassword(model.Password);
            //密码 加密后 string 与 明文密码 比对 是否一致
            PasswordVerificationResult nmm = SignInManager.UserManager.PasswordHasher.VerifyHashedPassword(PasswordStr, model.Password);
            //SignInManager.UserManager.MaxFailedAccessAttemptsBeforeLockout = 1;
            //SignInManager.UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(1);

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var ArrUserOperatPoint = dbContext.UserOperatePointLink.Where(x => x.UserId == userId && x.OperateOpintId == opId).ToList();

            //判断是否有操作点权限
            //if (!ArrUserOperatPoint.Any())
            //{
            //    if (LoginName != "admin")
            //    {
            //        ViewBag.OpList = this._opService.Query().Select(x => new { Id = x.ID, Name = x.OperatePointName }).ToList();
            //        ModelState.AddModelError("", "登录失败,该用户没有操作点权限");
            //        return View(model);
            //    }
            //}
            var result = SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

            switch (result.Result)
            {
                case SignInStatus.Success:
                    string name = "CURRENT-OPSITE-" + model.UserName;
                    var cookie = new HttpCookie(name, model.OP)
                    {
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTime.Now.AddDays(50)
                    };

                    HttpContext.Response.Cookies.Set(cookie);
                    //var UserOperatPoint = dbContext.OperatePoint.Where(x => x.ID == opId).ToList();
                    ////用户操作点
                    //Session["LoginUserOperatePoint"] = UserOperatPoint;

                    //缓存登录用户
                    Session[Common.GeSessionEnumByName("LoginUser").ToString()] = AppUser;

                    #region 获取用户权限和 显示的菜单

                    List<ApplicationRole> AppUserRoles = new List<ApplicationRole>();
                    AppUserRoles = RoleManager.Roles.Where(x => x.Users.Any(t => t.UserId == AppUser.Id)).ToList();
                    Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] = AppUserRoles;

                    #region 用户角色菜单动作

                    List<RoleMenuAction> UserRoleMenuActions = new List<RoleMenuAction>();
                    if (AppUserRoles.Any())
                    {
                        if (Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] == null)
                        {
                            var AppUserRoleIds = AppUserRoles.Select(n => n.Id).ToList();
                            UserRoleMenuActions = _unitOfWork.Repository<RoleMenuAction>().Queryable().Where(x => AppUserRoleIds.Contains(x.RoleId)).ToList();
                            Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] = UserRoleMenuActions;
                        }
                        else
                        {
                            UserRoleMenuActions = Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] as List<RoleMenuAction>;
                        }
                    }

                    #endregion

                    #region 缓存操作点 以及 验证用户是否选择了操作点

                    //缓存操作点
                    //List<OperatePoint> ArrOperatePoints = new List<OperatePoint>();
                    ////用户操作点
                    //List<OperatePoint> ArrUserOperatePoints = new List<OperatePoint>();
                    //var IQOperatePoints = from p in dbContext.OperatePoint.Include("OperatePointLists")
                    //                      //.Include("OperatePoint.OperatePointList")
                    //                      //where dbContext.UserOperatePointLink.Where(x => x.UserId == AppUser.Id).Select(x => x.OperateOpintId).Contains(p.ID)
                    //                      select p;

                    //#region 缓存操作点

                    ////加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                    //lock (Common.lockCacheOperatePoints)
                    //{
                    //    if (HttpContext.Cache[Common.GeCacheEnumByName("OperatePoints").ToString()] == null)
                    //    {
                    //        ArrOperatePoints = IQOperatePoints.ToList();
                    //        HttpContext.Cache.Insert(Common.GeCacheEnumByName("OperatePoints").ToString(), ArrOperatePoints, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                    //    }
                    //    else
                    //    {
                    //        ArrOperatePoints = HttpContext.Cache[Common.GeCacheEnumByName("OperatePoints").ToString()] as List<OperatePoint>;
                    //        if (!ArrOperatePoints.Any())
                    //        {
                    //            HttpContext.Cache.Remove(Common.GeCacheEnumByName("OperatePoints").ToString());
                    //            ArrOperatePoints = IQOperatePoints.ToList();
                    //            HttpContext.Cache.Insert(Common.GeCacheEnumByName("OperatePoints").ToString(), ArrOperatePoints, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                    //        }
                    //    }
                    //}

                    //#endregion

                    //if (AppUser.UserName.ToLower() == "admin" || AppUserRoles.Any(x => x.Name.Contains("超级管理员")))
                    //{
                    //    //HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] = ArrOperatePoints;
                    //}
                    //else
                    //{
                    //    if (HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] == null)
                    //    {
                    //        var IQUserOptPont = from p in dbContext.OperatePoint.Include("OperatePointLists")
                    //                            //.Include("OperatePoint.OperatePointList")
                    //                            where dbContext.UserOperatePointLink.Where(x => x.UserId == AppUser.Id).Select(x => x.OperateOpintId).Contains(p.ID)
                    //                            select p;
                    //        ArrUserOperatePoints = IQUserOptPont.ToList();
                    //        if (ArrOperatePoints.Any())
                    //        {
                    //            HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] = ArrUserOperatePoints;
                    //            //只有 一个操作点时无需选择
                    //            if (ArrUserOperatePoints.Count == 1)
                    //                HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] = ArrUserOperatePoints.Take(1).ToList();
                    //        }
                    //    }
                    //}

                    #endregion

                    #region 缓存菜单

                    List<MenuItem> ArrMenuItem = new List<MenuItem>();
                    //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                    lock (Common.lockCacheMenuItem)
                    {
                        if (HttpContext.Cache[Common.CacheNameS.MenuItem.ToString()] == null)
                        {
                            ArrMenuItem = _unitOfWork.Repository<MenuItem>().Query().Select().OrderBy(x => x.Code).ToList();
                            HttpContext.Cache.Insert(Common.CacheNameS.MenuItem.ToString(), ArrMenuItem, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                        else
                        {
                            ArrMenuItem = HttpContext.Cache[Common.CacheNameS.MenuItem.ToString()] as List<MenuItem>;
                            if (!ArrMenuItem.Any())
                            {
                                HttpContext.Cache.Remove(Common.CacheNameS.MenuItem.ToString());
                                ArrMenuItem = _unitOfWork.Repository<MenuItem>().Query().Select().OrderBy(x => x.Code).ToList();
                                HttpContext.Cache.Insert(Common.CacheNameS.MenuItem.ToString(), ArrMenuItem, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                            }
                        }
                    }

                    #endregion

                    #region 缓存菜单动作

                    List<MenuAction> ArrMenuActions = new List<MenuAction>();
                    //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                    lock (Common.lockCacheMenuAction)
                    {
                        if (HttpContext.Cache[Common.CacheNameS.MenuAction.ToString()] == null)
                        {
                            ArrMenuActions = _unitOfWork.Repository<MenuAction>().Queryable().Where(x => x.IsEnabled == true).ToList();
                            HttpContext.Cache.Insert(Common.CacheNameS.MenuAction.ToString(), ArrMenuActions, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                        else
                        {
                            ArrMenuActions = HttpContext.Cache[Common.CacheNameS.MenuAction.ToString()] as List<MenuAction>;
                            if (!ArrMenuActions.Any())
                            {
                                HttpContext.Cache.Remove(Common.CacheNameS.MenuAction.ToString());
                                ArrMenuActions = _unitOfWork.Repository<MenuAction>().Query().Select().OrderBy(x => x.Code).ToList();
                                HttpContext.Cache.Insert(Common.CacheNameS.MenuAction.ToString(), ArrMenuActions, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                            }
                        }
                    }

                    #endregion

                    #region 获取用户有权限的 菜单

                    List<MenuItem> ListMenuItem = ArrMenuItem;//HttpContext.Cache.Get(Common.GeCacheEnumByName("MenuItem").ToString()) as List<MenuItem>;
                    ListMenuItem = ListMenuItem.ToList();//.Where(x => x.IsEnabled == true)
                    var ListRoleNames = AppUserRoles.Select(x => x.Name).ToList();
                    if (ListRoleNames.Any())
                    {
                        if (!string.IsNullOrEmpty(LoginName))
                        {
                            if (LoginName.ToLower() == "admin" || ListRoleNames.Contains("超级管理员"))
                            {
                                Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] = ListMenuItem;
                            }
                            else
                            {
                                var MenuId_s = _unitOfWork.Repository<RoleMenu>().Query(x => ListRoleNames.Contains(x.RoleName)).Select(x => x.MenuId);
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
                                    Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] = OutPutMenuItem;
                                }
                                else
                                {
                                    Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] = OutPutMenuItem;
                                }

                                #endregion
                            }
                        }
                        else
                            Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] = new List<MenuItem>();
                    }
                    else
                    {
                        if (LoginName.ToLower() == "admin" || ListRoleNames.Contains("超级管理员"))
                        {
                            Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] = ListMenuItem;
                        }
                        else
                        {
                            Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] = new List<MenuItem>();
                        }
                    }

                    #endregion

                    #endregion

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                    //ApplicationUser NewAppUser = AppDbContext.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
                    //if (NewAppUser != null)
                    //{
                    //    NewAppUser.AccessFailedCount += 1;
                    //    //if (NewAppUser.AccessFailedCount > UserManager.MaxFailedAccessAttemptsBeforeLockout)
                    //    //{
                    //    //    NewAppUser.LockoutEnabled = true;
                    //    //    NewAppUser.AccessFailedCount = UserManager.MaxFailedAccessAttemptsBeforeLockout + 1;
                    //    //}
                    //    AppDbContext.Entry(NewAppUser).State = System.Data.Entity.EntityState.Modified;
                    //    AppDbContext.SaveChanges();
                    //}

                    AppUser.AccessFailedCount += 1;
                    if (AppUser.AccessFailedCount > UserManager.MaxFailedAccessAttemptsBeforeLockout)
                    {
                        AppUser.LockoutEnabled = true;
                        AppUser.AccessFailedCount = UserManager.MaxFailedAccessAttemptsBeforeLockout + 1;
                    }
                    UserManager.Update(AppUser);
                    ViewBag.OpList = this._opService.Query().Select(x => new { Id = x.ID, Name = x.OperatePointName }).ToList();
                    ModelState.AddModelError("", "登录失败,用户名或密码错误");
                    return View(model);
                default:
                    ModelState.AddModelError("", "登录失败.");
                    ViewBag.OpList = this._opService.Query().Select(x => new { Id = x.ID, Name = x.OperatePointName }).ToList();
                    return View(model);
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Lockout()
        {
            return View();
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.UserName = User.Identity.GetUserName();
            //model.Code = UserManager.GeneratePasswordResetTokenAsync(User.Identity.GetUserId());
            return View(model);
            //return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("UserName", "用户或密码不正确");
                // Don't reveal that the user does not exist
                return View(model);//RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            //明文密码加密成 HashPassword
            //string Password_Str = SignInManager.UserManager.PasswordHasher.HashPassword(model.Password);
            //密码 加密后 string 与 明文密码 比对 是否一致
            PasswordVerificationResult nmm = SignInManager.UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password);
            if (nmm != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("Password", "用户或密码不正确");
                // Don't reveal that the user does not exist
                return View(model);//RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            //model.Code = await UserManager.GetSecurityStampAsync(user.Id);
            model.Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var ArrSession = Common.GetEnumToDic("SessionNameS");
            HttpContext.Application.Clear();
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();

            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        /// <summary>
        /// 重置密码发送到Email
        /// </summary>
        /// <returns></returns>
        //public ActionResult ResetPasswordSendToEmail(string UserName,string Email)
        //{
        //    UserManager.SendEmailAsync()
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        //OnActionExecuting — — 控制器中的操作执行之前调用此方法。
        //OnActionExecuted — — 控制器中的操作执行后调用此方法。
        //OnResultExecuting — — 控制器操作结果执行之前调用此方法。
        //OnResultExecuted — — 这种方法称为后执行的控制器操作结果。
    }
}