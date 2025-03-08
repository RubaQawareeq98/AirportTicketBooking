using Model.Bookings;

namespace Data.Bookings;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllBookings();
    Task AddBooking(Booking booking);
    Task UpdateBooking(Booking booking);
    Task CancelBooking(Guid bookingId);
    Task SaveBookings(List<Booking> bookings);
    List<BookingDetails> GetFilteredBookings(List<BookingDetails> bookingDetails, BookingFilterOptions filterOptions, string value);
}