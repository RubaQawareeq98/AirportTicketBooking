using AutoFixture;
using FluentAssertions;
using Model.Users;
using Model.Users.Exceptions;

namespace Airport_Ticket_Management_System.Tests.Services;

public class UserServicesTest : TestBase
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task ValidateUserLogin_ShouldReturnUser()
    {
        // Arrange
        const string userName = "ruba";
        const string password = "123456";

        // Act
        var currentUser = await UserService.ValidateUser(userName, password);

        // Assert
        currentUser.Should().NotBeNull();
        currentUser.Should().BeOfType<User>();
    }

    [Fact]
    public async Task ValidateUserLogin_ShouldThrowException()
    {
        // Arrange
        var userName = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        const string expectedMessage = "Invalid username or password";

        // Act & Assert
        var act = async () => await UserService.ValidateUser(userName, password);
        await act.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GetUserBookings_ShouldReturnUserBookings()
    {
        // Arrange
        var userId = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484");

        // Act
        var userBookings = await UserService.GetPassengerBookings(userId);

        // Arrange
        userBookings.Should().NotBeNull();
        userBookings.Should().ContainSingle();
    }

    [Fact]
    public async Task GetUserBookings_ShouldThrowException()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var expectedMessage = $"No bookings found for passenger {userId}";

        // Act & Assert
        var act = async () => await UserService.GetPassengerBookings(userId);
        await act.Should().ThrowAsync<NoBookingFoundException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Act
        var users = await UserService.GetAllUsers();
        
        // Assert
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
    }

    [Fact]
    public async Task SaveUser_ShouldSaveUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        
        // Act
        await UserService.SaveUser(user);
        // Assert
        var users = await UserService.GetAllUsers();
        users.Should().NotBeNull();
        users.Should().HaveCount(3);
    }
}