using Model;
using Model.Bookings;
using Model.Users;
using Services.Bookings;
using Services.Flights;
using Services.Users;

namespace Views.Passengers;

public class PassengerView (IFlightService flightService, IUserService userService, 
    ICurrentUser currentUser, IBookingService bookingService) : IPassengerView
{
    public async Task ShowPassengerMainMenu()
    {
        while (true)
        {
            ShowPassengerOptions();
            var option = Console.ReadLine();
            if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
            if (optionNumber is < 1 or > 7) throw new InvalidOptionException("Invalid option number");
            if (optionNumber == 7) break;
            await HandlePassengerSelection((PassengerOptions)optionNumber);
        }
    }

    public void ShowFilterOptions()
    {
        Console.WriteLine("If you want to filter flights based on departure Country press 1");
        Console.WriteLine("If you want to filter flights based on destination Country press 2");
        Console.WriteLine("If you want to filter flights based on departure date press 3");
        Console.WriteLine("If you want to filter flights based on departure airport press 4");
        Console.WriteLine("If you want to filter flights based on arrival airport press 5");
        Console.WriteLine("If you want to filter flights based on price press 6");
        Console.WriteLine("If you want to filter flights based on class press 7");
    }
    
    public async Task <List<Flight>>HandleFilterOption()
    {
        ShowFilterOptions();
        Console.WriteLine("Enter the option number you want to filter flights based on it");
        var option = Console.ReadLine();
        Console.WriteLine("Enter value you looking for");
        var value = Console.ReadLine();
        if (string.IsNullOrEmpty(option) || string.IsNullOrEmpty(value))
        {
            throw new InvalidDataException("Invalid option");
        }
        if (!int.TryParse(option, out var optionNumber)) throw new InvalidOptionException("Invalid option number");
        if (optionNumber is < 1 or > 7) throw new InvalidOptionException("Invalid option number");

        try
        {
            var flights = await flightService.GetFilteredFlights((FlightSearchParams)optionNumber, value);
            return flights;
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
            throw;
        }
    }
    
    public async Task HandlePassengerSelection(PassengerOptions passengerOptions)
    {
        switch (passengerOptions)
        {
            case PassengerOptions.ViewFlights:
               var flights = await flightService.GetAllFlights();
                ShowFlights(flights);
                break;
            
            case PassengerOptions.FilterFlights:
                var filteredFlights = await HandleFilterOption();
                ShowFlights(filteredFlights);
                break;
            case PassengerOptions.ViewBookings:
                var bookings = await userService.GetPassengerBookings(currentUser.User.Id);
                ShowBookings(bookings);
                break;
            case PassengerOptions.AddBooking:
                await HandleAddBooking();
                break;
            case PassengerOptions.CancelBooking:
                await HandleCancelBooking();
                break;
            case PassengerOptions.ModifyBooking:
                await HandleModifyBooking();
                break;
            case PassengerOptions.Exit:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(passengerOptions), passengerOptions, null);
        }
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

    public async Task HandleAddBooking()
    {
        Console.WriteLine("Enter The flight ID you want to book");
        _ = Guid.TryParse(Console.ReadLine(), out var flightId)? flightId : throw new BookingNotFound("Invalid booking id");
        
        try
        {
            var flight = await flightService.GetFlightById(flightId);
            Console.WriteLine("Enter the flight class you want to book [Economy | Business | First]");
            var value = Console.ReadLine();

            if (string.IsNullOrEmpty(value) || !Enum.TryParse(value, true, out FlightClass flightClass))
            {
                throw new InvalidDataException("Invalid input. Please enter a valid flight class (Economy, Business, First).");
            }
        
            var booking = new Booking(currentUser.User.Id, flight.Id, flightClass, flight.Prices[flightClass]);
            await bookingService.AddBooking(booking);
            Console.WriteLine("Your booking is successfully booked");
        }
        catch (InvalidOperationException exp)
        {
            Console.WriteLine(exp.Message);
        }
    }

    public async Task HandleCancelBooking()
    {
        Console.WriteLine("Enter the ID of the booking you want to cancel");
        _ = Guid.TryParse(Console.ReadLine(), out var bookingId)? bookingId : throw new BookingNotFound("Invalid booking id");
        var booking = await bookingService.GetBookingById(bookingId);
        if (booking is null || booking.PassengerId != currentUser.User.Id)
        {
            throw new NoBookingFoundException("Invalid booking id");
        }
        await bookingService.DeleteBooking(booking.Id);
        Console.WriteLine("Your booking is successfully cancelled");
    }

    public async Task HandleModifyBooking()
    {
        Console.WriteLine("Enter the ID of the booking you want to modify");
        _ = Guid.TryParse(Console.ReadLine(), out var bookingId)? bookingId : throw new BookingNotFound("Invalid booking id");
        var booking = await bookingService.GetBookingById(bookingId);
        if (booking is null || booking.PassengerId != currentUser.User.Id)
        {
            throw new NoBookingFoundException("Invalid booking id");
        }
        
        Console.WriteLine("Enter the new Class you want to book [Economy | Business | First]");
        var value = Console.ReadLine();

        if (string.IsNullOrEmpty(value) || !Enum.TryParse(value, true, out FlightClass flightClass))
        {
            throw new InvalidDataException("Invalid input. Please enter a valid flight class (Economy, Business, First).");
        }

        try
        {
            var flight = await flightService.GetFlightById(booking.FlightId);
            booking.FlightClass = flightClass;
            booking.Price = flight.Prices[flightClass];
            await bookingService.UpdateBooking(booking);
            Console.WriteLine("Your booking is modified successfully");
        }
        catch (InvalidOperationException exp)
        {
            Console.WriteLine(exp.Message);
        }
    }
}