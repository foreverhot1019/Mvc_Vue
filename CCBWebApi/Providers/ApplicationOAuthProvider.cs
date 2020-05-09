using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CCBWebApi.Models;
using System.Collections.Concurrent;
using Microsoft.Owin.Security.Infrastructure;
using System.Security.Principal;

namespace CCBWebApi.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly string _name;
        private readonly string _ClientId;//客户端Key
        private readonly string _ClientSecret;//客户端密钥

        public ApplicationOAuthProvider(string publicClientId, string name = "access_token")
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
            _name = name;
            _ClientId = CacheHelper.Get_SetStringConfAppSettings("ApplicationOAuthProvider", "CCBKD_ClientId");
            _ClientSecret = CacheHelper.Get_SetStringConfAppSettings("ApplicationOAuthProvider", "CCBKD_ClientSecret");
        }

        /// <summary>
        /// 通过 Authorization-Code 生成access_token
        /// authorization_code 授权方式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                //产生 ClaimsIdentity 注册一些特有属性
                //ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ExternalBearer);
                //identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                //var props = new AuthenticationProperties()
                //{
                //    AllowRefresh = true,
                //    IssuedUtc = DateTime.UtcNow,
                //    ExpiresUtc = DateTime.UtcNow.AddMinutes(1),
                //};
                //var ticket = new AuthenticationTicket(identity, props);
                //context.Validated(ticket);
                context.Validated();
            });
        }

        /// <summary>
        /// 生成 access_token
        /// password/client_credentials 授权方式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ExternalBearer);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                var props = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(1),
                };
                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
            });

            //var guid = Guid.NewGuid().ToString();

            ////copy properties and set the desired lifetime of refresh token
            //var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            //{
            //    IssuedUtc = context.Ticket.Properties.IssuedUtc,
            //    ExpiresUtc = DateTime.UtcNow.AddMinutes(5) //SET DATETIME to 5 Minutes
            //    //ExpiresUtc = DateTime.UtcNow.AddMonths(3) 
            //};
            ///*CREATE A NEW TICKET WITH EXPIRATION TIME OF 5 MINUTES 
            // *INCLUDING THE VALUES OF THE CONTEXT TICKET: SO ALL WE 
            // *DO HERE IS TO ADD THE PROPERTIES IssuedUtc and 
            // *ExpiredUtc to the TICKET*/
            //var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            ////saving the new refreshTokenTicket to a local var of Type ConcurrentDictionary<string,AuthenticationTicket>
            //// consider storing only the hash of the handle
            //RefreshTokens.TryAdd(guid, refreshTokenTicket);
            //context.SetToken(guid);

            //var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket1);

            //var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            //if (user == null)
            //{
            //    context.SetError("invalid_grant", "用户名或密码不正确。");
            //    return;
            //}
            //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
            //   OAuthDefaults.AuthenticationType);
            //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
            //    CookieAuthenticationDefaults.AuthenticationType);
            //AuthenticationProperties properties = CreateProperties(user.UserName);
            //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            //context.Validated(ticket);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        /// <summary>
        /// 生成 access_token
        /// client_credentials 授权方式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                var identity = new ClaimsIdentity(new GenericIdentity(
                    context.ClientId, OAuthDefaults.AuthenticationType),
                    context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

                context.Validated(identity);
            });
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
                var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
                context.Validated(newTicket);
            });

            //return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 获取 access_token 认证策略请求地址
        /// 生成 authorization_code（authorization code 授权方式）
        /// 生成 access_token （implicit 授权模式）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 产生外部请求可使用的令牌-请求地址
        /// 生成 authorization_code（authorization code 授权方式）
        /// 生成 access_token （implicit 授权模式）
        /// </summary>
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            /*
             * response_type=token
             */
            if (context.AuthorizeRequest.IsImplicitGrantType)
            {
                //implicit 授权方式
                var identity = new ClaimsIdentity("Bearer");
                context.OwinContext.Authentication.SignIn(identity);
                context.RequestCompleted();
            }
            //response_type=code
            else if (context.AuthorizeRequest.IsAuthorizationCodeGrantType)
            {
                //authorization code 授权方式
                var redirectUri = context.Request.Query["redirect_uri"];
                var clientId = context.Request.Query["client_id"];
                var identity = new ClaimsIdentity(new GenericIdentity(clientId, OAuthDefaults.AuthenticationType));

                var authorizeCodeContext = new AuthenticationTokenCreateContext(
                    context.OwinContext,
                    context.Options.AuthorizationCodeFormat,
                    new AuthenticationTicket(
                        identity,
                        new AuthenticationProperties(new Dictionary<string, string>
                        {
                            {"client_id", clientId},
                            {"redirect_uri", redirectUri}
                        })
                        {
                            IssuedUtc = DateTimeOffset.UtcNow,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(context.Options.AuthorizationCodeExpireTimeSpan)
                        }));

                await context.Options.AuthorizationCodeProvider.CreateAsync(authorizeCodeContext);
                context.Response.Redirect(redirectUri + "?code=" + Uri.EscapeDataString(authorizeCodeContext.Token));
                context.RequestCompleted();
            }
        }

        /// <summary>
        /// 验证 client 信息
        /// 包括 主动让token过期等操作
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                string clientId;
                string clientSecret;
                if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                {
                    context.TryGetFormCredentials(out clientId, out clientSecret);
                }
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    context.SetError("clientId/clientSecret,not set;");
                }
                else if (clientId == _ClientId && clientSecret == _ClientSecret)
                {
                    context.Validated(clientId);
                }
                else
                {
                    context.SetError("clientId/clientSecret,not mapped;");
                }
            });
            //return Task.FromResult<object>(tf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _ClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");
                Uri RedirectUri = new Uri(context.RedirectUri);

                if (expectedRootUri.Host == RedirectUri.Host)
                {
                    context.Validated(context.RedirectUri);
                }
            }

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            var idt = context.Identity;

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            //var ArrClaim = context.OwinContext.Authentication.User.Claims;
            var value = context.Request.Query.Get(_name);
            if (!context.IsValidated)
                context.Validated();

            return Task.FromResult<object>(null);
        }

        public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 创建认证 自定义 属性
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }

    public class QueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;

        public QueryStringOAuthBearerProvider(string name)
        {
            _name = name;
        }

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get(_name);
            //var Authentication = context.OwinContext.Authentication;
            //AuthenticateResult OAuthenticateResult = null;
            //if (Authentication != null)
            //    OAuthenticateResult = Authentication.AuthenticateAsync(OAuthDefaults.AuthenticationType).Result;
            //if (OAuthenticateResult != null)
            //{
            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }
            else
            {
                //var Token = context.Request.Headers["Authorization"] ?? "";
                //if (Token.Length>7)
                //context.Token = Token.Substring(7);
                base.RequestToken(context);
            }
            //}
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 验证令牌是否有效(令牌没有过期时)
        /// 可在此处 增加 拦截验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            return base.ValidateIdentity(context);
        }
    }
}