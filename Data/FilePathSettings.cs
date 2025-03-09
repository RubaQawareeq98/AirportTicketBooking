namespace Airport_Ticket_Management_System;

public class FilePathSettings(string flights, string bookings, string users)
{
    public string Flights { get; init; } = flights;
    public string Bookings { get; init; } = bookings;
    public string Users { get; init; } = users;
}

