using Data;
using Model;

namespace Services.Flights;

public class FlightService(FlightRepository flightRepository) : IFlightService
{
    
    public async Task<Flight> GetFlightById(string flightId)
    {
        var flights = await flightRepository.GetAllFlights();
        var id = Guid.TryParse(flightId, out var flightIdGuid) ? flightIdGuid : throw new InvalidCastException("Invalid flight Id");
        var flight = flights.Find(f => f.Id == id);
        if (flight is null)
        {
            throw new FlightNotFoundException($"Flight with id {id} not found");  
        }
        return flight;
    }

    public async Task<List<Flight>> GetFilteredFlights(FlightSearchParams flightSearchParams, string value)
    {
        var flights = await flightRepository.GetFilteredFlights(flightSearchParams, value);
        if (flights.Count == 0)
        {
            throw new FlightNotFoundException("No Match flights found"); 
        }
        return flights;
    }

    public async Task AddFlight(Flight flight)
    {
        var flights = await flightRepository.GetAllFlights();
        if (flights.Contains(flight))
        {
            throw new FlightAlreadyExist("This flight already exists");
        }
        await flightRepository.AddFlight(flight);
    }

    public async Task UpdateFlight(Flight flight)
    {
        var flights = await flightRepository.GetAllFlights();
        if (!flights.Contains(flight))
        {
            throw new FlightNotFoundException("Flight not found");
        }
        
        await flightRepository.UpdateFlight(flight);
    }

    public async Task DeleteFlight(string flightId)
    {
        var flight = await GetFlightById(flightId);
        if (flight is null) throw new FlightNotFoundException("Flight not found");
        var id = Guid.TryParse(flightId, out var flightIdGuid) ? flightIdGuid : throw new InvalidCastException("Invalid flight Id");
        await flightRepository.DeleteFlight(id);
    }

    public async Task<ImportFlightResult> ImportFlight(string filePath)
    {
        return await flightRepository.ImportFlights(filePath);
    }
}