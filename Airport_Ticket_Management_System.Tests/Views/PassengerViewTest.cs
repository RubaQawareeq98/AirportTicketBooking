using Moq;
using Views.Consoles;
using Views.Passengers;
using AutoFixture;
using AutoFixture.AutoMoq;
using Model;
using Model.Bookings;
using Model.Flights;
using Data.Exceptions;
using Services.Bookings.Exceptions;
using Views;

namespace Airport_Ticket_Management_System.Tests.Views
{
    public class PassengerViewTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IConsoleService> _consoleServiceMock;
        private readonly PassengerView _view;

        public PassengerViewTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _consoleServiceMock = _fixture.Freeze<Mock<IConsoleService>>();
            _view = _fixture.Create<PassengerView>();
        }

        [Fact]
        public void ShowPassengerMainMenu_ShouldReturnValidOption()
        {
            // Arrange
            var optionInput = _fixture.Create<PassengerOptions>();
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(((int)optionInput).ToString());

            // Act
            var option = _view.ShowPassengerMainMenu();

            // Assert
            Assert.Equal(optionInput, option);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("8")]
        [InlineData("abc")]
        public void ShowPassengerMainMenu_ShouldThrowException_WhenInvalidOption(string input)
        {
            // Arrange
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(input);

            // Act & Assert
            Assert.Throws<InvalidOptionException>(() => _view.ShowPassengerMainMenu());
        }
        
        [Fact]
        public void ShowFilterOptions_ShouldWriteExpectedLines()
        {
            // Act
            _view.ShowFilterOptions();

            // Assert
            _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.Exactly(8));
        }

        [Fact]
        public void ReadFilterOptions_ShouldReturnValidOption()
        {
            // Arrange
            var filterInput = _fixture.Create<FlightFilterOptions>();
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(((int)filterInput).ToString());

            // Act
            var option = _view.ReadFilterOptions();

            // Assert
            Assert.Equal(filterInput, option);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("9")]
        [InlineData("asd")]
        public void ReadFilterOptions_ShouldThrowException_WhenInvalid(string input)
        {
            // Arrange
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(input);

            // Act & Assert
            Assert.Throws<InvalidOptionException>(() => _view.ReadFilterOptions());
        }

        [Fact]
        public void ReadFilterValue_ShouldReturnValidValue()
        {
            // Arrange
            var valueInput = _fixture.Create<string>();
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(valueInput);

            // Act
            var value = _view.ReadFilterValue();

            // Assert
            Assert.Equal(valueInput, value);
        }
        

        [Fact]
        public void ReadFilterValue_ShouldThrowException_WhenEmpty()
        {
            // Arrange
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns("");

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => _view.ReadFilterValue());
        }

        [Fact]
        public void ReadFlightId_ShouldReturnValidGuid()
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(validGuid.ToString());

            // Act
            var flightId = _view.ReadFlightId();

            // Assert
            Assert.Equal(validGuid, flightId);
        }

        [Fact]
        public void ReadFlightId_ShouldThrowException_WhenInvalidGuid()
        {
            // Arrange
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns("");

            // Act & Assert
            Assert.Throws<BookingNotFoundException>(() => _view.ReadFlightId());
        }

        [Fact]
        public void ReadFlightClass_ShouldReturnValidClass()
        {
            // Arrange
            var flightClassInput = _fixture.Create<FlightClass>();
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(flightClassInput.ToString());

            // Act
            var flightClass = _view.ReadFlightClass();

            // Assert
            Assert.Equal(flightClassInput, flightClass);
        }

        [Fact]
        public void ReadFlightClass_ShouldThrowException_WhenInvalid()
        {
            // Arrange
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns("InvalidClass");

            // Act & Assert
            Assert.Throws<InvalidClassException>(() => _view.ReadFlightClass());
        }

        [Fact]
        public void ReadBookingId_ShouldReturnValidGuid()
        {
            // Arrange
            var validGuid = _fixture.Create<Guid>();
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns(validGuid.ToString());

            // Act
            var bookingId = _view.ReadBookingId();

            // Assert
            Assert.Equal(validGuid, bookingId);
        }

        [Fact]
        public void ReadBookingId_ShouldThrowException_WhenInvalidGuid()
        {
            // Arrange
            _consoleServiceMock.Setup(cs => cs.ReadLine()).Returns("");

            // Act & Assert
            Assert.Throws<BookingNotFoundException>(() => _view.ReadBookingId());
        }

        [Fact]
        public void ShowFlights_ShouldDisplayFlightList()
        {
            // Arrange
            var flights = _fixture.Create<List<Flight>>();

            // Act
            _view.ShowFlights(flights);

            // Assert
            _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ShowBookings_ShouldDisplayBookingList()
        {
            // Arrange
            var bookings = _fixture.Create<List<Booking>>();

            // Act
            _view.ShowBookings(bookings);

            // Assert
            _consoleServiceMock.Verify(cs => cs.WriteLine(It.IsAny<string>()), Times.AtLeastOnce());
        }
    }
}
