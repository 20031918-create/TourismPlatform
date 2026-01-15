using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // Get featured tour packages
            var featuredPackages = db.TourPackages
                .Where(tp => tp.IsActive && tp.AvailableSlots > 0)
                .OrderByDescending(tp => tp.CreatedDate)
                .Take(6)
                .ToList();

            ViewBag.FeaturedPackages = featuredPackages;

            // Group members information
            ViewBag.GroupMembers = new[]
            {
                new { StudentId = "12345678", FullName = "Student One" },
                new { StudentId = "23456789", FullName = "Student Two" },
                new { StudentId = "34567890", FullName = "Student Three" },
                new { StudentId = "45678901", FullName = "Student Four" }
            };

            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Redirect to appropriate dashboard based on user type
            if (user.UserType == "TravelAgency")
            {
                return RedirectToAction("Dashboard", "TravelAgency");
            }
            else if (user.UserType == "Tourist")
            {
                return RedirectToAction("Dashboard", "Tourist");
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Redirect to appropriate profile based on user type
            if (user.UserType == "TravelAgency")
            {
                return RedirectToAction("Profile", "TravelAgency");
            }
            else if (user.UserType == "Tourist")
            {
                return RedirectToAction("Profile", "Tourist");
            }

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
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