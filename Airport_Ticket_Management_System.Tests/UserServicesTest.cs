using AutoFixture;
using Data;
using Data.Bookings;
using Model.Bookings;
using Model.Users;
using Model.Users.Exceptions;
using Moq;
using Services.Users;

namespace Airport_Ticket_Management_System.Tests;

public class UserServicesTest
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly UserService _userService;
    private readonly Fixture _fixture;
    

    public UserServicesTest()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _bookingRepoMock = new Mock<IBookingRepository>();
        _userService = new UserService(_userRepoMock.Object, _bookingRepoMock.Object);
        _fixture = new Fixture();
    }


    [Fact]
    public async Task ValidateUserLogin_ShouldReturnUser()
    {
        var users = _fixture.CreateMany<User>(3).ToList();
        var user = users.First();
        _userRepoMock.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);
        
        var currentUser = await _userService.ValidateUser(user.UserName, user.Password);
        
        Assert.Equal(user.UserName, currentUser.UserName);
    }
    
    [Fact]
    public async Task ValidateUserLogin_ShouldThrowException()
    {
        var fixture = new Fixture();
        var users = fixture.CreateMany<User>(3).ToList();
        var user = fixture.Create<User>();
        
        _userRepoMock.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);
        
        await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.ValidateUser(user.UserName, user.Password));
    }

    [Fact]
    public async Task GetUserBookings_ShouldReturnUserBookings()
    {
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        _bookingRepoMock.Setup(repo => repo.GetAllBookings()).ReturnsAsync(bookings);
        var userId = bookings.First().PassengerId;
        
        var userBookings = await _userService.GetPassengerBookings(userId);
        
        Assert.Single(userBookings);
    }
    
    [Fact]
    public async Task GetUserBookings_ShouldThrowException()
    {
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        _bookingRepoMock.Setup(repo => repo.GetAllBookings()).ReturnsAsync(bookings);
        var userId = _fixture.Create<Guid>();
        await Assert.ThrowsAsync<NoBookingFoundException>(() => _userService.GetPassengerBookings(userId));
    }
    
}