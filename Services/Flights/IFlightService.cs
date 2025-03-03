using Model;

namespace Services.Flights;

public interface IFlightService
{
    Task<Flight> GetFlightById(string flightId);
    Task<List<Flight>> GetFilteredFlights(FlightSearchParams flightSearchParams, string value);
    Task AddFlight(Flight flight);
    Task UpdateFlight(Flight flight);
    Task DeleteFlight(string flightId);
    Task<ImportFlightResult> ImportFlight(string filePath);
}