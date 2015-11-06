﻿using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ExpenseManager.Web.Binders;
using ExpenseManager.Web.DatabaseContexts;

namespace ExpenseManager.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MappingConfig.RegisterMappings();
            DatabaseInitializer.Initialize();
            ModelBinders.Binders.Add(typeof (decimal), new DecimalModelBinder());
        }
    }
}