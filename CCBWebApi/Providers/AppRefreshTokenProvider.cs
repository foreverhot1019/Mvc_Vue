using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CCBWebApi.Providers
{
    /// <summary>
    /// 刷新Token
    /// </summary>
    public class AppRefreshTokenProvider : IAuthenticationTokenProvider
    {
        //实际放到DB去存起來
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens =
            new ConcurrentDictionary<string, AuthenticationTicket>();

        /// <summary>
        /// 创建 刷新Token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            //copy properties and set the desired lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                //时间Ticket+上1小时
                ExpiresUtc = context.Ticket.Properties.ExpiresUtc.HasValue ? DateTimeOffset.UtcNow.AddHours(1) : context.Ticket.Properties.ExpiresUtc.Value.AddHours(1)
            };

            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            _refreshTokens.TryAdd(guid, refreshTokenTicket);
            context.SetToken(guid);

            await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 接收 刷新Token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;

            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
                //context.DeserializeTicket();
            }

            ////允许跨域
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            ////解密token
            //context.DeserializeTicket(context.Token);
            //if (context.Ticket == null)
            //{
            //    context.Response.StatusCode = 400; //Bad request
            //    context.Response.ContentType = "application/json";
            //    context.Response.ReasonPhrase = "Invalid Token";
            //    return;
            //}
            //if (context.Ticket.Properties.ExpiresUtc <= DateTime.UtcNow)
            //{
            //    context.Response.StatusCode = 401; //Unauthorized
            //    context.Response.ContentType = "application/json";
            //    context.Response.ReasonPhrase = "Unauthorized";
            //    return;
            //}
            //context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
            //context.SetTicket(context.Ticket);

            await Task.FromResult<object>(null);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}