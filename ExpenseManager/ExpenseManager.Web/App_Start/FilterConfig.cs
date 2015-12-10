using System.Web.Mvc;
using ExpenseManager.Web.Filters;

namespace ExpenseManager.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new SecurityExceptionFilterAttribute());
        }
    }
}