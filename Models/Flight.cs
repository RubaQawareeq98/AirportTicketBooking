namespace Model;

public class Flight
{ 
    public required int Id { get; set; }
    public required string DepartureCountry { get; set; }
    public required string DestinationCountry { get; set; }
    public required DateTime DepartureDate { get; set; }
    public required string DepartureAirport { get; set; }
    public required string ArrivalAirport { get; set; }
    public required Dictionary<FlightClass, double> Prices { get; set; } 
   
    private double GetPrice(FlightClass flightClass)
    {
        return Prices.GetValueOrDefault(flightClass, 0);
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
