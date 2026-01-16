using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TourismPlatform.Models;

namespace TourismPlatform.Controllers
{
    public class ToursController : Controller
    {
        // Dummy data so Tours section works without database
        private static readonly List<Tour> _tours = new List<Tour>
        {
            new Tour { Id=1, Title="Sydney Harbour Highlights", Location="Sydney", Category="City",
                Price=89, DurationHours=3, Rating=4.7, IsActive=true, CreatedDate=DateTime.Now.AddDays(-10),
                ShortDescription="Cruise + landmarks + photo stops.",
                LongDescription="Explore the best of Sydney Harbour with guided stops and scenic views." },

            new Tour { Id=2, Title="Blue Mountains Day Trip", Location="Blue Mountains", Category="Nature",
                Price=149, DurationHours=8, Rating=4.8, IsActive=true, CreatedDate=DateTime.Now.AddDays(-20),
                ShortDescription="Waterfalls, viewpoints and bushwalks.",
                LongDescription="A full-day tour visiting iconic lookouts and nature trails." },

            new Tour { Id=3, Title="Hunter Valley Wine Experience", Location="Hunter Valley", Category="Food & Wine",
                Price=199, DurationHours=9, Rating=4.6, IsActive=true, CreatedDate=DateTime.Now.AddDays(-5),
                ShortDescription="Cellar doors + tasting + lunch option.",
                LongDescription="A relaxed day visiting wineries and tasting local produce." },

            new Tour { Id=4, Title="Bondi to Coogee Coastal Walk", Location="Sydney", Category="Adventure",
                Price=59, DurationHours=4, Rating=4.5, IsActive=true, CreatedDate=DateTime.Now.AddDays(-15),
                ShortDescription="Guided coastal walk with swim stops.",
                LongDescription="A scenic coastal route with stories, photo spots, and optional swim breaks." },
        };

        // GET: /Tours
        public ActionResult Index(string search, string category, string location, string sort)
        {
            var query = _tours.Where(t => t.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Title.Contains(search) || t.ShortDescription.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(t => t.Category == category);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(t => t.Location == location);

            switch (sort)
            {
                case "price_asc":
                    query = query.OrderBy(t => t.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(t => t.Price);
                    break;
                case "rating":
                    query = query.OrderByDescending(t => t.Rating);
                    break;
                default:
                    query = query.OrderByDescending(t => t.CreatedDate);
                    break;
            }

            var vm = new ToursListViewModel
            {
                Tours = query.ToList(),
                Search = search,
                Category = category,
                Location = location,
                Sort = sort,
                AvailableCategories = _tours.Select(t => t.Category).Distinct().OrderBy(x => x).ToList(),
                AvailableLocations = _tours.Select(t => t.Location).Distinct().OrderBy(x => x).ToList()
            };

            return View(vm);
        }

        // GET: /Tours/Details/1
        public ActionResult Details(int id)
        {
            var tour = _tours.FirstOrDefault(t => t.Id == id);
            if (tour == null) return HttpNotFound();

            return View(tour);
        }
    }
}
