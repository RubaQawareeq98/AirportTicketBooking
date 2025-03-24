using Views.Consoles;

namespace Airport_Ticket_Management_System.Tests;

public class ConsoleService : IConsoleService
{
    private readonly Queue<string> _inputQueue = new();
    private readonly Queue<string> _outputQueue = new();

    public void SetInput(params string[] inputs)
    {
        foreach (var input in inputs)
        {
            _inputQueue.Enqueue(input);
        }
    }

    public List<string> GetOutput() => _outputQueue.ToList();

    public string ReadLine() => _inputQueue.Count > 0 ? _inputQueue.Dequeue() : throw new InvalidOperationException("No input provided");

    public void WriteLine(string message) => _outputQueue.Enqueue(message);
}