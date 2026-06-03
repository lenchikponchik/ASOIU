/// <summary>
/// Логгер-заглушка (Null Object).
/// Используется как безопасное значение по умолчанию вместо null.
/// </summary>
public class NullLogger : ILogger
{
    /// <inheritdoc/>
    public void Log(string message) { }
}
