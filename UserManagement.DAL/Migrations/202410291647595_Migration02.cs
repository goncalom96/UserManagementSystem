namespace UserManagement.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "ResetPasswordCode", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "ResetPasswordCode");
        }
    }
}
