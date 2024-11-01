namespace UserManagement.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "EmailAddress", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.UserLogins", "PhoneNumber", c => c.String(nullable: false, maxLength: 9));
            AddColumn("dbo.UserLogins", "PasswordRecoveryToken", c => c.String(maxLength: 100));
            DropColumn("dbo.UserLogins", "Email");
            DropColumn("dbo.UserLogins", "ResetPasswordCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserLogins", "ResetPasswordCode", c => c.String(maxLength: 100));
            AddColumn("dbo.UserLogins", "Email", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.UserLogins", "PasswordRecoveryToken");
            DropColumn("dbo.UserLogins", "PhoneNumber");
            DropColumn("dbo.UserLogins", "EmailAddress");
        }
    }
}
