using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

namespace ExpenseManager.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Id", "{controller}/{action}/{id}", null, new {id = new GuidRouteConstraint()}
                );

            routes.MapRoute("Default", "{controller}/{action}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}