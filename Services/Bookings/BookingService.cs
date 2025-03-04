using Data;
using Data.Bookings;
using Model.Bookings;

namespace Services.Bookings;

public class BookingService (IBookingRepository bookingRepository,
    IFlightRepository flightRepository, IUserRepository userRepository) : IBookingService
{
    public async Task<List<Booking>> GetAllBookings()
    {
        return await bookingRepository.GetAllBookings();
    }

    public async Task<Booking> GetBookingById(Guid bookingId)
    {
        var bookings = await bookingRepository.GetAllBookings();
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking is null)
        {
            throw new BookingNotFound("Invalid booking Id");
        }
        return booking;
    }

    public async Task<List<BookingDetails>> GetFilteredBooking(BookingSearchParameters searchParameter, string value)
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
                    bf.booking.PassengerId, 
                    bf.booking.FlightId, 
                    bf.booking.FlightClass, 
                    bf.booking.Price)
                {
                    Flight = bf.flight,
                    User = user
                }
            ).ToList();
        try
        {
            return bookingRepository.GetFilteredBookings(result, searchParameter, value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AddBooking(Booking booking)
    {
        var bookings = await bookingRepository.GetAllBookings();
        if (bookings.Contains(booking))
        {
            throw new BookingAlreadyExist("This Booing already exists");
        }
        await bookingRepository.AddBooking(booking);
    }

    public async Task UpdateBooking(Booking booking)
    {
        var bookings = await bookingRepository.GetAllBookings();
        if (!bookings.Contains(booking))
        {
            throw new BookingNotFound("Booking not found");
        }

        await bookingRepository.UpdateBooking(booking);
    }

    public async Task DeleteBooking(Guid bookingId)
    {
        var booking = await GetBookingById(bookingId);
        if (booking is null) throw new BookingNotFound("Booking not found");
        await bookingRepository.CancelBooking(bookingId);
    }
}