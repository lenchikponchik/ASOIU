/// <summary>Контракт для хранения записей о книгах.</summary>
public interface IBookStorage
{
    /// <summary>Сохранить книгу.</summary>
    void Save(string title, string author);

    /// <summary>Удалить книгу по названию.</summary>
    void Remove(string title);

    /// <summary>Вернуть все сохранённые книги.</summary>
    IReadOnlyList<string> GetAll();
}
