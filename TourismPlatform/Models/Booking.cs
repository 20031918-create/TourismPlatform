using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TouristId { get; set; }

        [Required]
        public int TourPackageId { get; set; }

        [Required(ErrorMessage = "Number of participants is required")]
        [Range(1, 50, ErrorMessage = "Number of participants must be between 1 and 50")]
        [Display(Name = "Number of Participants")]
        public int NumberOfParticipants { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Booking Status")]
        public string BookingStatus { get; set; } // Pending, Confirmed, Completed, Cancelled

        [Required]
        [StringLength(50)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } // Pending, Paid, Refunded

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [StringLength(500)]
        [Display(Name = "Cancellation Reason")]
        public string CancellationReason { get; set; }

        // Navigation properties
        [ForeignKey("TouristId")]
        public virtual Tourist Tourist { get; set; }

        [ForeignKey("TourPackageId")]
        public virtual TourPackage TourPackage { get; set; }

        public virtual Feedback Feedback { get; set; }

        public Booking()
        {
            BookingDate = DateTime.Now;
            BookingStatus = "Pending";
            PaymentStatus = "Pending";
        }
    }
}
