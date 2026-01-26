using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Models
{
    public class Destination
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(1000)]
        public string Attractions { get; set; }

        [Required]
        public decimal TourPrice { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
