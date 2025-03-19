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
    private readonly Mock<FlightValidator> _mockFlightValidator;
    private readonly Fixture _fixture;
    private readonly FlightRepository _flightRepository;

    public FlightRepositoryTest()
    {
        _mockFileRepository = new Mock<IFileRepository<Flight>>();
        _mockFlightValidator = new Mock<FlightValidator>();
        var mockFilePathSettings = new Mock<IFilePathSettings>();
        _fixture = new Fixture();

        mockFilePathSettings.Setup(s => s.Flights).Returns("./flights.json");

        _flightRepository = new FlightRepository(
            mockFilePathSettings.Object,
            new FlightValidator(),
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
        var result = await _flightRepository.GetAllFlights();

        // Assert
        result.Should().BeEquivalentTo(flights);
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
    
    [Fact]
    public async Task ImportFlights_ShouldReturnError_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = _fixture.Create<string>();
        
        // Act
        var result = await _flightRepository.ImportFlights(filePath);

        // Assert
        Assert.Single(result);
        Assert.Equal("File does not exist", result[0]);
    }
    
    [Fact]
    public async Task ImportFlights_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        // Arrange
        var validCsvPath = _fixture.Create<string>();
        await File.WriteAllTextAsync(validCsvPath, "");
        
        // Act
        var result = await _flightRepository.ImportFlights(validCsvPath);

        // Assert
        Assert.Empty(result); 
    }
    
    [Fact]
    public async Task ImportFlights_ShouldReturnError_WhenFlightAlreadyExists()
    {
        // Arrange
        var validCsvPath = _fixture.Create<string>();
        var flight = new Flight
        {
            Id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85483"),
            DepartureCountry = "USA",
            DestinationCountry = "UK",
            DepartureDate = new DateTime(2025, 02, 015),
            ArrivalAirport = "JFK",
            Prices = new Dictionary<FlightClass, double>
            {
                [FlightClass.Business]= 200,
                [FlightClass.Economy]= 500,
                [FlightClass.First]= 300
            },
            DepartureAirport = "LHK"
        };


        const string csvContent =
            "Id,DepartureCountry,DestinationCountry,DepartureDate,DepartureAirport,ArrivalAirport,EconomyPrice,BusinessPrice,FirstClassPrice\n" +
            "3da9efb3-185c-4233-bf4e-bbc0ece85483,USA,UK,2025-02-15 10:30,JFK,LHR,200.00,500.00,300\n";
        await File.WriteAllTextAsync(validCsvPath, csvContent);

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync([flight]);

        // Act
        var result = await _flightRepository.ImportFlights(validCsvPath);

        // Assert
        Assert.Single(result);
        Assert.Equal($"Flight with Id ={flight.Id} already exists", result[0]);
    }
    
    [Fact]
    public async Task ImportFlights_ShouldReturnError_WhenFlightIsInvalid()
    {
        // Arrange
        var validCsvPath = _fixture.Create<string>(); // Generate fake file path
        var flight = new Flight
        {
            Id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85485"),
            DepartureCountry = "", // INVALID (Empty)
            DestinationCountry = "UK",
            DepartureDate = new DateTime(2025, 05, 15), // Date was incorrect (015)
            ArrivalAirport = "JFK",
            Prices = new Dictionary<FlightClass, double>
            {
                [FlightClass.Business] = 200,
                [FlightClass.Economy] = 500,
                [FlightClass.First] = 300
            },
            DepartureAirport = "LHK"
        };

        const string csvContent =
            "Id,DepartureCountry,DestinationCountry,DepartureDate,DepartureAirport,ArrivalAirport,EconomyPrice,BusinessPrice,FirstClassPrice\n" +
            "3da9efb3-185c-4233-bf4e-bbc0ece85483,,UK,2025-04-15 10:30,JFK,LHR,200.00,500.00,300\n";
        await File.WriteAllTextAsync(validCsvPath, csvContent);

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync([flight]);

        // _mockFlightValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>())) // ✅ FIXED
        //     .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
        //     {
        //         new("DepartureCountry", "Departure country is required.") // ✅ MATCHES ASSERTION
        //     }));


        // Act
        var result = await _flightRepository.ImportFlights(validCsvPath);

        // Assert
        Assert.Single(result);
        Assert.Equal("Departure country is required.", result[0]); // ✅ MATCHES MOCKED ERROR MESSAGE
    }

    [Fact]
    public async Task ImportFlights_ShouldImportFlightsSuccessfully()
    {
        // Arrange
        var flight = new Flight
        {
            Id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85485"),
            DepartureCountry = "AMK", // INVALID (Empty)
            DestinationCountry = "UK",
            DepartureDate = new DateTime(2025, 05, 15), // Date was incorrect (015)
            ArrivalAirport = "JFK",
            Prices = new Dictionary<FlightClass, double>
            {
                [FlightClass.Business] = 200,
                [FlightClass.Economy] = 500,
                [FlightClass.First] = 300
            },
            DepartureAirport = "LHK"
        };
        var validCsvPath = _fixture.Create<string>();
        const string csvContent =
            "Id,DepartureCountry,DestinationCountry,DepartureDate,DepartureAirport,ArrivalAirport,EconomyPrice,BusinessPrice,FirstClassPrice\n" +
            "3da9efb3-185c-4233-bf4e-bbc0ece85483,USA,UK,2025-04-15 10:30,JFK,LHR,200.00,500.00,300\n";        File.WriteAllText(validCsvPath, csvContent);

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync([flight]);        
      
      //  _mockFlightRepo.Setup(repo => repo.AddFlight(It.IsAny<Flight>())).Returns(Task.CompletedTask);

        // Act
        var result = await _flightRepository.ImportFlights(validCsvPath);

        // Assert
        Assert.Single(result);
        Assert.Equal($"Flight with Id =3da9efb3-185c-4233-bf4e-bbc0ece85483 imported successfully", result[0]);
    }

    
    
}
