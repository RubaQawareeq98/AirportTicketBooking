namespace Services.Flights;

public class FlightAlreadyExist(string message) : Exception (message)
{
}