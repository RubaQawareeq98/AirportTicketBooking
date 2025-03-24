using AutoFixture;
using AutoFixture.AutoMoq;
using Data;
using Data.Exceptions;
using Data.Files;
using Data.Flights;
using FluentAssertions;
using Model.Flights;
using Moq;

namespace Airport_Ticket_Management_System.Tests.Data;

public class FlightRepositoryTest
{
    private readonly Mock<IFileRepository<Flight>> _mockFileRepository;
    private readonly Mock<IFilePathSettings> _mockFilePathSettings;
    private readonly Mock<FlightValidator> _mockFlightValidator;
    private readonly IFixture _fixture;
    private readonly FlightRepository _flightRepository;

    public FlightRepositoryTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockFileRepository = _fixture.Freeze<Mock<IFileRepository<Flight>>>();
        _mockFilePathSettings = _fixture.Freeze<Mock<IFilePathSettings>>();
        _mockFlightValidator = _fixture.Freeze<Mock<FlightValidator>>();

        _mockFilePathSettings.Setup(s => s.Flights).Returns("./flights.json");

        _flightRepository = _fixture.Create<FlightRepository>();
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
    }

    [Fact]
    public async Task SaveFlights_ShouldCallWriteDataToFile()
    {
        // Arrange
        var flights = _fixture.CreateMany<Flight>(3).ToList();

        // Act
        await _flightRepository.SaveFlights(flights);

        // Assert
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), flights), Times.Once);
    }

    [Fact]
    public async Task AddFlight_ShouldAddFlightToList()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        await _flightRepository.AddFlight(flight);

        // Assert
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Flight>>(f => f.Contains(flight))), Times.Once);
    }

    [Fact]
    public async Task UpdateFlight_ShouldUpdateExistingFlight()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        var modifiedFlight = _fixture.Build<Flight>()
            .With(f => f.Id, flight.Id)
            .Create();

        // Act
        await _flightRepository.UpdateFlight(modifiedFlight);

        // Assert
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Flight>>(f => f.Contains(modifiedFlight))), Times.Once);
    }

    [Fact]
    public async Task DeleteFlight_ShouldRemoveFlightFromList()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        await _flightRepository.DeleteFlight(flight.Id);

        // Assert
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Flight>>(f => !f.Contains(flight))), Times.Once);
    }

    [Fact]
    public async Task DeleteFlight_ShouldThrowException_WhenFlightNotFound()
    {
        // Arrange
        var flights = new List<Flight>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _flightRepository.DeleteFlight(Guid.NewGuid()));
    }
    
    [Fact]
    public async Task ImportFlights_ShouldReturnError_WhenFileDoesNotExist()
    {
        // Arrange
        var csvFilePath = "./nonexistent.csv";

        // Act
        var result = await _flightRepository.ImportFlights(csvFilePath);

        // Assert
        result.Should().Contain("File does not exist");
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterById()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.Id, flight.Id.ToString());

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldThrowException_WhenInvalidId()
    {
        // Arrange
        var flights = new List<Flight>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => _flightRepository.GetFilteredFlights(FlightFilterOptions.Id, "invalid-id"));
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByDepartureCountry()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.DepartureCountry, flight.DepartureCountry);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByDestinationCountry()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.DestinationCountry, flight.DestinationCountry);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByDepartureDate()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.DepartureDate, flight.DepartureDate.ToString("yyyy-MM-dd"));

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldThrowException_WhenInvalidDateFormat()
    {
        // Arrange
        var flights = new List<Flight>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDateFormatException>(() => _flightRepository.GetFilteredFlights(FlightFilterOptions.DepartureDate, "invalid-date"));
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByDepartureAirport()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.DepartureAirport, flight.DepartureAirport);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByArrivalAirport()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.ArrivalAirport, flight.ArrivalAirport);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByPrice()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        var price = flight.Prices.Values.First().ToString();

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.Price, price);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldThrowException_WhenInvalidPrice()
    {
        // Arrange
        var flights = new List<Flight>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => _flightRepository.GetFilteredFlights(FlightFilterOptions.Price, "invalid-price"));
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldFilterByClass()
    {
        // Arrange
        var flight = _fixture.Create<Flight>();
        var flights = new List<Flight> { flight };
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        var flightClass = flight.Prices.Keys.First().ToString();

        // Act
        var result = await _flightRepository.GetFilteredFlights(FlightFilterOptions.Class, flightClass);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(flight);
    }

    [Fact]
    public async Task GetFilteredFlights_ShouldThrowException_WhenInvalidClass()
    {
        // Arrange
        var flights = new List<Flight>();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(flights);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidClassException>(() => _flightRepository.GetFilteredFlights(FlightFilterOptions.Class, "invalid-class"));
    }
}