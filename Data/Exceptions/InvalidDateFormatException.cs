namespace Data;

public class InvalidDateFormatException : Exception 
{
    public override string Message { get; } = "Invalid date format, Expected format YYYY-MM-DD. ";
}