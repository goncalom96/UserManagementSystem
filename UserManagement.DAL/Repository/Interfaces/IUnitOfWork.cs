namespace UserManagement.DAL.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IUserLoginRepository UserLoginRepository { get; }
        IUserProfileRepository UserProfileRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }

        void SaveChanges();
    }
}