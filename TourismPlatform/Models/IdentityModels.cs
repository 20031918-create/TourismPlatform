using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public string UserType { get; set; } // "TravelAgency" or "Tourist"

        // Navigation properties
        public virtual TravelAgency TravelAgency { get; set; }
        public virtual Tourist Tourist { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("UserType", this.UserType));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        // DbSets for your entities
        public DbSet<TravelAgency> TravelAgencies { get; set; }
        public DbSet<Tourist> Tourists { get; set; }
        public DbSet<TourPackage> TourPackages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
   


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for all decimal properties
            modelBuilder.Properties<decimal>()
                .Configure(config => config.HasPrecision(18, 2));

            // Configure one-to-one relationship between ApplicationUser and TravelAgency
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.TravelAgency)
                .WithRequired(ta => ta.User);

            // Configure one-to-one relationship between ApplicationUser and Tourist
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.Tourist)
                .WithRequired(t => t.User);

            // Configure one-to-many relationship between TravelAgency and TourPackage
            modelBuilder.Entity<TourPackage>()
                .HasRequired(tp => tp.TravelAgency)
                .WithMany(ta => ta.TourPackages)
                .HasForeignKey(tp => tp.TravelAgencyId)
                .WillCascadeOnDelete(false);

            // Configure many-to-many relationship through Booking
            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Tourist)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TouristId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.TourPackage)
                .WithMany(tp => tp.Bookings)
                .HasForeignKey(b => b.TourPackageId)
                .WillCascadeOnDelete(false);

            // Configure one-to-one relationship between Booking and Feedback
            modelBuilder.Entity<Feedback>()
                .HasRequired(f => f.Booking)
                .WithOptional(b => b.Feedback);

            modelBuilder.Entity<TourPackage>()
      .Property(p => p.PricePerPerson)
      .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);
        }
    }
}