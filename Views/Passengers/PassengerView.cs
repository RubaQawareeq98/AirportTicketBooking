using Data.Exceptions;
using Model;
using Model.Bookings;
using Model.Flights;
using Services.Bookings.Exceptions;
using Views.Consoles;

namespace Views.Passengers;

public class PassengerView (IConsoleService consoleService): IPassengerView
{
    public PassengerOptions ShowPassengerMainMenu()
    {
            ShowPassengerOptions();
            var option = consoleService.ReadLine();
            if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
            if (optionNumber is < 1 or > 7) throw new InvalidOptionException("Invalid option number");
            return (PassengerOptions)optionNumber;
    }

    public void ShowFilterOptions()
    {
        consoleService.WriteLine("If you want to filter flights based on Flight ID press 1");
        consoleService.WriteLine("If you want to filter flights based on departure Country press 2");
        consoleService.WriteLine("If you want to filter flights based on destination Country press 3");
        consoleService.WriteLine("If you want to filter flights based on departure date press 4");
        consoleService.WriteLine("If you want to filter flights based on departure airport press 5");
        consoleService.WriteLine("If you want to filter flights based on arrival airport press 6");
        consoleService.WriteLine("If you want to filter flights based on price press 7");
        consoleService.WriteLine("If you want to filter flights based on class press 8");
    }

    public FlightFilterOptions ReadFilterOptions()
    {
        consoleService.WriteLine("Enter the option number you want to filter flights based on it");
        var option = consoleService.ReadLine();
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 8) throw new InvalidOptionException("Invalid option number");
        return (FlightFilterOptions)optionNumber;
    }

    public string ReadFilterValue()
    {
        consoleService.WriteLine("Enter value you looking for");
        var value = consoleService.ReadLine();
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidDataException("Invalid option");
        }
        return value;
    }
    
    private void ShowPassengerOptions()
    {
        consoleService.WriteLine("If you want to view available flights press 1");
        consoleService.WriteLine("If you want to filter flights press 2");
        consoleService.WriteLine("If you want to book a flight press 3");
        consoleService.WriteLine("If you want to see your bookings press 4");
        consoleService.WriteLine("If you want to Cancel booking press 5");
        consoleService.WriteLine("If you want to modify booking press 6");
        consoleService.WriteLine("If you want to exit press 7");
    }

    public void ShowFlights(List<Flight> flights)
    {
        consoleService.WriteLine($"{"".PadLeft(150, '=')}");
        consoleService.WriteLine($"| {"ID",-5} | {"Departure Country",-8} | {"Destination Country",-8} " +
                          $"| {"Departure Date",-20} | {"Departure Airport",-8} | {"Arrival Airport",-8} |" +
                          $" {"Economy Class", -16} | {"Business Class", -16} | {"First Class",-16}");
        consoleService.WriteLine($"{"".PadLeft(150, '=')}");
        foreach (var flight in flights)
        {
            consoleService.WriteLine(flight.ToString());
        }
        consoleService.WriteLine($"{"".PadLeft(150, '=')}");
    }

    public void ShowBookings(List<Booking> bookings)
    {
        consoleService.WriteLine($"{"".PadLeft(170, '=')}");
        consoleService.WriteLine($"| {"ID",-5} | {"Passenger Id", -8} |2 {"Flight Id",-8} | {"Booking Date",-8} " +
                          $"| {"Flight Class",-20} | {"Price",-8} | {"Cancelled?",-8} |");
                             
        consoleService.WriteLine($"{"".PadLeft(170, '=')}");
        foreach (var booking in bookings)
        {
            consoleService.WriteLine(booking.ToString());
        }        consoleService.WriteLine($"{"".PadLeft(170, '=')}");
    }

    public Guid ReadFlightId()
    {
        consoleService.WriteLine("Enter The flight ID you want to book");
        return Guid.TryParse(consoleService.ReadLine(), out var flightId)? flightId : throw new BookingNotFoundException("Invalid booking id");

    }

    public FlightClass ReadFlightClass()
    {
        consoleService.WriteLine("Enter the flight class you want to book [Economy | Business | First]");
        var value = consoleService.ReadLine();

        if (string.IsNullOrEmpty(value) || !Enum.TryParse(value, true, out FlightClass flightClass))
        {
            throw new InvalidClassException();
        }
        return flightClass;
    }

    public Guid ReadBookingId()
    {
        consoleService.WriteLine("Enter the ID of the booking you want to cancel");
        return Guid.TryParse(consoleService.ReadLine(), out var bookingId)? bookingId : throw new BookingNotFoundException("Invalid booking id");
    }
}