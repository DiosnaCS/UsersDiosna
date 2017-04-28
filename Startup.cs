using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dubravica.Startup))]
namespace Dubravica
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
