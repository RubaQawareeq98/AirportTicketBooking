namespace Model.Users;

public class UserNotFoundException(string message) : Exception (message)
{
    
}