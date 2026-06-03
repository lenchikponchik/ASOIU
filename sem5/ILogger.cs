/// <summary>
/// Контракт для записи журнала событий.
/// Интерфейс определяется исходя из потребностей потребителя,
/// а не поставщика — это и есть Dependency Inversion.
/// </summary>
public interface ILogger
{
    /// <summary>Записать сообщение в журнал.</summary>
    void Log(string message);
}
