using Data.Files;
using Model.Users;

namespace Data.Users;

public class UserRepository(IFilePathSettings settings, IFileRepository<User> fileRepository) : IUserRepository
{

    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            Console.WriteLine("Getting all users");
            Console.WriteLine(settings.Users, "  ", settings.Flights);
            var users = await fileRepository.ReadDataFromFile(settings.Users);
            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task AddUser(User user)
    {
        try
        {
            var users = await GetAllUsers();
            users.Add(user);
            await SaveUsers(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task SaveUsers(List<User> users)
    {
        try
        {
            await fileRepository.WriteDataToFile(settings.Users, users);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}