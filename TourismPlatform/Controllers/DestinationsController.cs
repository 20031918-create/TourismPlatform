using System.Web.Mvc;

namespace TourismPlatform.Controllers
{
    public class DestinationsController : Controller
    {
        // GET: Destinations
        public ActionResult Index()
        {
            return View();
        }

        // GET: Destinations/Details?name=Newcastle
        public ActionResult Details(string name)
        {
            ViewBag.Name = name;
            return View();
        }
    }
}
