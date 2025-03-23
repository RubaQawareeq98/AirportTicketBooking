using Airport_Ticket_Management_System.Tests.Data.MockingData;
using AutoFixture;
using FluentAssertions;
using Model.Bookings;
using Model.Flights;
using Model.Users.Exceptions;
using Services.Bookings.Exceptions;

namespace Airport_Ticket_Management_System.Tests.Services;

public class BookingServiceTest : TestBase
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GetAllBookings_ShouldReturnAllBookings()
    {
        // Act
        var result = await BookingService.GetAllBookings();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetAllBookings_ShouldThrowExceptionIfNoBookingsFound()
    {
        // Arrange
        await BookingRepository.SaveBookings([]);
        const string expectedMessage = "!!! No bookings found !!!";

        // Act & Assert
        var act = async () => await BookingService.GetAllBookings();
        await act.Should().ThrowAsync<NoBookingFoundException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task GetBookingById_ReturnsBooking()
    {
        // Arrange 
        var bookingId = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484");
        
        // Act
        var booking = await BookingService.GetBookingById(bookingId);
        
        // Assert
        booking.Should().NotBeNull();
        booking.Id.Should().Be(bookingId);
    }
    
    [Fact]
    public async Task GetBookingById_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange 
        var bookingId = Guid.NewGuid();
        const string expectedMessage = "Invalid booking Id";
        
        // Act & Assert
        var act = async () => await BookingService.GetBookingById(bookingId);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task GetFilteredBookings_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        const BookingFilterOptions filterOption = BookingFilterOptions.DepartureCountry;
        const string filterValue = "value";
        const string expectedMessage = "!!! No bookings found !!!";
        
        // Act & Assert
        var act = async () => await BookingService.GetFilteredBooking(filterOption, filterValue);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task AddBooking_ShouldAddsBooking()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
      
        // Act
        await BookingService.AddBooking(booking);
        
        // Assert
        var bookings = await BookingService.GetAllBookings();
        bookings.Should().NotBeNull();
        bookings.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task AddBooking_ShouldThrowExceptionIfBookingExists()
    {
        // Arrange
        var booking = MockBookings.GetMockBookings().Last();
        const string expectedMessage = "This Booing already exists";

        // Act & Assert
        var act = async () => await BookingService.AddBooking(booking);
        await act.Should().ThrowAsync<BookingAlreadyExistException>()
            .WithMessage(expectedMessage);
    }
    
    [Fact]
    public async Task UpdateBooking_ShouldUpdateBooking()
    {
        // Arrange
        var booking = MockBookings.GetMockBookings().Last();
        var newFlightClass = _fixture.Create<FlightClass>();
        booking.FlightClass = newFlightClass;
      
        // Act
        await BookingService.UpdateBooking(booking);
        
        // Assert
        var bookings = await BookingService.GetAllBookings();
        bookings.Should().NotBeNull();
        bookings.Should().ContainSingle(b => b != null && b.FlightClass == newFlightClass);
    }
    
    [Fact]
    public async Task UpdateBooking_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        const string expectedMessage = "Booking not found";

        // Act & Assert
        var act = async () => await BookingService.UpdateBooking(booking);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage(expectedMessage);
    } 
    
    [Fact]
    public async Task DeleteBooking_ShouldDeleteBooking()
    {
        // Arrange
        var bookingId = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484");
      
        // Act
        await BookingService.DeleteBooking(bookingId);
        
        // Assert
        var bookings = await BookingService.GetAllBookings();
        bookings.Should().NotBeNull(); 
        bookings.Should().ContainSingle(b => b != null && b.Id == bookingId && b.Cancelled);
    }
    
    [Fact]
    public async Task DeleteBooking_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        var bookingId = _fixture.Create<Guid>();
        const string expectedMessage = "Invalid booking Id";

        // Act & Assert
        var act = async () => await BookingService.DeleteBooking(bookingId);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage(expectedMessage);
    }
}
