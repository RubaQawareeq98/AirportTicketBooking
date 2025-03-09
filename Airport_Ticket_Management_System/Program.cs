using Controllers;
using Data;
using Data.Bookings;
using Data.Files;
using Data.Flights;
using Data.Users;
using Model.Bookings;
using Model.Users;
using Services.Bookings;
using Services.Flights;
using Services.Users;
using Views.Managers;
using Views.Passengers;
using Microsoft.Extensions.DependencyInjection;
using Model.Flights;
using Views;

namespace Airport_Ticket_Management_System;

class Program
{
    static async Task Main()
    {


        var filePaths = SettingsLoader.LoadFileSettings();
        var flightValidator = new FlightValidator();
        var services = new ServiceCollection();

        services.AddSingleton(filePaths);
        services.AddSingleton(flightValidator);

        services.AddTransient<IFileRepository<User>, FileRepository<User>>();
        services.AddTransient<IFileRepository<Booking>, FileRepository<Booking>>();
        services.AddTransient<IFileRepository<Flight>, FileRepository<Flight>>();


        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IBookingRepository, BookingRepository>();
        services.AddTransient<IFlightRepository, FlightRepository>();
        
        services.AddTransient<IBookingService, BookingService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICurrentUser, CurrentUser>();
        services.AddTransient<IFlightService, FlightService>();

        services.AddTransient<ILoginView, LoginView>();
        services.AddTransient<IManagerView, ManagerView>();
        services.AddTransient<IPassengerView, PassengerView>();

        services.AddTransient<LoginController>();
        services.AddTransient<ManagerController>();
        services.AddTransient<PassengerController>();

        var serviceProvider = services.BuildServiceProvider();
        
        var loginController = serviceProvider.GetRequiredService<LoginController>();
        
        try
        {
            var user = await loginController.Login();
        
            if (user.Role == UserRole.Manager)
            {
                var managerController = serviceProvider.GetRequiredService<ManagerController>();
                await managerController.ManagePage();
            }
            else
            {
                var passengerController = serviceProvider.GetRequiredService<PassengerController>();
                await passengerController.ShowPassengerPage();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}