using Model;
using Model.Flights;

namespace Data.Flights;

public interface IFlightRepository
{
    Task<List<Flight>> GetAllFlights();
    Task SavaFlights(List<Flight> flights);
    Task AddFlight(Flight flight);
    Task UpdateFlight(Flight flight);
    Task DeleteFlight(Guid id);
    Task<List<string>> ImportFlights(string csvFilePath);
    Task<List<Flight>> GetFilteredFlights(FlightFilterOptions filterOptions, string value);

}