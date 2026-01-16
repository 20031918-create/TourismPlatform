using System.Collections.Generic;

namespace TourismPlatform.Models
{
    public class ToursListViewModel
    {
        public List<Tour> Tours { get; set; } = new List<Tour>();

        public string Search { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Sort { get; set; }

        public List<string> AvailableCategories { get; set; } = new List<string>();
        public List<string> AvailableLocations { get; set; } = new List<string>();
    }
}
