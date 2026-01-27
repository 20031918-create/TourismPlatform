using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // Home page + Featured Tour Packages
        public ActionResult Index()
        {
            // Featured packages = active, available, and upcoming tours
            var featured = db.TourPackages
                .Include(t => t.TravelAgency)
                .Where(t => t.IsActive && t.AvailableSlots > 0 && t.StartDate >= DateTime.Today)
                .OrderByDescending(t => t.CreatedDate)
                .Take(6)
                .ToList();

            // Fallback in case sample data contains past dates
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

        // Dashboard redirect based on profile type
        [Authorize]
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();

            // If user has a Tourist profile -> Tourist dashboard
            if (db.Tourists.Any(t => t.Id == userId))
                return RedirectToAction("Dashboard", "Tourist");

            // If user has a TravelAgency profile -> Agency dashboard
            if (db.TravelAgencies.Any(a => a.Id == userId))
                return RedirectToAction("Dashboard", "TravelAgency");

            // Fallback (if profile not created for some reason)
            return RedirectToAction("Index", "Home");
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
