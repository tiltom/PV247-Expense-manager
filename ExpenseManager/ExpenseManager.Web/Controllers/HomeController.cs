using System.Web.Mvc;

namespace ExpenseManager.Web.Controllers
{
    [RequireHttps]
    public class HomeController : AbstractController
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}