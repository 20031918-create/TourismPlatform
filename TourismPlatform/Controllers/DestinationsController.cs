using System.Web.Mvc;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class DestinationsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Destination destination = null;

            switch (id.Value)
            {
                case 1:
                    destination = new Destination
                    {
                        Name = "Sydney",
                        Description = "....",
                        ImageUrl = "~/Images/Sydney.jpg",
                        Attractions = "....",
                        TourPrice = 1200
                    };
                    break;

                case 2:
                    destination = new Destination
                    {
                        Name = "Newcastle",
                        Description = "....",
                        ImageUrl = "~/Images/Newcastle.jpg",
                        Attractions = "....",
                        TourPrice = 900
                    };
                    break;

                case 3:
                    destination = new Destination
                    {
                        Name = "Perth",
                        Description = "....",
                        ImageUrl = "~/Images/Perth.jpg",
                        Attractions = "....",
                        TourPrice = 1100
                    };
                    break;
            }

            if (destination == null)
                return HttpNotFound();

            return View(destination);
        }
    }
}
