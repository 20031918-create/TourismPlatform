using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // Featured packages = active, available, and upcoming tours
            var featured = db.TourPackages
                .Include(t => t.TravelAgency)
                .Where(t => t.IsActive && t.AvailableSlots > 0 && t.StartDate >= DateTime.Today)
                .OrderByDescending(t => t.CreatedDate)
                .Take(6)
                .ToList();

            // Fallback in case your test data has past StartDates
            if (!featured.Any())
            {
                featured = db.TourPackages
                    .Include(t => t.TravelAgency)
                    .Where(t => t.IsActive && t.AvailableSlots > 0)
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(6)
                    .ToList();
            }

            ViewBag.FeaturedPackages = featured;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
