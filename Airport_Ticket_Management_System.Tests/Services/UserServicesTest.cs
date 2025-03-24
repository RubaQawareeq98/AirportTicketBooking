using AutoFixture;
using AutoFixture.AutoMoq;
using Data.Bookings;
using Data.Users;
using FluentAssertions;
using Model.Bookings;
using Model.Users;
using Model.Users.Exceptions;
using Moq;
using Services.Users;

namespace Airport_Ticket_Management_System.Tests.Services;

public class UserServicesTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly UserService _userService;

    public UserServicesTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUserRepository = _fixture.Freeze<Mock<IUserRepository>>();
        _mockBookingRepository = _fixture.Freeze<Mock<IBookingRepository>>();
        _userService = _fixture.Create<UserService>();
    }

    [Fact]
    public async Task ValidateUserLogin_ShouldReturnUser()
    {
        // Arrange
        const string userName = "ruba";
        const string password = "123456";

        var user = _fixture.Build<User>()
            .With(u => u.UserName, userName)
            .With(u => u.Password, password)
            .Create();

        _mockUserRepository.Setup(repo => repo.GetAllUsers())
            .ReturnsAsync([user]);

        // Act
        var currentUser = await _userService.ValidateUser(userName, password);

        // Assert
        currentUser.Should().NotBeNull();
        currentUser.Should().BeOfType<User>();
        currentUser.UserName.Should().Be(userName);
        currentUser.Password.Should().Be(password);
    }

    [Fact]
    public async Task ValidateUserLogin_ShouldThrowException()
    {
        // Arrange
        var userName = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        _mockUserRepository.Setup(repo => repo.GetAllUsers())
            .ReturnsAsync([]);

        // Act & Assert
        var act = async () => await _userService.ValidateUser(userName, password);
        await act.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage("Invalid username or password");
    }

    [Fact]
    public async Task GetUserBookings_ShouldReturnUserBookings()
    {
        // Arrange
        var userId = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484");

        var booking = _fixture.Build<Booking>()
            .With(b => b.PassengerId, userId)
            .Create();

        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync([booking]);

        // Act
        var userBookings = await _userService.GetPassengerBookings(userId);

        // Assert
        userBookings.Should().NotBeNull();
        userBookings.Should().ContainSingle();
        userBookings.First().PassengerId.Should().Be(userId);
    }

    [Fact]
    public async Task GetUserBookings_ShouldThrowException()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();

        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking>());

        // Act & Assert
        var act = async () => await _userService.GetPassengerBookings(userId);
        await act.Should().ThrowAsync<NoBookingFoundException>()
            .WithMessage($"No bookings found for passenger {userId}");
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(2).ToList();

        _mockUserRepository.Setup(repo => repo.GetAllUsers())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsers();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task SaveUser_ShouldSaveUser()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _mockUserRepository.Setup(repo => repo.AddUser(user))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(repo => repo.GetAllUsers())
            .ReturnsAsync([user]);

        // Act
        await _userService.SaveUser(user);

        // Assert
        var users = await _userService.GetAllUsers();
        users.Should().NotBeNull();
        users.Should().ContainSingle();
    }
}