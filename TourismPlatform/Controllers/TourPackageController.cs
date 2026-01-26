using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class TourPackageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // -----------------------------
        // PUBLIC: Browse all active packages
        // -----------------------------
        [AllowAnonymous]
        public ActionResult Index(string searchString, string destination)
        {
            var packages = db.TourPackages
                .Include(t => t.TravelAgency)
                .Where(t => t.IsActive && t.AvailableSlots > 0);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                packages = packages.Where(p =>
                    p.PackageName.Contains(searchString) ||
                    p.Description.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(destination))
            {
                packages = packages.Where(p => p.Destination.Contains(destination));
            }

            ViewBag.Destinations = db.TourPackages
                .Where(t => t.IsActive)
                .Select(t => t.Destination)
                .Distinct()
                .ToList();

            return View(packages.OrderByDescending(p => p.CreatedDate).ToList());
        }

        // -----------------------------
        // PUBLIC: Package details
        // -----------------------------
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tourPackage = db.TourPackages
                .Include(t => t.TravelAgency.User)
                .FirstOrDefault(t => t.Id == id);

            if (tourPackage == null)
                return HttpNotFound();

            var feedbacks = db.Feedbacks
                .Include(f => f.Booking.Tourist.User)
                .Where(f => f.Booking.TourPackageId == id && f.IsApproved)
                .OrderByDescending(f => f.FeedbackDate)
                .ToList();

            ViewBag.Feedbacks = feedbacks;
            ViewBag.AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;

            return View(tourPackage);
        }

        // -----------------------------
        // AGENCY: List my packages (modification options live here)
        // -----------------------------
        [Authorize]
        public ActionResult MyPackages()
        {
            var userId = User.Identity.GetUserId();

            // only TravelAgency accounts can access
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var myPackages = db.TourPackages
                .Where(tp => tp.TravelAgencyId == userId)
                .OrderByDescending(tp => tp.CreatedDate)
                .ToList();

            return View(myPackages);
        }

        // -----------------------------
        // AGENCY: Create
        // -----------------------------
        [Authorize]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TourPackage tourPackage)
        {
            var userId = User.Identity.GetUserId();
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (!ModelState.IsValid)
                return View(tourPackage);

            tourPackage.TravelAgencyId = userId;

            // sensible defaults
            tourPackage.CreatedDate = DateTime.Now;
            tourPackage.IsActive = true;

            // If you want slots to start equal to max group size:
            tourPackage.AvailableSlots = tourPackage.MaxGroupSize;

            db.TourPackages.Add(tourPackage);
            db.SaveChanges();

            TempData["Success"] = "Tour package created successfully!";
            return RedirectToAction("MyPackages");
        }

        // -----------------------------
        // AGENCY: Edit
        // -----------------------------
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userId = User.Identity.GetUserId();
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var tourPackage = db.TourPackages.FirstOrDefault(tp => tp.Id == id && tp.TravelAgencyId == userId);
            if (tourPackage == null)
                return HttpNotFound();

            return View(tourPackage);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TourPackage model)
        {
            var userId = User.Identity.GetUserId();
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Pull existing record from DB (prevents overposting / tampering)
            var existing = db.TourPackages.FirstOrDefault(tp => tp.Id == model.Id && tp.TravelAgencyId == userId);
            if (existing == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return View(model);

            // Update only allowed fields
            existing.PackageName = model.PackageName;
            existing.Description = model.Description;
            existing.Destination = model.Destination;
            existing.DurationDays = model.DurationDays;
            existing.PricePerPerson = model.PricePerPerson;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.MaxGroupSize = model.MaxGroupSize;
            existing.ImageUrl = model.ImageUrl;
            existing.Inclusions = model.Inclusions;
            existing.Exclusions = model.Exclusions;
            existing.IsActive = model.IsActive;

            // Optional: only set AvailableSlots if you WANT agencies to edit it manually.
            // If bookings control slots, you may want to remove this line.
            existing.AvailableSlots = model.AvailableSlots;

            db.SaveChanges();

            TempData["Success"] = "Tour package updated successfully!";
            return RedirectToAction("MyPackages");
        }

        // -----------------------------
        // AGENCY: Deactivate (soft delete)
        // -----------------------------
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userId = User.Identity.GetUserId();
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var tourPackage = db.TourPackages.FirstOrDefault(tp => tp.Id == id && tp.TravelAgencyId == userId);
            if (tourPackage == null)
                return HttpNotFound();

            return View(tourPackage);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            if (!db.TravelAgencies.Any(a => a.Id == userId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var tourPackage = db.TourPackages.FirstOrDefault(tp => tp.Id == id && tp.TravelAgencyId == userId);
            if (tourPackage == null)
                return HttpNotFound();

            // Soft delete
            tourPackage.IsActive = false;
            db.SaveChanges();

            TempData["Success"] = "Tour package deactivated successfully!";
            return RedirectToAction("MyPackages");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
