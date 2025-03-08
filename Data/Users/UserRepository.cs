using Model.Users;

namespace Data.Users;

public class UserRepository(string filePath, IFileRepository<User> fileRepository) : IUserRepository
{
    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            var users = await fileRepository.ReadDataFromFile(filePath);
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
            await fileRepository.WriteDataToFile(filePath, users);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}