using Model.Bookings;

namespace Services.Bookings;

public interface IBookingService
{
    Task<Booking> GetBookingById(string bookingId);
    Task<List<BookingDetails>> GetFilteredBooking(BookingSearchParameters searchParameter, string value);
    Task AddBooking(Booking booking);
    Task UpdateBooking(Booking booking);
    Task DeleteBooking(string bookingId);
}