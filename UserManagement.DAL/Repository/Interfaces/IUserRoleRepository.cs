using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Repository.Interfaces
{
    public interface IUserRoleRepository
    {

        IQueryable<UserRole> GetRoles();

        UserRole GetRole(Expression<Func<UserRole, bool>> predicate);
    }
}