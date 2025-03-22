using Model.Flights;

namespace Services.Flights;

public interface IFlightService
{
    Task<List<Flight>> GetAllFlights();
    Task<Flight> GetFlightById(Guid flightId);
    Task<List<Flight>> GetFilteredFlights(FlightFilterOptions flightFilterOptions, string value);
    Task AddFlight(Flight flight);
    Task UpdateFlight(Flight flight);
    Task DeleteFlight(Guid flightId);
    Task<List<string>> ImportFlight(string filePath);
}