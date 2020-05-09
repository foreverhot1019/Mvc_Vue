using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using TMI.Web.Models;
using TMI.Web.Extensions;
using System.Text;
using System.Web.WebPages;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web
{
    public static class AttributeHelper
    {
        public static string GetDisplayName(object obj, string propertyName)
        {
            if (obj == null) return null;
            return GetDisplayName(obj.GetType(), propertyName);

        }

        public static string GetDisplayName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            if (property == null) return null;

            return GetDisplayName(property);
        }

        public static string GetDisplayName(PropertyInfo property)
        {
            var attrName = GetAttributeDisplayName(property);
            if (!string.IsNullOrEmpty(attrName))
                return attrName;

            var metaName = GetMetaDisplayName(property);
            if (!string.IsNullOrEmpty(metaName))
                return metaName;

            return property.Name.ToString();
        }

        private static string GetAttributeDisplayName(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), true);
            if (atts.Length == 0)
                return null;
            return (atts[0] as System.ComponentModel.DataAnnotations.DisplayAttribute).Name;
        }

        private static string GetMetaDisplayName(PropertyInfo property)
        {
            var atts = property.DeclaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true);
            if (atts.Length == 0)
                return null;

            var metaAttr = atts[0] as MetadataTypeAttribute;
            var metaProperty = metaAttr.MetadataClassType.GetProperty(property.Name);
            if (metaProperty == null)
                return null;
            return GetAttributeDisplayName(metaProperty);
        }

        public static bool GetRequired(object obj, string propertyName)
        {
            if (obj == null) return false;
            return GetRequired(obj.GetType(), propertyName);
        }

        public static bool GetRequired(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            if (property == null) return false;

            return GetRequired(property);
        }

        public static bool GetRequired(PropertyInfo property)
        {
            var required = GetAttributeRequired(property);
            if (required)
                return required;

            required = GetMetaRequired(property);
            return required;
        }

        private static bool GetAttributeRequired(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), true);
            if (atts.Length == 0)
                return false;
            return true;
        }

        private static bool GetMetaRequired(PropertyInfo property)
        {
            var atts = property.DeclaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true);
            if (atts.Length == 0)
                return false;

            var metaAttr = atts[0] as MetadataTypeAttribute;
            var metaProperty = metaAttr.MetadataClassType.GetProperty(property.Name);
            if (metaProperty == null)
                return false;
            return GetAttributeRequired(metaProperty);
        }
    }

    public class ButtonAttribute : ActionMethodSelectorAttribute
    {
        public string Name { get; set; }

        public ButtonAttribute(string name)
        {
            Name = name;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.Controller.ValueProvider.GetValue(Name) != null;
        }
    }

    public static class CacheExtensions
    {
        static object sync = new object();

        public static T Add<T>(this System.Web.Caching.Cache cache, string cacheKey, int expirationSeconds, Func<T> method)
        {
            var data = cache == null ? default(T) : (T)cache[cacheKey];
            if (data == null)
            {
                data = method();

                if (expirationSeconds > 0 && data != null)
                {
                    lock (sync)
                    {
                        cache.Insert(cacheKey, data, null, DateTime.Now.AddSeconds(expirationSeconds), Cache.NoSlidingExpiration);
                    }
                }
            }
            return data;
        }

        public static T Get<T>(this System.Web.Caching.Cache cache, string key, Func<T> method) where T : class
        {
            var data = cache == null ? default(T) : (T)cache[key];
            if (data == null)
            {
                data = method();

                if (1000 > 0 && data != null)
                {
                    lock (sync)
                    {
                        cache.Insert(key, data, null, DateTime.Now.AddSeconds(1000), Cache.NoSlidingExpiration);
                    }
                }
            }
            return data;
        }
    }

    public static class HMTLHelperExtensions
    {
        /// <summary>
        /// 判断页面是否有权限
        /// </summary>
        /// <param name="html">页面Http信息</param>
        /// <param name="menuaction">菜单动作</param>
        /// <returns></returns>
        public static bool IsAuthorize(this HtmlHelper html, string menuaction)
        {
            try
            {
                if (!html.ViewContext.HttpContext.User.Identity.IsAuthenticated)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            var rolemanager = html.ViewContext.HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var usermanager = html.ViewContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userid = html.ViewContext.HttpContext.User.Identity.GetUserId();
            string userName = html.ViewContext.HttpContext.User.Identity.GetUserName();
            userid = userid == null ? "" : userid;
            var roles = usermanager.GetRoles(userid);

            if (roles.Any(x => x == "超级管理员") || userName == "admin")
                return true;

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];
            string key = userid + currentAction + currentController;

            //WebdbContext db = new WebdbContext();
            //var authorize = db.RoleMenuActions.Where(x => roles.Contains(x.RoleName) && x.MenuItem.Action == currentAction && (x.MenuItem.Controller == currentController || x.MenuItem.Controller==currentController+"Controller" )).ToList();
            //var data = authorize;

            //用户权限
            List<ApplicationRole> AppUserRoles = new List<ApplicationRole>();
            //用户菜单权限动作
            List<RoleMenuAction> UserRoleMenuActions = new List<RoleMenuAction>();
            //菜单动作
            List<MenuAction> MenuActions = new List<MenuAction>();
            //用户菜单
            List<MenuItem> UserListMenuItem = new List<MenuItem>();

            if (HttpContext.Current.Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] != null)
            {
                UserListMenuItem = HttpContext.Current.Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] as List<MenuItem>;
                if (!UserListMenuItem.Any())
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (HttpContext.Current.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
            {
                MenuActions = HttpContext.Current.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] as List<MenuAction>;
                if (!MenuActions.Any())
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] != null)
            {
                AppUserRoles = HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] as List<ApplicationRole>;
                if (AppUserRoles.Any())
                {
                    if (HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] != null)
                    {
                        UserRoleMenuActions = HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] as List<RoleMenuAction>;
                        if (!UserRoleMenuActions.Any())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (UserRoleMenuActions.Any())
            {
                if (MenuActions.Any())
                {
                    IEnumerable<int?> WhereUserMenuItem = new List<int?>();
                    var QUserMenuItem = UserListMenuItem.Where(x => x.Controller.ToLower().StartsWith(currentController.ToLower()));
                    if (QUserMenuItem.Any())
                    {
                        var QQUserMenuItem = QUserMenuItem.Where(x => x.Action.ToLower().StartsWith(currentAction.ToLower()));
                        if (QQUserMenuItem.Any())
                            QUserMenuItem = QQUserMenuItem;
                        WhereUserMenuItem = QUserMenuItem.Select(x => (int?)x.Id);
                    }
                    else
                        return false;

                    var WhereAction = MenuActions.Where(x => x.Code.ToLower() == menuaction.ToLower()).Select(x => (int?)x.Id);

                    if (WhereAction.Any() && WhereUserMenuItem.Any())
                    {
                        return UserRoleMenuActions.Any(x => WhereAction.Contains(x.ActionId) && WhereUserMenuItem.Contains(x.MenuId) && AppUserRoles.Select(n => n.Id).Contains(x.RoleId));
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断页面是否有权限
        /// </summary>
        /// <param name="html">页面Http信息</param>
        /// <param name="controller">控制器</param>
        /// <param name="action">操作</param>
        /// <param name="menuaction">菜单动作</param>
        /// <returns></returns>
        public static bool IsAuthorize(this HtmlHelper html, string controller, string action, string menuaction)
        {
            var rolemanager = html.ViewContext.HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var usermanager = html.ViewContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userid = html.ViewContext.HttpContext.User.Identity.GetUserId();
            string userName = html.ViewContext.HttpContext.User.Identity.GetUserName();
            var Users = usermanager.Users.Where(x => x.Id == userid);
            if (!Users.Any())
                return false;
            var roles = usermanager.GetRoles(userid);
            if (roles.Any(x => x == "超级管理员") || userName == "admin")
                return true;

            string currentAction = string.IsNullOrEmpty(action) ? (string)html.ViewContext.RouteData.Values["action"] : action;
            string currentController = string.IsNullOrEmpty(controller) ? (string)html.ViewContext.RouteData.Values["controller"] : controller;
            string key = userid + currentAction + currentController;

            //WebdbContext db = new WebdbContext();
            //var authorize = db.RoleMenuActions.Where(x => roles.Contains(x.RoleName) && x.MenuItem.Action == currentAction && (x.MenuItem.Controller == currentController || x.MenuItem.Controller==currentController+"Controller" )).ToList();
            //var data = authorize;

            //用户权限
            List<ApplicationRole> AppUserRoles = new List<ApplicationRole>();
            //用户菜单权限动作
            List<RoleMenuAction> UserRoleMenuActions = new List<RoleMenuAction>();
            //菜单动作
            List<MenuAction> MenuActions = new List<MenuAction>();
            //用户菜单
            List<MenuItem> UserListMenuItem = new List<MenuItem>();

            if (HttpContext.Current.Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] != null)
            {
                UserListMenuItem = HttpContext.Current.Session[Common.GeSessionEnumByName("Login_MenuItem").ToString()] as List<MenuItem>;
                if (!UserListMenuItem.Any())
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (HttpContext.Current.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
            {
                MenuActions = HttpContext.Current.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] as List<MenuAction>;
                if (!MenuActions.Any())
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] != null)
            {
                AppUserRoles = HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] as List<ApplicationRole>;
                if (AppUserRoles.Any())
                {
                    if (HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] != null)
                    {
                        UserRoleMenuActions = HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoleMenuActions").ToString()] as List<RoleMenuAction>;
                        if (!UserRoleMenuActions.Any())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (UserRoleMenuActions.Any())
            {
                if (MenuActions.Any())
                {
                    var WhereUserMenuItem = UserListMenuItem.Where(x => x.Url.ToLower().StartsWith("/" + currentController.ToLower())).Select(x => (int?)x.Id);

                    var WhereAction = MenuActions.Where(x => x.Code.ToLower() == menuaction.ToLower()).Select(x => (int?)x.Id);

                    if (WhereAction.Any() && WhereUserMenuItem.Any())
                    {
                        return UserRoleMenuActions.Any(x => WhereAction.Contains(x.ActionId) && WhereUserMenuItem.Contains(x.MenuId) && AppUserRoles.Select(n => n.Id).Contains(x.RoleId));
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.HttpContext.Request.RequestContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.HttpContext.Request.RequestContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
            {
                controller = currentController;
            }

            if (String.IsNullOrEmpty(action))
                action = currentAction;
            var ctrs = controller.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ctrs.Length > 1)
            {
                return ctrs.Contains(currentController) ? cssClass : String.Empty;
            }
            string ActiveClass = controller.ToUpper() == currentController.ToUpper() && action.ToUpper() == currentAction.ToUpper() ? cssClass : String.Empty;
            return ActiveClass;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        #region 输出菜单

        /// <summary>
        /// 输出菜单
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string WriteMenu(this HtmlHelper html)
        {
            int Num = 1;
            string cssActive = "active";
            //当前 控制器 和 动作
            string currentAction = (string)html.ViewContext.HttpContext.Request.RequestContext.RouteData.Values["action"] ?? "";
            string currentController = (string)html.ViewContext.HttpContext.Request.RequestContext.RouteData.Values["controller"] ?? "";
            currentAction = currentAction.ToLower().Trim();
            currentController = currentController.ToLower().Trim();

            var CurrSession = HttpContext.Current == null ? null : (HttpContext.Current.Session == null ? null : HttpContext.Current.Session);
            var SessionMenuItem = CurrSession == null ? null : CurrSession["Login_MenuItem"];
            //读取Session，已加载过菜单数据后，直接读取
            string MenuSession = CurrSession == null ? null : (CurrSession["MenuSession"] ?? "").ToString();
            if (!string.IsNullOrEmpty(MenuSession))
                return MenuSession;
            List<TMI.Web.Models.MenuItem> ArrMenuItem = new List<TMI.Web.Models.MenuItem>();
            if (SessionMenuItem != null)
            {
                ArrMenuItem = SessionMenuItem as List<TMI.Web.Models.MenuItem>;
                if (ArrMenuItem.Any())
                {
                    System.Text.StringBuilder StrB = new System.Text.StringBuilder();
                    ArrMenuItem = ArrMenuItem.Where(x => x.IsEnabled == true).ToList();
                    var Menu_Parents = ArrMenuItem.Where(x => x.ParentId == null || x.ParentId == 0).OrderBy(x => x.Code).ToList();
                    if (Menu_Parents.Any())
                    {
                        bool ClassActive = false;
                        foreach (var item in Menu_Parents)
                        {
                            ClassActive = false;
                            if (item.Controller.ToLower().Trim() == currentController)
                            {
                                if (!ClassActive)
                                    ClassActive = true;
                            }
                            StringBuilder StrBSub = new StringBuilder();
                            var Menu_Items = ArrMenuItem.Where(x => x.ParentId == item.Id).OrderBy(x => x.Code).ToList();
                            var Class_Active = _WriteMenu(ArrMenuItem, Menu_Items, currentAction, currentController, StrBSub, ref ClassActive, Num, cssActive);
                            if (Class_Active)
                                ClassActive = true;
                            StrB.Append("<li class=\"" + (ClassActive ? cssActive : "") + "\"> \r\n");
                            StrB.Append("    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"#\"><i class=\"" + (string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls) + "\"></i> <span class=\"nav-label\">" + item.Title + "</span> <span class=\"fa arrow\"></span></a>\r\n");
                            StrB.Append("    <ul class=\"nav nav-second-level collapse\"> \r\n");
                            StrB.Append("        " + StrBSub);
                            StrB.Append("    </ul>\r\n");
                            StrB.Append("</li>\r\n");
                        }
                    }
                    CurrSession["MenuSession"] = StrB.ToString();
                    return StrB.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 循环输出 子菜单
        /// </summary>
        /// <param name="ArrMenuItem"></param>
        /// <param name="Menu_Items"></param>
        /// <param name="currentAction"></param>
        /// <param name="currentController"></param>
        /// <param name="StrB"></param>
        /// <param name="ClassActive"></param>
        /// <param name="Num"></param>
        /// <param name="cssActive"></param>
        /// <returns></returns>
        private static bool _WriteMenu(List<TMI.Web.Models.MenuItem> ArrMenuItem, List<TMI.Web.Models.MenuItem> Menu_Items, string currentAction, string currentController, System.Text.StringBuilder StrB, ref bool ClassActive, int Num = 1, string cssActive = "active")
        {
            var ArrOneToManyCtrl = (Dictionary<string, List<string>>)CacheHelper.Get_SetCache(Common.CacheNameS.dictOnetoManyControllerName);
            Num++;
            if (StrB == null)
                StrB.Append("");
            bool Class_Active = false;
            bool Class_Active_Return = false;
            foreach (var item in Menu_Items)
            {
                if (item.Controller.ToLower().Trim() == currentController || ArrOneToManyCtrl.Any(x => x.Value.Any(n => n.ToLower().Trim() == currentController) && item.Controller.ToLower().Trim() == x.Key.ToLower().Trim()))
                {
                    if (!ClassActive)
                    {
                        ClassActive = true;
                    }
                    Class_Active = true;
                }
                else
                {
                    if (ClassActive)
                    {
                        ClassActive = false;
                        Class_Active_Return = true;
                    }
                }

                var SubMenu_Items = ArrMenuItem.Where(x => x.ParentId == item.Id).OrderBy(x => x.Code).ToList();
                if (SubMenu_Items.Any())
                {
                    StringBuilder StrBSub = new StringBuilder();
                    _WriteMenu(ArrMenuItem, SubMenu_Items, currentAction, currentController, StrBSub, ref Class_Active, Num, cssActive);
                    if (Class_Active)
                        ClassActive = Class_Active;
                    StrB.Append("<li class=\"" + ((ClassActive || Class_Active) ? cssActive : "") + "\"> \r\n");
                    StrB.Append("    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"#\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + "><i class=\"" + (string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls) + "\"></i> <span class=\"nav-label\">" + item.Title + "</span> <span class=\"fa arrow\"></span></a>\r\n");
                    StrB.Append("    <ul class=\"nav nav-second-level collapse\"> \r\n");
                    StrB.Append("        " + StrBSub);
                    StrB.Append("    </ul>\r\n");
                    StrB.Append("</li>\r\n");
                }
                else
                {
                    if (item.Action.ToLower().Trim() == currentAction)
                    {
                        if (item.Controller.ToLower().Trim() == currentController || ArrOneToManyCtrl.Any(x => x.Value.Any(n => n.ToLower().Trim() == currentController && item.Controller.ToLower().Trim() == x.Key.ToLower().Trim())))
                        {
                            StrB.Append("                <li class=\"" + cssActive + "\">\r\n");
                            StrB.Append("                    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + " target=\"" + (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self") + "\"><i class=\"" + ((string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls)) + "\"></i>" + item.Title + "</a>\r\n");
                            StrB.Append("                </li>\r\n");
                        }
                        else
                        {
                            StrB.Append("                <li class=\"\">\r\n");
                            StrB.Append("                    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + " target=\"" + (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self") + "\"><i class=\"" + ((string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls)) + "\"></i>" + item.Title + "</a>\r\n");
                            StrB.Append("                </li>\r\n");
                        }
                    }
                    else
                    {
                        StrB.Append("                <li class=\"\">\r\n");
                        StrB.Append("                    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + " target=\"" + (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self") + "\"><i class=\"" + ((string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls)) + "\"></i>" + item.Title + "</a>\r\n");
                        StrB.Append("                </li>\r\n");
                    }
                }
            }
            return Class_Active_Return;
        }

        #endregion

        #region 输出菜单 Ace风格

        /// <summary>
        /// 输出菜单
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string WriteMenuAce(this HtmlHelper html)
        {
            int Num = 1;
            string cssActive = "active";
            //当前 控制器 和 动作
            string currentAction = (string)html.ViewContext.HttpContext.Request.RequestContext.RouteData.Values["action"] ?? "";
            string currentController = (string)html.ViewContext.HttpContext.Request.RequestContext.RouteData.Values["controller"] ?? "";
            currentAction = currentAction.ToLower().Trim();
            currentController = currentController.ToLower().Trim();

            var CurrSession = HttpContext.Current == null ? null : (HttpContext.Current.Session == null ? null : HttpContext.Current.Session);
            var SessionMenuItem = CurrSession == null ? null : CurrSession["Login_MenuItem"];
            //读取Session，已加载过菜单数据后，直接读取
            string MenuSession = CurrSession == null ? null : (CurrSession["MenuSessionAce"] ?? "").ToString();
            if (!string.IsNullOrEmpty(MenuSession))
                return MenuSession;
            List<TMI.Web.Models.MenuItem> ArrMenuItem = new List<TMI.Web.Models.MenuItem>();
            if (SessionMenuItem != null)
            {
                ArrMenuItem = SessionMenuItem as List<TMI.Web.Models.MenuItem>;
                if (ArrMenuItem.Any())
                {
                    System.Text.StringBuilder StrB = new System.Text.StringBuilder();
                    ArrMenuItem = ArrMenuItem.Where(x => x.IsEnabled == true).ToList();
                    var Menu_Parents = ArrMenuItem.Where(x => x.ParentId == null || x.ParentId == 0).OrderBy(x => x.Code).ToList();
                    if (Menu_Parents.Any())
                    {
                        bool ClassActive = false;
                        foreach (var item in Menu_Parents)
                        {
                            ClassActive = false;
                            if (item.Controller.ToLower().Trim() == currentController)
                            {
                                if (!ClassActive)
                                    ClassActive = true;
                            }
                            StringBuilder StrBSub = new StringBuilder();
                            var Menu_Items = ArrMenuItem.Where(x => x.ParentId == item.Id).OrderBy(x => x.Code).ToList();
                            var Class_Active = _WriteMenuAce(ArrMenuItem, Menu_Items, currentAction, currentController, StrBSub, ref ClassActive, Num, cssActive);
                            if (Class_Active)
                                ClassActive = true;
                            StrB.Append("<li class=\"" + (ClassActive ? cssActive : "") + "\"> \r\n");
                            StrB.Append("    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" " + (StrBSub.Length > 0 ? "class=\"dropdown-toggle\"" : "") + ">\r\n");
                            StrB.Append("        <i class=\"menu-icon " + (string.IsNullOrEmpty(item.IconCls) ? "fa fa-list" : item.IconCls) + "\"></i>\r\n");
                            StrB.Append("        <span class=\"menu-text\">" + item.Title + "</span>\r\n");
                            if (StrBSub.Length > 0)
                                StrB.Append("        <b class=\"arrow fa fa-angle-down\"></b>");
                            StrB.Append("    </a>\r\n");
                            StrB.Append("    <b class=\"arrow\"></b> \r\n");
                            if (StrBSub.Length > 0)
                            {
                                StrB.Append("    <ul class=\"submenu\"> \r\n");
                                StrB.Append("        " + StrBSub);
                                StrB.Append("    </ul>\r\n");
                            }
                            StrB.Append("</li>\r\n");
                        }
                    }
                    CurrSession["MenuSessionAce"] = StrB.ToString();
                    return StrB.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 循环输出 子菜单Ace
        /// </summary>
        /// <param name="ArrMenuItem"></param>
        /// <param name="Menu_Items"></param>
        /// <param name="currentAction"></param>
        /// <param name="currentController"></param>
        /// <param name="StrB"></param>
        /// <param name="ClassActive"></param>
        /// <param name="Num"></param>
        /// <param name="cssActive"></param>
        /// <returns></returns>
        private static bool _WriteMenuAce(List<TMI.Web.Models.MenuItem> ArrMenuItem, List<TMI.Web.Models.MenuItem> Menu_Items, string currentAction, string currentController, System.Text.StringBuilder StrB, ref bool ClassActive, int Num = 1, string cssActive = "active")
        {
            var ArrOneToManyCtrl = (Dictionary<string, List<string>>)CacheHelper.Get_SetCache(Common.CacheNameS.dictOnetoManyControllerName);
            Num++;
            if (StrB == null)
                StrB.Append("");
            bool Class_Active = false;
            bool Class_Active_Return = false;
            var NavUrlTarget = (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self");
            foreach (var item in Menu_Items)
            {
                if (item.Controller.ToLower().Trim() == currentController || ArrOneToManyCtrl.Any(x => x.Value.Any(n => n.ToLower().Trim() == currentController) && item.Controller.ToLower().Trim() == x.Key.ToLower().Trim()))
                {
                    if (!ClassActive)
                    {
                        ClassActive = true;
                    }
                    Class_Active = true;
                }
                else
                {
                    if (ClassActive)
                    {
                        ClassActive = false;
                        Class_Active_Return = true;
                    }
                }

                var SubMenu_Items = ArrMenuItem.Where(x => x.ParentId == item.Id).OrderBy(x => x.Code).ToList();
                if (SubMenu_Items.Any())
                {
                    StringBuilder StrBSub = new StringBuilder();
                    _WriteMenuAce(ArrMenuItem, SubMenu_Items, currentAction, currentController, StrBSub, ref Class_Active, Num, cssActive);
                    if (Class_Active)
                        ClassActive = Class_Active;

                    StrB.Append("<li class=\"" + ((ClassActive || Class_Active) ? cssActive : "") + "\"> \r\n");
                    StrB.Append("    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" " + (StrBSub.Length > 0 ? "class=\"dropdown-toggle\"" : "") + " >\r\n");
                    StrB.Append("        <i class=\"menu-icon fa fa-caret-right\"></i>\r\n");
                    StrB.Append("        <span class=\"menu-text\">" + (string.IsNullOrEmpty(item.IconCls) ? "" : "<i class=\"" + item.IconCls + "\"</i>") + item.Title + "</span>\r\n");
                    if (StrBSub.Length > 0)
                        StrB.Append("        <b class=\"arrow fa fa-angle-down\"></b>\r\n");
                    StrB.Append("    </a>\r\n");
                    StrB.Append("    <b class=\"arrow\"></b>\r\n");
                    if (StrBSub.Length > 0)
                    {
                        StrB.Append("    <ul class=\"submenu\"> \r\n");
                        StrB.Append("        " + StrBSub);
                        StrB.Append("    </ul>\r\n");
                    }
                    StrB.Append("</li>\r\n");
                }
                else
                {
                    if (item.Action.ToLower().Trim() == currentAction)
                    {
                        if (item.Controller.ToLower().Trim() == currentController || ArrOneToManyCtrl.Any(x => x.Value.Any(n => n.ToLower().Trim() == currentController && item.Controller.ToLower().Trim() == x.Key.ToLower().Trim())))
                        {
                            StrB.Append("                <li class=\"" + cssActive + "\">\r\n");
                            StrB.Append("                    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" target=\"" + NavUrlTarget + "\">\r\n");
                            StrB.Append("                        <i class=\"menu-icon fa fa-caret-right\"></i>\r\n");
                            StrB.Append("                        " + ((string.IsNullOrEmpty(item.IconCls) ? "" : "<i class=\"" + item.IconCls + "\"></i>")) + "\r\n");
                            StrB.Append("                        " + item.Title + "\r\n");
                            StrB.Append("                    </a>\r\n");
                            StrB.Append("                </li>\r\n");
                        }
                        else
                        {
                            StrB.Append("                <li class=\"\">\r\n");
                            StrB.Append("                    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" target=\"" + NavUrlTarget + "\">\r\n");
                            StrB.Append("                        <i class=\"menu-icon fa fa-caret-right\"></i>\r\n");
                            StrB.Append("                        " + ((string.IsNullOrEmpty(item.IconCls) ? "" : "<i class=\"" + item.IconCls + "\"></i>")) + "\r\n");
                            StrB.Append("                        " + item.Title + "\r\n");
                            StrB.Append("                    </a>\r\n");
                            StrB.Append("                </li>\r\n");
                        }
                    }
                    else
                    {
                        StrB.Append("                <li class=\"\">\r\n");
                        StrB.Append("                    <a id=\"" + item.Id + "\" pid=\"" + item.ParentId + "\" href=\"" + item.Url + "\" target=\"" + NavUrlTarget + "\">\r\n");
                        StrB.Append("                        <i class=\"menu-icon fa fa-caret-right\"></i>\r\n");
                        StrB.Append("                        " + ((string.IsNullOrEmpty(item.IconCls) ? "" : "<i class=\"" + item.IconCls + "\"></i>")) + "\r\n");
                        StrB.Append("                        " + item.Title + "\r\n");
                        StrB.Append("                    </a>\r\n");
                        StrB.Append("                </li>\r\n");
                    }
                }
            }
            return Class_Active_Return;
        }

        #endregion

        public static MvcHtmlString ActionButton(this HtmlHelper html, string linkText, string action, string controllerName, string iconClass)
        {
            //<a href="/@lLink.ControllerName/@lLink.ActionName" title="@lLink.LinkText"><i class="@lLink.IconClass"></i><span class="">@lLink.LinkText</span></a>
            var lURL = new UrlHelper(html.ViewContext.RequestContext);

            // build the <span class="">@lLink.LinkText</span> tag
            var lSpanBuilder = new TagBuilder("span");
            lSpanBuilder.MergeAttribute("class", "");
            lSpanBuilder.SetInnerText(linkText);
            string lSpanHtml = lSpanBuilder.ToString(TagRenderMode.Normal);

            // build the <i class="@lLink.IconClass"></i> tag
            var lIconBuilder = new TagBuilder("i");
            lIconBuilder.MergeAttribute("class", iconClass);
            string lIconHtml = lIconBuilder.ToString(TagRenderMode.Normal);

            // build the <a href="@lLink.ControllerName/@lLink.ActionName" title="@lLink.LinkText">...</a> tag
            var lAnchorBuilder = new TagBuilder("a");
            lAnchorBuilder.MergeAttribute("href", lURL.Action(action, controllerName));
            lAnchorBuilder.InnerHtml = lIconHtml + lSpanHtml; // include the <i> and <span> tags inside
            string lAnchorHtml = lAnchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(lAnchorHtml);
        }

        /// <summary>
        /// 获取 页面Model转 json
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static string RenderVue_ModelJson(this HtmlHelper htmlHelper)
        {
            var ArrField = new List<VuePagePropty>();//存储字段配置
            Dictionary<string, string> DictForeignKey = new Dictionary<string, string>();//存储外键字段
            var IsSearchOrder = false;//设置搜索排序
            var IsListOrder = false;//设置列表排序
            var IsFormOrder = false;//设置Form排序
            var OModel = htmlHelper.ViewData.Model;
            var TypeModel = htmlHelper.ViewData.ModelMetadata.ModelType;
            if (TypeModel.GetInterface("IEnumerable", false) != null)
            {
                var GenericArgs = TypeModel.GetGenericArguments();
                TypeModel = GenericArgs[0];
            }
            var TypeModelFullName = TypeModel.FullName;
            PropertyInfo[] ProptyModel;
            var CacheProptyModel = CacheHelper.GetCache(TypeModelFullName);
            if (CacheProptyModel == null)
            {
                ProptyModel = TypeModel.GetProperties();
                CacheHelper.SetCache(TypeModelFullName, ProptyModel);
            }
            else
                ProptyModel = (PropertyInfo[])CacheProptyModel;

            foreach (var item in ProptyModel)
            {
                var OVuePagePropty = new VuePagePropty();
                if (item.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), false).Any())
                {
                    continue;
                }
                //Name;//名称 
                //DisplayName;//显示名称 
                //Sortable;//是否可排序 
                //Width_List //列表-列宽度
                //Width_input //Form-input宽度
                //Type = "datetime/number/string/boolean";//类型 
                //Precision //Type为number时，可设置小数位 
                //inputType = "password/datetime/text";//form中的input类型
                //Required//必填
                //IsKey //主键
                //Editable //可编辑
                //SearchShow = true;//搜索中展示
                //FormShow = true;//Form中展示
                //ListShow = true;//列表展示
                //MaxLength //最大长度
                //MinLength //最小长度
                //ListOrder //列表排序
                //SearchOrder //搜索排序
                //FormOrder //Form排序

                OVuePagePropty.Name = item.Name;//名称
                OVuePagePropty.DisplayName = "";
                OVuePagePropty.ListOrder = 0; //列表排序
                var DisplayAttr = Common.GetDataDisplayAttr(item);
                if (DisplayAttr != null)
                {
                    OVuePagePropty.DisplayName = DisplayAttr.GetName();//显示名称
                    OVuePagePropty.ListOrder = DisplayAttr.GetOrder() ?? 0;
                    if (!IsListOrder && ArrField.Any() && ArrField.Any(x => x.ListOrder != OVuePagePropty.ListOrder))
                        IsListOrder = true;
                }
                if (string.IsNullOrEmpty(OVuePagePropty.DisplayName))
                    OVuePagePropty.DisplayName = OVuePagePropty.Name;
                OVuePagePropty.Sortable = true;//是否可排序
                OVuePagePropty.Width_List = "120";//列表-列宽度
                OVuePagePropty.Width_input = "178";//Form-input宽度
                OVuePagePropty.SearchOrder = 0; //搜索排序
                OVuePagePropty.FormOrder = 0; //Form排序
                var Nullable = false;//是否可为空

                #region  获取数据类型

                if (item.PropertyType.IsGenericType)
                {
                    var GenericArgs = item.PropertyType.GetGenericArguments();
                    //判断带？类型
                    if (item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var _type = item.PropertyType.GetGenericArguments()[0];
                        if (_type.IsClass)
                        {
                            //自定义类跳过 数组 跳过
                            continue;
                        }
                        OVuePagePropty.Type = _type.Name;//类型
                        Nullable = true;
                        if (_type.IsEnum)
                        {
                            OVuePagePropty.Type = "Enum";
                        }
                    }
                    else
                    {
                        //IEnumerable 数组 跳过
                        continue;
                    }
                }//判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                //if ((fi.FieldType.IsPrimitive || fi.FieldType.IsValueType || fi.FieldType == typeof(string) || fi.FieldType == typeof(decimal) || fi.FieldType == typeof(DateTime)) && fi.FieldType.Name.ToLower().IndexOf("struct") < 0)
                else if (item.PropertyType.IsClass && !item.PropertyType.IsPrimitive && item.PropertyType.Name.ToLower().IndexOf("string") < 0)
                {
                    var ArrForeignKeyAttr = item.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                    if (ArrForeignKeyAttr != null && ArrForeignKeyAttr.Any())
                    {
                        var OForeignKey = (ForeignKeyAttribute)ArrForeignKeyAttr.FirstOrDefault();
                        DictForeignKey.Add(OForeignKey.Name, item.PropertyType.Name);
                    }
                    //自定义类跳过 数组 跳过
                    continue;
                }
                else if (item.PropertyType.IsEnum)
                {
                    OVuePagePropty.Type = "Enum";
                }
                else
                    OVuePagePropty.Type = item.PropertyType.Name;

                #endregion

                switch ((string)OVuePagePropty.Type)
                {
                    case "Int32":
                    case "Int64":
                    case "Long":
                    case "Decimal":
                    case "Float":
                    case "Double":
                        if (OVuePagePropty.Type.IndexOf("Int") < 0)
                        {
                            OVuePagePropty.Precision = 2;//小数位
                        }
                        else
                            OVuePagePropty.Precision = 0;
                        if (!Nullable)
                            OVuePagePropty.Required = true;
                        OVuePagePropty.Type = "number";
                        break;
                    case "DateTime":
                        if (!Nullable)
                            OVuePagePropty.Required = true;
                        OVuePagePropty.Type = "datetime";
                        OVuePagePropty.inputType = "datetime";
                        break;
                    case "Enum":
                        if (!Nullable)
                            OVuePagePropty.Required = true;
                        OVuePagePropty.IsForeignKey = true;
                        OVuePagePropty.ForeignKeyGetListUrl = "/Home/GetPagerEnum?EnumName=" + item.PropertyType.Name;
                        OVuePagePropty.inputType = "text";
                        OVuePagePropty.Type = OVuePagePropty.Type.ToLower();
                        break;
                    default:
                        Nullable = true;//是否可为空
                        OVuePagePropty.inputType = "text";
                        OVuePagePropty.Type = OVuePagePropty.Type.ToLower();
                        break;
                }
                if (item.Name.ToLower().IndexOf("password") == 0)
                {
                    OVuePagePropty.inputType = "password";//form中的input类型
                }
                if (!OVuePagePropty.Required)
                    OVuePagePropty.Required = Common.GetAttributeRequired(item);//必填
                OVuePagePropty.IsKey = false;//主键
                OVuePagePropty.Editable = true;//Form中是否可编辑
                //-------默认值----可在 属性Vue_PagePropty中配置
                OVuePagePropty.SearchShow = false;//搜索中展示
                OVuePagePropty.FormShow = true;//Form中展示
                OVuePagePropty.ListShow = true;//列表展示

                var ArrKeyAttr = item.GetCustomAttributes(typeof(KeyAttribute), false);
                if (ArrKeyAttr.Any())
                {
                    OVuePagePropty.IsKey = true;
                    OVuePagePropty.Editable = false;
                    OVuePagePropty.SearchShow = false;//搜索中展示
                    OVuePagePropty.FormShow = false;//Form中展示
                    OVuePagePropty.ListShow = false;//列表展示
                }

                var ArrMaxLenAttr = item.GetCustomAttributes(typeof(MaxLengthAttribute), false);
                if (ArrMaxLenAttr.Any())
                {
                    var MaxLenAttr = (MaxLengthAttribute)ArrMaxLenAttr.FirstOrDefault();
                    OVuePagePropty.MaxLength = MaxLenAttr.Length;
                }

                var ArrMinLenAttr = item.GetCustomAttributes(typeof(StringLengthAttribute), false);
                if (ArrMinLenAttr.Any())
                {
                    var MinLenAttr = (StringLengthAttribute)ArrMinLenAttr.FirstOrDefault();
                    OVuePagePropty.MaxLength = MinLenAttr.MaximumLength;//最大长度
                    OVuePagePropty.MinLength = MinLenAttr.MinimumLength;//最小长度
                }

                var ArrScaffoldColumnAttr = item.GetCustomAttributes(typeof(ScaffoldColumnAttribute), false);
                if (ArrScaffoldColumnAttr.Any())
                {
                    var ScaffoldColumnAttr = (ScaffoldColumnAttribute)ArrScaffoldColumnAttr.FirstOrDefault();
                    if (!ScaffoldColumnAttr.Scaffold)
                    {
                        OVuePagePropty.SearchShow = false;//搜索中展示
                        OVuePagePropty.ListShow = false;//列表展示
                        OVuePagePropty.FormShow = false;//Form中展示
                    }
                }

                #region 获取自定义属性Vue_PagePropty

                var ArrVue_PageProptyAttr = item.GetCustomAttributes(typeof(Vue_PageProptyAttribute), false);
                if (ArrVue_PageProptyAttr.Any())
                {
                    foreach (var VueAttr in ArrVue_PageProptyAttr)
                    {
                        var Vue_PageProptyAttr = (Vue_PageProptyAttribute)VueAttr;
                        OVuePagePropty.Editable = Vue_PageProptyAttr.Editable;//可编辑（table-column）
                        OVuePagePropty.Sortable = Vue_PageProptyAttr.Sortable;//可排序（form）
                        OVuePagePropty.SearchShow = Vue_PageProptyAttr.SearchShow;//搜索中展示
                        OVuePagePropty.ListShow = Vue_PageProptyAttr.ListShow;//列表展示
                        OVuePagePropty.FormShow = Vue_PageProptyAttr.FormShow;//Form中展示
                        OVuePagePropty.SearchOrder = Vue_PageProptyAttr.SearchOrder;//Search排序
                        OVuePagePropty.FormOrder = Vue_PageProptyAttr.FormOrder;//Form排序
                        if (Vue_PageProptyAttr.IsForeignKey)
                            OVuePagePropty.IsForeignKey = Vue_PageProptyAttr.IsForeignKey;//外键关联
                        if(!string.IsNullOrEmpty(Vue_PageProptyAttr.ForeignKeyGetListUrl))
                            OVuePagePropty.ForeignKeyGetListUrl = Vue_PageProptyAttr.ForeignKeyGetListUrl;//外键关联 获取数据Url

                        if (Vue_PageProptyAttr.Width_List.HasValue)
                            OVuePagePropty.Width_List = Vue_PageProptyAttr.Width_List.Value > 0 ? Vue_PageProptyAttr.Width_List.Value.ToString() : "*";// 列表-列宽度
                        if (Vue_PageProptyAttr.Width_input.HasValue)
                            OVuePagePropty.Width_input = Vue_PageProptyAttr.Width_input.Value > 0 ? Vue_PageProptyAttr.Width_input.Value.ToString() : "*";// Form-input宽度

                        if (!IsSearchOrder && ArrField.Any() && ArrField.Any(x => x.SearchOrder != Vue_PageProptyAttr.SearchOrder))
                            IsSearchOrder = true;
                        if (!IsFormOrder && ArrField.Any() && ArrField.Any(x => x.FormOrder != Vue_PageProptyAttr.FormOrder))
                            IsFormOrder = true;
                    }
                }

                #endregion

                //dynamic BaseDynamic = new System.Dynamic.ExpandoObject();
                //var dic = (IDictionary<string, object>)BaseDynamic;
                //dic.Add(item.Name, ODynamic);
                ArrField.Add(OVuePagePropty);
            }
            //没有设置 默认宽度的 默认最后一个为*
            if (!ArrField.Any(x => x.Width_List == "*"))
                ArrField.LastOrDefault().Width_List = "*";

            #region 外键

            foreach (var item in DictForeignKey)
            {
                var OVuePagePropty = ArrField.Where(x => x.Name == item.Key).FirstOrDefault();
                OVuePagePropty.IsForeignKey = true;
                OVuePagePropty.ForeignKeyGetListUrl = "/" + WebdbContext.StringUtil.ToPlural(item.Value) + "/GetData";
            }

            #endregion

            //没有任何搜索时，默认添加时间
            if (!ArrField.Any(x => x.SearchShow))
            {
                var WhereArrField = ArrField.Where(x => x.Name == "ADDTS");
                if (WhereArrField.Any())
                {
                    var item = WhereArrField.FirstOrDefault();
                    item.SearchShow = true;//搜索中展示
                }
            }

            dynamic BaseDynamic = new System.Dynamic.ExpandoObject();
            BaseDynamic.IsListOrder = IsListOrder;
            BaseDynamic.IsSearchOrder = IsSearchOrder;
            BaseDynamic.IsFormOrder = IsFormOrder;
            BaseDynamic.ArrField = ArrField;
            //BaseDynamic.ArrListField = IsListOrder ? ArrField.OrderBy(x => x.ListOrder).ToList() : ArrField;
            //BaseDynamic.ArrSearchField = IsSearchOrder ? ArrField.OrderBy(x => x.SearchOrder).ToList() : ArrField;
            //BaseDynamic.ArrFormField = IsFormOrder ? ArrField.OrderBy(x => x.FormOrder).ToList() : ArrField;

            return Newtonsoft.Json.JsonConvert.SerializeObject(BaseDynamic);
        }

        #region Partial View 支持RenderScripts

        //<body>
        //...
        //@Html.RenderScripts()
        //</body>
        //and somewhere in some template:
        //
        //@Html.Script(
        //    @<script src="@Url.Content("~/Scripts/jquery-1.4.4.min.js")" type="text/javascript"></script>
        //)
        ///// <summary>
        ///// Partial View 支持RenderScripts
        ///// </summary>
        ///// <param name="htmlHelper"></param>
        ///// <param name="template"></param>
        ///// <returns></returns>
        //public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        //{
        //    htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
        //    return MvcHtmlString.Empty;
        //}

        ///// <summary>
        ///// 合并Script时，支持Partial View中的Section Scripts
        ///// </summary>
        ///// <param name="htmlHelper"></param>
        ///// <returns></returns>
        //public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        //{
        //    foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
        //    {
        //        if (key.ToString().StartsWith("_script_"))
        //        {
        //            var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
        //            if (template != null)
        //            {
        //                htmlHelper.ViewContext.Writer.Write(template(null));
        //            }
        //        }
        //    }
        //    return MvcHtmlString.Empty;
        //}

        #endregion
    }

    /// <summary>
    /// Vue 前端页面 配置
    /// </summary>
    public class VuePagePropty
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 列表-列宽度
        /// </summary>
        public string Width_List { get; set; }

        /// <summary>
        /// Form-input宽度
        /// </summary>
        public string Width_input { get; set; }

        /// <summary>
        /// 类型 
        /// datetime/number/string/boolean
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Type为number时，可设置小数位 
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// form中的input类型
        /// password/datetime/text
        /// </summary>
        public string inputType { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 必填
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 是否可排序 
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// 可编辑
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// 搜索中展示
        /// </summary>
        public bool SearchShow { get; set; }

        /// <summary>
        /// Form中展示
        /// </summary>
        public bool FormShow { get; set; }

        /// <summary>
        /// 列表展示
        /// </summary>
        public bool ListShow { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// 列表排序
        /// </summary>
        public int ListOrder { get; set; }

        /// <summary>
        /// 搜索排序
        /// </summary>
        public int SearchOrder { get; set; }

        /// <summary>
        /// Form排序
        /// </summary>
        public int FormOrder { get; set; }

        /// <summary>
        /// 外键关联
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 外键关联 获取数据Url
        /// </summary>
        public string ForeignKeyGetListUrl { get; set; }

    }
}
