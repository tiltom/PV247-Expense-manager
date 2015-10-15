using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExpenseManager.Web.Startup))]
namespace ExpenseManager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
