using Model.Bookings;

namespace Data.Bookings;

public class BookingRepository(string filePath, IFileRepository<Booking> fileRepository) : IBookingRepository
{
    
    public async Task<List<Booking>> GetAllBookings()
    {
        try
        {
            var bookings = await fileRepository.ReadDataFromFile(filePath);
            return bookings;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    
    public async Task AddBooking(Booking booking)
    {
        var bookings =  await GetAllBookings();
        bookings.Add(booking);
        await SaveBookings(bookings);
    }

    public async Task UpdateBooking(Booking booking)
    {
        var bookings = await GetAllBookings();
        for (var i = 0; i < bookings.Count; i++)
            if (bookings[i].Equals(booking))
            {
                bookings[i] = booking;
                break;
            }
        await SaveBookings(bookings);
    }

    public async Task CancelBooking(Guid bookingId)
    {
        var bookings = await GetAllBookings();
        var booking = bookings.Find(b => b.Id == bookingId);
        if (booking is null)
        {
            throw new InvalidOperationException("Booking not found"); 
        }
        booking.Cancelled = true;
        await SaveBookings(bookings);
    }

    public async Task SaveBookings(List<Booking> bookings)
    {
        try
        {
            await fileRepository.WriteDataToFile(filePath, bookings);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public List<BookingDetails> GetFilteredBookings(List<BookingDetails> bookingDetails,
        BookingSearchParameters searchParameters, string value)
    {
        switch (searchParameters)
        {
            case BookingSearchParameters.Id:
                if (Guid.TryParse(value, out var bookingId))
                    return bookingDetails.Where(item => item.Flight.Id == bookingId).ToList();
                throw new InvalidOperationException("Invalid booking id");
            case BookingSearchParameters.FlightId:
                if (Guid.TryParse(value, out var flightId))
                    return bookingDetails.Where(item => item.Flight.Id == flightId).ToList();
                throw new InvalidOperationException("Invalid flight id");
            case BookingSearchParameters.BookingDate:
                if (DateTime.TryParse(value, out var bookingDate))
                    return bookingDetails.Where(item => item.BookingDate.Date == bookingDate.Date).ToList();
                throw new InvalidOperationException("Invalid booking date");
            case BookingSearchParameters.Cancelled:
                return bookingDetails.Where(item => item.Cancelled).Select(item => item).ToList();
            case BookingSearchParameters.DepartureDate:
                if (DateTime.TryParse(value, out var departureDate))
                    return bookingDetails.Where(item => item.Flight.DepartureDate.Date == departureDate.Date).ToList();
                break;
            case BookingSearchParameters.DepartureCountry:
                return bookingDetails.Where(item => item.Flight.DepartureCountry == value).Select(item => item).ToList();
            case BookingSearchParameters.DestinationCountry:
                return bookingDetails.Where(item => item.Flight.DestinationCountry == value).Select(item => item).ToList();
            case BookingSearchParameters.PassengerName:
                return bookingDetails.Where(item => item.User.FullName == value).Select(item => item).ToList();
            case BookingSearchParameters.PassengerId:
                return bookingDetails.Where(item => item.PassengerId == Guid.Parse(value)).Select(item => item).ToList();
            case BookingSearchParameters.ClassType:
                return bookingDetails.Where(item => nameof(item.FlightClass) == value).Select(item => item).ToList();
            case BookingSearchParameters.Price:
                return bookingDetails.Where(item => Math.Abs(item.Price - double.Parse(value)) < 0).Select(item => item).ToList();
            default:
                Console.WriteLine("Please enter a valid option");
                break;
        }
        return [];
    }
}