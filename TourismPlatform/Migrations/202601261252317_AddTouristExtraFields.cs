namespace TourismPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTouristExtraFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tourists", "Address", c => c.String(maxLength: 200));
            AddColumn("dbo.Tourists", "City", c => c.String(maxLength: 100));
            AddColumn("dbo.Tourists", "EmergencyContactName", c => c.String(maxLength: 100));
            AddColumn("dbo.Tourists", "EmergencyContactPhone", c => c.String(maxLength: 20));
            AddColumn("dbo.Tourists", "PassportNumber", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tourists", "PassportNumber");
            DropColumn("dbo.Tourists", "EmergencyContactPhone");
            DropColumn("dbo.Tourists", "EmergencyContactName");
            DropColumn("dbo.Tourists", "City");
            DropColumn("dbo.Tourists", "Address");
        }
    }
}
