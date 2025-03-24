using Airport_Ticket_Management_System.Tests.Data.MockingData;
using Data;
using Newtonsoft.Json;

namespace Airport_Ticket_Management_System.Tests;

public class FileFixture : IDisposable
{
    public IFilePathSettings FilePath { get; }

    public FileFixture()
    {
        FilePath = new FilePathSettings(
            Path.Combine( "./flights.json"),
            Path.Combine( "./bookings.json"),
            Path.Combine( "./users.json")
        );

        ResetFiles();
    }

    private void ResetFiles()
    {
        var flights = MockFlights.GetFlights();
        var bookings = MockBookings.GetMockBookings();
        var users = MockUsers.GetMockUsers();

        WriteToFile(FilePath.Flights, flights);
        WriteToFile(FilePath.Bookings, bookings);
        WriteToFile(FilePath.Users, users);
    }

    private void WriteToFile(string filePath, object data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public void Dispose()
    {
        DeleteFileIfExists(FilePath.Flights);
        DeleteFileIfExists(FilePath.Bookings);
        DeleteFileIfExists(FilePath.Users);
    }

    private void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}