using System.Web.Mvc;
using ExpenseManager.BusinessLogic;

namespace ExpenseManager.Web.Helpers
{
    public static class MvcValidationExtension
    {
        public static void AddModelErrors(this ModelStateDictionary state,
            ServiceValidationException exception)
        {
            foreach (var error in exception.Errors)
            {
                state.AddModelError(error.Key, error.Value);
            }
        }
    }
}