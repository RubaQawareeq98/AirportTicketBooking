using AutoFixture;
using FluentAssertions;
using Views;

namespace Airport_Ticket_Management_System.Tests.Views;

public class LoginViewTest 
{
    private readonly IFixture _fixture;
    private readonly ConsoleService _consoleService;
    private readonly LoginView _view;

    public LoginViewTest()
    {
        _fixture = new Fixture();
        _consoleService = new ConsoleService();
        _view = new LoginView(_consoleService);
    }

    [Fact]
    public void WelcomeMessage_ShouldDisplayMessage()
    {
        // Arrange
        const string expectedMessage = "Welcome to Airport_Ticket_Booking_System";

        // Act
        _view.WelcomeMessage();

        // Assert
        _consoleService.GetOutput().Should().ContainSingle().Which.Should().Be(expectedMessage);
    }

    [Fact]
    public void ReadUsername_ShouldReturnUsername()
    {
        // Arrange
        var userNameInput = _fixture.Create<string>();
        _consoleService.SetInput(userNameInput);

        // Act
        var userName = _view.ReadUserName();

        // Assert
        userName.Should().Be(userNameInput);
    }

    [Fact]
    public void ReadUserName_ShouldPromptAgain_WhenInputIsEmpty()
    {
        // Arrange
        const string userNameInput1 = "";
        var userNameInput2 = _fixture.Create<string>();
        const string expectedMessage = "Please enter a valid username";

        _consoleService.SetInput(userNameInput1, userNameInput2);

        // Act
        var userName = _view.ReadUserName();

        // Assert
        userName.Should().Be(userNameInput2);
        _consoleService.GetOutput().Should().Contain(expectedMessage);
    }

    [Fact]
    public void ReadPassword_ShouldReturnPassword_WhenValidInput()
    {
        // Arrange
        var passwordInput = _fixture.Create<string>();
        _consoleService.SetInput(passwordInput);

        // Act
        var password = _view.ReadPassword();

        // Assert
        password.Should().Be(passwordInput);
    }

    [Fact]
    public void ReadPassword_ShouldPromptAgain_WhenInputIsEmpty()
    {
        // Arrange
        const string passwordInput1 = "";
        var passwordInput2 = _fixture.Create<string>();
        const string expectedMessage = "Please enter a valid password";

        _consoleService.SetInput(passwordInput1, passwordInput2);

        // Act
        var password = _view.ReadPassword();

        // Assert
        password.Should().Be(passwordInput2);
        _consoleService.GetOutput().Should().Contain(expectedMessage);
    }
}
