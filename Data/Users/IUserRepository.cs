using Model;

namespace Data;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task AddUser(User user);
    Task SaveUsers(List<User> users);
}