using Airport_Ticket_Management_System.Tests.Data.MockingData;
using AutoFixture;
using AutoFixture.AutoMoq;
using Data;
using Data.Bookings;
using Data.Exceptions;
using Data.Files;
using FluentAssertions;
using Model.Bookings;
using Model.Flights;
using Model.Users.Exceptions;
using Moq;

namespace Airport_Ticket_Management_System.Tests.Data;

public class BookingRepositoryTest
{
    private readonly Mock<IFileRepository<Booking>> _mockFileRepository;
    private readonly IFixture _fixture;
    private readonly BookingRepository _bookingRepository;

    public BookingRepositoryTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockFileRepository = _fixture.Freeze<Mock<IFileRepository<Booking>>>();
        var mockFilePathSettings = _fixture.Freeze<Mock<IFilePathSettings>>();


        mockFilePathSettings.Setup(s => s.Bookings).Returns("./bookings.json");

        _bookingRepository = _fixture.Create<BookingRepository>();
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
    public async Task SaveBookings_ShouldThrowExceptionIfFileDoesNotExist()
    {
        // Arrange
        var bookings = _fixture.CreateMany<Booking>(3).ToList(); 
        _mockFileRepository.Setup(repo => repo.WriteDataToFile(It.IsAny<string>(), bookings))
            .Throws<FileNotFoundException>();

        // Act && Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _bookingRepository.SaveBookings(bookings));
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
    public async Task GetBookings_ShouldThrowExceptionIfFileDoesNotExist()
    {
        // Arrange
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>()))
            .Throws<FileNotFoundException>();

        // Act && Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _bookingRepository.GetAllBookings());
    }

    [Fact]
    public async Task AddBooking_ShouldAddBooking()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        var bookings = _fixture.CreateMany<Booking>(3).ToList();
        _mockFileRepository.Setup(repo => repo.ReadDataFromFile(It.IsAny<string>())).ReturnsAsync(bookings);
        
        // Act
        await _bookingRepository.AddBooking(booking);
        
        // Assert 
        _mockFileRepository.Verify(repo => repo.WriteDataToFile(It.IsAny<string>(), It.IsAny<List<Booking>>()), Times.Once());
    }
    
    [Theory]
    [InlineData(BookingFilterOptions.DepartureCountry, "USA", 1)]
    [InlineData(BookingFilterOptions.DestinationCountry, "UK", 1)]
    [InlineData(BookingFilterOptions.PassengerName, "Ruba", 1)]
    [InlineData(BookingFilterOptions.ClassType, "Business", 1)]
    [InlineData(BookingFilterOptions.Cancelled, "", 1)] 
    [InlineData((BookingFilterOptions)99, "any", 0)] 
   
    public void GetFilteredBookings_ShouldReturnExpectedBookings(BookingFilterOptions filter, string value, int expectedCount)
    {
        // Act
        var mockBookings = MockBookings.GetMockBookings();
        var result = _bookingRepository.GetFilteredBookings(mockBookings, filter, value);

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    [Theory]
    [InlineData(BookingFilterOptions.Id, "invalidGuid", typeof(InvalidOperationException))]
    [InlineData(BookingFilterOptions.FlightId, "invalidGuid", typeof(InvalidOperationException))]
    [InlineData(BookingFilterOptions.BookingDate, "invalidDate", typeof(InvalidDateFormatException))]
    [InlineData(BookingFilterOptions.DepartureDate, "invalidDate", typeof(InvalidDateFormatException))]
    [InlineData(BookingFilterOptions.PassengerId, "invalidGuid", typeof(FormatException))]
    [InlineData(BookingFilterOptions.Price, "invalidPrice", typeof(FormatException))]
    public void GetFilteredBookings_ShouldThrowException_ForInvalidInputs(BookingFilterOptions filter, string value, Type expectedException)
    {
        // Act & Assert
        var mockBookings = MockBookings.GetMockBookings();
        Assert.Throws(expectedException, () => _bookingRepository.GetFilteredBookings(mockBookings, filter, value));
    }

    [Fact]
    public void GetFilteredBookings_ShouldReturnBookingById()
    {
        // Arrange
        var mockBookings = MockBookings.GetMockBookings();
        var targetBooking = mockBookings[0];
        const BookingFilterOptions filterOption = BookingFilterOptions.Id;
        var validId = targetBooking.Id.ToString();

        // Act
        var result = _bookingRepository.GetFilteredBookings(mockBookings, filterOption, validId);

        // Assert
        Assert.Single(result);
        Assert.Equal(targetBooking.Id, result[0].Id);
    }

    [Fact]
    public void GetFilteredBookings_ShouldReturnBookingByFlightId()
    {
        // Arrange
        var mockBookings = MockBookings.GetMockBookings();
        var targetFlight = mockBookings[0].Flight;
        const BookingFilterOptions filterOption = BookingFilterOptions.FlightId;
        var validFlightId = targetFlight.Id.ToString();

        // Act
        var result = _bookingRepository.GetFilteredBookings(mockBookings, filterOption, validFlightId);

        // Assert
        Assert.Single(result);
        Assert.Equal(targetFlight.Id, result[0].Flight.Id);
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
