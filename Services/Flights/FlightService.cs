using Data;
using Model;

namespace Services.Flights;

public class FlightService(FlightRepository flightRepository) : IFlightService
{
    public async Task<List<Flight>> GetAllFlights()
    {
        return await flightRepository.GetAllFlights();
    }

    public async Task<Flight> GetFlightById(Guid flightId)
    {
        var flights = await flightRepository.GetAllFlights();
        var flight = flights.Find(f => f.Id == flightId);
        if (flight is null)
        {
            throw new FlightNotFoundException($"Flight with id {flightId} not found");  
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

    public async Task DeleteFlight(Guid flightId)
    {
        var flight = await GetFlightById(flightId);
        if (flight is null) throw new FlightNotFoundException("Flight not found");
        await flightRepository.DeleteFlight(flightId);
    }

    public async Task<ImportFlightResult> ImportFlight(string filePath)
    {
        return await flightRepository.ImportFlights(filePath);
    }
}