using System;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Data;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository.Interfaces;

namespace UserManagement.DAL.Repository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private UserManagementContext context;

        public UserRoleRepository(UserManagementContext context)
        {
            this.context = context;
        }

        public IQueryable<UserRole> GetRoles()
        {
            IQueryable<UserRole> roles = context.UserRoles;

            return roles != null ? roles : null;
        }

        public UserRole GetRole(Expression<Func<UserRole, bool>> predicate)
        {
            return context.UserRoles.SingleOrDefault(predicate);
        }
    }
}