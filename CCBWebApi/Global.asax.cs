using CCBWebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CCBWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #region 异步写日志 数据暂存在Redis 队列

            string ConfigName = "CreateAsyncWriteLogThread";
            bool? CreateAsyncWriteLogThread = CacheHelper.Get_SetBoolConfAppSettings("Global", ConfigName);
            if (CreateAsyncWriteLogThread.HasValue && CreateAsyncWriteLogThread.Value)
                CCBWebApi.AsyncWriteLogByRedis.AsyncWriteLog.OAsyncWriteLog.StartNewThread();

            #endregion
        }

        /// <summary>
        /// 在接收到一个应用程序请求时触发对于一个请求来说，它是第一个被触发的事件，请求一般是用户输入的一个页面请求（URL）
        /// </summary>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                var context = HttpContext.Current;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
