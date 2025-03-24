using Model.Users;

namespace Data.Users;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task AddUser(User user);
}