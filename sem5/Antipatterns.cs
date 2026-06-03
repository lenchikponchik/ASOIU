using Microsoft.Extensions.DependencyInjection;

// =============================================================================
// Антипаттерны внедрения зависимостей — §7 семинара
// Показаны «плохой» вариант и способ исправления для каждого.
// =============================================================================

// -----------------------------------------------------------------------------
// 1. Control Freak — класс сам создаёт свои зависимости
// -----------------------------------------------------------------------------

/// <summary>
/// ПЛОХО: Control Freak.
/// Сервис сам решает, какой логгер использовать → невозможно подменить на тест.
/// </summary>
public class BookService_ControlFreak
{
    // «Жёсткая» зависимость — класс намертво привязан к ConsoleLogger
    private readonly ILogger _logger = new ConsoleLogger();

    public void AddBook(string title) =>
        _logger.Log($"Добавлена книга: «{title}»");
}

/// <summary>
/// ХОРОШО: зависимость внедряется через конструктор.
/// </summary>
public class BookService_Fixed_ControlFreak(ILogger logger)
{
    public void AddBook(string title) =>
        logger.Log($"Добавлена книга: «{title}»");
}

// -----------------------------------------------------------------------------
// 2. Bastard Injection — «удобный» конструктор без аргументов
// -----------------------------------------------------------------------------

/// <summary>
/// ПЛОХО: Bastard Injection.
/// Конструктор без параметров кажется удобным, но скрывает зависимость.
/// </summary>
public class BookService_BastardInjection
{
    private readonly ILogger _logger;

    /// <summary>«Удобный» конструктор по умолчанию скрывает реальную зависимость.</summary>
    public BookService_BastardInjection() : this(new ConsoleLogger()) { }

    /// <summary>Правильный конструктор с внедрением — но он в тени первого.</summary>
    public BookService_BastardInjection(ILogger logger) => _logger = logger;

    public void AddBook(string title) =>
        _logger.Log($"Добавлена книга: «{title}»");
}

/// <summary>
/// ХОРОШО: только один конструктор, принимающий зависимость явно.
/// </summary>
public class BookService_Fixed_BastardInjection(ILogger logger)
{
    public void AddBook(string title) =>
        logger.Log($"Добавлена книга: «{title}»");
}

// -----------------------------------------------------------------------------
// 3. Service Locator — зависимости запрашиваются внутри метода
// -----------------------------------------------------------------------------

/// <summary>
/// Глобальный реестр зависимостей — основа антипаттерна Service Locator.
/// </summary>
public static class ServiceLocator
{
    private static ServiceProvider? _provider;

    /// <summary>Инициализировать реестр контейнером.</summary>
    public static void Init(ServiceProvider provider) => _provider = provider;

    /// <summary>Получить зависимость по типу.</summary>
    public static T Get<T>() where T : notnull
    {
        if (_provider is null)
            throw new InvalidOperationException("ServiceLocator не инициализирован.");
        return _provider.GetRequiredService<T>();
    }
}

/// <summary>
/// ПЛОХО: Service Locator.
/// Зависимости скрыты внутри методов — код невозможно протестировать изолированно.
/// </summary>
public class BookService_ServiceLocator
{
    public void AddBook(string title)
    {
        // Зависимость берётся из глобального реестра — скрытая связанность
        ILogger logger = ServiceLocator.Get<ILogger>();
        logger.Log($"Добавлена книга: «{title}»");
    }
}

/// <summary>
/// ХОРОШО: зависимость явно объявлена в конструкторе.
/// </summary>
public class BookService_Fixed_ServiceLocator(ILogger logger)
{
    public void AddBook(string title) =>
        logger.Log($"Добавлена книга: «{title}»");
}

// -----------------------------------------------------------------------------
// 4. Ambient Context — глобальное изменяемое состояние вместо DI
// -----------------------------------------------------------------------------

/// <summary>
/// ПЛОХО: Ambient Context.
/// Любой код может сменить логгер в любой момент — скрытая глобальная связность.
/// </summary>
public static class LoggerContext
{
    /// <summary>Глобальный текущий логгер.</summary>
    public static ILogger Current { get; set; } = new NullLogger();
}

/// <summary>
/// ПЛОХО: использует Ambient Context.
/// </summary>
public class BookService_AmbientContext
{
    public void AddBook(string title) =>
        // Неявная зависимость от глобального состояния
        LoggerContext.Current.Log($"Добавлена книга: «{title}»");
}

/// <summary>
/// ХОРОШО: явный конструктор.
/// </summary>
public class BookService_Fixed_AmbientContext(ILogger logger)
{
    public void AddBook(string title) =>
        logger.Log($"Добавлена книга: «{title}»");
}
