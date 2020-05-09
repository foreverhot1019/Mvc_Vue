using System.Web;
using System.Web.Mvc;

namespace CCBWebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());//异常 异常筛选器
            filters.Add(new Models.UserAuthorizationAttr());//注册 权限筛选器
        }
    }
}