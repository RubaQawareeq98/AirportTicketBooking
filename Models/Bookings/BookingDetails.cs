using Model.Users;

namespace Model.Bookings;

public class BookingDetails(Guid passengerId, Guid flightId, FlightClass flightClass, double price) : Booking(passengerId, flightId, flightClass, price)
{
    public Flight Flight { get; set; }
    public User User { get; set; }

    public override string ToString()
    {
        return $"Booking: {base.ToString()}, Flight: {Flight.Id}: {Flight.DepartureCountry} --> {Flight.DestinationCountry}, Passenger: {User}";
    }
}