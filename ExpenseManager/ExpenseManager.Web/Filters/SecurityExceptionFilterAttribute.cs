using System.Net;
using System.Security;
using System.Web.Mvc;
using ExpenseManager.Resources.TransactionResources;

namespace ExpenseManager.Web.Filters
{
    public class SecurityExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            if (filterContext.Exception is SecurityException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    TransactionResource.PermissionError);
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}