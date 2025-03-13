using Model.Bookings;
using Model.Users.Exceptions;

namespace Data.Bookings;

public class BookingRepository(FilePathSettings settings, IFileRepository<Booking> fileRepository) : IBookingRepository
{

    public async Task<List<Booking>> GetAllBookings()
    {
        try
        {
            var bookings = await fileRepository.ReadDataFromFile(settings.Bookings);
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
            throw new NoBookingFoundException("Booking not found"); 
        }
        booking.Cancelled = true;
        await SaveBookings(bookings);
    }

    public async Task SaveBookings(List<Booking> bookings)
    {
        try
        {
            await fileRepository.WriteDataToFile(settings.Bookings, bookings);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public List<BookingDetails> GetFilteredBookings(List<BookingDetails> bookingDetails,
        BookingFilterOptions filterOptions, string value)
    {
        switch (filterOptions)
        {
            case BookingFilterOptions.Id:
                if (Guid.TryParse(value, out var bookingId))
                    return bookingDetails.Where(item => item.Id == bookingId).ToList();
                throw new InvalidOperationException("Invalid booking id");
            
            case BookingFilterOptions.FlightId:
                if (Guid.TryParse(value, out var flightId))
                    return bookingDetails.Where(item => item.Flight.Id == flightId).ToList();
                throw new InvalidOperationException("Invalid flight id");
            
            case BookingFilterOptions.BookingDate:
                if (DateTime.TryParse(value, out var bookingDate))
                    return bookingDetails.Where(item => item.BookingDate.Date == bookingDate.Date).ToList();
                throw new InvalidDateFormatException();
            
            case BookingFilterOptions.Cancelled:
                return bookingDetails.Where(item => item.Cancelled).Select(item => item).ToList();
            
            case BookingFilterOptions.DepartureDate:
                if (DateTime.TryParse(value, out var departureDate))
                    return bookingDetails.Where(item => item.Flight.DepartureDate.Date == departureDate.Date).ToList();
                throw new InvalidDateFormatException();
            
            case BookingFilterOptions.DepartureCountry:
                return bookingDetails.Where(item => item.Flight.DepartureCountry == value).Select(item => item).ToList();
            
            case BookingFilterOptions.DestinationCountry:
                return bookingDetails.Where(item => item.Flight.DestinationCountry == value).Select(item => item).ToList();
            
            case BookingFilterOptions.PassengerName:
                return bookingDetails.Where(item => item.User.FullName == value).Select(item => item).ToList();
            
            case BookingFilterOptions.PassengerId:
                return bookingDetails.Where(item => item.PassengerId == Guid.Parse(value)).Select(item => item).ToList();
            
            case BookingFilterOptions.ClassType:
                return bookingDetails.Where(item => nameof(item.FlightClass) == value).Select(item => item).ToList();
            
            case BookingFilterOptions.Price:
                return bookingDetails.Where(item => Math.Abs(item.Price - double.Parse(value)) < 0).Select(item => item).ToList();
            
            default:
                Console.WriteLine("Please enter a valid option");
                break;
        }
        return [];
    }
}