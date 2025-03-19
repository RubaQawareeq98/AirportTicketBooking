namespace Data.Exceptions;

public class InvalidClassException : Exception
{
    public override string Message { get; } = "Invalid Flight Class. Expected value (Economy, Business, First). ";

}public class TestExp(string value) : Exception
{
    public override string Message { get; } = $"Invalid Flight Class !!{value}. Expected value (Economy, Business, First). ";

}