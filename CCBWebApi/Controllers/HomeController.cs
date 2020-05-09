using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCBWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var OwinContxt = Request.GetOwinContext();
            var Authentication = OwinContxt.Authentication;
            var ApplicationCookie = Request.Cookies[".AspNet.ApplicationCookie"] == null ? "" : Request.Cookies[".AspNet.ApplicationCookie"].Value;
            if (!string.IsNullOrEmpty(ApplicationCookie))
            {
                var identity = Startup.OAuthAppCookieOptions.TicketDataFormat.Unprotect(ApplicationCookie);
                if (identity.Properties.ExpiresUtc.HasValue)
                {
                    var now = new DateTimeOffset(DateTime.Now);
                    if (identity.Properties.ExpiresUtc.Value.CompareTo(now) <= 0)
                    {
                        return Redirect(Startup.OAuthAppCookieOptions.LoginPath.Value);
                    }
                }
                else
                    return Redirect(Startup.OAuthAppCookieOptions.LoginPath.Value);
            }
            else
                return Redirect(Startup.OAuthAppCookieOptions.LoginPath.Value);
            return View();
        }

        /// <summary>
        /// 获取Enum 数据
        /// </summary>
        /// <param name="EnumName">枚举名称</param>
        /// <param name="page">页</param>
        /// <param name="rows">每页展示数量</param>
        /// <param name="q">搜素条件</param>
        /// <returns></returns>
        public ActionResult GetPagerEnum(string EnumName, int page = 1, int rows = 10, string q = "")
        {
            var ArrStatus = CCBWebApi.Extensions.Common.GetEnumToDic(EnumName, "CCBWebApi.Models.EnumType").
                Select(n => new
                {
                    ID = n.Value.ToString(),
                    TEXT = n.DisplayName,
                    Value = n.Value.ToString(),
                    DisplayName = n.DisplayName
                });
            if (!string.IsNullOrWhiteSpace(q))
                ArrStatus = ArrStatus.Where(x => x.ID.Contains(q) || x.TEXT.Contains(q));

            return Json(ArrStatus, JsonRequestBehavior.AllowGet);
        }
    }
}
