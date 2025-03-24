namespace Views.Consoles;

public interface IConsoleService
{
    string? ReadLine();
    void WriteLine(string message);
}
