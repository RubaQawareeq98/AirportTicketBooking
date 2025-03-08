using Model.Bookings;

namespace Services.Bookings;

public interface IBookingService
{
    Task<List<Booking>> GetAllBookings();
    Task<Booking> GetBookingById(Guid bookingId);
    Task<List<BookingDetails>> GetFilteredBooking(BookingFilterOptions filterOption, string value);
    Task AddBooking(Booking booking);
    Task UpdateBooking(Booking booking);
    Task DeleteBooking(Guid bookingId);
}