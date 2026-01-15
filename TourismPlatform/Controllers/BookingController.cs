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

        // Create Booking
        public ActionResult Create(int? tourPackageId)
        {
            if (tourPackageId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tourPackage = db.TourPackages
                .Include(t => t.TravelAgency)
                .FirstOrDefault(t => t.Id == tourPackageId);

            if (tourPackage == null)
            {
                return HttpNotFound();
            }

            if (tourPackage.AvailableSlots <= 0)
            {
                TempData["Error"] = "Sorry, this tour package is fully booked.";
                return RedirectToAction("Details", "TourPackage", new { id = tourPackageId });
            }

            ViewBag.TourPackage = tourPackage;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var tourPackage = db.TourPackages.Find(booking.TourPackageId);
                if (tourPackage == null)
                {
                    return HttpNotFound();
                }

                // Validate available slots
                if (booking.NumberOfParticipants > tourPackage.AvailableSlots)
                {
                    ModelState.AddModelError("NumberOfParticipants",
                        $"Only {tourPackage.AvailableSlots} slots are available.");
                    ViewBag.TourPackage = tourPackage;
                    return View(booking);
                }

                // Calculate total amount
                booking.TouristId = User.Identity.GetUserId();
                booking.TotalAmount = tourPackage.PricePerPerson * booking.NumberOfParticipants;
                booking.BookingDate = DateTime.Now;
                booking.BookingStatus = "Pending";
                booking.PaymentStatus = "Pending";

                // Update available slots
                tourPackage.AvailableSlots -= booking.NumberOfParticipants;

                db.Bookings.Add(booking);
                db.Entry(tourPackage).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Booking created successfully! Please proceed with payment.";
                return RedirectToAction("Payment", new { id = booking.Id });
            }

            var tourPkg = db.TourPackages
                .Include(t => t.TravelAgency)
                .FirstOrDefault(t => t.Id == booking.TourPackageId);
            ViewBag.TourPackage = tourPkg;

            return View(booking);
        }

        // Payment (simplified for demo)
        public ActionResult Payment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var booking = db.Bookings
                .Include(b => b.TourPackage)
                .Include(b => b.Tourist.User)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Ensure booking belongs to current user
            if (booking.TouristId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPayment(int bookingId)
        {
            var booking = db.Bookings.Find(bookingId);
            if (booking == null || booking.TouristId != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }

            // Simulate payment processing
            booking.PaymentStatus = "Paid";
            booking.BookingStatus = "Confirmed";

            db.Entry(booking).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Payment successful! Your booking is confirmed.";
            return RedirectToAction("MyBookings", "Tourist");
        }

        // Booking Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var booking = db.Bookings
                .Include(b => b.TourPackage.TravelAgency)
                .Include(b => b.Tourist.User)
                .Include(b => b.Feedback)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            // Check authorization
            if (user.UserType == "Tourist" && booking.TouristId != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            else if (user.UserType == "TravelAgency" && booking.TourPackage.TravelAgencyId != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(booking);
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