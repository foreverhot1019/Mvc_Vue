using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TMI.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.asmx/{*pathInfo}");
            //routes.RouteExistingFiles = true;
            routes.MapMvcAttributeRoutes();//允许Action中制定Route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //    name: "RouteToHome",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    name: "HomeNoQXErrRoute",
            //    url: "Home/NoQXErr/{id}",
            //    defaults: new { controller = "Home", action = "NoQXErr", id = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    name: "LoginRoute",
            //    url: "Account/Login/{id}",
            //    defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            //);
            ////防盗文件
            //routes.MapRoute(
            //    name: "DownLoad",
            //    url: "DownLoad/{filename}"
            //).RouteHandler = new DownLoadRouteHandler();
            //routes.MapRoute(
            //    name: "DiskFile",
            //    url: "DownLoad/20160401083022175.xls",
            //    defaults: new { controller = "Customer", action = "List", }).RouteHandler = new DownLoadRouteHandler();
            ////防止盗图
            //routes.Add("ImagesRoute", new Route("imgs/{filename}", new ImageRouteHandler()));
        }
    }
    public class DownLoadRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ImageHandler(requestContext);
        }
    }

    #region 防止盗图

    public class ImageRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ImageHandler(requestContext);
        }
    }

    public class ImageHandler : IHttpHandler
    {
        public ImageHandler(RequestContext context)
        {
            ProcessRequest(context);
        }

        /// <summary>
        /// 分析 访问图片Http头信息
        /// </summary>
        /// <param name="requestContext"></param>
        private static void ProcessRequest(RequestContext requestContext)
        {
            // 获取文件服务器端物理路径
            string FileName = requestContext.HttpContext.Server.MapPath(requestContext.HttpContext.Request.FilePath);

            //指定请求的服务器的域名和端口号
            var HostAddress = requestContext.HttpContext.Request.Url.Host;//.UserHostAddress;

            var response = requestContext.HttpContext.Response;
            var request = requestContext.HttpContext.Request;
            var server = requestContext.HttpContext.Server;
            var validRequestFile = requestContext.RouteData.Values["filename"].ToString();
            //const string invalidRequestFile = "thief.gif";
            var path = server.MapPath("~/graphics/");

            //浏览器向 WEB 服务器表明自己是从哪个 网页/URL 获得/点击 当前请求中的网址/URL
            if (request.ServerVariables["HTTP_REFERER"] != null && request.UrlReferrer.Host.ToLower() != HostAddress.ToLower())
            {
                response.Clear();
                response.ContentType = GetContentType(request.Url.ToString());
                //response.TransmitFile(path + validRequestFile);
                response.End();
            }
            else
            {
                //response.Clear();
                //response.ContentType = GetContentType(request.Url.ToString());
                ////response.TransmitFile(path + invalidRequestFile);
                //response.End();
            }
        }

        private static string GetContentType(string url)
        {
            switch (System.IO.Path.GetExtension(url))
            {
                case ".gif":
                    return "Image/gif";
                case ".jpg":
                    return "Image/jpeg";
                case ".png":
                    return "Image/png";
                default:
                    break;
            }
            return null;
        }

        public void ProcessRequest(HttpContext context)
        {
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    #endregion
}
