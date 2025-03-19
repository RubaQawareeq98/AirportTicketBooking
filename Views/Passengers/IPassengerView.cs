using Model;
using Model.Bookings;
using Model.Flights;

namespace Views.Passengers;

public interface IPassengerView
{
    PassengerOptions ShowPassengerMainMenu();
    void ShowFilterOptions();
    void ShowFlights(List<Flight> flights);
    void ShowBookings(List<Booking> bookings);
    string ReadFilterValue();
    Guid ReadBookingId();
    FlightClass ReadFlightClass();
    Guid ReadFlightId();
    FlightFilterOptions ReadFilterOptions();


}