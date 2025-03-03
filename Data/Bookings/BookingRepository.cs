using Model;

namespace Data;

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

    public async Task<Booking> GetBookingById(Guid id)
    {
        var bookings = await GetAllBookings();
        var booking = bookings.Find(b => b.Id == id);
        if (booking is null)
        {
            throw new InvalidOperationException("Booking not found");
        }
        return booking;
    }

    public async Task CreateBooking(Booking booking)
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
}