using System.Web.Mvc;

namespace ColorInteraction.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Join ColorInteraction.";

            return View();
        }
    }
}