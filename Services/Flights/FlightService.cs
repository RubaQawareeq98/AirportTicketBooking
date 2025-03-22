using Data.Flights;
using Model.Flights;
using Services.Flights.Exceptions;

namespace Services.Flights;

public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    public async Task<List<Flight>> GetAllFlights()
    {
        var flights = await flightRepository.GetAllFlights();
        if (flights.Count == 0) throw new FlightNotFoundException("!!! No flight was found !!!");
        return flights;
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

    public async Task<List<Flight>> GetFilteredFlights(FlightFilterOptions flightFilterOptions, string value)
    {
        var flights = await flightRepository.GetFilteredFlights(flightFilterOptions, value);
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
            throw new FlightAlreadyExistException("This flight already exists");
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
        var flights = await flightRepository.GetAllFlights();
        var flight = flights.Find(f => f.Id == flightId);
        if (flight is null)
        {
            throw new FlightNotFoundException($"Flight with id {flightId} not found");  
        }
        await flightRepository.DeleteFlight(flightId);
    }

    public async Task<List<string>> ImportFlight(string filePath)
    {
        return await flightRepository.ImportFlights(filePath);
    }
}