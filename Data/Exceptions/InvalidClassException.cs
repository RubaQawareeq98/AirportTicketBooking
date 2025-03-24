namespace Data.Exceptions;

public class InvalidClassException : Exception
{
    public override string Message => "Invalid Flight Class. Expected value (Economy, Business, First). ";
}