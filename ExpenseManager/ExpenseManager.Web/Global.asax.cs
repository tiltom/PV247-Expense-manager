using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ExpenseManager.Web.Binders;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Database;

namespace ExpenseManager.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Initialize MVC
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Initialize Database
            RegisterContexts.Register();
            Database.Database.SetDatabaseInitializer();

            // Model type binders
            ModelBinders.Binders.Add(typeof (decimal), new DecimalModelBinder());
        }
    }
}