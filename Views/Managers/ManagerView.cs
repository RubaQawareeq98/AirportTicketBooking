using Model.Bookings;
using Model.Flights;
using Views.Passengers;

namespace Views.Managers;

public class ManagerView : IManagerView
{
    public void ShowManagerMenu()
    {
            Console.WriteLine("If You Want to Show All Flights press 1");
            Console.WriteLine("If You Want to Show All Bookings press 2");
            Console.WriteLine("If You Want to Show All Passengers press 3");
            Console.WriteLine("If You Want to Filter Bookings press 4");
            Console.WriteLine("If You Want to Import new Flights press 5");
            Console.WriteLine("If You Want to Exit press 6");
            Console.WriteLine("Please select an option:");
    }

    public ManagerOptions ReadManagerOptions()
    {
        var option = Console.ReadLine();
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 6) throw new InvalidOptionException("Invalid option number");
        return (ManagerOptions) optionNumber;
    }
    
    public string ReadCsvPath()
    {
        Console.WriteLine("*** Importing Flights ***");
        Console.WriteLine("Please enter the path of the flights csv file you would like to import:");
        var path = Console.ReadLine();
        if (string.IsNullOrEmpty(path)) throw new InvalidDataException("Invalid path...");
        return path;
    }

    public void ShowFilterOptions()
    {
        Console.WriteLine("Please select a valid option");
        Console.WriteLine("If You Want to filter bookings based on booking ID press 1");
        Console.WriteLine("If you want to filter bookings based on flight ID press 2");
        Console.WriteLine("If you want to filter bookings based on class press 3");
        Console.WriteLine("If you want to show Cancelled bookings press 4");
        Console.WriteLine("If you want to filter bookings based on booking date press 5");
        Console.WriteLine("If you want to filter bookings based on departure date press 6");
        Console.WriteLine("If you want to filter bookings based on departure Country press 7");
        Console.WriteLine("If you want to filter bookings based on destination Country press 8");
        Console.WriteLine("If you want to filter bookings based on price press 9");
        Console.WriteLine("If you want to filter bookings based on Passenger ID press 10");
        Console.WriteLine("If you want to filter bookings based on Passenger name press 11");
    }

    public BookingFilterOptions ReadFilterOption()
    {
        Console.WriteLine("Enter the option number you want to filter bookings based on it");
        var option = Console.ReadLine();
        
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 11) throw new InvalidOptionException("Invalid option number");
        return (BookingFilterOptions) optionNumber;
    }

    public string ReadFilterValue()
    {
        Console.WriteLine("Enter value you looking for");
        var value = Console.ReadLine();
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidDataException("Invalid input, value should not be empty");
        }
        return value;
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

    public void ShowBookingDetails(List<BookingDetails> bookingDetails)
    {
        Console.WriteLine($"{"".PadLeft(170, '=')}");
        bookingDetails.ForEach(Console.WriteLine);
        Console.WriteLine($"{"".PadLeft(170, '=')}");
    }
}