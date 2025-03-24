namespace Model.Flights;

public class Flight
{ 
    
    public required Guid Id { get; init; }
    public required string DepartureCountry { get; set; }
    public required string DestinationCountry { get; init; }
    public required DateTime DepartureDate { get; init; }
    public required string DepartureAirport { get; set; }
    public required string ArrivalAirport { get; init; }
    public required Dictionary<FlightClass, double> Prices { get; init; } 
   
    private double GetPrice(FlightClass flightClass)
    {
        return Prices.GetValueOrDefault(flightClass, 0);
    }

    public override bool Equals(object? obj)
    {
        return obj is Flight flight && Id.Equals(flight.Id);
    }

    public override string ToString()
    {
        return $"| {Id.ToString(),-5} | {DepartureCountry,-16} | {DestinationCountry,-16} | {DepartureDate,-16} | " +
               $"{DepartureAirport,-16} | {ArrivalAirport,-16} | " +
               $" ${GetPrice(FlightClass.Economy)} | " +
               $" ${GetPrice(FlightClass.Business)} | " +
               $" ${GetPrice(FlightClass.First)} | ";
    }
}
