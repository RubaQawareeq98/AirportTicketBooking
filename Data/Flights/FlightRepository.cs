using System.Globalization;
using CsvHelper;
using Data.Exceptions;
using Model;
using Model.Flights;

namespace Data.Flights;

public class FlightRepository(string filePath, IFileRepository<Flight> fileRepository) : IFlightRepository
{

    public async Task<List<Flight>> GetAllFlights()
    {
        try
        {
            var flights = await fileRepository.ReadDataFromFile(filePath);
            return flights;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task SavaFlights(List<Flight> flights)
    {
        try
        {
            await fileRepository.WriteDataToFile(filePath, flights);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task AddFlight(Flight flight)
    {
        try
        {
            var flights = await GetAllFlights();
            flights.Add(flight);
            await SavaFlights(flights);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
       
    }

    public async Task UpdateFlight(Flight modifiedFlight)
    {
        var flights = await GetAllFlights();
        for (var i = 0; i < flights.Count; i++)
            if (flights[i].Equals(modifiedFlight))
            {
                flights[i] = modifiedFlight;
                break;
            }
        await SavaFlights(flights);
    }


    public async Task DeleteFlight(Guid id)
    {
        var flights = await GetAllFlights();
        var flight = flights.Find(f => f.Id == id);
        if (flight is null)
        {
            throw new InvalidOperationException("Flight not found");
        }
        flights.Remove(flight);
        await SavaFlights(flights);
    }

    public async Task<List<string>> ImportFlights(string csvFilePath)
    {
        var existFlights = await GetAllFlights();
        var responses = new List<string>();
        try
        {
            if (!File.Exists(csvFilePath))
            {
                responses.Add("File does not exist");
                return responses;
            }

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<FlightMap>(); 
            var flights = csv.GetRecords<Flight>().ToList();

            foreach (var flight in flights)
            {
                if (existFlights.Contains(flight))
                {
                    responses.Add($"Flight with Id ={flight.Id} already exists");
                    continue;
                }
                var validator = new FlightValidator();  
                var result = await validator.ValidateAsync(flight);
                if (!result.IsValid)
                {
                    return result.Errors.Select(e => e.ErrorMessage).ToList();
                }
                await AddFlight(flight);
                responses.Add($"Flight with Id ={flight.Id} imported successfully");
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
        return responses;
    }

    public async Task<List<Flight>> GetFilteredFlights(FlightFilterOptions filterOptions, string value)
    {
        var flights = await GetAllFlights();
        return filterOptions switch
        {
            FlightFilterOptions.Id => Guid.TryParse(value, out var id)
                ? flights.Where(flight => flight.Id == id).ToList()
                : throw new InvalidDataException("Invalid Flight Id."),
            
            FlightFilterOptions.DepartureCountry => flights.Where(flight => flight.DepartureCountry == value).ToList(),
            
            FlightFilterOptions.DestinationCountry => flights.Where(flight => flight.DestinationCountry == value).ToList(),
            
            FlightFilterOptions.DepartureDate => DateTime.TryParse(value, out var dateTime)
                ? flights.Where(flight => flight.DepartureDate.Date == dateTime).ToList()
                : throw new InvalidDateFormatException(),
            
            FlightFilterOptions.DepartureAirport => flights.Where(flight => flight.DepartureAirport == value).ToList(),
            
            FlightFilterOptions.ArrivalAirport => flights.Where(flight => flight.ArrivalAirport == value).ToList(),
            
            FlightFilterOptions.Price => double.TryParse(value, out var price)
                ? flights.Where(flight => flight.Prices.ContainsValue(price)).ToList()
                : throw new InvalidDataException("Invalid Price value input."),
            
            FlightFilterOptions.Class => Enum.TryParse(typeof(FlightClass), value, true, out var flightClass)
                ? flights.Where(flight => flight.Prices.ContainsKey((FlightClass)flightClass)).ToList()
                : throw new InvalidClassException(),
            
            _ => flights
        };
    }
}