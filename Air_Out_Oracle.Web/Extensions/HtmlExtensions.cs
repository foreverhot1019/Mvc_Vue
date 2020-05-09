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
using AirOut.Web.Models;
using AirOut.Web.Extensions;
using System.Text;
using System.Web.WebPages;

//using AirOut.Web.Models;
namespace AirOut.Web
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
            List<AirOut.Web.Models.MenuItem> ArrMenuItem = new List<AirOut.Web.Models.MenuItem>();
            if (SessionMenuItem != null)
            {
                ArrMenuItem = SessionMenuItem as List<AirOut.Web.Models.MenuItem>;
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
                            StrB.Append("    <a href=\"#\"><i class=\"" + (string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls) + "\"></i> <span class=\"nav-label\">" + item.Title + "</span> <span class=\"fa arrow\"></span></a>\r\n");
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

        private static bool _WriteMenu(List<AirOut.Web.Models.MenuItem> ArrMenuItem, List<AirOut.Web.Models.MenuItem> Menu_Items, string currentAction, string currentController, System.Text.StringBuilder StrB, ref bool ClassActive, int Num = 1, string cssActive = "active")
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
                    StrB.Append("    <a href=\"#\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + "><i class=\"" + (string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls) + "\"></i> <span class=\"nav-label\">" + item.Title + "</span> <span class=\"fa arrow\"></span></a>\r\n");
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
                            StrB.Append("                    <a href=\"" + item.Url + "\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + " target=\"" + (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self") + "\"><i class=\"" + ((string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls)) + "\"></i>" + item.Title + "</a>\r\n");
                            StrB.Append("                </li>\r\n");
                        }
                        else
                        {
                            StrB.Append("                <li class=\"\">\r\n");
                            StrB.Append("                    <a href=\"" + item.Url + "\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + " target=\"" + (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self") + "\"><i class=\"" + ((string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls)) + "\"></i>" + item.Title + "</a>\r\n");
                            StrB.Append("                </li>\r\n");
                        }
                    }
                    else
                    {
                        StrB.Append("                <li class=\"\">\r\n");
                        StrB.Append("                    <a href=\"" + item.Url + "\" " + (Num > 2 ? "Style=\"padding-left:" + (52 + (Num - 2) * 15) + "px;\"" : "") + " target=\"" + (System.Configuration.ConfigurationManager.AppSettings["NavUrlTarget"] ?? "_self") + "\"><i class=\"" + ((string.IsNullOrEmpty(item.IconCls) ? "fa fa-link" : item.IconCls)) + "\"></i>" + item.Title + "</a>\r\n");
                        StrB.Append("                </li>\r\n");
                    }
                }
            }
            return Class_Active_Return;
        }

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
}