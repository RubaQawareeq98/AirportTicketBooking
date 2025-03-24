using AutoFixture;
using AutoFixture.AutoMoq;
using Controllers;
using FluentAssertions;
using Model.Bookings;
using Model.Flights;
using Model.Users.Exceptions;
using Moq;
using Services.Bookings;
using Services.Flights;
using Services.Flights.Exceptions;
using Views.Managers;

namespace Airport_Ticket_Management_System.Tests.Controllers;

public class ManagerControllerTest
{
    private readonly IFixture _fixture;
    private readonly ManagerController _controller;
    private readonly Mock<IManagerView> _managerViewMock;
    private readonly Mock<IFlightService> _flightServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    
    public ManagerControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _managerViewMock = _fixture.Freeze<Mock<IManagerView>>();
        _flightServiceMock = _fixture.Freeze<Mock<IFlightService>>();
        _bookingServiceMock = _fixture.Freeze<Mock<IBookingService>>();
        _controller = _fixture.Create<ManagerController>();
    }

    [Fact]
    public async Task ManagerPage_ShouldCallViewFlights()
    {
        // Arrange
        var flights = _fixture.Create<List<Flight>>();
        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.ViewFlights)
            .Returns(ManagerOptions.Exit);
            
        _flightServiceMock.Setup(s => s.GetAllFlights()).ReturnsAsync(flights);

        // Act
        await _controller.ManagePage();

        // Assert
        _flightServiceMock.Verify(s => s.GetAllFlights(), Times.Once);
        _managerViewMock.Verify(v => v.ShowFlights(flights), Times.Once);
    }
    
    [Fact]
    public async Task ManagerPage_ShouldCallViewBookings()
    {
        // Arrange
        var bookings = _fixture.Create<List<Booking>>();
        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.ViewBookings)
            .Returns(ManagerOptions.Exit);
            
        _bookingServiceMock.Setup(s => s.GetAllBookings()).ReturnsAsync(bookings);

        // Act
        await _controller.ManagePage();

        // Assert
        _bookingServiceMock.Verify(s => s.GetAllBookings(), Times.Once);
        _managerViewMock.Verify(v => v.ShowBookings(bookings), Times.Once);
    }
    
    [Fact]
    public async Task ManagerPage_ShouldHandleExceptionCallViewFlights()
    {
        // Arrange
        _bookingServiceMock.Setup(s => s.GetAllBookings()).ThrowsAsync(new NoBookingFoundException("!!! No bookings found !!!"));
        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.ViewBookings)
            .Returns(ManagerOptions.Exit);

        // Act
       var exception = await Record.ExceptionAsync(() => _controller.ManagePage());

        // Assert
        exception.Should().BeNull();
        _bookingServiceMock.Verify(s => s.GetAllBookings(), Times.Once);
    }
    
    [Fact]
    public async Task ManagerPage_ShouldHandleExceptionCallViewBookings()
    {
        // Arrange
        _flightServiceMock.Setup(s => s.GetAllFlights()).ThrowsAsync(new FlightNotFoundException("!!! No flight was found !!!"));
        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.ViewFlights)
            .Returns(ManagerOptions.Exit);

        // Act
        var exception = await Record.ExceptionAsync(() => _controller.ManagePage());

        // Assert
        exception.Should().BeNull();
        _flightServiceMock.Verify(s => s.GetAllFlights(), Times.Once);
    }
    
    [Fact]
    public async Task ManagerPage_ShouldCallFilterBookings()
    {
        // Arrange
        var bookings = _fixture.Create<List<BookingDetails>>();
        var filterOption = _fixture.Create<BookingFilterOptions>();
        var filterValue = _fixture.Create<string>();

        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.FilterBookings)
            .Returns(ManagerOptions.Exit);
        _managerViewMock.Setup(v => v.ReadFilterOption()).Returns(filterOption);
        _managerViewMock.Setup(v => v.ReadFilterValue()).Returns(filterValue);

        _bookingServiceMock.Setup(s => s.GetFilteredBooking(filterOption, filterValue))
            .ReturnsAsync(bookings);

        // Act
        await _controller.ManagePage();

        // Assert
        _bookingServiceMock.Verify(s => s.GetFilteredBooking(filterOption, filterValue), Times.Once);
        _managerViewMock.Verify(v => v.ShowBookingDetails(bookings), Times.Once);
    }
   
    [Fact]
    public async Task ManagerPage_ShouldCallImportFlights()
    {
        // Arrange
        var importResults = _fixture.Create<List<string>>();
        var csvPath = _fixture.Create<string>();

        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.ImportFlights)
            .Returns(ManagerOptions.Exit);
        
        _managerViewMock.Setup(v => v.ReadCsvPath()).Returns(csvPath);
        _flightServiceMock.Setup(s => s.ImportFlight(csvPath)).ReturnsAsync(importResults);

        // Act
        await _controller.ManagePage();

        // Assert
        _flightServiceMock.Verify(s => s.ImportFlight(csvPath), Times.Once);
    }

    [Fact]
    public async Task ManagerPage_ShouldExit_WhenExitOptionSelected()
    {
        // Arrange
        _managerViewMock.SetupSequence(v => v.ReadManagerOptions())
            .Returns(ManagerOptions.Exit);

        // Act
        await _controller.ManagePage();

        // Assert
        _managerViewMock.Verify(v => v.ShowManagerMenu(), Times.Once);
    }
}
