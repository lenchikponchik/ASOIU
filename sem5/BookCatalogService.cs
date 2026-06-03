/// <summary>
/// Сервис каталога книг.
/// Зависит только от интерфейсов — ни одного конкретного класса.
/// </summary>
public class BookCatalogService(ILogger logger, IBookStorage storage)
{
    /// <summary>Добавить книгу в каталог.</summary>
    public void AddBook(string title, string author)
    {
        storage.Save(title, author);
        logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    /// <summary>Удалить книгу из каталога.</summary>
    public void RemoveBook(string title)
    {
        storage.Remove(title);
        logger.Log($"Удалена книга: «{title}»");
    }

    /// <summary>Напечатать все книги каталога.</summary>
    public void ListBooks()
    {
        var books = storage.GetAll();
        if (books.Count == 0)
        {
            Console.WriteLine("Каталог пуст.");
            return;
        }

        foreach (string book in books)
        {
            Console.WriteLine($"  {book}");
        }
    }
}
