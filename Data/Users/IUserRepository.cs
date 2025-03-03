using Model;
using Model.Users;

namespace Data;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task AddUser(User user);
    Task SaveUsers(List<User> users);
}