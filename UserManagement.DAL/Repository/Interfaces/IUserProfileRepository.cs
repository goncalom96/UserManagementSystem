using System;
using System.Linq.Expressions;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Repository.Interfaces
{
    public interface IUserProfileRepository
    {
        UserProfile GetUserProfile(Expression<Func<UserProfile, bool>> predicate);

        void Create(UserProfile user);
    }
}