using Airport_Ticket_Management_System.Tests.Data.MockingData;
using AutoFixture;
using Data.Exceptions;
using FluentAssertions;
using Model;
using Model.Flights;

namespace Airport_Ticket_Management_System.Tests.Data;

public class FlightRepositoryTest : TestBase
{
    private readonly Fixture _fixture = new ();

    [Fact]
    public async Task GetAllFlights_ShouldReturnSavedFlights()
    {
        // Act
        var result = await FlightRepository.GetAllFlights();

        // Assert
        result.Should().HaveCount(2);
    }
    
    [Theory]
    [InlineData(FlightFilterOptions.DepartureCountry, "USA", 1)]
    [InlineData(FlightFilterOptions.DestinationCountry, "UK", 1)]
    [InlineData(FlightFilterOptions.DepartureAirport, "JFK", 1)]
    [InlineData(FlightFilterOptions.ArrivalAirport, "LHR", 1)]
    [InlineData(FlightFilterOptions.Price, "500.00", 1)]
    [InlineData(FlightFilterOptions.Class, "Economy", 2)]
    [InlineData(FlightFilterOptions.Class, "Business", 1)]
    [InlineData((FlightFilterOptions)99, "any", 2)]
    public async Task GetFilteredFlights_ShouldReturnExpectedFlights(FlightFilterOptions filter, string value, int expectedCount)
    {
        // Act
        var result = await FlightRepository.GetFilteredFlights(filter, value);

        // Assert
        result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task AddFlight_ShouldAddFlight()
    {
        // Arrange
        var newFlight = _fixture.Create<Flight>();

        // Act
        await FlightRepository.AddFlight(newFlight);
        var flights = await FlightRepository.GetAllFlights();

        // Assert
        flights.Should().HaveCount(3);
        flights.Find(f => f.Id == newFlight.Id).Should().BeEquivalentTo(newFlight);
    }

    [Fact]
    public async Task AddFlight_ShouldHandleException()
    {
        // Arrange
        var newFlight = _fixture.Create<Flight>();

        // Act
        await FlightRepository.AddFlight(newFlight);
        var flights = await FlightRepository.GetAllFlights();

        // Assert
        flights.Should().HaveCount(3);
        flights.Find(f => f.Id == newFlight.Id).Should().BeEquivalentTo(newFlight);
    }

    [Fact]
    public async Task UpdateFlight_ShouldModifyExistingFlight()
    {
        // Arrange
        var flight = MockFlights.GetFlights().First();
        const string newDepartureCountry = "Jordan";
        flight.DepartureCountry = newDepartureCountry;

        // Act
        await FlightRepository.UpdateFlight(flight);
        var updatedFlights = await FlightRepository.GetAllFlights();

        // Assert
        updatedFlights.Should().HaveCount(2);
        updatedFlights.First().DepartureCountry.Should().Be(newDepartureCountry);
    }

    [Theory]
    [InlineData(FlightFilterOptions.Id, "invalidGuid", typeof(InvalidDataException))]
    [InlineData(FlightFilterOptions.DepartureDate, "invalidDate", typeof(InvalidDateFormatException))]
    [InlineData(FlightFilterOptions.Price, "invalidPrice", typeof(InvalidDataException))]
    [InlineData(FlightFilterOptions.Class, "InvalidClass", typeof(InvalidClassException))]

    public async Task GetFilteredFlights_ShouldThrowException_ForInvalidInputs(FlightFilterOptions filter, string value,
        Type expectedException)
    {
        // Act & Assert
        await Assert.ThrowsAsync(expectedException, () => FlightRepository.GetFilteredFlights(filter, value));
    }

    [Fact]
    public async Task ImportFlights_ShouldReturnError_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = _fixture.Create<string>();

        // Act
        var result = await FlightRepository.ImportFlights(filePath);

        // Assert
        result.Should().ContainSingle()
            .Which.Should().Contain("File does not exist");
    }
}