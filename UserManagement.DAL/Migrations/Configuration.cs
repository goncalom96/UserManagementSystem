namespace UserManagement.DAL.Migrations
{
    using System.Data.Entity.Migrations;
    using UserManagement.DAL.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<UserManagement.DAL.Data.UserManagementContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UserManagement.DAL.Data.UserManagementContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.


            // Dados iniciais para a aplicação
            context.UserRoles.AddOrUpdate(
                us => us.RoleType, StartUpData.GetUserRoles().ToArray());
            context.SaveChanges();

            context.UserLogins.AddOrUpdate(
                ul => new { ul.UserName, ul.EmailAddress, ul.Password }, StartUpData.GetUserLogins(context).ToArray());
            context.SaveChanges();

            context.UserProfiles.AddOrUpdate(
                up => new { up.FirstName, up.LastName, up.Gender, up.DateOfBirth }, StartUpData.GetUserProfiles(context).ToArray());
            context.SaveChanges();
        }
    }
}