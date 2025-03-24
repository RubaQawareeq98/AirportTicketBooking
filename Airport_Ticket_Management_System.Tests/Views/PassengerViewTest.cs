using Views.Passengers;
using AutoFixture;
using AutoFixture.AutoMoq;
using Model.Bookings;
using Model.Flights;
using Data.Exceptions;
using FluentAssertions;
using Services.Bookings.Exceptions;
using Services.Flights.Exceptions;
using Views;

namespace Airport_Ticket_Management_System.Tests.Views;

public class PassengerViewTests
{
    private readonly IFixture _fixture;
    private readonly ConsoleService _consoleService;
    private readonly PassengerView _view;

    public PassengerViewTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consoleService = new ConsoleService();
        _view = new PassengerView(_consoleService);
    }

    [Fact]
    public void ShowPassengerMainMenu_ShouldReturnValidOption()
    {
        // Arrange
        const string optionInput = "4";
        const PassengerOptions validOption = PassengerOptions.ViewBookings;
        _consoleService.SetInput(optionInput);

        // Act
        var option = _view.ShowPassengerMainMenu();

        // Assert
        option.Should().Be(validOption);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("8")]
    [InlineData("abc")]
    public void ShowPassengerMainMenu_ShouldThrowException_WhenInvalidOption(string input)
    {
        // Act
        _consoleService.SetInput(input);

        // Assert
        _view.Invoking(x => x.ShowPassengerMainMenu()).Should().Throw<InvalidOptionException>();
    }
    
    [Fact]
    public void ShowFilterOptions_ShouldWriteExpectedOptions()
    {
        // Act
        _view.ShowFilterOptions();

        // Assert
        _consoleService.GetOutput().Count.Should().Be(8);
    }

    [Fact]
    public void ReadFilterOptions_ShouldReturnValidOption()
    {
        // Arrange
        const string filterInput = "4";
        const FlightFilterOptions validOption = FlightFilterOptions.DepartureDate;
        _consoleService.SetInput(filterInput);
        
        // Act
        var option = _view.ReadFilterOptions();

        // Assert
        option.Should().Be(validOption);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("9")]
    [InlineData("abc")]
    public void ReadFilterOptions_ShouldThrowException_WhenInvalid(string input)
    {
        // Act
        _consoleService.SetInput(input);

        // Assert
        _view.Invoking(x => x.ReadFilterOptions()).Should().Throw<InvalidOptionException>();
    }

    [Fact]
    public void ReadFilterValue_ShouldReturnValidValue()
    {
        // Arrange
        var valueInput = _fixture.Create<string>();
        _consoleService.SetInput(valueInput);
        
        // Act
        var value = _view.ReadFilterValue();

        // Assert
        value.Should().Be(valueInput);
    }
    
    [Fact]
    public void ReadFilterValue_ShouldThrowException_WhenEmpty()
    {
        // Arrange
        var filterValue = string.Empty;
        
        // Act
        _consoleService.SetInput(filterValue);

        // Assert
        _view.Invoking(x => x.ReadFilterValue()).Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void ReadFlightId_ShouldReturnValidGuid()
    {
        // Assert
        var validGuid = Guid.NewGuid();
        _consoleService.SetInput(validGuid.ToString());
        
        // Act
        var flightId = _view.ReadFlightId();

        // Assert
        flightId.Should().Be(validGuid);
    }

    [Fact]
    public void ReadFlightId_ShouldThrowException_WhenInvalidGuid()
    {
        // Arrange
        var flightId = string.Empty;
        
        // Act
        _consoleService.SetInput(flightId);

        // Assert
        _view.Invoking(x => x.ReadFlightId()).Should().Throw<FlightNotFoundException>();
    }

    [Fact]
    public void ReadFlightClass_ShouldReturnValidClass()
    {
        // Arrange
        var flightClassInput = _fixture.Create<FlightClass>();
        _consoleService.SetInput(flightClassInput.ToString());

        // Act
        var flightClass = _view.ReadFlightClass();

        // Assert
        flightClass.Should().Be(flightClassInput);
    }

    [Fact]
    public void ReadFlightClass_ShouldThrowException_WhenInvalid()
    {
        // Arrange
        const string flightClassInput = "InvalidClass";
        
        // Act
        _consoleService.SetInput(flightClassInput);

        // Assert
        _view.Invoking(x => x.ReadFlightClass()).Should().Throw<InvalidClassException>();
    }

    [Fact]
    public void ReadBookingId_ShouldReturnValidGuid()
    {
        // Arrange
        var validGuid = _fixture.Create<Guid>();
        _consoleService.SetInput(validGuid.ToString());
        
        // Act
        var bookingId = _view.ReadBookingId();

        // Assert
        bookingId.Should().Be(validGuid);
    }

    [Fact]
    public void ReadBookingId_ShouldThrowException_WhenInvalidGuid()
    {
        // Arrange
        var bookingId = string.Empty;
        
        // Act
        _consoleService.SetInput(bookingId);

        // Assert
        _view.Invoking(x => x.ReadBookingId()).Should().Throw<BookingNotFoundException>();
    }

    [Fact]
    public void ShowFlights_ShouldDisplayFlightList()
    {
        // Arrange
        var flights = _fixture.CreateMany<Flight>(3).ToList();
        var expectedCount = flights.Count + 4;
       
        // Act
        _view.ShowFlights(flights);

        // Assert
        _consoleService.GetOutput().Count.Should().Be(expectedCount);
    }

    [Fact]
    public void ShowBookings_ShouldDisplayBookingList()
    {
        // Arrange
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        var expectedCount = bookings.Count + 4;

        // Act
        _view.ShowBookings(bookings);

        // Assert
        _consoleService.GetOutput().Count.Should().Be(expectedCount);
    }
}
