using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace CCBWebApi.Providers
{
    public class AuthorizationCookieProvider : CookieAuthenticationProvider
    {
        private readonly string _publicClientId;

        public AuthorizationCookieProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        /// <summary>
        /// 验证令牌是否有效
        /// 可在此处 增加 拦截验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateIdentity(CookieValidateIdentityContext context)
        {
            return base.ValidateIdentity(context);
        }
    }
}