namespace Views.Consoles;

public class ConsoleService : IConsoleService
{
    public string? ReadLine() => Console.ReadLine();

    public void WriteLine(string message) => Console.WriteLine(message);
}