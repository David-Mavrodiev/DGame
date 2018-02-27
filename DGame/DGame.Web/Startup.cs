using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DGame.Web.Startup))]
namespace DGame.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
