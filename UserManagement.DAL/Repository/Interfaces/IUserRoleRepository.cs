using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Repository.Interfaces
{
    public interface IUserRoleRepository
    {
        //IEnumerable<EnumRole> GetRoles(Expression<Func<UserLogin, bool>> predicate);

        UserRole GetRoles(Expression<Func<UserRole, bool>> predicate);
    }
}