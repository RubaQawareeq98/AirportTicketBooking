using AutoFixture;
using Data;
using Data.Users;
using FluentAssertions;
using Model.Users;
using Moq;

namespace Airport_Ticket_Management_System.Tests;

public class UserRepositoryTest
{
    private readonly Mock<IFileRepository<User>> _mockFileRepository;
    private readonly Fixture _fixture;
    private readonly UserRepository _userRepository;

    public UserRepositoryTest()
    {
        _mockFileRepository = new Mock<IFileRepository<User>>();
        var mockFilePathSettings = new Mock<IFilePathSettings>();
        _fixture = new Fixture();

        mockFilePathSettings.Setup(s => s.Users).Returns("./users.json");

        _userRepository = new UserRepository(
            mockFilePathSettings.Object,
            _mockFileRepository.Object);
    }
    
    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(3).ToList();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userRepository.GetAllUsers();

        // Assert
        result.Should().BeEquivalentTo(users);
        _mockFileRepository.Verify(fileRepo => fileRepo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task SaveUsers_ShouldSaveAllUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(3).ToList(); 
        
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), users))
            .Returns(Task.CompletedTask);

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(users); 

        // Act
        await _userRepository.SaveUsers(users);
        var retrievedUsers = await _userRepository.GetAllUsers();

        // Assert
        Assert.Equal(users, retrievedUsers);
        _mockFileRepository.Verify(fileRepo => fileRepo.WriteDataToFile(It.IsAny<string>(), users), Times.Once);
    }
}
