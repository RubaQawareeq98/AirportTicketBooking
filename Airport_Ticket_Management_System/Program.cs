using Data;
using Microsoft.Extensions.Configuration;
using Model;

namespace Airport_Ticket_Management_System;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Set base path
            .AddJsonFile(@"C:\Users\Ruba\OneDrive\Desktop\BE\Airport_Ticket_Management_System\Airport_Ticket_Management_System\appsettings.json", optional: false, reloadOnChange: true)  // Load appsettings.json
            .Build();
        var directorySettings = new DirectorySettings();
        configuration.GetSection("DirectorySettings").Bind(directorySettings);
        
        var filePath = Path.Combine(directorySettings.BaseDirectory,"flights.json");
        IFileRepository<Flight> fileRepository = new FileRepository<Flight>();
        var flightRepository = new FlightRepository(filePath, fileRepository);
       var result = await flightRepository.ImportFlights(@"C:\Users\Ruba\Downloads\Book.csv");

       // var id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece8548a");
       // await flightRepository.DeleteFlight(id);
        // foreach (var error in result.errorMessages)
        // {
        //     Console.WriteLine(error);
        // }
        Console.WriteLine("Hello, World!");
    }
}