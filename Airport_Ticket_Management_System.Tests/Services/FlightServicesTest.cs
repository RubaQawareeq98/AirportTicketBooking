using Airport_Ticket_Management_System.Tests.Data.MockingData;
using AutoFixture;
using FluentAssertions;
using Model.Flights;
using Services.Flights.Exceptions;

namespace Airport_Ticket_Management_System.Tests.Services;

public class FlightServicesTest  : TestBase
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GetAllFlights_ReturnsFlights()
    {
        // Act
        var result = await FlightService.GetAllFlights();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetAllFlights_ShouldThrowExceptionIfNoFlightsFound()
    {
        // Arrange
        await FlightRepository.SaveFlights([]);

        // Act & Assert
        var act = async () => await FlightService.GetAllFlights();
        await act.Should().ThrowAsync<FlightNotFoundException>()
            .WithMessage("!!! No flight was found !!!");
    }

    [Fact]
    public async Task GetFlightById_ReturnsFlight()
    {
        // Arrange 
        var flightId = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85486");
        
        // Act
        var flight = await FlightService.GetFlightById(flightId);
        
        // Assert
        flight.Should().NotBeNull();
        flight.Id.Should().Be(flightId);
    }
    
    [Fact]
    public async Task GetFlightById_ShouldThrowExceptionIfFlightNotFound()
    {
        // Arrange 
        var flightId = Guid.NewGuid();
        var expectedMessage = $"Flight with id {flightId} not found";
        
        // Act & Assert
        var act = async () => await FlightService.GetFlightById(flightId);
        await act.Should().ThrowAsync<FlightNotFoundException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task GetFilteredFlights_ShouldReturnsFlights()
    {
        // Arrange
        const FlightFilterOptions filterOption = FlightFilterOptions.DepartureAirport;
        const string filterValue = "JFK";
        
        // Act
        var result = await FlightService.GetFilteredFlights(filterOption, filterValue);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
    }
    
    [Fact]
    public async Task GetFilteredFlights_ShouldThrowExceptionIfFlightNotFound()
    {
        // Arrange
        const FlightFilterOptions filterOption = FlightFilterOptions.DepartureAirport;
        const string filterValue = "value";
        const string expectedMessage = "No Match flights found";
        
        // Act & Assert
        var act = async () => await FlightService.GetFilteredFlights(filterOption, filterValue);
        await act.Should().ThrowAsync<FlightNotFoundException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task AddFlight_ShouldAddsFlight()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
      
        // Act
        await FlightService.AddFlight(flight);
        
        // Assert
        var flights = await FlightService.GetAllFlights();
        flights.Should().NotBeNull();
        flights.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task AddFlight_ShouldThrowExceptionIfFlightExists()
    {
        // Arrange
        var flight = MockFlights.GetFlights().First();
        const string expectedMessage = "This flight already exists";

        // Act & Assert
        var act = async () => await FlightService.AddFlight(flight);
        await act.Should().ThrowAsync<FlightAlreadyExistException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task UpdateFlight_ShouldUpdateFlight()
    {
        // Arrange
        var flight = MockFlights.GetFlights().First();
        var newDepartureAirport = _fixture.Create<string>();
        flight.DepartureAirport = newDepartureAirport;
      
        // Act
        await FlightService.UpdateFlight(flight);
        
        // Assert
        var flights = await FlightService.GetAllFlights();
        flights.Should().NotBeNull();
        flights.Should().ContainSingle(f => f != null && f.DepartureAirport == newDepartureAirport);
    }
    
    [Fact]
    public async Task UpdateFlight_ShouldThrowExceptionIfFlightNotFound()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        const string expectedMessage = "Flight not found";

        // Act & Assert
        var act = async () => await FlightService.UpdateFlight(flight);
        await act.Should().ThrowAsync<FlightNotFoundException>()
            .WithMessage(expectedMessage);
    } 
    
    [Fact]
    public async Task DeleteFlight_ShouldDeleteFlight()
    {
        // Arrange
        var flightId = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85486");
      
        // Act
        await FlightService.DeleteFlight(flightId);
        
        // Assert
        var flights = await FlightService.GetAllFlights();
        flights.Should().NotBeNull();
        flights.Should().ContainSingle();

    }
    
    [Fact]
    public async Task DeleteFlight_ShouldThrowExceptionIfFlightNotFound()
    {
        // Arrange
        var flightId = _fixture.Create<Guid>();
        var expectedMessage = $"Flight with id {flightId} not found";

        // Act & Assert
        var act = async () => await FlightService.DeleteFlight(flightId);
        await act.Should().ThrowAsync<FlightNotFoundException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public async Task ImportFlights_ShouldImportFlights()
    {
        // Arrange
        var filePath = _fixture.Create<string>();
        // Act
        var result = await FlightService.ImportFlight(filePath);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
    }
}
