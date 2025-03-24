using AutoFixture;
using AutoFixture.AutoMoq;
using Controllers;
using FluentAssertions;
using Model.Users;
using Moq;
using Services.Users;
using Views;

namespace Airport_Ticket_Management_System.Tests.Controllers;

public class LoginControllerTest
{
    private readonly IFixture _fixture;
    private readonly LoginController _controller;
    private readonly Mock<ILoginView> _loginViewMock;
    private readonly Mock<IUserService> _userServiceMock;


    public LoginControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _loginViewMock = _fixture.Freeze<Mock<ILoginView>>();
        _userServiceMock = _fixture.Freeze<Mock<IUserService>>();
        _controller = _fixture.Create<LoginController>();
    }

    [Fact]
    public async Task Login_ShouldReturnValidUser()
    {
        // Arrange
        var expectedUser = _fixture.Create<User>();
        _loginViewMock.Setup(v => v.ReadUserName()).Returns(expectedUser.UserName);
        _loginViewMock.Setup(v => v.ReadPassword()).Returns(expectedUser.Password);
        _userServiceMock.Setup(s => s.ValidateUser(expectedUser.UserName, expectedUser.Password))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.Login();

        // Assert
        result.Should().NotBeNull();
        result.UserName.Should().Be(expectedUser.UserName);
       
        _loginViewMock.Verify(v => v.WelcomeMessage(), Times.Once);
        _loginViewMock.Verify(v => v.ReadUserName(), Times.Once);
        _loginViewMock.Verify(v => v.ReadPassword(), Times.Once);
        _userServiceMock.Verify(s => s.ValidateUser(expectedUser.UserName, expectedUser.Password), Times.Once);
    }
}