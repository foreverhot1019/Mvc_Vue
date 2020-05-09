using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AirOut.Web.Controllers
{
    /// <summary>  
    /// 自定义AntiForgeryToken校验  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomerValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        internal Action ValidateAction
        {
            get;
            private set;
        }

        public CustomerValidateAntiForgeryTokenAttribute()
            : this(new Action(AntiForgery.Validate))
        {
        }

        internal CustomerValidateAntiForgeryTokenAttribute(Action validateAction)
        {
            this.ValidateAction = validateAction;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            try
            {
                this.ValidateAction();
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("/Account/Login", true);
                return;
            }
        }
    }

    public class LoginAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            return base.AuthorizeCore(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string userName = filterContext.HttpContext.User.Identity.Name;
            if (userName != null)
            {
                filterContext.Result = new RedirectResult("/home", true);
            }

            base.OnAuthorization(filterContext);

        }
    }
}