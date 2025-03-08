using Data.Exceptions;
using Model;
using Model.Bookings;
using Model.Flights;
using Services.Bookings.Exceptions;

namespace Views.Passengers;

public class PassengerView : IPassengerView
{
    public PassengerOptions ShowPassengerMainMenu()
    {
            ShowPassengerOptions();
            var option = Console.ReadLine();
            if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
            if (optionNumber is < 1 or > 7) throw new InvalidOptionException("Invalid option number");
            return (PassengerOptions)optionNumber;
    }

    public void ShowFilterOptions()
    {
        Console.WriteLine("If you want to filter flights based on Flight ID press 1");
        Console.WriteLine("If you want to filter flights based on departure Country press 2");
        Console.WriteLine("If you want to filter flights based on destination Country press 3");
        Console.WriteLine("If you want to filter flights based on departure date press 4");
        Console.WriteLine("If you want to filter flights based on departure airport press 5");
        Console.WriteLine("If you want to filter flights based on arrival airport press 6");
        Console.WriteLine("If you want to filter flights based on price press 7");
        Console.WriteLine("If you want to filter flights based on class press 8");
    }

    public FlightFilterOptions ReadFilterOptions()
    {
        Console.WriteLine("Enter the option number you want to filter flights based on it");
        var option = Console.ReadLine();
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 7) throw new InvalidOptionException("Invalid option number");
        return (FlightFilterOptions)optionNumber;
    }

    public string ReadFilterValue()
    {
        Console.WriteLine("Enter value you looking for");
        var value = Console.ReadLine();
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidDataException("Invalid option");
        }
        return value;
    }
    
    private static void ShowPassengerOptions()
    {
        Console.WriteLine("If you want to view available flights press 1");
        Console.WriteLine("If you want to filter flights press 2");
        Console.WriteLine("If you want to book a flight press 3");
        Console.WriteLine("If you want to see your bookings press 4");
        Console.WriteLine("If you want to Cancel booking press 5");
        Console.WriteLine("If you want to modify booking press 6");
        Console.WriteLine("If you want to exit press 7");
    }

    public void ShowFlights(List<Flight> flights)
    {
        Console.WriteLine($"{"".PadLeft(150, '=')}");
        Console.WriteLine($"| {"ID",-5} | {"Departure Country",-8} | {"Destination Country",-8} " +
                          $"| {"Departure Date",-20} | {"Departure Airport",-8} | {"Arrival Airport",-8} |" +
                          $" {"Economy Class", -16} | {"Business Class", -16} | {"First Class",-16}");
        Console.WriteLine($"{"".PadLeft(150, '=')}");
        flights.ForEach(Console.WriteLine);
        Console.WriteLine($"{"".PadLeft(150, '=')}");
    }

    public void ShowBookings(List<Booking> bookings)
    {
        Console.WriteLine($"{"".PadLeft(170, '=')}");
        Console.WriteLine($"| {"ID",-5} | {"Passenger Id", -8} |2 {"Flight Id",-8} | {"Booking Date",-8} " +
                          $"| {"Flight Class",-20} | {"Price",-8} | {"Cancelled?",-8} |");
                             
        Console.WriteLine($"{"".PadLeft(170, '=')}");
        bookings.ForEach(Console.WriteLine);
        Console.WriteLine($"{"".PadLeft(170, '=')}");
    }

    public static Guid ReadFlightId()
    {
        Console.WriteLine("Enter The flight ID you want to book");
        return Guid.TryParse(Console.ReadLine(), out var flightId)? flightId : throw new BookingNotFoundException("Invalid booking id");

    }

    public static FlightClass ReadFlightClass()
    {
        Console.WriteLine("Enter the flight class you want to book [Economy | Business | First]");
        var value = Console.ReadLine();

        if (string.IsNullOrEmpty(value) || !Enum.TryParse(value, true, out FlightClass flightClass))
        {
            throw new InvalidClassException();
        }
        return flightClass;
    }

    public static Guid ReadBookingId()
    {
        Console.WriteLine("Enter the ID of the booking you want to cancel");
        return Guid.TryParse(Console.ReadLine(), out var bookingId)? bookingId : throw new BookingNotFoundException("Invalid booking id");
    }
  
}