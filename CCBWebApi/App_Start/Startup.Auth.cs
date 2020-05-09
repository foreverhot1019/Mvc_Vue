using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using CCBWebApi.Providers;
using CCBWebApi.Models;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace CCBWebApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static CookieAuthenticationOptions OAuthAppCookieOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public static int TokenExpireTimeSpan
        {
            get
            {
                var strTokenExpireTimeSpan = CacheHelper.Get_SetStringConfAppSettings("Startup", "TokenExpireTimeSpan");
                int ret = 5;
                int.TryParse(strTokenExpireTimeSpan, out ret);
                return ret;
            }
        }

        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // 将数据库上下文和用户管理器配置为对每个请求使用单个实例
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            #region 验证客户端token 是否过期等设置

            #region Cookie-Authentication

            //验证cookie 是否过期等设置
            OAuthAppCookieOptions = new CookieAuthenticationOptions
            {
                //设置默认 哪种类型时，走这个验证
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(TokenExpireTimeSpan),
                //CookieSecure = CookieSecureOption.Never,
                LoginPath = new PathString("/Test/Login"),
                LogoutPath = new PathString("/Home/Index"),
                Provider = new AuthorizationCookieProvider("Michael_Cookie"),
                /* 
                 * 解密
                 * var AccessIdentity = Startup.OAuthAppCookieOptions.TicketDataFormat.Unprotect(access_token);
                 * 
                 * UseCookieAuthentication 验证 token 
                 * ValidateIdentity(CookieValidateIdentityContext context)
                 */
            };
            // 使应用程序可以使用 Cookie 来存储已登录用户的信息
            app.UseCookieAuthentication(OAuthAppCookieOptions);
            //// Enable the External Sign In Cookie.
            //app.SetDefaultSignInAsAuthenticationType("External");
            // 并使用 Cookie 来临时存储有关使用第三方登录提供程序登录的用户的信息
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            #endregion

            #region Bearer-Authentication

            /*
             * 通过 Bearer-Authentication进来的验证都会走以下代码
             * url-参数 access_token不记名令牌来验证用户身份
            */
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
                //从url中获取token，兼容hearder方式
                Provider = new QueryStringOAuthBearerProvider("access_token")
            });

            #endregion

            #endregion

            #region 产生token-服务端

            // 针对基于 OAuth 的流配置应用程序
            PublicClientId = "Michael_Bearer";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AuthenticationType = OAuthDefaults.AuthenticationType,
                AllowInsecureHttp = true,//允许http请求及Url中验证token,在生产模式下设 false
                AuthenticationMode = AuthenticationMode.Active,//OWin中间件，可修改Response
                TokenEndpointPath = new PathString("/Token"),//获取 access_token 认证策略请求地址
                /*产生外部请求可使用的令牌-请求地址*/
                AuthorizeEndpointPath = new PathString("/Authoriza"),// new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(TokenExpireTimeSpan),//access_token 过期时间
                Provider = new ApplicationOAuthProvider(PublicClientId),//access_token 认证策略
                AuthorizationCodeProvider = new OpenAuthorizationCodeProvider(), //authorization_code 认证策略 授权码，授权服务端和客户端之间传输。
                RefreshTokenProvider = new AppRefreshTokenProvider(),//refresh_token 认证策略
                //var AccessIdentity = Startup.OAuthOptions.AccessTokenFormat.Unprotect(access_token);//解密
            };
            /* 
             * 使应用程序可以使用不记名令牌来验证用户身份 服务端
             * 用来产生 token 令牌，以及一些产生令牌所需的验证
            */
            //app.UseOAuthBearerTokens(OAuthOptions);//只是加了一个AuthenticationType 其他与UseOAuthAuthorizationServer一致
            app.UseOAuthAuthorizationServer(OAuthOptions);
            /*验证流程
             * grant_type - client_credentials & response_type:'token'
             * ValidateClientAuthentication
             * ValidateTokenRequest
             * GrantClientCredentials
             * TokenEndpoint
             * TokenEndpointResponse
             * 
             * grant_type - password & response_type:'token'
             * ValidateClientAuthentication
             * ValidateTokenRequest
             * GrantResourceOwnerCredentials
             * TokenEndpoint
             * TokenEndpointResponse
             * 
             * grant_type - password & response_type:'code'
             * ValidateClientRedirectUri
             * ValidateAuthorizeRequest
             * AuthorizeEndpoint
             */


            /* 
             * Install-Package System.IdentityModel.Tokens.Jwt -Version 5.4.0
             * JSON Web Tokens
             * app.UseJWTOAuthAuthorizationServer()
            */

            /*
             * 为了能让资源服务器识别认证服务器颁发的令牌， 需要配置两个应用的 machineConfig 为相同的 key ， 如下所示：
             *  <machineKey
             *  decryptionKey="C11B54C2F10E4689AC59A84F79CDB494AE326344F26B1DC5"
             *  validation="SHA1"
             *  validationKey="7E1457A6E6DF475AA972D2106C0A2C3A44BC023F3E274B6FB598A1265C3C5374EA17DC9669C143BDB125E319164438974061AFCAA42A4478A07C3EA093517A48" />
             */

            #endregion

            // 取消注释以下行可允许使用第三方登录提供程序登录
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}
