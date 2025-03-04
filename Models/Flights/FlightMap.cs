using CsvHelper.Configuration;

namespace Model.Flights;

public sealed class FlightMap : ClassMap<Flight>
{
    public FlightMap()
    {
        Map(f => f.Id);
        Map(f => f.DepartureCountry);
        Map(f => f.DestinationCountry);
        Map(f => f.DepartureDate);
        Map(f => f.DepartureAirport);
        Map(f => f.ArrivalAirport);
        
        Map(f => f.Prices).Convert(row =>
        {
            var prices = new Dictionary<FlightClass, double>
            {
                [FlightClass.Economy] = row.Row.GetField<double>("EconomyPrice"),
                [FlightClass.Business] = row.Row.GetField<double>("BusinessPrice"),
                [FlightClass.First] = row.Row.GetField<double>("FirstClassPrice")
            };
            return prices;
        });
    }
}