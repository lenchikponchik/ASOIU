/// <summary>
/// Абстракция системного времени — позволяет подменить дату в тестах.
/// </summary>
public interface IClock
{
    /// <summary>Текущая дата и время.</summary>
    DateTime Now { get; }
}

/// <summary>Реальное системное время.</summary>
public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
