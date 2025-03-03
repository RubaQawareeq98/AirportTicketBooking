using System.Globalization;
using CsvHelper;
using Model;

namespace Data;

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
        var flights =  await GetAllFlights();
        flights.Add(flight);
        await SavaFlights(flights);
    }

    public async Task<Flight> GetFlightById(Guid id)
    {
        var flights = await GetAllFlights();
        var flight = flights.Find(f => f.Id == id);
        if (flight is null)
        {
            throw new InvalidOperationException("Flight not found");
        }
        return flight;
    }

    public async Task UpdateFlight(Flight flight)
    {
        var flights = await GetAllFlights();
        var oldFlight = flights.Find(f => f.Id == flight.Id);
        if (oldFlight is null)
        {
            throw new InvalidOperationException("Flight not found"); 
        }
        oldFlight = flight;
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

    public async Task<ImportFlightResult> ImportFlights(string csvfFilePath)
    {
        var existFlights = await GetAllFlights();
        var importFlightsResult = new ImportFlightResult();
        try
        {
            if (!File.Exists(csvfFilePath))
            {
                importFlightsResult.status = ImportFlightStatus.Failure;
                importFlightsResult.errorMessages.Add("File does not exist");
                return importFlightsResult;
            }

            using var reader = new StreamReader(csvfFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<FlightMap>(); 
            var flights = csv.GetRecords<Flight>().ToList();

            foreach (var flight in flights)
            {
                if (existFlights.Contains(flight))
                {
                    importFlightsResult.status = ImportFlightStatus.InvalidFormat;
                    importFlightsResult.errorMessages.Add("Flight already exists");
                    continue;
                }
                var validationResult = ValidateFlight(flight);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    importFlightsResult.status = ImportFlightStatus.InvalidFormat;
                    importFlightsResult.errorMessages.Add(validationResult);
                    continue;
                }
                await AddFlight(flight);
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
        return importFlightsResult;
    }
    
    private static string ValidateFlight(Flight flight)
    {
        if (string.IsNullOrEmpty(flight.DepartureCountry))
            return $"Invalid Flight {flight.Id}: Departure country cannot be empty.";

        if (string.IsNullOrEmpty(flight.DestinationCountry))
            return $"Flight {flight.Id}: Destination country cannot be empty.";

        if (string.IsNullOrEmpty(flight.DepartureAirport))
            return $"Invalid Flight {flight.Id}: Departure airport cannot be empty.";

        if (string.IsNullOrEmpty(flight.ArrivalAirport))
            return $"Invalid Flight {flight.Id}: Arrival airport cannot be empty.";

        return flight.DepartureDate.Date < DateTime.Now.Date ? $"Invalid Flight {flight.Id}: Departure date cannot be in the past." : string.Empty;
    }

}