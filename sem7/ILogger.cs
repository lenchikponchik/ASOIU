/// <summary>Контракт журналирования для sem7.</summary>
public interface ILogger
{
    void Log(string message);
}

/// <summary>Консольный логгер.</summary>
public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

/// <summary>Логгер-заглушка (ничего не делает).</summary>
public class NullLogger : ILogger
{
    public void Log(string message) { }
}
