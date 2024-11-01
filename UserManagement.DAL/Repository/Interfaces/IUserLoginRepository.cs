using System;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Models.Users;

namespace UserManagement.DAL.Repository.Interfaces
{
    public interface IUserLoginRepository
    {
        UserLogin UserValidation(string username, string password);

        UserLogin GetUser(Expression<Func<UserLogin, bool>> predicate);

        UserLogin GetUserById(int id);

        IQueryable<UserLogin> GetUsers();

        //IEnumerable<UserLogin> GetUsers2(Expression<Func<UserLogin, bool>> predicate = null);

        void Create(UserLogin user);

        void Update(int id);

        void Delete(int id);
    }
}