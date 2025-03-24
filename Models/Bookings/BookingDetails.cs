using Model.Flights;
using Model.Users;

namespace Model.Bookings;

public class BookingDetails : Booking
{
    public BookingDetails(Guid bookingId, DateTime bookingDate, Guid passengerId, Guid flightId, FlightClass flightClass,
        double price, Flight flight, User user) : base(passengerId, flightId, flightClass, price)
    {
        Flight = flight;
        User = user;
        Id = bookingId;
        BookingDate = bookingDate;
    }

    public Flight Flight { get; }
    public User User { get; }

    public override string ToString()
    {
        return $"Booking: {base.ToString()}, Flight: {Flight.Id}: {Flight.DepartureCountry} --> {Flight.DestinationCountry}, Passenger: {User}";
    }
}