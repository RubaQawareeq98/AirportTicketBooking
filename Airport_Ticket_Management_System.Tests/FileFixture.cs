using Airport_Ticket_Management_System.Tests.Data.MockingData;
using Data;
using Newtonsoft.Json;

namespace Airport_Ticket_Management_System.Tests;

public class FileFixture : IDisposable
{
    public IFilePathSettings FilePath{ get; } =new FilePathSettings("./flights.json", "./bookings.json", "./users.json");

    public FileFixture()
    {
        FilePath.Flights = Path.Combine(Path.GetTempPath(), "./flights.json");
        FilePath.Bookings = Path.Combine(Path.GetTempPath(), "./bookings.json");
        FilePath.Users = Path.Combine(Path.GetTempPath(), "./users.json");
        ResetFile();
    }

    private void ResetFile()
    {
        var flights = MockFlights.GetFlights();
        var bookings = MockBookings.GetMockBookings();
        var users = MockUsers.GetMockUsers();

        var flightsJson = JsonConvert.SerializeObject(flights, Formatting.Indented);
        var bookingsJson = JsonConvert.SerializeObject(bookings, Formatting.Indented);
        var usersJson = JsonConvert.SerializeObject(users, Formatting.Indented);

        File.WriteAllText(FilePath.Flights, flightsJson);
        File.WriteAllText(FilePath.Bookings, bookingsJson);
        File.WriteAllText(FilePath.Users, usersJson);
    }


    public void Dispose()
    {
        if (!File.Exists(FilePath.Flights) || !File.Exists(FilePath.Bookings) || !File.Exists(FilePath.Users)) return;
        File.Delete(FilePath.Flights);
        File.Delete(FilePath.Bookings);
        File.Delete(FilePath.Users);
    }
}