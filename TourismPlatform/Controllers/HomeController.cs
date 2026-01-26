using System.Web.Mvc;

namespace TourismPlatform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

       
        public ActionResult Review()
        {
            return View();
        }

        
        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
