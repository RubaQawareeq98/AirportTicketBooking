using AutoFixture;
using AutoFixture.AutoMoq;
using Data.Bookings;
using FluentAssertions;
using Model.Bookings;
using Model.Users.Exceptions;
using Moq;
using Services.Bookings;
using Services.Bookings.Exceptions;

namespace Airport_Ticket_Management_System.Tests.Services;

public class BookingServiceTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IBookingRepository> _mockBookingRepository;

    private readonly BookingService _bookingService;

    public BookingServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockBookingRepository = _fixture.Freeze<Mock<IBookingRepository>>();
        _bookingService = _fixture.Create<BookingService>();
    }

    [Fact]
    public async Task GetAllBookings_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = _fixture.CreateMany<Booking>(2).ToList();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetAllBookings();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllBookings_ShouldThrowExceptionIfNoBookingsFound()
    {
        // Arrange
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking>());

        // Act & Assert
        var act = async () => await _bookingService.GetAllBookings();
        await act.Should().ThrowAsync<NoBookingFoundException>()
            .WithMessage("!!! No bookings found !!!");
    }

    [Fact]
    public async Task GetBookingById_ReturnsBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, bookingId)
            .Create();

        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking> { booking });

        // Act
        var result = await _bookingService.GetBookingById(bookingId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(bookingId);
    }

    [Fact]
    public async Task GetBookingById_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking>());

        // Act & Assert
        var act = async () => await _bookingService.GetBookingById(bookingId);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage("Invalid booking Id");
    }

    [Fact]
    public async Task GetFilteredBookings_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        const BookingFilterOptions filterOption = BookingFilterOptions.DepartureCountry;
        const string filterValue = "value";

        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking>());

        // Act & Assert
        var act = async () => await _bookingService.GetFilteredBooking(filterOption, filterValue);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage("!!! No bookings found !!!");
    }

    [Fact]
    public async Task AddBooking_ShouldAddBooking()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking>());

        // Act
        await _bookingService.AddBooking(booking);

        // Assert
        _mockBookingRepository.Verify(repo => repo.AddBooking(booking), Times.Once);
    }

    [Fact]
    public async Task AddBooking_ShouldThrowExceptionIfBookingExists()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking> { booking });

        // Act & Assert
        var act = async () => await _bookingService.AddBooking(booking);
        await act.Should().ThrowAsync<BookingAlreadyExistException>()
            .WithMessage("This Booing already exists");
    }

    [Fact]
    public async Task UpdateBooking_ShouldUpdateBooking()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking> { booking });

        // Act
        await _bookingService.UpdateBooking(booking);

        // Assert
        _mockBookingRepository.Verify(repo => repo.UpdateBooking(booking), Times.Once);
    }

    [Fact]
    public async Task UpdateBooking_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync(new List<Booking>());

        // Act & Assert
        var act = async () => await _bookingService.UpdateBooking(booking);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage("Booking not found");
    }

    [Fact]
    public async Task DeleteBooking_ShouldDeleteBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, bookingId)
            .Create();

        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync([booking]);

        // Act
        await _bookingService.DeleteBooking(bookingId);

        // Assert
        _mockBookingRepository.Verify(repo => repo.CancelBooking(bookingId), Times.Once);
    }

    [Fact]
    public async Task DeleteBooking_ShouldThrowExceptionIfBookingNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _mockBookingRepository.Setup(repo => repo.GetAllBookings())
            .ReturnsAsync([]);

        // Act & Assert
        var act = async () => await _bookingService.DeleteBooking(bookingId);
        await act.Should().ThrowAsync<BookingNotFoundException>()
            .WithMessage("Invalid booking Id");
    }
}