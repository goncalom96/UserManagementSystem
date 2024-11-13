using System;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Repository.Interfaces
{
    public interface IUserRoleRepository
    {
        IQueryable<UserRole> GetRoles();

        UserRole GetRole(Expression<Func<UserRole, bool>> predicate);

        UserRole GetRoleById(int id);

        void Create(UserRole userRole);

        void Edit(UserRole userRole);

        void Delete(UserRole userRole);
    }
}