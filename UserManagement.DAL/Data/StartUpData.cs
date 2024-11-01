using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Data
{
    public class StartUpData
    {
        public static List<UserRole> GetUserRoles()
        {
            //return new List<UserRole> // também outra forma de retorna a lista
            List<UserRole> roles = new List<UserRole>()
            {
                new UserRole() { RoleType = UserRole.EnumRole.Administrator },
                new UserRole() { RoleType = UserRole.EnumRole.Guest }
            };

            return roles;
        }

        public static List<UserLogin> GetUserLogins(UserManagementContext context)
        {
            List<UserLogin> logins = new List<UserLogin>()
            {
                new UserLogin() {
                    UserName = "admin",
                    EmailAddress = "admin@hotmail.com",
                    PhoneNumber = "123456789",
                    Password = "12345",
                    UserRoleId = context.UserRoles.SingleOrDefault(r => r.RoleType == UserRole.EnumRole.Administrator).UserRoleId,
                    IsActived = false,
                    CreatedAt = DateTime.Now
                },
                new UserLogin() {
                    UserName = "guest",
                    EmailAddress = "guest@hotmail.com",
                    PhoneNumber = "123456789",
                    Password = "12345",
                    UserRoleId = context.UserRoles.SingleOrDefault(r => r.RoleType == UserRole.EnumRole.Guest).UserRoleId,
                    IsActived = false,
                    CreatedAt = DateTime.Now
                }
            };

            return logins;
        }

        public static List<UserProfile> GetUserProfiles(UserManagementContext context)
        {
            List<UserProfile> profiles = new List<UserProfile>()
            {
                new UserProfile() {
                    FirstName = "Gonçalo",
                    LastName = "Marques",
                    Gender = UserProfile.EnumGender.Male,
                    DateOfBirth = new DateTime (1996, 02, 26),
                    UserLoginId = context.UserLogins.SingleOrDefault(ul => ul.UserName == "admin").UserLoginId,
                    LastModified = DateTime.Now
                },
                new UserProfile() {
                    FirstName = "Guest",
                    LastName = "Test",
                    Gender = UserProfile.EnumGender.Female,
                    DateOfBirth = new DateTime (2000, 10, 10),
                    UserLoginId = context.UserLogins.SingleOrDefault(ul => ul.UserName == "guest").UserLoginId,
                    LastModified = DateTime.Now
                }
            };

            return profiles;
        }
    }
}