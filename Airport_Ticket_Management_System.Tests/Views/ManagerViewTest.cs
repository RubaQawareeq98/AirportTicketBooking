using AutoFixture;
using FluentAssertions;
using Model.Bookings;
using Model.Flights;
using Views.Managers;
using Views;

namespace Airport_Ticket_Management_System.Tests.Views;

public class ManagerViewTest
{
    private readonly IFixture _fixture;
    private readonly ConsoleService _consoleService;
    private readonly ManagerView _view;

    public ManagerViewTest()
    {
        _fixture = new Fixture();
        _consoleService = new ConsoleService();
        _view = new ManagerView(_consoleService);
    }

    [Fact]
    public void ShowManagerMenu_ShouldWriteExpectedMenuItems()
    {
        // Act
        _view.ShowManagerMenu();

        // Assert
        _consoleService.GetOutput().Count.Should().Be(7);
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
        _consoleService.SetInput(input);

        // Act
        var result = _view.ReadManagerOptions();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("7")]
    [InlineData("abc")]
    public void ReadManagerOptions_ShouldThrowException_WhenInvalidInput(string input)
    {
        // Arrange
        _consoleService.SetInput(input);

        // Act & Assert
        _view.Invoking(_ => _view.ReadManagerOptions()).Should().Throw<InvalidOptionException>();
    }

    [Fact]
    public void ReadCsvPath_ShouldReturnValidCsvPath()
    {
        // Arrange
        var expectedPath = _fixture.Create<string>();
        _consoleService.SetInput(expectedPath);

        // Act
        var result = _view.ReadCsvPath();

        // Assert
        result.Should().Be(expectedPath);
    }

    [Fact]
    public void ReadCsvPath_ShouldThrowException_WhenInvalidInput()
    {
        // Arrange
        _consoleService.SetInput(string.Empty);

        // Act & Assert
        _view.Invoking(_ => _view.ReadCsvPath()).Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void ShowFilterOptions_ShouldWriteExpectedOptions()
    {
        // Act
        _view.ShowFilterOptions();

        // Assert
        _consoleService.GetOutput().Count.Should().Be(12);
    }

    [Fact]
    public void ShowFlights_ShouldWriteFlightDetails()
    {
        // Arrange
        var flights = _fixture.Create<List<Flight>>();

        // Act
        _view.ShowFlights(flights);

        // Assert
        _consoleService.GetOutput().Count.Should().BeGreaterThanOrEqualTo(flights.Count + 3);
    }

    [Fact]
    public void ShowBookings_ShouldWriteBookingDetails()
    {
        // Arrange
        var bookings = _fixture.Create<List<Booking>>();

        // Act
        _view.ShowBookings(bookings);

        // Assert
        _consoleService.GetOutput().Count.Should().BeGreaterThanOrEqualTo(bookings.Count + 3);
    }

    [Fact]
    public void ShowBookingDetails_ShouldWriteBookingDetails()
    {
        // Arrange
        var bookingDetails = _fixture.Create<List<BookingDetails>>();

        // Act
        _view.ShowBookingDetails(bookingDetails);

        // Assert
        _consoleService.GetOutput().Count.Should().BeGreaterThanOrEqualTo(bookingDetails.Count + 2);
    }
    
    [Theory]
    [InlineData("1", BookingFilterOptions.Id)]
    [InlineData("5", BookingFilterOptions.BookingDate)]
    [InlineData("11", BookingFilterOptions.PassengerName)]
    public void ReadFilterOption_ShouldReadValidOption(string input, BookingFilterOptions expected)
    {
        // Arrange
        _consoleService.SetInput(input);

        // Act
        var result = _view.ReadFilterOption();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("12")]
    [InlineData("abc")]
    public void ReadFilterOption_ShouldThrowsException_WhenInvalidInput(string input)
    {
        // Arrange
        _consoleService.SetInput(input);

        // Act & Assert
        _view.Invoking(v => v.ReadFilterOption()).Should().Throw<InvalidOptionException>();
    }

    [Fact]
    public void ReadFilterValue_ValidValue_ReturnsValue()
    {
        // Arrange
        var expectedValue = _fixture.Create<string>();
        _consoleService.SetInput(expectedValue);

        // Act
        var result = _view.ReadFilterValue();

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public void ReadFilterValue_ShouldThrowsException_WhenInvalidInput()
    {
        // Arrange
        _consoleService.SetInput(string.Empty);

        // Act & Assert
        _view.Invoking(v => v.ReadFilterValue()).Should().Throw<InvalidDataException>();
    }
}
