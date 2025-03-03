using Model;

namespace Data;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllBookings();
    Task<Booking> GetBookingById(Guid id);
    Task CreateBooking(Booking booking);
    Task UpdateBooking(Booking booking);
    Task CancelBooking(Guid bookingId);
    Task SaveBookings(List<Booking> bookings);
}