using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Models
{
    public class Tourist
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }

        [StringLength(15)]
        [Phone]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [StringLength(200)]
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(500)]
        [Display(Name = "Profile Image URL")]
        public string ProfileImageUrl { get; set; }

        // ✅ NEW: Additional profile info
        [StringLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(100)]
        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; }

        [StringLength(20)]
        [Display(Name = "Emergency Contact Phone")]
        public string EmergencyContactPhone { get; set; }

        [StringLength(50)]
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }

        public Tourist()
        {
            Bookings = new HashSet<Booking>();
            RegistrationDate = DateTime.Now;
        }
    }
}
