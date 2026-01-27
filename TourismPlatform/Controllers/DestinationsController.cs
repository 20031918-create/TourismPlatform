using System;
using System.Web.Mvc;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class DestinationsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Destination destination = null;

            switch (id.Value)
            {
                case 1:
                    destination = new Destination
                    {
                        Id = 1,
                        Name = "Sydney",
                        Description = "Sydney is Australia’s most iconic harbour city, known for its Opera House, Harbour Bridge, beautiful beaches, and vibrant food and culture. Perfect for sightseeing, shopping, and coastal experiences.",
                        ImageUrl = "~/Images/Sydney.jpg",
                        Attractions = "Sydney Opera House, Harbour Bridge, Bondi Beach, Darling Harbour, The Rocks",
                        TourPrice = 1200,
                        Rating = 5,
                        CreatedDate = DateTime.Now
                    };
                    break;

                case 2:
                    destination = new Destination
                    {
                        Id = 2,
                        Name = "Newcastle",
                        Description = "Newcastle is a relaxed coastal destination with stunning beaches, surf spots, scenic walks, and a lively café scene. A great getaway for nature, history, and ocean views.",
                        ImageUrl = "~/Images/Newcastle.jpg",
                        Attractions = "Nobbys Beach, Fort Scratchley, Newcastle Memorial Walk, Merewether Beach, Honeysuckle Wharf",
                        TourPrice = 900,
                        Rating = 4,
                        CreatedDate = DateTime.Now
                    };
                    break;

                case 3:
                    destination = new Destination
                    {
                        Id = 3,
                        Name = "Perth",
                        Description = "Perth is a sunny city on the west coast with river views, clean beaches, and modern city attractions. Ideal for families, couples, and anyone who loves warm weather and outdoor activities.",
                        ImageUrl = "~/Images/Perth.jpg",
                        Attractions = "Kings Park, Elizabeth Quay, Cottesloe Beach, Swan River, Fremantle Markets",
                        TourPrice = 1100,
                        Rating = 5,
                        CreatedDate = DateTime.Now
                    };
                    break;
            }

            if (destination == null)
                return HttpNotFound();

            return View(destination);
        }
    }
}
