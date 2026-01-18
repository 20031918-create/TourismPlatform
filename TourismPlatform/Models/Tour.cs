using System;

namespace TourismPlatform.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }

        public decimal Price { get; set; }
        public int DurationHours { get; set; }

        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public double Rating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
