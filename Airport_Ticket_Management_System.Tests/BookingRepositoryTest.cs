using AutoFixture;
using Data;
using Data.Bookings;
using FluentAssertions;
using Model;
using Model.Bookings;
using Model.Users.Exceptions;
using Moq;

namespace Airport_Ticket_Management_System.Tests;

public class BookingRepositoryTest
{
    private readonly Mock<IFileRepository<Booking>> _mockFileRepository;
    private readonly IFixture _fixture;
    private readonly BookingRepository _bookingRepository;

    public BookingRepositoryTest()
    {
        _mockFileRepository = new Mock<IFileRepository<Booking>>();
        var mockFilePathSettings = new Mock<IFilePathSettings>();
        _fixture = new Fixture();

        mockFilePathSettings.Setup(s => s.Bookings).Returns("./bookings.json");

        _bookingRepository = new BookingRepository(
            mockFilePathSettings.Object,
            _mockFileRepository.Object);
    }
    
    [Fact]
    public async Task SaveBookings_ShouldSaveAllBookings()
    {
        // Arrange
        var bookings = _fixture.CreateMany<Booking>(3).ToList(); 
        
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), bookings))
            .Returns(Task.CompletedTask);

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(bookings); 

        // Act
        await _bookingRepository.SaveBookings(bookings);
        var result = await _bookingRepository.GetAllBookings();

        // Assert
        result.Should().BeEquivalentTo(bookings);
        _mockFileRepository.Verify(fileRepo => fileRepo.WriteDataToFile(It.IsAny<string>(), bookings), Times.Once);
    }
    
    [Fact]
    public async Task GetAllBookings_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingRepository.GetAllBookings();

        // Assert
        result.Should().BeEquivalentTo(bookings);
        _mockFileRepository.Verify(fileRepo => fileRepo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
    }

   [Fact]
    public async Task UpdateBookings_ShouldUpdateBookings()
    {
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        var modifiedBooking = bookings[0];
        var newBookingClass = _fixture.Create<FlightClass>();
        modifiedBooking.FlightClass = newBookingClass;
        
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(bookings);
    
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Booking>>()))
            .Returns(Task.CompletedTask); 
    
        await _bookingRepository.UpdateBooking(modifiedBooking);
        
        // Assert
        _mockFileRepository.Verify(repo => repo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Booking>>(b => 
            b.Any(booking => booking.Id == modifiedBooking.Id && booking.FlightClass == newBookingClass)
        )), Times.Once); 
    }
    
    [Fact]
    public async Task UpdateBookings_ShouldNotUpdateBookings()
    {
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        var modifiedBooking = _fixture.Create<Booking>();
        var newBookingClass = _fixture.Create<FlightClass>();
        modifiedBooking.FlightClass = newBookingClass;
        
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(bookings);
    
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Booking>>()))
            .Returns(Task.CompletedTask); 
    
        await _bookingRepository.UpdateBooking(modifiedBooking);
        
        // Assert
        _mockFileRepository.Verify(repo => repo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Booking>>(b => 
            b.Any(booking => booking.Id == modifiedBooking.Id && booking.FlightClass == newBookingClass)
        )), Times.Never); 
    }

    [Fact]
    public async Task CancelBooking_ShouldCancelBooking()
    {
        var bookings = _fixture.Build<Booking>()
            .With(b => b.Cancelled, false)
            .CreateMany(3)
            .ToList();
        var bookingId = bookings[0].Id;

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(bookings);

        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Booking>>()))
            .Returns(Task.CompletedTask);

        await _bookingRepository.CancelBooking(bookingId);
        // Assert
        _mockFileRepository.Verify(repo => repo.ReadDataFromFile(It.IsAny<string>()), Times.Once);
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.Is<List<Booking>>(f => 
            f.Any(booking => booking.Id == bookingId && booking.Cancelled == true)
        )), Times.Once); 

    }
    
    [Fact]
    public async Task CancelBooking_ShouldNotCancelBooking()
    {
        var bookings = _fixture.Build<Booking>()
            .With(b => b.Cancelled, false)
            .CreateMany(3)
            .ToList();
        var bookingId = _fixture.Create<Guid>();

        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .ReturnsAsync(bookings);

        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Booking>>()))
            .Returns(Task.CompletedTask);

        // Assert
        await Assert.ThrowsAsync<NoBookingFoundException>(() => _bookingRepository.CancelBooking(bookingId));

    }

    
    [Fact]
    public void FilterBookings_ShouldReturnFilteredBookings()
    {
        var bookingDetails = _fixture.CreateMany<BookingDetails>(3).ToList();
        var filterValue = bookingDetails[0].Flight.DepartureCountry;
    
        var filteredBookings = _bookingRepository.GetFilteredBookings(bookingDetails, BookingFilterOptions.DepartureCountry, filterValue);
        Assert.Single(filteredBookings);
    }
    
}