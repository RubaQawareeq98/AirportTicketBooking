using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Model.Bookings;
using Model.Flights;
using Views.Consoles;
using Views.Managers;
using Moq;
using Views;

namespace Airport_Ticket_Management_System.Tests.Views;

public class ManagerViewTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsoleService> _consoleServiceMock;
    private readonly ManagerView _view;

    public ManagerViewTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consoleServiceMock = _fixture.Freeze<Mock<IConsoleService>>();
        _view = _fixture.Create<ManagerView>();
    }

    [Fact]
    public void ShowManagerMenu_ShouldWriteExpectedMenuItems()
    {
        // Act
        _view.ShowManagerMenu();

        // Assert
        _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.Exactly(7));
    }

    [Theory]
    [InlineData("1", ManagerOptions.ViewFlights)]
    [InlineData("2", ManagerOptions.ViewBookings)]
    [InlineData("3", ManagerOptions.ViewPassengers)]
    [InlineData("4", ManagerOptions.FilterBookings)]
    [InlineData("5", ManagerOptions.ImportFlights)]
    [InlineData("6", ManagerOptions.Exit)]
    public void ReadManagerOptions_ShouldReturnValidOption(string input, ManagerOptions expected)
    {
        // Arrange
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(input);

        // Act
        var result = _view.ReadManagerOptions();

        // Assert
        expected.Should().Be(result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("7")]
    [InlineData("abc")]
    public void ReadManagerOptions_ShouldThrowsException_WhenInvalidInput(string input)
    {
        // Arrange
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(input);

        // Act & Assert
        _view.Invoking(_ => _view.ReadManagerOptions()).Should().Throw<InvalidOptionException>();
    }

    [Fact]
    public void ReadCsvPath_ShouldReturnValidCsvPath()
    {
        // Arrange
        var expectedPath = _fixture.Create<string>();
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(expectedPath);

        // Act
        var result = _view.ReadCsvPath();

        // Assert
        expectedPath.Should().Be(result);
    }

    [Fact]
    public void ReadCsvPath_ShouldThrowsException_WhenInvalidInput()
    {
        // Arrange
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(string.Empty);

        // Act & Assert
        _view.Invoking(_ => _view.ReadCsvPath()).Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void ShowFilterOptions_ShouldWriteExpectedOptions()
    {
        // Act
        _view.ShowFilterOptions();

        // Assert
        _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.Exactly(12));
    }

    [Theory]
    [InlineData("1", BookingFilterOptions.Id)]
    [InlineData("5", BookingFilterOptions.BookingDate)]
    [InlineData("11", BookingFilterOptions.PassengerName)]
    public void ReadFilterOption_ShouldReadValidOption(string input, BookingFilterOptions expected)
    {
        // Arrange
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(input);

        // Act
        var result = _view.ReadFilterOption();

        // Assert
        expected.Should().Be(result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("12")]
    [InlineData("abc")]
    public void ReadFilterOption_ShouldThrowsException_WhenInvalidInput(string input)
    {
        // Arrange
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(input);

        // Act & Assert
        _view.Invoking(v => v.ReadFilterOption()).Should().Throw<InvalidOptionException>();
    }

    [Fact]
    public void ReadFilterValue_ValidValue_ReturnsValue()
    {
        // Arrange
        var expectedValue = _fixture.Create<string>();
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(expectedValue);

        // Act
        var result = _view.ReadFilterValue();

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void ReadFilterValue_ShouldThrowsException_WhenInvalidInput()
    {
        // Arrange
        _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(string.Empty);

        // Act & Assert
        _view.Invoking(v => v.ReadFilterValue()).Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void ShowFlights_ShouldWriteFlightDetails()
    {
        // Arrange
        var flights = _fixture.Create<List<Flight>>();

        // Act
        _view.ShowFlights(flights);

        // Assert
        _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.AtLeast(flights.Count + 3));
    }

    [Fact]
    public void ShowBookings_ShouldWriteBookingDetails()
    {
        // Arrange
        var bookings = _fixture.Create<List<Booking>>();

        // Act
        _view.ShowBookings(bookings);

        // Assert
        _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.AtLeast(bookings.Count + 3));
    }

    [Fact]
    public void ShowBookingDetails_ShouldWriteBookingDetails()
    {
        // Arrange
        var bookingDetails = _fixture.Create<List<BookingDetails>>();

        // Act
        _view.ShowBookingDetails(bookingDetails);

        // Assert
        _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.AtLeast(bookingDetails.Count + 2));
    }
}
