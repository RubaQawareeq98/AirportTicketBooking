namespace Data.Exceptions;

public class InvalidClassException : Exception
{
    public override string Message { get; } = "Invalid Flight Class. Expected value (Economy, Business, First). ";

}