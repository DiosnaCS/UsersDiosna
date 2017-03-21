using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UsersDiosna.Startup))]
namespace UsersDiosna
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
