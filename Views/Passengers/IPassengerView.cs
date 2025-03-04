using Model;
using Model.Bookings;

namespace Views.Passengers;

public interface IPassengerView
{
    Task ShowPassengerMainMenu();
    void ShowFilterOptions();
    Task<List<Flight>> HandleFilterOption();
    Task HandlePassengerSelection(PassengerOptions passengerOptions);
    void ShowFlights(List<Flight> flights);
    void ShowBookings(List<Booking> bookings);
    Task HandleAddBooking();
    Task HandleCancelBooking();
    Task HandleModifyBooking();


}