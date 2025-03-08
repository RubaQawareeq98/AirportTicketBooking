using FluentValidation;

namespace Model.Flights;

public class FlightValidator : AbstractValidator<Flight>
{
    public FlightValidator()
    {
        RuleFor(flight => flight.Id)
            .NotEqual(Guid.Empty).WithMessage("Flight ID cannot be empty.");

        RuleFor(flight => flight.DepartureCountry)
            .NotEmpty().WithMessage("Departure country is required.")
            .MaximumLength(100).WithMessage("Departure country cannot exceed 100 characters.");

        RuleFor(flight => flight.DestinationCountry)
            .NotEmpty().WithMessage("Destination country is required.")
            .MaximumLength(100).WithMessage("Destination country cannot exceed 100 characters.");

        RuleFor(flight => flight.DepartureDate)
            .GreaterThan(DateTime.Now).WithMessage("Departure date must be in the future.");

        RuleFor(flight => flight.DepartureAirport)
            .NotEmpty().WithMessage("Departure airport is required.")
            .MaximumLength(100).WithMessage("Departure airport cannot exceed 100 characters.");

        RuleFor(flight => flight.ArrivalAirport)
            .NotEmpty().WithMessage("Arrival airport is required.")
            .MaximumLength(100).WithMessage("Arrival airport cannot exceed 100 characters.");

        RuleFor(flight => flight.Prices)
            .NotNull().WithMessage("Prices must not be null.")
            .Must(prices => prices.Count > 0).WithMessage("Prices must contain at least one entry.")
            .Must(prices => prices.Values.All(price => price > 0))
            .WithMessage("All prices must be greater than zero.");
    }
}