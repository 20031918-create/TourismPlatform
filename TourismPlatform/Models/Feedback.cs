using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Models
{
    public class Feedback
    {
        [Key]
        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Display(Name = "Feedback Date")]
        public DateTime FeedbackDate { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [StringLength(500)]
        [Display(Name = "Agency Response")]
        public string AgencyResponse { get; set; }

        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }

        // Navigation property
        public virtual Booking Booking { get; set; }

        public Feedback()
        {
            FeedbackDate = DateTime.Now;
            IsApproved = false;
        }
    }
}