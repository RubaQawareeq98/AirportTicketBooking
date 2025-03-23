using AutoFixture;
using FluentAssertions;
using Model.Users;

namespace Airport_Ticket_Management_System.Tests.Data;

public class UserRepositoryTest : TestBase
{
    private readonly Fixture _fixture = new();
    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Act
        var result = await UserRepository.GetAllUsers();

        // Assert
        result.Should().NotBeNull();
    }
  
    [Fact]
    public async Task AddUser_ShouldAddUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        
        // Act
        await UserRepository.AddUser(user);
        
        // Assert
        var retrievedUsers = await UserRepository.GetAllUsers();
        retrievedUsers.Should().HaveCount(3);
        retrievedUsers.Should().ContainEquivalentOf(user);
    }
}