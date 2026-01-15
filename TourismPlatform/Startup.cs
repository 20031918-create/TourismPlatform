using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TourismPlatform.Startup))]
namespace TourismPlatform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
