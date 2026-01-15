namespace TourismPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TouristId = c.String(nullable: false, maxLength: 128),
                        TourPackageId = c.Int(nullable: false),
                        NumberOfParticipants = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookingStatus = c.String(nullable: false, maxLength: 50),
                        PaymentStatus = c.String(nullable: false, maxLength: 50),
                        BookingDate = c.DateTime(nullable: false),
                        SpecialRequests = c.String(maxLength: 500),
                        CancellationReason = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tourists", t => t.TouristId)
                .ForeignKey("dbo.TourPackages", t => t.TourPackageId)
                .Index(t => t.TouristId)
                .Index(t => t.TourPackageId);
            
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        BookingId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(nullable: false, maxLength: 1000),
                        FeedbackDate = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        AgencyResponse = c.String(maxLength: 500),
                        ResponseDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Bookings", t => t.BookingId)
                .Index(t => t.BookingId);
            
            CreateTable(
                "dbo.Tourists",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContactNumber = c.String(maxLength: 15),
                        Nationality = c.String(maxLength: 200),
                        DateOfBirth = c.DateTime(),
                        ProfileImageUrl = c.String(maxLength: 500),
                        RegistrationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false, maxLength: 100),
                        UserType = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.TravelAgencies",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AgencyName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, maxLength: 2000),
                        ServicesOffered = c.String(maxLength: 500),
                        ContactNumber = c.String(maxLength: 15),
                        Address = c.String(maxLength: 500),
                        ProfileImageUrl = c.String(maxLength: 500),
                        RegistrationDate = c.DateTime(nullable: false),
                        IsVerified = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.TourPackages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PackageName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, maxLength: 2000),
                        Destination = c.String(nullable: false, maxLength: 200),
                        DurationDays = c.Int(nullable: false),
                        PricePerPerson = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        MaxGroupSize = c.Int(nullable: false),
                        AvailableSlots = c.Int(nullable: false),
                        ImageUrl = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Inclusions = c.String(maxLength: 500),
                        Exclusions = c.String(maxLength: 500),
                        TravelAgencyId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TravelAgencies", t => t.TravelAgencyId)
                .Index(t => t.TravelAgencyId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Bookings", "TourPackageId", "dbo.TourPackages");
            DropForeignKey("dbo.Bookings", "TouristId", "dbo.Tourists");
            DropForeignKey("dbo.TravelAgencies", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TourPackages", "TravelAgencyId", "dbo.TravelAgencies");
            DropForeignKey("dbo.Tourists", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Feedbacks", "BookingId", "dbo.Bookings");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TourPackages", new[] { "TravelAgencyId" });
            DropIndex("dbo.TravelAgencies", new[] { "Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Tourists", new[] { "Id" });
            DropIndex("dbo.Feedbacks", new[] { "BookingId" });
            DropIndex("dbo.Bookings", new[] { "TourPackageId" });
            DropIndex("dbo.Bookings", new[] { "TouristId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TourPackages");
            DropTable("dbo.TravelAgencies");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tourists");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Bookings");
        }
    }
}
