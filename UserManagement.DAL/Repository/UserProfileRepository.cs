using System;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Data;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository.Interfaces;

namespace UserManagement.DAL.Repository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private UserManagementContext context;

        public UserProfileRepository(UserManagementContext context)
        {
            this.context = context;
        }

        public UserProfile GetUserProfile(Expression<Func<UserProfile, bool>> predicate)
        {
            return context.UserProfiles.SingleOrDefault(predicate);
        }

        public void Create(UserProfile userProfile)
        {
            context.UserProfiles.Add(userProfile);
        }
    }
}