using Model;

namespace Services.Flights;

public interface IFlightService
{
    Task<List<Flight>> GetAllFlights();
    Task<Flight> GetFlightById(Guid flightId);
    Task<List<Flight>> GetFilteredFlights(FlightSearchParams flightSearchParams, string value);
    Task AddFlight(Flight flight);
    Task UpdateFlight(Flight flight);
    Task DeleteFlight(Guid flightId);
    Task<ImportFlightResult> ImportFlight(string filePath);
}