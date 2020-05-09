using AirOut.Web.Extensions;
using AirOut.Web.Models;
using AirOut.Web.Services;
using log4net;
using log4net.Config;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace AirOut.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // Application_Init：在应用程序被实例化或第一次被调用时，该事件被触发对于所有的HttpApplication 对象实例，它都会被调用
        // Application_Disposed：在应用程序被销毁之前触发这是清除以前所用资源的理想位置
        // Application_Error：当应用程序中遇到一个未处理的异常时，该事件被触发
        // Application_Start：在HttpApplication 类的第一个实例被创建时，该事件被触发它允许你创建可以由所有HttpApplication 实例访问的对象
        // Application_End：在HttpApplication 类的最后一个实例被销毁时，该事件被触发在一个应用程序的生命周期内它只被触发一次
        // Application_AuthenticateRequest：在安全模块建立起当前用户的有效的身份时，该事件被触发在这个时候，用户的凭据将会被验证
        // Application_AuthorizeRequest：当安全模块确认一个用户可以访问资源之后，该事件被触发
        // Session_Start：在一个新用户访问应用程序 Web 站点时，该事件被触发
        // Session_End：在一个用户的会话超时结束或他们离开应用程序 Web 站点时，该事件被触发
        // Application_BeginRequest：在接收到一个应用程序请求时触发对于一个请求来说，它是第一个被触发的事件，请求一般是用户输入的一个页面请求（URL）
        // Application_EndRequest：针对应用程序请求的最后一个事件
        // Application_PreRequestHandlerExecute：在 ASP.NET 页面框架开始执行诸如页面或 Web 服务之类的事件处理程序之前，该事件被触发
        // Application_PostRequestHandlerExecute：在 ASP.NET 页面框架结束执行一个事件处理程序时，该事件被触发
        // Applcation_PreSendRequestHeaders：在 ASP.NET 页面框架发送 HTTP 头给请求客户（浏览器）时，该事件被触发·Application_PreSendContent：在 ASP.NET 页面框架发送内容给请求客户（浏览器）时，该事件被触发
        // Application_AcquireRequestState：在 ASP.NET 页面框架得到与当前请求相关的当前状态（Session 状态）时，该事件被触发
        // Application_ReleaseRequestState：在 ASP.NET 页面框架执行完所有的事件处理程序时，该事件被触发这将导致所有的状态模块保存它们当前的状态数据
        // Application_ResolveRequestCache：在 ASP.NET 页面框架完成一个授权请求时，该事件被触发它允许缓存模块从缓存中为请求提供服务，从而绕过事件处理程序的执行
        // Application_UpdateRequestCache：在 ASP.NET 页面框架完成事件处理程序的执行时，该事件被触发，从而使缓存模块存储响应数据，以供响应后续的请求时使用
        // 这个事件列表看起来好像多得吓人，但是在不同环境下这些事件可能会非常有用
        // 使用这些事件的一个关键问题是知道它们被触发的顺序Application_Init 和Application_Start 事件在应用程序第一次启动时被触发一次相似地，
        // Application_Disposed 和 Application_End 事件在应用程序终止时被触发一次此外，
        // 基于会话的事件（Session_Start 和 Session_End）只在用户进入和离开站点时被使用其余的事件则处理应用程序请求，这些事件被触发的顺序是：
        // Application_BeginRequest
        // Application_AuthenticateRequest
        // Application_AuthorizeRequest
        // Application_ResolveRequestCache
        // Application_AcquireRequestState
        // Application_PreRequestHandlerExecute         
        // Application_PreSendRequestHeaders
        // Application_PreSendRequestContent
        // <<执行代码>>
        // Application_PostRequestHandlerExecute
        // Application_ReleaseRequestState
        // Application_UpdateRequestCache
        // Application_EndRequest

        /// <summary>
        /// 在HttpApplication 类的第一个实例被创建时，该事件被触发它允许你创建可以由所有HttpApplication 实例访问的对象
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //后台 定时 循环 捞数据
            HostingEnvironment.RegisterObject(new TimerDeltSequenceRedisKey());

            var Upload2DataBaseFtp = (bool)CacheHelper.Get_SetBoolConfAppSettings("Global", "Upload2DataBaseFtp");
            if (Upload2DataBaseFtp)
            {
                //每天后台 定时 循环 抛数据到数据仓库
                HostingEnvironment.RegisterObject(new DataBase_FL());
            }

            #region 获取基础数据 加载到Cache缓存

            WebdbContext dbContext = new WebdbContext();
            ApplicationDbContext AppdbContext = new ApplicationDbContext();
            var OHttpContext = HttpContext.Current;
            //缓存名称
            string CacheName = "";

            #region 特殊的Cahche-键值设置
            
            #region 帐户

            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCacheUser)
            {
                CacheHelper.Get_SetApplicationUser(AppdbContext, "Global");
            }
            
            #endregion

            #region 角色

            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCacheUser)
            {
                CacheHelper.Get_SetApplicationRole(AppdbContext, "Global");
            }

            #endregion

            #region Models.UserAuthorizeAttribute 中的 OnAuthorization 方法 的一些配置

            //在 CacheHelper中也必须做相应的设置
            //一个 controll对应 多个controll的权限
            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCachedictOnetoManyControllerName)
            {
                CacheHelper.Get_SetOnetoManyCntrlerName("Global");
            }

            //无需验证权限的页面
            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCacheNoQxValid)
            {
                CacheHelper.Get_SetOnetoManyCntrlerName("Global");
            }

            #endregion

            #region 缓存 dbContext 所有Member

            CacheName = "dbContextMember";
            lock (Common.lockCachePARA)
            {
                CacheHelper.Get_SetdbContextMember(dbContext, "Global", CacheName);
            }

            #endregion

            #region 缓存 网站所有反射类

            CacheName = "AllEntityAssembly";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.AllEntityAssembly);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存Notifications-Message类型

            CacheName = "Notification";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.Notification);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 BillFormulaXML

            CacheName = "BillFormulaXML";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.BillFormulaXML);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #endregion

            #region 缓存 UserOperatePointLink

            CacheName = "UserOperatePointLink";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.UserOperatePointLink);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存操作点

            CacheName = "OperatePoint";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.OperatePoint);
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存菜单

            CacheName = "MenuItem";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.MenuItem);
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存菜单动作

            CacheName = "MenuAction";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.MenuItem);
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 Message类型

            CacheName = "Notification";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.Notification);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 PARA_CUSTOMS

            CacheName = "PARA_Customs";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Customs);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 PARA_Package

            CacheName = "PARA_Package";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Package);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 PARA_COUNTRY

            CacheName = "PARA_Country";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Country);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 PARA_Area

            CacheName = "PARA_Area";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 PARA_CURR

            CacheName = "PARA_CURR";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 CusBusInfo

            CacheName = "CusBusInfo";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 QuotedPrice

            CacheName = "QuotedPrice";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.QuotedPrice);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 CustomerQuotedPrice

            CacheName = "CustomerQuotedPrice";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.CustomerQuotedPrice);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 CoPoKind

            CacheName = "CoPoKind";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.CoPoKind);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 TradeType

            CacheName = "TradeType";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.TradeType);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 Eai_Group

            CacheName = "Eai_Group";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.Eai_Group);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 FeeType

            CacheName = "FeeType";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 FeeUnit

            CacheName = "FeeUnit";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 Rate

            CacheName = "Rate";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.Rate);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #region 缓存 Saller

            CacheName = "Saller";
            try
            {
                CacheHelper.Get_SetCache(Common.CacheNameS.Saller);
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
            }

            #endregion

            #endregion

            #region 开启全文检索功能 分词库配置文件需要放置在 bin文件夹中 在Context中插入数据

            string ConfigName = "IsWriteDataToLunece";
            string IsWriteDataToLuneceStr = System.Configuration.ConfigurationManager.AppSettings[ConfigName] ?? "";
            bool IsWriteDataToLunece = Common.ChangStrToBool(IsWriteDataToLuneceStr);
            if (IsWriteDataToLunece)
                AirOut.Web.LuneceManager.IndexManager.OIndexManager.StartNewThread();
            CacheHelper.SetCache(ConfigName, IsWriteDataToLunece);

            #endregion

            #region 开启Redis缓存 在Context中插入数据

            ConfigName = "IsWriteDataToRedis";
            string IsWriteDataToRedisStr = System.Configuration.ConfigurationManager.AppSettings["IsWriteDataToRedis"] ?? "";
            bool IsWriteDataToRedis = Common.ChangStrToBool(IsWriteDataToRedisStr);
            if (IsWriteDataToRedis)
                AirOut.Web.RedisCacheManager.RedisManager.ORedisManager.StartNewThread();
            CacheHelper.SetCache(ConfigName, IsWriteDataToRedis);

            #endregion

            #region 异步写日志 数据暂存在Redis 队列

            ConfigName = "AsyncWriteLog";
            string AsyncWriteLogStr = System.Configuration.ConfigurationManager.AppSettings["AsyncWriteLog"] ?? "";
            bool AsyncWriteLog = Common.ChangStrToBool(AsyncWriteLogStr);
            if (AsyncWriteLog)
                AirOut.Web.AsyncWriteLogByRedis.AsyncWriteLog.OAsyncWriteLog.StartNewThread();
            CacheHelper.SetCache(ConfigName, AsyncWriteLogStr);

            #endregion
        }

        /// <summary>
        /// 当应用程序中遇到一个未处理的异常时，该事件被触发
        /// 自定义异常筛选器
        /// 在 App_Start/FilterConfig.cs中配置HandleErrorAttribute
        /// 也可以在Web.Config system.web 下设置（可以分配处理错误 控制器）
        ///<customErrors mode="On" defaultRedirect="~/Home/ServerError">
        ///  <error statusCode="404" redirect="~/Home/PageNotFound" />
        ///</customErrors>
        /// 先走 App_Start/FilterConfig.cs中配置的 ExceptionHandle-OnException=>BaseController-OnException=>Global-Application_Error
        /// View 访问的是 出错控制器下的 ViewName（默认Error），没有会去找 Shared 目录下的ViewName
        /// </summary>
        /// <param name="filterContext"></param>
        /// </summary>
        protected void Application_Error(Object sender, EventArgs e)
        {
            var raisedException = Server.GetLastError();
            //在Global.asax中调用Server.ClearError方法相当于是告诉Asp.Net系统抛出的异常已经被处理过了，不需要系统跳转到Asp.Net的错误黄页了。
            //如果想在Global.asax外调用ClearError方法可以使用HttpContext.Current.ApplicationInstance.Server.ClearError()。
            Server.ClearError();
            HttpContext httpContext = HttpContext.Current; 

            ////log4net
            //var dd = raisedException.StackTrace.Split(new char[] { '在' });
            //var query = dd.Where(s => s.IndexOf("位置") > 0 && s.IndexOf("行号") > 0).FirstOrDefault();
            //log4net.Config.XmlConfigurator.Configure();
            //ILog logWriter = log4net.LogManager.GetLogger(string.Empty);
            //logWriter.Error(query.ToString() + raisedException.Message + "\r\n" + "=======================================================");

            if (httpContext != null)
            {
                var ResponseStatusCode = Context.Response.StatusCode;
                string returnUrlStr = HttpContext.Current.Response.RedirectLocation == null ? "/Account/Login" : HttpContext.Current.Response.RedirectLocation;
                int returlIndex = returnUrlStr.ToLower().IndexOf("returnurl=");
                if (returlIndex > 0)
                {
                    returnUrlStr = returnUrlStr.Substring(returlIndex + 10);
                    if (returnUrlStr.IndexOf("&") > 0)
                    {
                        returnUrlStr = returnUrlStr.Substring(0, returnUrlStr.IndexOf("&"));
                    }
                }
                var HttpExcptStatusCode = (raisedException is HttpException) ? (raisedException as HttpException).GetHttpCode() : 500; //这里仅仅区分两种错误
                if (HttpExcptStatusCode == 404)
                {
                    httpContext.Response.Clear();
                    httpContext.Response.RedirectToRoute("Default", new { controller = "Home", action = "PageNotFound", returnUrl = returnUrlStr, ViewMsg = "系统异常:非常抱歉，您要访问的页面不存在。" });
                    httpContext.Response.End();
                    return;
                }

                string ErrMsg = "";
                try
                {
                    RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                    string controllerName = requestContext.RouteData.GetRequiredString("controller");
                    string actionName = requestContext.RouteData.GetRequiredString("action");
                    string content = string.Format("{0}-StackTrace:{1}", Common.GetExceptionMsg(raisedException), raisedException.StackTrace);
                    ErrMsg = content;

                    EventLogHelper.RawWrite(NotificationTag.Sys, "系统异常", controllerName, actionName, content, MessageType.Error);
                }
                catch (Exception ex)
                {
                    ErrMsg += " " + Common.GetExceptionMsg(ex);
                }
                if (string.IsNullOrEmpty(ErrMsg))
                    ErrMsg = "系统异常:非常抱歉，您要访问的页面出现了一些异常。";
                else
                    ErrMsg = "系统异常:" + ErrMsg;

                //当前请求 转换为 HttpRequestWrapper
                var httpRequestWrapper = new HttpRequestWrapper(System.Web.HttpContext.Current.Request);
                if (httpRequestWrapper.IsAjaxRequest())
                {
                    //httpContext.Response.Clear();
                    ////httpContext.Response.Headers = "";Transfer-Encoding
                    //httpContext.Response.Charset = "utf-8";
                    //httpContext.Response.ContentType = "application/json; charset=utf-8";
                    ////httpContext.Response.SuppressContent = true;
                    Response.StatusCode = 500;
                    httpContext.Response.Write("{\"Success\":false,\"ErrMsg\":\"" + ErrMsg.Replace("\"", "”") + "\"}");
                    Response.End();
                }
                else
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Home");
                    routeData.Values.Add("action", "ServerError");
                    routeData.Values.Add("exception", raisedException);

                    if (raisedException.GetType() == typeof(HttpException))
                    {
                        routeData.Values.Add("statusCode", ((HttpException)raisedException).GetHttpCode());
                    }
                    else
                    {
                        routeData.Values.Add("statusCode", 500);
                    }
                    routeData.Values.Add("returnUrl", returnUrlStr);
                    routeData.Values.Add("ViewMsg", ErrMsg);
                    //禁用IIS 自定义错误页面
                    Response.TrySkipIisCustomErrors = true;
                    var MyContainer = (Microsoft.Practices.Unity.IUnityContainer)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer();
                    var _UnitOfWork = MyContainer.Resolve<IUnitOfWorkAsync>();
                    IController controller = new AirOut.Web.Controllers.HomeController(_UnitOfWork);
                    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    Response.End();

                    //httpContext.Response.Clear();
                    //httpContext.Response.RedirectToRoute("Default", new { controller = "Home", action = "ServerError", returnUrl = returnUrlStr, ViewMsg = ErrMsg });
                    //httpContext.Response.Close();
                }
            }
        }

        /// <summary>
        /// 不是每次请求都调用
        /// 在HttpApplication 类的最后一个实例被销毁时，该事件被触发在一个应用程序的生命周期内它只被触发一次
        /// 比如IIS重启，文件更新，进程回收导致应用程序转换到另一个应用程序域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(object sender, EventArgs e)
        {
            #region 清除基础数据 Cache缓存

            var ArrCache = AirOut.Web.Extensions.Common.GetEnumToDic("CacheNameS");
            foreach (var item in ArrCache)
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Cache.Get(item.DisplayName) != null)
                        HttpContext.Current.Cache.Remove(item.DisplayName);
                }
            }

            #endregion
        }

        /// <summary>
        /// 在接收到一个应用程序请求时触发对于一个请求来说，它是第一个被触发的事件，请求一般是用户输入的一个页面请求（URL）
        /// </summary>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //var context = HttpContext.Current;
            //var ss = context.Request.QueryString;
            //var ss1 = context.Request.Url;
        }

        /// <summary>
        /// 针对应用程序请求的最后一个事件
        /// </summary>
        protected void Application_EndRequest()
        {
            //var statusCode = Context.Response.StatusCode;
            //var routingData = Context.Request.RequestContext.RouteData;
            //if (statusCode == 404 || statusCode == 500)
            //{
            //    Response.Clear();
            //    string returnUrlStr = HttpContext.Current.Response.RedirectLocation == null ? "/Home/Index" : HttpContext.Current.Response.RedirectLocation;
            //    if (statusCode == 404)
            //    {
            //        Response.RedirectToRoute("Default", new { controller = "Home", action = "PageNotFound", returnUrl = returnUrlStr, ViewMsg = "系统异常:我们非常抱歉，您要访问的页面不存在。" });
            //    }
            //    else if (statusCode == 500)
            //    {
            //        Response.RedirectToRoute("Default", new { controller = "Home", action = "ServerError", returnUrl = returnUrlStr, ViewMsg = "系统异常:我们非常抱歉，您要访问的页面出现了一些异常。" });
            //    }
            //}
        }

        /// <summary>
        /// 在一个新用户访问应用程序 Web 站点时，该事件被触发
        /// </summary>
        protected void Session_Start(object sender, EventArgs e)
        {
            ////会话状态已创建一个会话 ID,但由于响应已被应用程序刷新而无法保存它。
            //string SessionId = Session.SessionID;
        }

        /// <summary>
        /// 在一个用户的会话超时结束或他们离开应用程序 Web 站点时，该事件被触发
        /// </summary>
        public void Session_End()
        {

        }
    }
}
