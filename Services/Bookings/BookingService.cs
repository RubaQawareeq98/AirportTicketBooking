using Data;
using Data.Bookings;
using Data.Flights;
using Model.Bookings;
using Model.Users;
using Model.Users.Exceptions;
using Services.Bookings.Exceptions;

namespace Services.Bookings;

public class BookingService (IBookingRepository bookingRepository,
    IFlightRepository flightRepository, IUserRepository userRepository) : IBookingService
{
    public async Task<List<Booking>> GetAllBookings()
    {
        var bookings = await bookingRepository.GetAllBookings();
        if (bookings.Count == 0) throw new NoBookingFoundException("!!! No bookings found !!!");
        return bookings;
    }

    public async Task<Booking> GetBookingById(Guid bookingId)
    {
        var bookings = await bookingRepository.GetAllBookings();
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking is null)
        {
            throw new BookingNotFoundException("Invalid booking Id");
        }
        return booking;
    }

    public async Task<List<BookingDetails>> GetFilteredBooking(BookingFilterOptions filterOption, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidDataException("search value is required");
        }
        
        var bookings = await bookingRepository.GetAllBookings();
        var flights = await flightRepository.GetAllFlights();
        var users = await userRepository.GetAllUsers();

        var result = bookings.Join(flights, 
                booking => booking.FlightId,
                flight => flight.Id, 
                (booking, flight) => new { booking, flight })
            .Join(users, 
                bf => bf.booking.PassengerId,
                user => user.Id, 
                (bf, user) => new BookingDetails(
                    bf.booking.Id,
                    bf.booking.BookingDate,
                    bf.booking.PassengerId, 
                    bf.booking.FlightId, 
                    bf.booking.FlightClass, 
                    bf.booking.Price,
                    bf.flight,
                    user)
            ).ToList();
        
        var filteredBookings = bookingRepository.GetFilteredBookings(result, filterOption, value);
        if (filteredBookings.Count == 0) throw new BookingNotFoundException("!!! No bookings found !!!");
        return filteredBookings;
    }

    public async Task AddBooking(Booking booking)
    {
        var bookings = await bookingRepository.GetAllBookings();
        if (bookings.Contains(booking))
        {
            throw new BookingAlreadyExistException("This Booing already exists");
        }
        await bookingRepository.AddBooking(booking);
    }

    public async Task UpdateBooking(Booking booking)
    {
        var bookings = await bookingRepository.GetAllBookings();
        if (!bookings.Contains(booking))
        {
            throw new BookingNotFoundException("Booking not found");
        }

        await bookingRepository.UpdateBooking(booking);
    }

    public async Task DeleteBooking(Guid bookingId)
    {
        var booking = await GetBookingById(bookingId);
        if (booking is null) throw new BookingNotFoundException("Booking not found");
        await bookingRepository.CancelBooking(bookingId);
    }
}