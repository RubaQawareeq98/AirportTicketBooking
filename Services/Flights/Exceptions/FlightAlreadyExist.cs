namespace Services.Flights.Exceptions;

public class FlightAlreadyExistException(string message) : Exception (message);