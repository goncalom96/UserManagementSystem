using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using UserManagement.DAL.Data;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository.Interfaces;

namespace UserManagement.DAL.Repository
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private UserManagementContext context;

        public UserLoginRepository(UserManagementContext context)
        {
            this.context = context;
        }

        public UserLogin UserValidation(string username, string password)
        {
            UserLogin userFound = context.UserLogins
                   .Include(u => u.UserProfile)
                   .Include(u => u.UserRole)
                   .SingleOrDefault(u => u.UserName == username && u.Password == password);

            return userFound != null ? userFound : null;
        }

        public UserLogin GetUser(Expression<Func<UserLogin, bool>> predicate)
        {
            return context.UserLogins.Include(u => u.UserProfile).Include(u => u.UserRole).SingleOrDefault(predicate);
        }

        public UserLogin GetUserById(int id)
        {
            UserLogin userFound = context.UserLogins
                   .Include(u => u.UserProfile)
                   .Include(u => u.UserRole)
                   .SingleOrDefault(u => u.UserLoginId == id);

            return userFound != null ? userFound : null;
        }

        /// <summary>
        /// Get all users data, include role and profile
        /// </summary>
        /// <returns>IQueryable<UserLogin> users</returns>
        public IQueryable<UserLogin> GetUsers()
        {
            // NOTA: Usar o Include só quando se tem a certeza que se vai utilizar as outras entidades na view para melhorar o desempenho

            IQueryable<UserLogin> users = context.UserLogins
                .Include(u => u.UserProfile)
                .Include(u => u.UserRole);

            return users != null ? users : null;

            #region Outras alternativas

            // Opção 2
            // Referenciar "using System.Data.Entity;"
            //using (WebAppContext context = new WebAppContext())
            //{
            //    IQueryable<UserLogin> users =
            //    (from u in context.UserLogins select u)
            //    .Include(u => u.UserProfile)
            //    .Include(u => u.UserRole);
            //}

            // Opção 3
            //Model tem de ser do tipo List em vez de IQueryable
            //using (WebAppContext context = new WebAppContext())
            //{
            //IQueryable<UserLogin> users =
            //    from u in context.UserLogins.Include(u => u.UserProfile).Include(u => u.UserRole)
            //    select u;

            //    return View(users.ToList());
            //}

            // Opção 4
            // Model tem de ser do tipo List em vez de IQueryable
            //using (WebAppContext context = new WebAppContext())
            //{
            //    List<UserLogin> users = context.UserLogins.Include(u => u.UserProfile).Include(u => u.UserRole).ToList();
            //    return View(users);
            //}

            #endregion Outras alternativas
        }

        /// <summary>
        /// Get all users data, include role and profile
        /// </summary>
        /// <returns>IEnumerable<UserLogin> users</returns>
        //public IEnumerable<UserLogin> GetUsers2(Expression<Func<UserLogin, bool>> predicate = null)
        //{
        //    if (predicate != null)
        //    {
        //        return context.UserLogins.Where(predicate);
        //    }

        //    // Se o predicate for nulo, retorna todos os usuários
        //    return context.UserLogins.Include(u => u.UserProfile).Include(u => u.UserRole);
        //}

        public void Create(UserLogin user)
        {
            context.UserLogins.Add(user);
        }

        public void Update(int userLoginId)
        {
            UserLogin userLogin = context.UserLogins.Find(userLoginId);
            context.Entry(userLogin).State = EntityState.Modified;

            //context.UserLogins.AddOrUpdate(userLogin);
        }

        public void Delete(int userLoginId)
        {
            //UserLogin userLogin = GetUserById(userLoginId);
            UserLogin userLogin = context.UserLogins.Find(userLoginId);

            context.UserLogins.Remove(userLogin);
        }
    }
}