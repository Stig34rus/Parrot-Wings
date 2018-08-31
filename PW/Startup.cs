using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PW.Startup))]
namespace PW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
