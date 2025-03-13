namespace Services.Flights.Exceptions;

public class FlightNotFoundException(string message) : Exception(message);