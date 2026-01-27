using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Booking/Create?tourPackageId=2
        public ActionResult Create(int? tourPackageId)
        {
            if (tourPackageId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tour = db.TourPackages
                .Include(t => t.TravelAgency)
                .FirstOrDefault(t => t.Id == tourPackageId.Value);

            if (tour == null)
                return HttpNotFound();

            // Always pass a model to the view
            var model = new Booking
            {
                TourPackageId = tour.Id,
                NumberOfParticipants = 1,
                TotalAmount = tour.PricePerPerson * 1
            };

            ViewBag.Tour = tour;
            return View(model);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Booking booking)
        {
            if (booking == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Reload tour (needed for validation + pricing + redisplay)
            var tour = db.TourPackages.FirstOrDefault(t => t.Id == booking.TourPackageId);
            if (tour == null)
                return HttpNotFound();

            // Important: keep tour for redisplay when errors happen
            ViewBag.Tour = tour;

            var userId = User.Identity.GetUserId();

            // Ensure Tourist row exists (prevents FK / DbUpdateException)
            var touristExists = db.Tourists.Any(t => t.Id == userId);
            if (!touristExists)
            {
                db.Tourists.Add(new Tourist
                {
                    Id = userId,
                    RegistrationDate = DateTime.Now
                });
                db.SaveChanges();
            }

            // Attach logged-in user to booking
            booking.TouristId = userId;

            // Calculate total server-side
            booking.TotalAmount = tour.PricePerPerson * booking.NumberOfParticipants;

            // Validation
            if (booking.NumberOfParticipants < 1 || booking.NumberOfParticipants > 50)
                ModelState.AddModelError("NumberOfParticipants", "Number of participants must be between 1 and 50.");

            if (booking.NumberOfParticipants > tour.AvailableSlots)
                ModelState.AddModelError("NumberOfParticipants", "Not enough available slots for this tour.");

            if (!ModelState.IsValid)
            {
                // Return the same model back so the view doesn't get null
                return View(booking);
            }

            // Reduce slots
            tour.AvailableSlots -= booking.NumberOfParticipants;

            db.Bookings.Add(booking);
            db.SaveChanges();

            return RedirectToAction("Confirmation", new { id = booking.Id });
        }

        // GET: Booking/Confirmation/5
        public ActionResult Confirmation(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.Bookings
                .Include(b => b.TourPackage)
                .FirstOrDefault(b => b.Id == id.Value);

            if (booking == null)
                return HttpNotFound();

            return View(booking);
        }

        // GET: Booking/Details/5 (optional, used by dashboard "eye" button)
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.Bookings
                .Include(b => b.TourPackage)
                .FirstOrDefault(b => b.Id == id.Value);

            if (booking == null)
                return HttpNotFound();

            return View(booking);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
