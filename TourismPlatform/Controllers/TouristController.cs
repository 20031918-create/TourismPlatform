using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    [Authorize]
    public class TouristController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();

            // load tourist + user
            var tourist = db.Tourists
                .Include(t => t.User)
                .FirstOrDefault(t => t.Id == userId);

            if (tourist == null)
            {
                return RedirectToAction("Create", "Tourist");
            }

            // load bookings for this tourist
            var bookings = db.Bookings
                .Include(b => b.TourPackage)
                .Where(b => b.TouristId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            // ✅ These names MUST match your Dashboard.cshtml
            ViewBag.TotalBookings = bookings.Count;

            // treat Pending + Confirmed as upcoming
            ViewBag.UpcomingBookings = bookings.Count(b =>
                b.BookingStatus == "Pending" || b.BookingStatus == "Confirmed");

            ViewBag.CompletedBookings = bookings.Count(b => b.BookingStatus == "Completed");

            // ✅ Your model uses TotalAmount (NOT TotalPrice)
            ViewBag.TotalSpent = bookings.Sum(b => (decimal?)b.TotalAmount) ?? 0m;

            // recent bookings (top 5)
            ViewBag.RecentBookings = bookings.Take(5).ToList();

            // view expects Tourist model
            return View(tourist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
