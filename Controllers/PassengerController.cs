using Model.Bookings;
using Model.Users.Exceptions;
using Services.Bookings;
using Services.Flights;
using Services.Flights.Exceptions;
using Services.Users;
using Views.Passengers;

namespace Controllers;

public class PassengerController(IPassengerView passengerView,  IFlightService flightService,
    IUserService userService, ICurrentUser currentUser, IBookingService bookingService)
{

    public async Task ShowPassengerPage()
    {
        while (true)
        {
            var option = passengerView.ShowPassengerMainMenu();
            if (option == PassengerOptions.Exit)
            {
                break;
            }
            await HandlePassengerSelection(option);
        }
        
    }

    private async Task HandlePassengerSelection(PassengerOptions passengerOptions)
    {
        switch (passengerOptions)
        {
            case PassengerOptions.ViewFlights:
                await HandleViewFlights();
                break;
            case PassengerOptions.FilterFlights:
                await HandleFilterFlights();
                break;
            case PassengerOptions.ViewBookings:
                await HandleViewBookings();
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

    private async Task HandleViewBookings()
    {
        try
        {
            var bookings = await userService.GetPassengerBookings(currentUser.User.Id);
            passengerView.ShowBookings(bookings);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task HandleViewFlights()
    {
        try
        {
            var flights = await flightService.GetAllFlights();
            passengerView.ShowFlights(flights);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task HandleModifyBooking()
    {
        var bookingId = passengerView.ReadBookingId();
        var booking = await bookingService.GetBookingById(bookingId);
        if (booking is null || booking.PassengerId != currentUser.User.Id)
        {
            throw new NoBookingFoundException("Invalid booking id");
        }
        var flightClass = passengerView.ReadFlightClass();
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

    private async Task HandleFilterFlights()
    {
        try
        {
            passengerView.ShowFilterOptions();
            var option = passengerView.ReadFilterOptions();
            var value = passengerView.ReadFilterValue();
            var filteredFlights = await flightService.GetFilteredFlights(option, value);
            passengerView.ShowFlights(filteredFlights);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private async Task HandleAddBooking()
    {
        try
        {
            var flightId = passengerView.ReadFlightId();
            var flight = await flightService.GetFlightById(flightId);
            if (flight is null)
            {
                throw new FlightNotFoundException("Flight not found");
            }
            var flightClass = passengerView.ReadFlightClass();
            var booking = new Booking(currentUser.User.Id, flight.Id, flightClass, flight.Prices[flightClass]);
            await bookingService.AddBooking(booking);
            Console.WriteLine("Your booking is successfully booked");
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
        }
    }

    private async Task HandleCancelBooking()
    {
        try
        {
            var bookingId = passengerView.ReadBookingId();
            var booking1 = await bookingService.GetBookingById(bookingId);
            if (booking1 is null || booking1.PassengerId != currentUser.User.Id)
            {
                throw new NoBookingFoundException("Invalid booking id");
            }

            await bookingService.DeleteBooking(booking1.Id);
            Console.WriteLine("Your booking is successfully cancelled");
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
        }
    }
}