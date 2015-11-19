using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ExpenseManager.Database;
using ExpenseManager.Web.Binders;

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
            MappingConfig.RegisterMappings();

            // Initialize Database
            RegisterContexts.Register();
            Database.Database.Initialize();

            // Model type binders
            ModelBinders.Binders.Add(typeof (decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof (DateTime), new DateModelBinder());
            ModelBinders.Binders.Add(typeof (DateTime?), new DateModelBinder());
        }
    }
}