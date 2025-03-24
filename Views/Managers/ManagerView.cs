using Model.Bookings;
using Model.Flights;
using Views.Consoles;

namespace Views.Managers;

public class ManagerView(IConsoleService consoleService) : IManagerView
{
    public void ShowManagerMenu()
    {
            consoleService.WriteLine("If You Want to Show All Flights press 1");
            consoleService.WriteLine("If You Want to Show All Bookings press 2");
            consoleService.WriteLine("If You Want to Show All Passengers press 3");
            consoleService.WriteLine("If You Want to Filter Bookings press 4");
            consoleService.WriteLine("If You Want to Import new Flights press 5");
            consoleService.WriteLine("If You Want to Exit press 6");
            consoleService.WriteLine("Please select an option:");
    }

    public ManagerOptions ReadManagerOptions()
    {
        var option = consoleService.ReadLine();
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 6) throw new InvalidOptionException("Invalid option number");
        return (ManagerOptions) optionNumber;
    }
    
    public string ReadCsvPath()
    {
        consoleService.WriteLine("*** Importing Flights ***");
        consoleService.WriteLine("Please enter the path of the flights csv file you would like to import:");
        var path = consoleService.ReadLine();
        if (string.IsNullOrEmpty(path)) throw new InvalidDataException("Invalid path...");
        return path;
    }

    public void ShowFilterOptions()
    {
        consoleService.WriteLine("Please select a valid option");
        consoleService.WriteLine("If You Want to filter bookings based on booking ID press 1");
        consoleService.WriteLine("If you want to filter bookings based on flight ID press 2");
        consoleService.WriteLine("If you want to filter bookings based on class press 3");
        consoleService.WriteLine("If you want to show Cancelled bookings press 4");
        consoleService.WriteLine("If you want to filter bookings based on booking date press 5");
        consoleService.WriteLine("If you want to filter bookings based on departure date press 6");
        consoleService.WriteLine("If you want to filter bookings based on departure Country press 7");
        consoleService.WriteLine("If you want to filter bookings based on destination Country press 8");
        consoleService.WriteLine("If you want to filter bookings based on price press 9");
        consoleService.WriteLine("If you want to filter bookings based on Passenger ID press 10");
        consoleService.WriteLine("If you want to filter bookings based on Passenger name press 11");
    }

    public BookingFilterOptions ReadFilterOption()
    {
        consoleService.WriteLine("Enter the option number you want to filter bookings based on it");
        var option = consoleService.ReadLine();
        
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 11) throw new InvalidOptionException("Invalid option number");
        return (BookingFilterOptions) optionNumber;
    }

    public string ReadFilterValue()
    {
        consoleService.WriteLine("Enter value you looking for");
        var value = consoleService.ReadLine();
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidDataException("Invalid input, value should not be empty");
        }
        return value;
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
        }
        consoleService.WriteLine($"{"".PadLeft(170, '=')}");
    }

    public void ShowBookingDetails(List<BookingDetails> bookingDetails)
    {
        consoleService.WriteLine($"{"".PadLeft(170, '=')}");
        foreach (var booking in bookingDetails)
        {
            consoleService.WriteLine(booking.ToString());
        }
        consoleService.WriteLine($"{"".PadLeft(170, '=')}");
    }
}