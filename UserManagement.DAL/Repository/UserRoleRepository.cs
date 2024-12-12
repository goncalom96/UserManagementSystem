using System;
using System.Data.Entity;
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
            IQueryable<UserRole> userRoles = context.UserRoles;

            return userRoles != null ? userRoles : null;
        }

        public UserRole GetRole(Expression<Func<UserRole, bool>> predicate)
        {
            return context.UserRoles.SingleOrDefault(predicate);
        }

        public UserRole GetRoleById(int id)
        {
            //Filtro personalizado qualquer condição.
            //UserRole roleFound = context.UserRoles.SingleOrDefault(r => r.UserRoleId == id);

            // Filtro personalizado apenas por chave primária.
            UserRole userRoleFound = context.UserRoles.Find(id);

            return userRoleFound != null ? userRoleFound : null;
        }

        public void Create(UserRole userRole)
        {
            context.UserRoles.Add(userRole);
        }

        public void Edit(UserRole userRole)
        {
            //Associa o objeto ao contexto e altera o seu estado para "Modified".
            context.Entry(userRole).State = EntityState.Modified;

            // Deve ser evitado
            // Funciona no EF6, mas foi removido no EF Core.
            //context.UserRoles.AddOrUpdate(userRole);
        }

        public void Delete(UserRole userRole)
        {
            UserRole userRoleDeleted = context.UserRoles.Find(userRole.UserRoleId);

            context.UserRoles.Remove(userRoleDeleted);
        }
    }
}