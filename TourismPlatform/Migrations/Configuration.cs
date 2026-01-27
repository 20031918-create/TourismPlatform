namespace TourismPlatform.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using TourismPlatform.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TourismPlatform.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TourismPlatform.Models.ApplicationDbContext context)
        {
            // Don't reseed if tour packages already exist
            if (context.TourPackages.Count() >= 4)
                return;

            // ---------------------------------------------------------
            // 1) Ensure a TravelAgency user + TravelAgency profile exists
            // ---------------------------------------------------------
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            const string agencyEmail = "agency@sydneytravel.com";
            const string agencyPassword = "Password@123"; // demo password (change if you want)

            var agencyUser = context.Users.FirstOrDefault(u => u.Email == agencyEmail);

            if (agencyUser == null)
            {
                agencyUser = new ApplicationUser
                {
                    UserName = agencyEmail,
                    Email = agencyEmail,
                    FullName = "Sydney Travel Co",
                    UserType = "TravelAgency"
                };

                var createResult = userManager.Create(agencyUser, agencyPassword);

                // If creation fails, stop seeding to avoid broken FK data
                if (!createResult.Succeeded)
                    return;
            }

            // TravelAgency profile (Id must match ApplicationUser.Id)
            var agencyProfile = context.TravelAgencies.SingleOrDefault(a => a.Id == agencyUser.Id);
            if (agencyProfile == null)
            {
                agencyProfile = new TravelAgency
                {
                    Id = agencyUser.Id,
                    AgencyName = "Sydney Travel Co",
                    Description = "Sydney-based agency offering curated tours and experiences.",
                    ServicesOffered = "City tours, day trips, adventure tours",
                    ContactNumber = "0400000000",
                    Address = "Sydney NSW",
                    ProfileImageUrl = "https://placehold.co/600x400",
                    RegistrationDate = DateTime.Now,
                    IsVerified = true
                };

                context.TravelAgencies.Add(agencyProfile);
                context.SaveChanges();
            }

            // ---------------------------------------------------------
            // 2) Seed Tour Packages (match your TourPackage model fields)
            // ---------------------------------------------------------
            var tours = new List<TourPackage>
            {
                new TourPackage
                {
                    PackageName = "Sydney Eye Tower",
                    Description = "Enjoy stunning city views from the top of Sydney.",
                    Destination = "Townhall, Sydney",
                    DurationDays = 1,
                    PricePerPerson = 90m,
                    StartDate = DateTime.Today.AddDays(5),
                    EndDate = DateTime.Today.AddDays(6),
                    MaxGroupSize = 30,
                    AvailableSlots = 10,
                    ImageUrl = "https://placehold.co/600x400",
                    Inclusions = "Entry tickets, guide",
                    Exclusions = "Meals, transport",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    TravelAgencyId = agencyProfile.Id
                },
                new TourPackage
                {
                    PackageName = "Blue Mountains Adventure",
                    Description = "Scenic lookouts, waterfalls, and nature walks.",
                    Destination = "Blue Mountains",
                    DurationDays = 2,
                    PricePerPerson = 220m,
                    StartDate = DateTime.Today.AddDays(10),
                    EndDate = DateTime.Today.AddDays(12),
                    MaxGroupSize = 40,
                    AvailableSlots = 15,
                    ImageUrl = "https://placehold.co/600x400",
                    Inclusions = "Transport, guide",
                    Exclusions = "Personal expenses",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    TravelAgencyId = agencyProfile.Id
                },
                new TourPackage
                {
                    PackageName = "Bondi Beach Experience",
                    Description = "Relax, explore, and enjoy iconic Bondi Beach.",
                    Destination = "Bondi Beach",
                    DurationDays = 1,
                    PricePerPerson = 120m,
                    StartDate = DateTime.Today.AddDays(7),
                    EndDate = DateTime.Today.AddDays(8),
                    MaxGroupSize = 50,
                    AvailableSlots = 20,
                    ImageUrl = "https://placehold.co/600x400",
                    Inclusions = "Guide",
                    Exclusions = "Surf lessons, meals",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    TravelAgencyId = agencyProfile.Id
                },
                new TourPackage
                {
                    PackageName = "Hunter Valley Wine Tour",
                    Description = "Vineyard visits and wine tasting experience.",
                    Destination = "Hunter Valley",
                    DurationDays = 1,
                    PricePerPerson = 280m,
                    StartDate = DateTime.Today.AddDays(14),
                    EndDate = DateTime.Today.AddDays(15),
                    MaxGroupSize = 25,
                    AvailableSlots = 12,
                    ImageUrl = "https://placehold.co/600x400",
                    Inclusions = "Transport, tastings",
                    Exclusions = "Extra purchases",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    TravelAgencyId = agencyProfile.Id
                }
            };

            context.TourPackages.AddRange(tours);
            context.SaveChanges();
        }
    }
}
