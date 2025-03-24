using Services.Bookings;
using Services.Flights;
using Views.Consoles;
using Views.Managers;

namespace Controllers;

public class ManagerController(IConsoleService consoleService, IManagerView managerView, IFlightService flightService, IBookingService bookingService)
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
                    consoleService.WriteLine("Goodbye!");
                    return;
                default:
                    consoleService.WriteLine("Please select a valid option");
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
            consoleService.WriteLine(res);
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
            consoleService.WriteLine(e.Message);
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
            consoleService.WriteLine(e.Message);
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
            consoleService.WriteLine(e.Message);
        }
    }
}