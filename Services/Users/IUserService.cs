using Model.Bookings;
using Model.Users;

namespace Services.Users;

public interface IUserService
{
    Task<List<User>> GetAllUsers();
    Task<User> ValidateUser(string username, string password);
    Task<List<Booking>> GetPassengerBookings(Guid userId);
    Task SaveUser(User user);
}