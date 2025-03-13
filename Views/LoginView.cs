namespace Views;

public class LoginView : ILoginView
{
    public void WelcomeMessage()
    {
        Console.WriteLine("Welcome to Airport_Ticket_Booking_System");
    }

    public string ReadUserName()
    {
        string? username;
        do
        {
            Console.WriteLine("Enter your username: ");
            username = Console.ReadLine();
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Please enter a valid username");
            }
        }while(string.IsNullOrEmpty(username));
        
        return username;
    }
    
    public string ReadPassword()
    {
        string? password;
        do
        {
            Console.WriteLine("Enter your password: ");
            password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Please enter a valid password");
            }
        }while(string.IsNullOrEmpty(password));
        
        return password;
    }
}