using AirOut.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirOut.Web
{
    /// <summary>
    /// 自定义异常筛选器
    /// 在 App_Start/FilterConfig.cs中配置HandleErrorAttribute
    /// 也可以在Web.Config system.web 下设置（可以分配处理错误 控制器）
    ///<customErrors mode="On" defaultRedirect="~/Home/ServerError">
    ///  <error statusCode="404" redirect="~/Home/PageNotFound" />
    ///</customErrors>
    /// 先走 App_Start/FilterConfig.cs中配置的 ExceptionHandle-OnException=>BaseController-OnException=>Global-Application_Error
    /// View 访问的是 出错控制器下的 ViewName（默认Error），没有会去找 Shared 目录下的ViewName
    /// </summary>
    public class CustomerExceptionHandleFilter : HandleErrorAttribute
    {
        /// <summary>
        /// 在发生异常时调用
        /// </summary>
        public override void OnException(ExceptionContext filterContext)
        {
            try
            {
                //错误堆栈
                var ExceptionErr = filterContext.Exception;
                //Ajax请求错误 交给 Global-Application_Error 处理
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //获取出现异常的controller名和action名，用于记录
                    string controllerName = (string)filterContext.RouteData.Values["controller"];
                    string actionName = (string)filterContext.RouteData.Values["action"];
                    if (!filterContext.ExceptionHandled)
                    {
                        //定义一个HandErrorInfo，用于Error视图展示异常信息
                        HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                        //var ViewDataDict = new ViewDataDictionary<HandleErrorInfo>(model);
                        //View 访问的是 出错控制器下的 ViewName（默认Error），没有会去找 Shared 目录下的ViewName
                        var ViewDataDict = new ViewDataDictionary();
                        ViewDataDict.Add("ExpMessage", AirOut.Web.Extensions.Common.GetExceptionMsg(ExceptionErr));
                        ViewDataDict.Model = model;
                        ViewResult result = new ViewResult
                        {
                            ViewName = "ApplicationError",
                            ViewData = ViewDataDict  //定义ViewData，泛型
                        };
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;//不报告异常
                    }
                }
                Common.WriteLogByLog4Net(ExceptionErr, Common.Log4NetMsgType.Error);
            }
            catch (Exception ex)
            {
                Common.WriteLogByLog4Net(ex, Common.Log4NetMsgType.Fatal);
                base.OnException(filterContext);
            }
        }
    }
}