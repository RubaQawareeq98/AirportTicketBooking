using AutoFixture;
using AutoFixture.AutoMoq;
using Data;
using Data.Users;
using FluentAssertions;
using Model.Users;
using Moq;

namespace Airport_Ticket_Management_System.Tests.Data;

public class UserRepositoryTest
{
    private readonly Mock<IFileRepository<User>> _mockFileRepository;
    private readonly IFixture _fixture;
    private readonly UserRepository _userRepository;

    public UserRepositoryTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockFileRepository = _fixture.Freeze<Mock<IFileRepository<User>>>();
        var mockFilePathSettings = _fixture.Freeze<Mock<IFilePathSettings>>();

        mockFilePathSettings.Setup(s => s.Users).Returns("./users.json");

        _userRepository = _fixture.Create<UserRepository>();
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
    public async Task GetAllUsers_ShouldThrowExceptionIfFileDoesNotExist()
    {
        // Arrange
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .Throws<FileNotFoundException>();
        
        // Act && Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _userRepository.GetAllUsers());
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
    
     [Fact]
    public async Task SaveUsers_ShouldThrowExceptionIfFileDoesNotExist()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(3).ToList(); 
        
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), users))
            .Throws<FileNotFoundException>();
        
        // Act && Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _userRepository.SaveUsers(users));
    }

    [Fact]
    public async Task AddUser_ShouldAddUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var users = _fixture.CreateMany<User>(3).ToList();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>())).ReturnsAsync(users);
        
        // Act
        await _userRepository.AddUser(user);
        
        // Assert
        var retrievedUsers = await _userRepository.GetAllUsers();
        
        retrievedUsers.Should().BeEquivalentTo(users);
        _mockFileRepository.Verify(fileRepo => fileRepo.WriteDataToFile(It.IsAny<string>(), users), Times.Once);
    }
    
    [Fact]
    public async Task AddUser_ShouldThrowExceptionIfFileDoesNotExist()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .Throws<FileNotFoundException>();
        
        // Act
        await _userRepository.AddUser(user);
        
        // Assert
        _mockFileRepository.Verify(fileRepo => fileRepo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<User>>()), Times.Never);
    }
    
}
