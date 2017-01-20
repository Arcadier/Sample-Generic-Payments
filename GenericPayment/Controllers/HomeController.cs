using System.Web.Mvc;

namespace GenericPayment.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View("Welcome");
        }
    }
}