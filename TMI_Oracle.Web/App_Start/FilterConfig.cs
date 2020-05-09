using System.Web;
using System.Web.Mvc;

namespace TMI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());//异常 异常筛选器
            filters.Add(new CustomerExceptionHandleFilter());//注册 异常筛选器
            filters.Add(new Models.UserAuthorizeAttribute());//注册 权限筛选器
        }
    }
}
