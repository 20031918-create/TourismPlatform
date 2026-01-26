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
    public class TravelAgencyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Dashboard
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var agency = db.TravelAgencies.Find(userId);

            if (agency == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Keep as IQueryable (DO NOT ToList before using in bookings query)
            var tourQuery = db.TourPackages.Where(tp => tp.TravelAgencyId == userId);
            var tourIds = tourQuery.Select(tp => tp.Id);

            var bookingsQuery = db.Bookings.Where(b => tourIds.Contains(b.TourPackageId));

            // Stats
            ViewBag.TotalPackages = tourQuery.Count();
            ViewBag.ActivePackages = tourQuery.Count(tp => tp.IsActive);

            ViewBag.TotalBookings = bookingsQuery.Count();
            ViewBag.PendingBookings = bookingsQuery.Count(b => b.BookingStatus == "Pending");

            // Safe Sum (won't crash if there are 0 paid bookings)
            ViewBag.TotalRevenue = bookingsQuery
                .Where(b => b.PaymentStatus == "Paid")
                .Sum(b => (decimal?)b.TotalAmount) ?? 0m;

            ViewBag.RecentBookings = bookingsQuery
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToList();

            return View(agency);
        }

        // Profile Management (renamed to avoid warning CS0108)
        public ActionResult AgencyProfile()
        {
            var userId = User.Identity.GetUserId();
            var agency = db.TravelAgencies.Find(userId);

            if (agency == null)
            {
                return HttpNotFound();
            }

            return View("Profile", agency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgencyProfile(TravelAgency model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var userId = User.Identity.GetUserId();
            var agency = db.TravelAgencies.Find(userId);

            if (agency == null)
            {
                return HttpNotFound();
            }

            agency.AgencyName = model.AgencyName;
            agency.Description = model.Description;
            agency.ServicesOffered = model.ServicesOffered;
            agency.ContactNumber = model.ContactNumber;
            agency.Address = model.Address;
            agency.ProfileImageUrl = model.ProfileImageUrl;

            db.Entry(agency).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("AgencyProfile");
        }

        // Manage Bookings
        public ActionResult ManageBookings(string status = "All")
        {
            var userId = User.Identity.GetUserId();
            var tourPackageIds = db.TourPackages
                .Where(tp => tp.TravelAgencyId == userId)
                .Select(tp => tp.Id)
                .ToList();

            var bookingsQuery = db.Bookings
                .Include(b => b.Tourist.User)
                .Include(b => b.TourPackage)
                .Where(b => tourPackageIds.Contains(b.TourPackageId));

            if (status != "All")
            {
                bookingsQuery = bookingsQuery.Where(b => b.BookingStatus == status);
            }

            var bookings = bookingsQuery.OrderByDescending(b => b.BookingDate).ToList();

            ViewBag.Status = status;
            return View(bookings);
        }

        // Update Booking Status
        [HttpPost]
        public ActionResult UpdateBookingStatus(int bookingId, string status)
        {
            var booking = db.Bookings.Find(bookingId);
            if (booking == null)
            {
                return Json(new { success = false, message = "Booking not found" });
            }

            booking.BookingStatus = status;
            db.Entry(booking).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Booking status updated successfully" });
        }

        // View Feedbacks
        public ActionResult ViewFeedbacks()
        {
            var userId = User.Identity.GetUserId();
            var tourPackageIds = db.TourPackages
                .Where(tp => tp.TravelAgencyId == userId)
                .Select(tp => tp.Id)
                .ToList();

            var feedbacks = db.Feedbacks
                .Include(f => f.Booking.Tourist.User)
                .Include(f => f.Booking.TourPackage)
                .Where(f => tourPackageIds.Contains(f.Booking.TourPackageId))
                .OrderByDescending(f => f.FeedbackDate)
                .ToList();

            return View(feedbacks);
        }

        // Respond to Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RespondToFeedback(int feedbackId, string response)
        {
            var feedback = db.Feedbacks.Find(feedbackId);
            if (feedback == null)
            {
                return HttpNotFound();
            }

            feedback.AgencyResponse = response;
            feedback.ResponseDate = DateTime.Now;
            feedback.IsApproved = true;

            db.Entry(feedback).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Response submitted successfully!";
            return RedirectToAction("ViewFeedbacks");
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
