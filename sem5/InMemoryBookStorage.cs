/// <summary>Хранилище книг в оперативной памяти.</summary>
public class InMemoryBookStorage(ILogger logger) : IBookStorage
{
    private readonly List<string> _books = [];

    /// <inheritdoc/>
    public void Save(string title, string author)
    {
        _books.Add($"«{title}» — {author}");
        logger.Log($"[STORAGE] Сохранено: «{title}»");
    }

    /// <inheritdoc/>
    public void Remove(string title)
    {
        int removed = _books.RemoveAll(b => b.StartsWith($"«{title}»"));
        logger.Log($"[STORAGE] Удалено записей: {removed}");
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetAll() => _books.AsReadOnly();
}
