using Services.Bookings;
using Services.Flights;
using Views.Managers;

namespace Controllers;

public class ManagerController(IManagerView managerView, IFlightService flightService, IBookingService bookingService)
{
    public async Task ManagePage()
    {
        while (true)
        {
            managerView.ShowManagerMenu();
            var option = managerView.ReadManagerOptions();
            switch (option)
            {
                case ManagerOptions.ViewFlights:
                    await HandleViewFlights();
                    break;
                case ManagerOptions.ViewBookings:
                    await HandleViewBookings();
                    break;
                case ManagerOptions.FilterBookings:
                    await HandleFilterBooking();
                    break;
                case ManagerOptions.ImportFlights:
                    await HandleImportFlights();
                    break;
                case ManagerOptions.Exit:
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Please select a valid option");
                    break;
            }
        }
    }

    private async Task HandleImportFlights()
    {
        var path = managerView.ReadCsvPath();
        var result = await flightService.ImportFlight(path);
        foreach (var res in result)
        {
            Console.WriteLine(res);
        }
    }

    private async Task HandleViewFlights()
    {
        try
        {
            var flights = await flightService.GetAllFlights();
            managerView.ShowFlights(flights);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    private async Task HandleViewBookings()
    {
        try
        {
            var bookings = await bookingService.GetAllBookings();
            managerView.ShowBookings(bookings);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    private async Task HandleFilterBooking()
    {
        try
        {
            managerView.ShowFilterOptions();
            var option = managerView.ReadFilterOption();
            var value = managerView.ReadFilterValue();
            var bookings = await bookingService.GetFilteredBooking(option, value);
            managerView.ShowBookingDetails(bookings);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}