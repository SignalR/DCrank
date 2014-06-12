using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Controller.Startup))]
namespace Controller
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
