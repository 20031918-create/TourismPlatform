using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Models
{
    public class TourPackage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Package name is required")]
        [StringLength(200)]
        [Display(Name = "Package Name")]
        public string PackageName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        [StringLength(200)]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price per Person")]
        public decimal PricePerPerson { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Maximum group size is required")]
        [Range(1, 1000, ErrorMessage = "Group size must be between 1 and 1000")]
        [Display(Name = "Maximum Group Size")]
        public int MaxGroupSize { get; set; }

        [Display(Name = "Available Slots")]
        public int AvailableSlots { get; set; }

        [StringLength(500)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(500)]
        public string Inclusions { get; set; }

        [StringLength(500)]
        public string Exclusions { get; set; }

        // Foreign Key
        [Required]
        public string TravelAgencyId { get; set; }

        // Navigation properties
        [ForeignKey("TravelAgencyId")]
        public virtual TravelAgency TravelAgency { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }

        public TourPackage()
        {
            Bookings = new HashSet<Booking>();
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}