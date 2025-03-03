namespace Services.Flights;

public class FlightNotFoundException(string message) : Exception(message);