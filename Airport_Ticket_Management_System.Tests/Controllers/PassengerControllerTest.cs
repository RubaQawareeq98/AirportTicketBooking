using AutoFixture;
using AutoFixture.AutoMoq;
using Controllers;
using FluentAssertions;
using Model.Bookings;
using Model.Flights;
using Model.Users;
using Model.Users.Exceptions;
using Moq;
using Services.Bookings;
using Services.Flights;
using Services.Flights.Exceptions;
using Services.Users;
using Views.Passengers;

namespace Airport_Ticket_Management_System.Tests.Controllers;

    public class PassengerControllerTest
    {
        private readonly IFixture _fixture;
        private readonly PassengerController _controller;
        private readonly Mock<IPassengerView> _passengerViewMock;
        private readonly Mock<IFlightService> _flightServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ICurrentUser> _currentUserMock;

        public PassengerControllerTest()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _passengerViewMock = _fixture.Freeze<Mock<IPassengerView>>();
            _flightServiceMock = _fixture.Freeze<Mock<IFlightService>>();
            _bookingServiceMock = _fixture.Freeze<Mock<IBookingService>>();
            _userServiceMock = _fixture.Freeze<Mock<IUserService>>();
            _currentUserMock = _fixture.Freeze<Mock<ICurrentUser>>();

            _controller = _fixture.Create<PassengerController>();
        }

        [Fact]
        public async Task ShowPassengerPage_ShouldExitOnExitOption()
        {
            // Arrange
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.Exit);
            
            // Act
            await _controller.ShowPassengerPage();

            // Assert
            _passengerViewMock.Verify(v => v.ShowPassengerMainMenu(), Times.Once);
        }

        [Fact]
        public async Task HandleViewFlights_ShouldCallShowFlights_WhenFlightsExist()
        {
            // Arrange
            var flights = _fixture.Create<List<Flight>>();
            _flightServiceMock.Setup(s => s.GetAllFlights()).ReturnsAsync(flights);
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.ViewFlights)
                .Returns(PassengerOptions.Exit);

            // Act
            await _controller.ShowPassengerPage();

            // Assert
            _passengerViewMock.Verify(v => v.ShowFlights(flights), Times.Once);
        }
        
        [Fact]
        public async Task HandleViewFlights_ShouldHandleException_WhenNoFlightsExist()
        {
            // Arrange
            _flightServiceMock.Setup(s => s.GetAllFlights()).ThrowsAsync(new FlightNotFoundException("!!! No flight was found !!!"));
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.ViewFlights)
                .Returns(PassengerOptions.Exit);

            // Act
            var exception = await Record.ExceptionAsync(() => _controller.ShowPassengerPage());

            // Assert
            exception.Should().BeNull();
            _flightServiceMock.Verify(s => s.GetAllFlights(), Times.Once);
        }
        
        [Fact]
        public async Task HandleViewBookings_ShouldCallShowBookings()
        {
            // Arrange
            var bookings = _fixture.Create<List<Booking>>();
            var user = _fixture.Create<User>();
            
            _userServiceMock.Setup(s => s.GetPassengerBookings(user.Id)).ReturnsAsync(bookings);
            _currentUserMock.Setup(c => c.User).Returns(user);
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.ViewBookings)
                .Returns(PassengerOptions.Exit);

            // Act
            await _controller.ShowPassengerPage();

            // Assert
            _passengerViewMock.Verify(v => v.ShowBookings(bookings), Times.Once);
        }

        [Fact]
        public async Task HandleViewBookings_ShouldHandleException_WhenNoBookingsExist()
        {
            // Arrange
            var user = _fixture.Create<User>();
            
            _userServiceMock.Setup(s => s.GetPassengerBookings(user.Id)).ThrowsAsync(new NoBookingFoundException($"No bookings found for passenger {user.Id}"));
            _currentUserMock.Setup(c => c.User).Returns(user);
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.ViewBookings)
                .Returns(PassengerOptions.Exit);

            // Act
            var exception = await Record.ExceptionAsync(() => _controller.ShowPassengerPage());
            
            // Assert
            exception.Should().BeNull();
            _userServiceMock.Verify(s => s.GetPassengerBookings(user.Id), Times.Once);
        }

        [Fact]
        public async Task HandleAddBooking_ShouldCallAddBooking()
        {
            // Arrange
            var flight = _fixture.Create<Flight>();
            var flightClass = _fixture.Create<FlightClass>();
            var user = _fixture.Create<User>();
            
            _passengerViewMock.Setup(v => v.ReadFlightId()).Returns(flight.Id);
            _passengerViewMock.Setup(v => v.ReadFlightClass()).Returns(flightClass);
            _flightServiceMock.Setup(s => s.GetFlightById(flight.Id)).ReturnsAsync(flight);
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.AddBooking)
                .Returns(PassengerOptions.Exit);
           
            _currentUserMock.Setup(c => c.User).Returns(user);
        
            // Act
            await _controller.ShowPassengerPage();
        
            // Assert
            _bookingServiceMock.Verify(s => s.AddBooking(It.IsAny<Booking>()), Times.Once);
        }
        
        [Fact]
        public async Task HandleAddBooking_ShouldNotCallAddBooking()
        {
            // Arrange
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.AddBooking)
                .Returns(PassengerOptions.Exit);
           
            // Act
            await _controller.ShowPassengerPage();
        
            // Assert
            _bookingServiceMock.Verify(s => s.AddBooking(It.IsAny<Booking>()), Times.Never);
        }
        
        [Fact]
        public async Task HandleCancelBooking_ShouldDeleteBooking()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var booking = _fixture.Create<Booking>();
            booking.PassengerId = user.Id;

            _passengerViewMock.Setup(v => v.ReadBookingId()).Returns(booking.Id);
            _bookingServiceMock.Setup(s => s.GetBookingById(booking.Id)).ReturnsAsync(booking);
            _currentUserMock.Setup(c => c.User).Returns(user);
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.CancelBooking)
                .Returns(PassengerOptions.Exit);
        
            // Act
            await _controller.ShowPassengerPage();
        
            // Assert
            _bookingServiceMock.Verify(s => s.DeleteBooking(booking.Id), Times.Once);
        }
        
        [Fact]
        public async Task HandleCancelBooking_ShouldThrowException_WhenBookingNotFound()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var booking = _fixture.Create<Booking>();

            _passengerViewMock.Setup(v => v.ReadBookingId()).Returns(booking.Id);
            _bookingServiceMock.Setup(s => s.GetBookingById(booking.Id)).ReturnsAsync(booking);
            _currentUserMock.Setup(c => c.User).Returns(user);
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.CancelBooking)
                .Returns(PassengerOptions.Exit);
        
            // Act
            await _controller.ShowPassengerPage();
        
            // Assert
            _bookingServiceMock.Verify(s => s.DeleteBooking(booking.Id), Times.Never);
        }
        
        [Fact]
        public async Task HandleModifyBooking_ShouldModifyBooking()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var booking = _fixture.Create<Booking>();
            var flight = _fixture.Create<Flight>();
            var newFlightClass = _fixture.Create<FlightClass>();
    
            booking.PassengerId = user.Id;
            booking.FlightId = flight.Id;
            booking.Cancelled = false;
            
            _passengerViewMock.Setup(v => v.ReadBookingId()).Returns(booking.Id);
            _passengerViewMock.Setup(v => v.ReadFlightClass()).Returns(newFlightClass);
            _bookingServiceMock.Setup(s => s.GetBookingById(booking.Id)).ReturnsAsync(booking);
            _flightServiceMock.Setup(f => f.GetFlightById(booking.FlightId)).ReturnsAsync(flight);
            _currentUserMock.Setup(c => c.User).Returns(user);
            
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.ModifyBooking)
                .Returns(PassengerOptions.Exit);
        
            // Act
            await _controller.ShowPassengerPage();
        
            // Assert
            _bookingServiceMock.Verify(s => s.UpdateBooking(It.Is<Booking>(
                b => b.FlightClass == newFlightClass
            )), Times.Once);
        }

        [Fact]
        public async Task HandleModifyBooking_ShouldThrowException_WhenBookingNotFound()
        {
            // Arrange
            var booking = _fixture.Create<Booking>();
            
            _passengerViewMock.Setup(v => v.ReadBookingId()).Returns(booking.Id);
            _bookingServiceMock.Setup(s => s.GetBookingById(booking.Id)).ReturnsAsync(_fixture.Create<Booking>());
          
            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.ModifyBooking)
                .Returns(PassengerOptions.Exit);
        
            // Act && Assert
            await _controller.Invoking(c => c.ShowPassengerPage()).Should().ThrowAsync<NoBookingFoundException>();
        }
        
        [Fact]
        public async Task HandleFilterFlights_ShouldShowFilteredFlights()
        {
            // Arrange
            var flights = _fixture.CreateMany<Flight>(3).ToList();
            var filterOption = _fixture.Create<FlightFilterOptions>();
            var filterValue = _fixture.Create<string>();

            _passengerViewMock.Setup(v => v.ShowFilterOptions());
            _passengerViewMock.Setup(v => v.ReadFilterOptions()).Returns(filterOption);
            _passengerViewMock.Setup(v => v.ReadFilterValue()).Returns(filterValue);
            _flightServiceMock.Setup(s => s.GetFilteredFlights(filterOption, filterValue)).ReturnsAsync(flights);

            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.FilterFlights)
                .Returns(PassengerOptions.Exit);
            
            // Act
            await _controller.ShowPassengerPage();

            // Assert
            _passengerViewMock.Verify(v => v.ShowFlights(flights), Times.Once);
        }
        
        [Fact]
        public async Task HandleFilterFlights_ShouldCatchException_WhenNoFlightsFound()
        {
            // Arrange
            var filterOption = _fixture.Create<FlightFilterOptions>();
            var filterValue = _fixture.Create<string>();

            _passengerViewMock.Setup(v => v.ShowFilterOptions());
            _passengerViewMock.Setup(v => v.ReadFilterOptions()).Returns(filterOption);
            _passengerViewMock.Setup(v => v.ReadFilterValue()).Returns(filterValue);
            _flightServiceMock.Setup(s => s.GetFilteredFlights(filterOption, filterValue)).ThrowsAsync(new FlightNotFoundException("!!! No flight was found !!!"));

            _passengerViewMock.SetupSequence(v => v.ShowPassengerMainMenu())
                .Returns(PassengerOptions.FilterFlights)
                .Returns(PassengerOptions.Exit);
            // Act
            await _controller.ShowPassengerPage();

            // Assert
            _passengerViewMock.Verify(v => v.ShowFlights(It.IsAny<List<Flight>>()), Times.Never);
        }
    }
    