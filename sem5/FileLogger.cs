/// <summary>Логгер, дописывающий сообщения в текстовый файл.</summary>
public class FileLogger(string filePath) : ILogger
{
    /// <inheritdoc/>
    public void Log(string message) =>
        File.AppendAllText(filePath, $"[LOG] {message}{Environment.NewLine}");
}
