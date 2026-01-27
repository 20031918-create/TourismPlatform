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

            // send tour to view for display
            ViewBag.Tour = tour;

            // prepare model
            var model = new Booking
            {
                TourPackageId = tour.Id,
                NumberOfParticipants = 1,
                TotalAmount = tour.PricePerPerson * 1
            };

            return View(model);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Booking booking)
        {
            // TourPackageId MUST exist
            var tour = db.TourPackages.FirstOrDefault(t => t.Id == booking.TourPackageId);
            if (tour == null)
            {
                ModelState.AddModelError("", "Selected tour package does not exist.");
                return View(booking);
            }

            // For redisplay if validation fails
            ViewBag.Tour = tour;

            // Set required fields that should NOT come from the form
            var userId = User.Identity.GetUserId();
            booking.TouristId = userId;
            booking.BookingDate = DateTime.Now;

            // IMPORTANT: remove required fields from ModelState that are set server-side
            ModelState.Remove("TouristId");

            // Basic validation
            if (booking.NumberOfParticipants < 1)
                ModelState.AddModelError("NumberOfParticipants", "Number of participants must be at least 1.");

            if (booking.NumberOfParticipants > tour.AvailableSlots)
                ModelState.AddModelError("NumberOfParticipants", "Not enough available slots for this tour.");

            // Calculate amount on server (never trust client)
            booking.TotalAmount = tour.PricePerPerson * booking.NumberOfParticipants;

            if (!ModelState.IsValid)
            {
                // stay on page, show validation
                return View(booking);
            }

            // Reduce slots
            tour.AvailableSlots -= booking.NumberOfParticipants;

            // Save booking
            db.Bookings.Add(booking);
            db.SaveChanges();

            // Redirect to confirmation
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

            // Security: ensure user can only see their own booking
            var userId = User.Identity.GetUserId();
            if (booking.TouristId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(booking);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
