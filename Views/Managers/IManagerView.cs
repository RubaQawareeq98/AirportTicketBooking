using Model;
using Model.Bookings;

namespace Views.Managers;

public interface IManagerView
{
    Task ShowManagerMenu();
    Task HandleManagerSelection(ManagerOptions option);
    Task HandleImportFlights();
    Task HandleFilterBooking();
    Task HandleViewBookings();
    Task HandleViewFlights();
    void ShowFlights(List<Flight> flights);
    void ShowBookings(List<Booking> bookings);
    void ShowBookingDetails(List<BookingDetails> bookingDetails);
}