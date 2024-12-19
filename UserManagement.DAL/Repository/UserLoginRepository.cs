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

        public IQueryable<UserLogin> GetUsers()
        {
            // NOTA: Usar o Include só quando se tem a certeza que se vai utilizar as outras entidades na view para melhorar o desempenho

            IQueryable<UserLogin> userLogins = context.UserLogins
                .Include(u => u.UserProfile)
                .Include(u => u.UserRole);

            return userLogins != null ? userLogins : null;

            #region Outras alternativas

            // Opção 2
            // Referenciar "using System.Data.Entity;"
            //using (WebAppContext context = new WebAppContext())
            //{
            //    IQueryable<UserLogin> userLogins =
            //    (from u in context.UserLogins select u)
            //    .Include(u => u.UserProfile)
            //    .Include(u => u.UserRole);
            //}

            // Opção 3
            //Model tem de ser do tipo List em vez de IQueryable
            //using (WebAppContext context = new WebAppContext())
            //{
            //IQueryable<UserLogin> userLogins =
            //    from u in context.UserLogins.Include(u => u.UserProfile).Include(u => u.UserRole)
            //    select u;

            //    return View(userLogins.ToList());
            //}

            // Opção 4
            // Model tem de ser do tipo List em vez de IQueryable
            //using (WebAppContext context = new WebAppContext())
            //{
            //    List<UserLogin> userLogins = context.UserLogins.Include(u => u.UserProfile).Include(u => u.UserRole).ToList();
            //    return View(userLogins);
            //}

            #endregion Outras alternativas
        }

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

        public void Update(UserLogin userLogin)
        {
            context.Entry(userLogin).State = EntityState.Modified;
        }

        public void Delete(int userLoginId)
        {
            // Neste caso é preferível esta opção para ir buscar todas as relações do userLogin
            UserLogin userLogin = GetUserById(userLoginId);

            //UserLogin userLogin = context.UserLogins.Find(userLoginId);

            context.UserLogins.Remove(userLogin);
        }
    }
}