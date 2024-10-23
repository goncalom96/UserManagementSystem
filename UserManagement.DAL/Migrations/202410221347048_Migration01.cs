namespace UserManagement.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Migration01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLogins",
                c => new
                {
                    UserLoginId = c.Int(nullable: false, identity: true),
                    UserRoleId = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 15),
                    Email = c.String(nullable: false, maxLength: 100),
                    Password = c.String(nullable: false, maxLength: 20),
                    CreatedAt = c.DateTime(nullable: false),
                    IsActived = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.UserLoginId)
                .ForeignKey("dbo.UserRoles", t => t.UserRoleId, cascadeDelete: true)
                .Index(t => t.UserRoleId);

            CreateTable(
                "dbo.UserProfiles",
                c => new
                {
                    UserLoginId = c.Int(nullable: false),
                    FirstName = c.String(nullable: false, maxLength: 30),
                    LastName = c.String(nullable: false, maxLength: 30),
                    Gender = c.Int(nullable: false),
                    DateOfBirth = c.DateTime(nullable: false, storeType: "date"),
                    ProfilePictureUrl = c.String(maxLength: 200),
                    LastModified = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.UserLoginId)
                .ForeignKey("dbo.UserLogins", t => t.UserLoginId)
                .Index(t => t.UserLoginId);

            CreateTable(
                "dbo.UserRoles",
                c => new
                {
                    UserRoleId = c.Int(nullable: false, identity: true),
                    RoleType = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.UserRoleId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.UserLogins", "UserRoleId", "dbo.UserRoles");
            DropForeignKey("dbo.UserProfiles", "UserLoginId", "dbo.UserLogins");
            DropIndex("dbo.UserProfiles", new[] { "UserLoginId" });
            DropIndex("dbo.UserLogins", new[] { "UserRoleId" });
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.UserLogins");
        }
    }
}