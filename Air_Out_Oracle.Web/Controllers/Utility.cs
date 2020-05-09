using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirOut.Web.Models;
using AirOut.Web.Extensions;

namespace AirOut.Web.Controllers
{
    public static class Utility
    {
        public static ApplicationUser CurrentAppUser
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.User != null)
                    {
                        if (HttpContext.Current.User.Identity.IsAuthenticated)
                        {
                            var SessionLoginUser = HttpContext.Current.Session == null ? null : HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUser").ToString()];
                            if (SessionLoginUser != null)
                                return SessionLoginUser as ApplicationUser;//HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUser").ToString()] as ApplicationUser;
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUser").ToString()] = value;
            }
        }

        public static List<ApplicationRole> CurrentUserRoles
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.User != null)
                    {
                        if (HttpContext.Current.User.Identity.IsAuthenticated)
                        {
                            var SessionLoginUserRoles = HttpContext.Current.Session == null ? null : HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()];
                            if (SessionLoginUserRoles != null)
                                return SessionLoginUserRoles as List<ApplicationRole>;//HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] as List<ApplicationRole>;
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] = value;
            }
        }

        /// <summary>
        /// 当前用户登录的操作点，只有 admin 或 权限 含有 超级管理员时 会又多个操作点
        /// </summary>
        public static List<OperatePoint> CurrentUserOperatePoint
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.User == null || HttpContext.Current.User.Identity == null)
                {
                    return null;
                }
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    using (WebdbContext appContext = new WebdbContext())
                    {
                        Repository.Pattern.Ef6.UnitOfWork unitOfWork_ = new Repository.Pattern.Ef6.UnitOfWork(appContext);
                        string name = "CURRENT-OPSITE-" + HttpContext.Current.User.Identity.Name;
                        HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(name);
                        if (cookie == null)
                        {
                            //HttpContext.Current.Response.Redirect("/Login");
                            return null;
                        }
                        else
                        {
                            int id = Convert.ToInt32(cookie.Value);
                            var query = unitOfWork_.Repository<OperatePoint>().Queryable().Where(x => x.ID == id).ToList();
                            return query;
                        }
                    }
                }
                else
                {
                    //HttpContext.Current.Response.Redirect("/Login");
                    return null;
                }

                //if (HttpContext.Current != null)
                //{
                //    if (HttpContext.Current.User != null)
                //    {
                //        if (HttpContext.Current.User.Identity.IsAuthenticated)
                //        {
                //            var ArrLOP = HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()];
                //            if (ArrLOP != null)
                //                return ArrLOP as List<OperatePoint>;
                //            else
                //            {
                //                if (HttpContext.Current==null)
                //                {
                //                    return null;
                //                }
                //                else if (HttpContext.Current.Request == null)
                //                {
                //                    return null;
                //                }
                //                if (HttpContext.Current.Request.Cookies[CurrentAppUser.Id] == null)
                //                {
                //                    return null;
                //                }
                //                else
                //                {
                //                    ArrLOP = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OperatePoint>>(HttpContext.Current.Request.Cookies[CurrentAppUser.Id].Value);
                //                    HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] = ArrLOP;
                //                    return ArrLOP as List<OperatePoint>;
                //                }
                //            }
                //        }
                //        else
                //            return null;
                //    }
                //    else
                //        return null;
                //}
                //else
                //    return null;
            }
            //set
            //{
            //    HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] = value;
            //}
        }

        /// <summary>
        /// 当前用户设置的操作点，只有 admin 或 权限 含有 超级管理员时 所有操作点
        /// </summary>
        public static List<OperatePoint> CurrentUserOperatePoint_s
        {
            get
            {
                var ArrUserOperatePoints = new List<OperatePoint>();
                if (HttpContext.Current.Session == null || HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] == null)
                {
                    if (CurrentAppUser != null)
                    {
                        WebdbContext dbContext = new WebdbContext();
                        var IQUserOptPont = from p in dbContext.OperatePoint
                                            //.Include("OperatePoint.OperatePointList")
                                            //where dbContext.UserOperatePointLink.Where(x => x.UserId == CurrentAppUser.Id).Select(x => x.OperateOpintId).Contains(p.ID)
                                            select p;
                        if (CurrentAppUser.UserName.ToLower() != "admin" && !CurrentUserRoles.Any(x => x.Name.Contains("超级管理员")))
                        {
                            IQUserOptPont = IQUserOptPont.Where(p => dbContext.UserOperatePointLink.Where(x => x.UserId == CurrentAppUser.Id).Select(x => x.OperateOpintId).Contains(p.ID));
                        }
                        ArrUserOperatePoints = IQUserOptPont.ToList();
                        if (ArrUserOperatePoints.Any())
                            HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] = ArrUserOperatePoints;
                        else
                            ArrUserOperatePoints = null;
                    }
                    else
                        ArrUserOperatePoints = null;
                    return ArrUserOperatePoints;
                }
                else
                {
                    ArrUserOperatePoints = HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] as List<OperatePoint>;
                    return ArrUserOperatePoints;
                }
            }
            set
            {
                HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] = value;
            }
        }
    }
}