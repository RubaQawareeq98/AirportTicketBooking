using Model;

namespace Data;

public interface IFlightRepository
{
    Task<List<Flight>> GetAllFlights();
    Task SavaFlights(List<Flight> flights);
    Task AddFlight(Flight flight);
    Task<Flight> GetFlightById(Guid id);
    Task UpdateFlight(Flight flight);
    Task DeleteFlight(Guid id);
    Task<ImportFlightResult> ImportFlights(string csvfFilePath);
}