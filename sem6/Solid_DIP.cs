// =============================================================================
// D — Dependency Inversion Principle
// Модули высокого уровня не зависят от модулей низкого уровня —
// оба зависят от абстракций
// =============================================================================

// ---- ПЛОХО: Library напрямую зависит от SqliteBookRepository ----
public class SqliteBookRepository_Dirty
{
    public List<string> GetAll() => ["Война и мир", "Анна Каренина"];
    public void Add(string title) => Console.WriteLine($"[SQLite] Добавлено: {title}");
}

public class Library_Dirty
{
    // Прямая зависимость от конкретного класса — нельзя подменить на тест
    private readonly SqliteBookRepository_Dirty _repo = new();

    public void AddBook(string title) => _repo.Add(title);
    public void PrintBooks()
    {
        foreach (var book in _repo.GetAll())
            Console.WriteLine($"  {book}");
    }
}

// ---- ХОРОШО: оба уровня зависят от интерфейсов ----

/// <summary>Контракт репозитория книг.</summary>
public interface ILibraryRepository
{
    List<string> GetAll();
    void Add(string title);
}

/// <summary>SQLite-реализация репозитория.</summary>
public class SqliteBookRepository : ILibraryRepository
{
    public List<string> GetAll() => ["Война и мир", "Анна Каренина"];
    public void Add(string title) => Console.WriteLine($"[SQLite] Добавлено: {title}");
}

/// <summary>In-memory реализация — удобна для тестов.</summary>
public class InMemoryLibraryRepository : ILibraryRepository
{
    private readonly List<string> _books = [];
    public List<string> GetAll() => [.. _books];
    public void Add(string title) => _books.Add(title);
}

// ILogger уже объявлен в sem5; здесь — свой локальный вариант для изоляции

/// <summary>Контракт логгера (локальный для sem6).</summary>
public interface ILibraryLogger
{
    void Log(string message);
}

public class ConsoleLibraryLogger : ILibraryLogger
{
    public void Log(string message) => Console.WriteLine($"[LIB] {message}");
}

/// <summary>
/// Библиотека зависит только от интерфейсов — легко тестируется и расширяется.
/// </summary>
public class Library(ILibraryRepository repo, ILibraryLogger log)
{
    public void AddBook(string title)
    {
        repo.Add(title);
        log.Log($"Книга добавлена: {title}");
    }

    public void PrintBooks()
    {
        log.Log("Список книг:");
        foreach (var book in repo.GetAll())
            Console.WriteLine($"  {book}");
    }
}
