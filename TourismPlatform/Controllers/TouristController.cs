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
    public class TouristController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Tourist Dashboard
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var tourist = db.Tourists.Find(userId);

            if (tourist == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bookings = db.Bookings
                .Include(b => b.TourPackage)
                .Where(b => b.TouristId == userId)
                .ToList();

            ViewBag.TotalBookings = bookings.Count;
            ViewBag.UpcomingBookings = bookings.Count(b => b.BookingStatus == "Confirmed" && b.TourPackage.StartDate > DateTime.Now);
            ViewBag.CompletedBookings = bookings.Count(b => b.BookingStatus == "Completed");
            ViewBag.TotalSpent = bookings.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalAmount);

            ViewBag.RecentBookings = bookings.OrderByDescending(b => b.BookingDate).Take(5).ToList();

            return View(tourist);
        }

        // My Bookings
        public ActionResult MyBookings(string status = "All")
        {
            var userId = User.Identity.GetUserId();
            var bookingsQuery = db.Bookings
                .Include(b => b.TourPackage.TravelAgency)
                .Include(b => b.Feedback)
                .Where(b => b.TouristId == userId);

            if (status != "All")
            {
                bookingsQuery = bookingsQuery.Where(b => b.BookingStatus == status);
            }

            var bookings = bookingsQuery.OrderByDescending(b => b.BookingDate).ToList();

            ViewBag.Status = status;
            return View(bookings);
        }

        // Profile Management
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            var tourist = db.Tourists
                .Include(t => t.User)
                .FirstOrDefault(t => t.Id == userId);

            if (tourist == null)
            {
                return HttpNotFound();
            }

            return View(tourist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(Tourist model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var tourist = db.Tourists.Find(userId);

                if (tourist == null)
                {
                    return HttpNotFound();
                }

                tourist.ContactNumber = model.ContactNumber;
                tourist.Nationality = model.Nationality;
                tourist.DateOfBirth = model.DateOfBirth;
                tourist.ProfileImageUrl = model.ProfileImageUrl;

                db.Entry(tourist).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        // Submit Feedback
        public ActionResult SubmitFeedback(int? bookingId)
        {
            if (bookingId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var booking = db.Bookings
                .Include(b => b.TourPackage)
                .Include(b => b.Feedback)
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Check if booking belongs to current user
            if (booking.TouristId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Check if feedback already exists
            if (booking.Feedback != null)
            {
                TempData["Error"] = "You have already submitted feedback for this booking.";
                return RedirectToAction("MyBookings");
            }

            // Check if booking is completed
            if (booking.BookingStatus != "Completed")
            {
                TempData["Error"] = "You can only submit feedback for completed bookings.";
                return RedirectToAction("MyBookings");
            }

            ViewBag.Booking = booking;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                var booking = db.Bookings.Find(feedback.BookingId);
                if (booking == null || booking.TouristId != User.Identity.GetUserId())
                {
                    return HttpNotFound();
                }

                feedback.FeedbackDate = DateTime.Now;
                feedback.IsApproved = false;

                db.Feedbacks.Add(feedback);
                db.SaveChanges();

                TempData["Success"] = "Feedback submitted successfully!";
                return RedirectToAction("MyBookings");
            }

            var bookingData = db.Bookings
                .Include(b => b.TourPackage)
                .FirstOrDefault(b => b.Id == feedback.BookingId);
            ViewBag.Booking = bookingData;

            return View(feedback);
        }

        // Cancel Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(int bookingId, string reason)
        {
            var booking = db.Bookings.Find(bookingId);
            if (booking == null || booking.TouristId != User.Identity.GetUserId())
            {
                return Json(new { success = false, message = "Booking not found" });
            }

            if (booking.BookingStatus == "Completed" || booking.BookingStatus == "Cancelled")
            {
                return Json(new { success = false, message = "Cannot cancel this booking" });
            }

            booking.BookingStatus = "Cancelled";
            booking.CancellationReason = reason;

            // Restore available slots
            var tourPackage = db.TourPackages.Find(booking.TourPackageId);
            if (tourPackage != null)
            {
                tourPackage.AvailableSlots += booking.NumberOfParticipants;
            }

            db.Entry(booking).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Booking cancelled successfully" });
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