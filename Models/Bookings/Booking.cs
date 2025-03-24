using Model.Flights;

namespace Model.Bookings;

public class Booking(Guid passengerId, Guid flightId, FlightClass flightClass, double price)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid PassengerId { get; set; } = passengerId;
    public Guid FlightId { get; set; } = flightId;
    public DateTime BookingDate { get; init; } = DateTime.Now;
    public FlightClass FlightClass { get; set; } = flightClass;
    public double Price { get; set; } = price;
    public bool Cancelled { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Booking booking && Id == booking.Id;
    }

    public override string ToString()
    {
        return $"{Id,-16}|{PassengerId, -10} | {FlightId,-10}| {BookingDate,-10} | {FlightClass, -12} | {Price, -5} | {Cancelled} | ";
    }
}
