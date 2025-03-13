using Data;
using Data.Bookings;
using Model.Bookings;
using Model.Users;
using Model.Users.Exceptions;

namespace Services.Users;

public class UserService (IUserRepository userRepository, IBookingRepository bookingRepository) : IUserService
{
    public async Task<List<User>> GetAllUsers()
    {
        return await userRepository.GetAllUsers();
    }

    public async Task<User> ValidateUser(string username, string password)
    {
        var users = await userRepository.GetAllUsers();
        var user = users.Find(user => user.UserName == username && user.Password == password);
        if (user is null)
        {
           throw new UserNotFoundException("Invalid username or password");
        }
        return user;
    }

    public async Task<List<Booking>> GetPassengerBookings(Guid userId)
    {
        var bookings = await bookingRepository.GetAllBookings();
        var userBookings = bookings.Where(booking => booking.PassengerId == userId).ToList();
        if (userBookings.Count == 0)
        {
            throw new NoBookingFoundException($"No bookings found for passenger {userId}");
        }
        return userBookings;
    }

    public async Task SaveUser(User user)
    {
        await userRepository.AddUser(user);
    }
}