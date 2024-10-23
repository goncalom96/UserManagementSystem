using System.Data.Entity;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Data
{
    public class UserManagementContext : DbContext
    {
        // Constructor
        public UserManagementContext() : base("UserManagementDBConnection")
        {
        }

        // DbSets
        public DbSet<UserLogin> UserLogins { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        // DB Creation
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //Configuração do Entity Framework para desativar a convenção de pluralização automática dos nomes das tabelas
        //    //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
}