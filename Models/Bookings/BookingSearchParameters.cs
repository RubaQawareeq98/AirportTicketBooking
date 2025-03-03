namespace Model;

public class BookingSearchParameters
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public FlightClass ClassType { get; set; }
    public bool Cancelled { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public string? DepartureCountry { get; set; }
    public string? DestinationCountry { get; set; }
    public double Price { get; set; }
    public string? PassengerName { get; set; }
    public Guid PassengerId { get; set; }
}

