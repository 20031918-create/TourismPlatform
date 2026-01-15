using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Models
{
    public class TravelAgency
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Agency name is required")]
        [StringLength(200)]
        [Display(Name = "Agency Name")]
        public string AgencyName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Services Offered")]
        public string ServicesOffered { get; set; }

        [StringLength(15)]
        [Phone]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [StringLength(500)]
        [Display(Name = "Profile Image URL")]
        public string ProfileImageUrl { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Is Verified")]
        public bool IsVerified { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<TourPackage> TourPackages { get; set; }

        public TravelAgency()
        {
            TourPackages = new HashSet<TourPackage>();
            RegistrationDate = DateTime.Now;
            IsVerified = false;
        }
    }
}