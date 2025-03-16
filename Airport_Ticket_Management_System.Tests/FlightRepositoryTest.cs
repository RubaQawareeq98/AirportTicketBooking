using AutoFixture;
using Data;
using Data.Flights;
using FluentAssertions;
using Model;
using Model.Flights;
using Moq;

namespace Airport_Ticket_Management_System.Tests;

public class FlightRepositoryTest
{
    private readonly Mock<IFileRepository<Flight>> _mockFileRepository;
    private readonly Fixture _fixture;
    private readonly FlightRepository _flightRepository;

    public FlightRepositoryTest()
    {
        _mockFileRepository = new Mock<IFileRepository<Flight>>();
        var mockFlightValidator = new Mock<FlightValidator>();
        var mockFilePathSettings = new Mock<IFilePathSettings>();
        _fixture = new Fixture();

        mockFilePathSettings.Setup(s => s.Flights).Returns("./appsettings.json");

        _flightRepository = new FlightRepository(
            mockFilePathSettings.Object,
            mockFlightValidator.Object,
            _mockFileRepository.Object);
    }
    
    [Fact]
    public async Task SaveFlights_ShouldSaveAllFlights()
    {
        // Arrange
        var flights = _fixture.CreateMany<Flight>(3).ToList(); 
        
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), flights))
            .Returns(Task.CompletedTask);

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights); 

        // Act
        await _flightRepository.SaveFlights(flights);
        var retrievedFlights = await _flightRepository.GetAllFlights();

        // Assert
        Assert.Equal(flights, retrievedFlights);
        _mockFileRepository.Verify(fileRepo => fileRepo.WriteDataToFile(It.IsAny<string>(), flights), Times.Once);
    }
    
    [Fact]
    public async Task GetAllFlights_ShouldReturnAllFlights()
    {
        // Arrange
        var flights = _fixture.CreateMany<Flight>(3).ToList();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetAllFlights();

        // Assert
        result.Should().BeEquivalentTo(flights);
        _mockFileRepository.Verify(fileRepo => fileRepo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFlights_ShouldUpdateFlights()
    {
        var flights = _fixture.CreateMany<Flight>(3).ToList();
        var modifiedFlight = flights[0];
        var newDepartureCountry = _fixture.Create<string>();
        modifiedFlight.DepartureCountry = newDepartureCountry;
        
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Flight>>()))
            .Returns(Task.CompletedTask); 

        await _flightRepository.UpdateFlight(modifiedFlight);
        
        // Assert
        _mockFileRepository.Verify(repo => repo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Flight>>(f => 
            f.Any(flight => flight.Id == modifiedFlight.Id && flight.DepartureCountry == newDepartureCountry)
        )), Times.Once); 
    }

    [Fact]
    public async Task DeleteFlights_ShouldDeleteFlightIfExists()
    {
        var flights = _fixture.CreateMany<Flight>(3).ToList();
        var deletedFlightId = flights[0].Id;
        
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Flight>>()))
            .Returns(Task.CompletedTask); 
        
        await _flightRepository.DeleteFlight(deletedFlightId);
        // Assert
        Assert.Equal(2, flights.Count);
    }
    
    [Fact]
    public async Task DeleteFlights_ShouldThrowExceptionIfNotExists()
    {
        var flights = _fixture.CreateMany<Flight>(3).ToList();
        var deletedFlightId = _fixture.Create<Guid>();
        
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);
        
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _flightRepository.DeleteFlight(deletedFlightId));
        Assert.Equal(3, flights.Count);
    }

    [Fact]
    public async Task FilterFlights_ShouldReturnFilteredFlights()
    {
        var flights = new List<Flight>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DepartureCountry = "Germany",
                DestinationCountry = "France",
                DepartureDate = new DateTime(2020, 01, 01),
                DepartureAirport = "GRU",
                ArrivalAirport = "EZZ",
                Prices = new Dictionary<FlightClass, double>(500)
            },
            new()
            {
                Id = Guid.NewGuid(),
                DepartureCountry = "Jordan",
                DestinationCountry = "Turkey",
                DepartureDate = new DateTime(2020, 01, 01),
                DepartureAirport = "GRU",
                ArrivalAirport = "EZZ",
                Prices = new Dictionary<FlightClass, double>(500)
            },
      
        };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        var filteredFlights = await _flightRepository.GetFilteredFlights(FlightFilterOptions.DepartureCountry, "Germany");
        Assert.Single(filteredFlights);
    }
    
}