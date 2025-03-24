namespace Data.Exceptions;

public class InvalidDateFormatException : Exception 
{
    public override string Message => "Invalid date format, Expected format YYYY-MM-DD. ";
}