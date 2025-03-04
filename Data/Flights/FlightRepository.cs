using System.Globalization;
using CsvHelper;
using Model;
using Model.Flights;

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

    public async Task<List<Flight>> GetFilteredFlights(FlightSearchParams searchParams, string value)
    {
        var flights = await GetAllFlights();
        return searchParams switch
        {
            FlightSearchParams.Id => Guid.TryParse(value, out var id)? flights.Where(flight => flight.Id == id).ToList() 
            : throw new InvalidDataException("Invalid Flight Id."),
            FlightSearchParams.DepartureCountry => flights.Where(flight => flight.DepartureCountry == value).ToList(),
            FlightSearchParams.DestinationCountry => flights.Where(flight => flight.DestinationCountry == value).ToList(),
            FlightSearchParams.DepartureDate => DateTime.TryParse(value, out var dateTime)
                ? flights.Where(flight => flight.DepartureDate.Date == dateTime)
                    .ToList()
                : throw new InvalidDataException("Invalid Departure date value input."),
            FlightSearchParams.DepartureAirport => flights.Where(flight => flight.DepartureAirport == value).ToList(),
            FlightSearchParams.ArrivalAirport => flights.Where(flight => flight.ArrivalAirport == value).ToList(),
            FlightSearchParams.Price => double.TryParse(value, out var price)
                ? flights.Where(flight => flight.Prices.ContainsValue(price)).ToList()
                : throw new InvalidDataException("Invalid Price value input."),
            FlightSearchParams.Class => Enum.TryParse(typeof(FlightClass), value, true, out var flightClass)
                ? flights.Where(flight => flight.Prices.ContainsKey((FlightClass)flightClass)).ToList()
                : throw new InvalidDataException("Invalid Flight class value input."),
            _ => flights
        };
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