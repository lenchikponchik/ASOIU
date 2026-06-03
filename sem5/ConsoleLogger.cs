/// <summary>Логгер, выводящий сообщения в консоль.</summary>
public class ConsoleLogger : ILogger
{
    /// <inheritdoc/>
    public void Log(string message) =>
        Console.WriteLine($"[LOG] {message}");
}
