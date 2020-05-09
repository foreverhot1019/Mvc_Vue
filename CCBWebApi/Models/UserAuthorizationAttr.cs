using CCBWebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CCBWebApi.Models
{
    public class UserAuthorizationAttr : AuthorizeAttribute//IAuthorizationFilter
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

            var OwinContext = filterContext.HttpContext.GetOwinContext();
            var IAuthenManager = OwinContext.Authentication;

            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            #region 验证 filterContext是否有效

            if (filterContext == null || filterContext.HttpContext == null)
            {
                var MsgStr = "未登录，无法访问：" + controllerName;
                RedictUrl("Home", "Index", IsAjaxRequest, filterContext, MsgStr);
                return;
            }
            else
            {
                var MsgStr = "未登录，无法访问：" + controllerName;
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
                return;
                //if (OHttpContext.Session["LoginUser"] != null)
                //    return;
            }

            //登陆用户
            ApplicationUser AppUser = null;
            ////缓存操作点
            //List<OperatePoint> ArrOperatePoints = new List<OperatePoint>();
            ////缓存用户操作点和关联表
            //List<UserOperatePointLink> ArrUserOperatePointLink = new List<UserOperatePointLink>();
            ////缓存菜单
            //List<MenuItem> ArrMenuItem = new List<MenuItem>();
            ////缓存菜单动作
            //List<MenuAction> ArrMenuActions = new List<MenuAction>();
            ////用户操作点
            //List<OperatePoint> ArrUserOperatePoints = new List<OperatePoint>();
            ////用户角色
            //List<ApplicationRole> ArrAppUserRoles = new List<ApplicationRole>();
            ////角色菜单动作
            //List<RoleMenuAction> ArrUserRoleMenuActions = new List<RoleMenuAction>();

            if (IAuthenManager.User != null)
            {
                if (IAuthenManager.User.Identity.IsAuthenticated)
                {
                    #region 一个 controll对应 多个controll的权限

                    Dictionary<string, List<string>> dictOnetoManyControllerName = new Dictionary<string, List<string>>();
                    lock (Common.lockCacheHelper)
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
                        if (ArrUsers == null)
                            ArrUsers = new List<ApplicationUser>();
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

                        #region 验证用户是否选择了操作点

                        
                        #endregion

                        #region 获取用户权限和 显示的菜单
                       
                        #region 用户角色菜单动作

                        #endregion

                        #region 缓存菜单

                        
                        #endregion

                        #region 缓存菜单动作

                        #endregion

                        //验证代码
                        #endregion
                    }
                    else
                    {
                        IAuthenManager.SignOut();
                    }

                    #region 验证 是否有权限

                    //超级管理员 不需要验证 权限
                    if (Users.Any(x => x.UserName.ToLower() == "admin"))
                        return;

                    #region 无需验证权限的页面配置


                    #endregion


                    #endregion
                }
                else
                {
                    var MsgStr = "未登录，无法访问：" + controllerName;
                }
            }
            else
            {
                var MsgStr = "未登录，无法访问：" + controllerName;
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
        /// 跳转
        /// </summary>
        /// <param name="filterContext"></param>
        private void RedictUrl(string controllerName, string actionName, bool IsAjaxRequest, AuthorizationContext filterContext, string MsgStr = "")
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