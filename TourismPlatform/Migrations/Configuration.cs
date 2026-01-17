namespace TourismPlatform.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TourismPlatform.Models.ApplicationDbContext>
    {
        public Configurations()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TourismPlatform.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
