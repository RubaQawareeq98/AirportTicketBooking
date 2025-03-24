using Model.Bookings;
using Model.Flights;

namespace Views.Managers;

public interface IManagerView
{
    void ShowManagerMenu();
    ManagerOptions ReadManagerOptions();
    string ReadFilterValue();
    BookingFilterOptions ReadFilterOption();
    void ShowFlights(List<Flight> flights);
    void ShowBookings(List<Booking> bookings);
    void ShowBookingDetails(List<BookingDetails> bookingDetails);
    void ShowFilterOptions();
    string ReadCsvPath();
}