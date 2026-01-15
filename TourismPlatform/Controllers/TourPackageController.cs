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

        // Browse all packages (public)
        [AllowAnonymous]
        public ActionResult Index(string searchString, string destination)
        {
            var packages = db.TourPackages
                .Include(t => t.TravelAgency)
                .Where(t => t.IsActive && t.AvailableSlots > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                packages = packages.Where(p => p.PackageName.Contains(searchString) ||
                                             p.Description.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(destination))
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

        // Package details (public)
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TourPackage tourPackage = db.TourPackages
                .Include(t => t.TravelAgency.User)
                .FirstOrDefault(t => t.Id == id);

            if (tourPackage == null)
            {
                return HttpNotFound();
            }

            // Get feedbacks for this package
            var feedbacks = db.Feedbacks
                .Include(f => f.Booking.Tourist.User)
                .Where(f => f.Booking.TourPackageId == id && f.IsApproved)
                .OrderByDescending(f => f.FeedbackDate)
                .ToList();

            ViewBag.Feedbacks = feedbacks;
            ViewBag.AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;

            return View(tourPackage);
        }

        // Create package (Travel Agency only)
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TourPackage tourPackage)
        {
            if (ModelState.IsValid)
            {
                tourPackage.TravelAgencyId = User.Identity.GetUserId();
                tourPackage.AvailableSlots = tourPackage.MaxGroupSize;
                tourPackage.CreatedDate = DateTime.Now;
                tourPackage.IsActive = true;

                db.TourPackages.Add(tourPackage);
                db.SaveChanges();

                TempData["Success"] = "Tour package created successfully!";
                return RedirectToAction("Dashboard", "TravelAgency");
            }

            return View(tourPackage);
        }

        // Edit package (Travel Agency only)
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TourPackage tourPackage = db.TourPackages.Find(id);
            if (tourPackage == null)
            {
                return HttpNotFound();
            }

            // Ensure the tour package belongs to the current user
            if (tourPackage.TravelAgencyId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(tourPackage);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TourPackage tourPackage)
        {
            if (ModelState.IsValid)
            {
                // Ensure the tour package belongs to the current user
                if (tourPackage.TravelAgencyId != User.Identity.GetUserId())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                db.Entry(tourPackage).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Tour package updated successfully!";
                return RedirectToAction("Dashboard", "TravelAgency");
            }
            return View(tourPackage);
        }

        // Delete package (Travel Agency only)
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TourPackage tourPackage = db.TourPackages.Find(id);
            if (tourPackage == null)
            {
                return HttpNotFound();
            }

            // Ensure the tour package belongs to the current user
            if (tourPackage.TravelAgencyId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(tourPackage);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TourPackage tourPackage = db.TourPackages.Find(id);

            // Ensure the tour package belongs to the current user
            if (tourPackage.TravelAgencyId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Soft delete - just mark as inactive
            tourPackage.IsActive = false;
            db.Entry(tourPackage).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Tour package deleted successfully!";
            return RedirectToAction("Dashboard", "TravelAgency");
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