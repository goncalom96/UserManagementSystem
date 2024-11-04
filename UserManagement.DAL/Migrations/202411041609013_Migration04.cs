namespace UserManagement.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "ImageUrl", c => c.String(maxLength: 200));
            DropColumn("dbo.UserProfiles", "ProfilePictureUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "ProfilePictureUrl", c => c.String(maxLength: 200));
            DropColumn("dbo.UserProfiles", "ImageUrl");
        }
    }
}
