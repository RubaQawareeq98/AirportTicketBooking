using Views.Consoles;

namespace Views;

public class LoginView (IConsoleService consoleService): ILoginView
{
    public void WelcomeMessage()
    {
        consoleService.WriteLine("Welcome to Airport_Ticket_Booking_System");
    }

    public string ReadUserName()
    {
        string? username;
        do
        {
            consoleService.WriteLine("Enter your username: ");
            username = consoleService.ReadLine();
            if (string.IsNullOrEmpty(username))
            {
                consoleService.WriteLine("Please enter a valid username");
            }
        }while(string.IsNullOrEmpty(username));
        
        return username;
    }
    
    public string ReadPassword()
    {
        string? password;
        do
        {
            consoleService.WriteLine("Enter your password: ");
            password = consoleService.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                consoleService.WriteLine("Please enter a valid password");
            }
        }while(string.IsNullOrEmpty(password));
        
        return password;
    }
}