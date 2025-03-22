using Model;
using Model.Bookings;
namespace Airport_Ticket_Management_System.Tests.Data.MockingData;

public class MockBookings
{
    public static List<BookingDetails> GetMockBookings()
    {
        var flights = MockFlights.GetFlights();
        var users = MockUsers.GetMockUsers();

        return
        [
            new BookingDetails (
            Guid.NewGuid(),
            new DateTime(2025, 4, 1),
            users[0].Id,
            flights[0].Id,
            FlightClass.Business,
            500,
            flights[0],
            users[0]
            )
            {
                Cancelled = true
            },

            new BookingDetails(
                new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484"),
                new DateTime(2025, 5, 1),
                users[1].Id,
                flights[1].Id,
                FlightClass.Economy,
                500,
                flights[1],
                users[1]
            )
            {
                Cancelled = false
            }
        ];
    }
}