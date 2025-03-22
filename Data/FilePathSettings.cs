namespace Data;

public class FilePathSettings(string flights, string bookings, string users) : IFilePathSettings
{
    public string Flights { get; set; } = flights;
    public string Bookings { get; set; } = bookings;
    public string Users { get; set; } = users;
}

