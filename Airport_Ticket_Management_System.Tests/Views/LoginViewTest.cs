using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Views;
using Views.Consoles;

namespace Airport_Ticket_Management_System.Tests.Views;

public class LoginViewTest 
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsoleService> _consoleServiceMock;
    private readonly LoginView _view;

    public LoginViewTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consoleServiceMock = _fixture.Freeze<Mock<IConsoleService>>();
        _view = new LoginView(_consoleServiceMock.Object); 
    }

    [Fact]
    public void WelcomeMessage_ShouldDisplayMessage()
    {
        // Act
        _view.WelcomeMessage();
        
        // Assert
        _consoleServiceMock.Verify(cs => cs.WriteLine("Welcome to Airport_Ticket_Booking_System"), Times.Once);
    }

    [Fact]
    public void ReadUsername_ShouldReturnUsername()
    {
        // Arrange
        var userNameInput = _fixture.Create<string>();
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(userNameInput);
        
        // Act
        var userName = _view.ReadUserName();
        
        // Assert
        Assert.Equal(userNameInput, userName);
    }
    
    [Fact]
    public void ReadUserName_ShouldPromptAgain_WhenInputIsEmpty()
    {
        // Arrange
        const string userNameInput1 = "";
        var userNameInput2 = _fixture.Create<string>();
        const string expectedMessage = "Please enter a valid username";

        _consoleServiceMock
            .SetupSequence(cs => cs.ReadLine())
            .Returns(userNameInput1)
            .Returns(userNameInput2);
        
        // Act
        var userName = _view.ReadUserName();
        
        // Assert
        Assert.Equal(userNameInput2, userName);
        _consoleServiceMock.Verify(cs => cs.WriteLine(expectedMessage), Times.Once);
    }

    [Fact]
    public void ReadPassword_ShouldReturnPassword_WhenValidInput()
    {
        // Arrange
        var passwordInput = _fixture.Create<string>();
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(passwordInput);
        
        // Act
        var password = _view.ReadPassword();
        
        // Assert
        Assert.Equal(passwordInput, password);
    }

    [Fact]
    public void ReadPassword_ShouldPromptAgain_WhenInputIsEmpty()
    {
        // Arrange
        const string passwordInput1 = "";
        var passwordInput2 = _fixture.Create<string>();
        const string expectedMessage = "Please enter a valid password";

        _consoleServiceMock
            .SetupSequence(cs => cs.ReadLine())
            .Returns(passwordInput1)
            .Returns(passwordInput2);
        
        // Act
        var password = _view.ReadPassword();
        
        // Assert
        Assert.Equal(passwordInput2, password);
        _consoleServiceMock.Verify(cs => cs.WriteLine(expectedMessage), Times.Once);
    }
}
