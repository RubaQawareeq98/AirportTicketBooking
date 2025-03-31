using Data.Bookings;
using Data.Files;
using Data.Flights;
using Data.Users;
using Model.Bookings;
using Model.Flights;
using Model.Users;
using Services.Bookings;
using Services.Flights;
using Services.Users;

namespace Airport_Ticket_Management_System.Tests;

public class TestBase : IDisposable
{
    protected readonly IFlightRepository FlightRepository;
    protected readonly IFlightService FlightService;
    protected readonly IBookingService BookingService;
    protected readonly IBookingRepository BookingRepository;
    protected readonly IUserRepository UserRepository;
    protected readonly IUserService UserService;
    private readonly FileFixture _fileFixture;

    protected TestBase()
    {
        _fileFixture = new FileFixture();
        FlightRepository = new FlightRepository(_fileFixture.FilePath, new FlightValidator(), new FileRepository<Flight>());
        FlightService = new FlightService(FlightRepository);
        UserRepository = new UserRepository(_fileFixture.FilePath, new FileRepository<User>());
        BookingRepository = new BookingRepository(_fileFixture.FilePath, new FileRepository<Booking>());
        UserService = new UserService(UserRepository, BookingRepository);
        BookingService = new BookingService(BookingRepository, FlightRepository, UserRepository);
    }

    public void Dispose()
    {
        _fileFixture.Dispose();
    }
}