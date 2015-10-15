using ExpenseManager.Web;
using ExpenseManager.Web.DatabaseContexts;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace ExpenseManager.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);            
            DatabaseInitializer.Initialize();
        }
    }
}