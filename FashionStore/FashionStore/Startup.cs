using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FashionStore.Startup))]
namespace FashionStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
