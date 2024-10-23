using System;
using UserManagement.DAL.Data;
using UserManagement.DAL.Repository.Interfaces;

namespace UserManagement.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private UserManagementContext context;

        private UserLoginRepository _userLoginRepository;

        private UserProfileRepository _userProfileRepository;

        private UserRoleRepository _userRoleRepository;

        private bool disposed = false;

        public UnitOfWork()
        {
            context = new UserManagementContext();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public IUserLoginRepository UserLoginRepository
        {
            get
            {
                if (_userLoginRepository == null)
                {
                    _userLoginRepository = new UserLoginRepository(context);
                }
                return _userLoginRepository;
            }
        }

        public IUserProfileRepository UserProfileRepository
        {
            get
            {
                if (_userProfileRepository == null)
                {
                    _userProfileRepository = new UserProfileRepository(context);
                }
                return _userProfileRepository;
            }
        }

        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                if (_userRoleRepository == null)
                {
                    _userRoleRepository = new UserRoleRepository(context);
                }
                return _userRoleRepository;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}