using Model.Flights;

namespace Airport_Ticket_Management_System.Tests.Data.MockingData;

public abstract class MockFlights
{
    public static List<Flight> GetFlights()
    {
        return
        [
            new Flight
            {
                Id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484"),
                DepartureCountry = "USA",
                DestinationCountry = "UK",
                DepartureDate = new DateTime(2025, 5, 10),
                DepartureAirport = "JFK",
                ArrivalAirport = "LHR",
                Prices = new Dictionary<FlightClass, double>
                {
                    { FlightClass.Economy, 500.00 },
                    { FlightClass.Business, 1200.00 }
                }
            },

            new Flight
            {
                Id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85486"),
                DepartureCountry = "France",
                DestinationCountry = "Germany",
                DepartureDate = new DateTime(2025, 6, 15),
                DepartureAirport = "CDG",
                ArrivalAirport = "FRA",
                Prices = new Dictionary<FlightClass, double>
                {
                    { FlightClass.Economy, 450.00 }
                }
            }
        ];
    }
}