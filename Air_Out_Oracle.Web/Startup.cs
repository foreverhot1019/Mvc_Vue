using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AirOut.Web.Startup))]
namespace AirOut.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
