using System.Globalization;
using Microsoft.Data.Sqlite;

/// <summary>
/// Инкапсулирует работу с SQLite для справочника ресторанов и таблицы блюд.
/// </summary>
public class DatabaseManager
{
    private readonly string _connectionString;

    /// <summary>
    /// Конструктор. Создает файл БД и таблицы, если они отсутствуют.
    /// </summary>
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        EnsureDatabase();
    }

    /// <summary>
    /// Импортирует данные из CSV-файлов в таблицы restaurant и dish.
    /// </summary>
    public void ImportFromCsv(string restaurantsCsvPath, string dishesCsvPath)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        var clearCommand = connection.CreateCommand();
        clearCommand.Transaction = transaction;
        clearCommand.CommandText = "DELETE FROM dish; DELETE FROM restaurant;";
        clearCommand.ExecuteNonQuery();

        string[] restaurantLines = File.ReadAllLines(restaurantsCsvPath);
        for (int i = 1; i < restaurantLines.Length; i++)
        {
            string line = restaurantLines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(';');
            if (parts.Length < 2)
            {
                continue;
            }

            var insertRestaurant = connection.CreateCommand();
            insertRestaurant.Transaction = transaction;
            insertRestaurant.CommandText = @"
INSERT INTO restaurant (rest_id, rest_name)
VALUES (@id, @name);";
            insertRestaurant.Parameters.AddWithValue("@id", int.Parse(parts[0], CultureInfo.InvariantCulture));
            insertRestaurant.Parameters.AddWithValue("@name", parts[1]);
            insertRestaurant.ExecuteNonQuery();
        }

        string[] dishLines = File.ReadAllLines(dishesCsvPath);
        for (int i = 1; i < dishLines.Length; i++)
        {
            string line = dishLines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(';');
            if (parts.Length < 4)
            {
                continue;
            }

            var insertDish = connection.CreateCommand();
            insertDish.Transaction = transaction;
            insertDish.CommandText = @"
INSERT INTO dish (dish_id, rest_id, dish_name, price)
VALUES (@id, @restId, @name, @price);";
            insertDish.Parameters.AddWithValue("@id", int.Parse(parts[0], CultureInfo.InvariantCulture));
            insertDish.Parameters.AddWithValue("@restId", int.Parse(parts[1], CultureInfo.InvariantCulture));
            insertDish.Parameters.AddWithValue("@name", parts[2]);
            insertDish.Parameters.AddWithValue("@price", int.Parse(parts[3], CultureInfo.InvariantCulture));
            insertDish.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    /// <summary>
    /// Возвращает все рестораны.
    /// </summary>
    public List<Restaurant> GetAllRestaurants()
    {
        var result = new List<Restaurant>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT rest_id, rest_name FROM restaurant ORDER BY rest_id;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Restaurant(
                reader.GetInt32(0),
                reader.GetString(1)));
        }

        return result;
    }

    /// <summary>
    /// Возвращает все блюда.
    /// </summary>
    public List<MenuDish> GetAllDishes()
    {
        var result = new List<MenuDish>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT dish_id, rest_id, dish_name, price FROM dish ORDER BY dish_id;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new MenuDish(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return result;
    }

    /// <summary>
    /// Возвращает блюдо по идентификатору или null, если запись не найдена.
    /// </summary>
    public MenuDish? GetDishById(int dishId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT dish_id, rest_id, dish_name, price
FROM dish
WHERE dish_id = @id;";
        command.Parameters.AddWithValue("@id", dishId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new MenuDish(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetInt32(3));
    }

    /// <summary>
    /// Добавляет новое блюдо.
    /// </summary>
    public void AddDish(MenuDish dish)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO dish (dish_id, rest_id, dish_name, price)
VALUES (@id, @restId, @name, @price);";
        command.Parameters.AddWithValue("@id", dish.Id);
        command.Parameters.AddWithValue("@restId", dish.RestaurantId);
        command.Parameters.AddWithValue("@name", dish.Name);
        command.Parameters.AddWithValue("@price", dish.Price);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Обновляет блюдо по идентификатору.
    /// </summary>
    public void UpdateDish(MenuDish dish)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE dish
SET rest_id = @restId,
    dish_name = @name,
    price = @price
WHERE dish_id = @id;";
        command.Parameters.AddWithValue("@id", dish.Id);
        command.Parameters.AddWithValue("@restId", dish.RestaurantId);
        command.Parameters.AddWithValue("@name", dish.Name);
        command.Parameters.AddWithValue("@price", dish.Price);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Удаляет блюдо по идентификатору.
    /// </summary>
    public void DeleteDish(int dishId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dish WHERE dish_id = @id;";
        command.Parameters.AddWithValue("@id", dishId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Выполняет произвольный SQL-запрос для формирования отчётов.
    /// </summary>
    public QueryResult ExecuteQuery(string sql)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = sql;

        using var reader = command.ExecuteReader();

        var headers = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            headers[i] = reader.GetName(i);
        }

        var rows = new List<string[]>();
        while (reader.Read())
        {
            var values = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                values[i] = reader.GetValue(i).ToString() ?? "";
            }

            rows.Add(values);
        }

        return new QueryResult(headers, rows);
    }

    /// <summary>
    /// Проверяет, что таблицы restaurant и dish пока не содержат данных.
    /// </summary>
    public bool IsDataEmpty()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var restaurantCommand = connection.CreateCommand();
        restaurantCommand.CommandText = "SELECT COUNT(*) FROM restaurant;";
        long restaurantCount = (long)(restaurantCommand.ExecuteScalar() ?? 0L);

        var dishCommand = connection.CreateCommand();
        dishCommand.CommandText = "SELECT COUNT(*) FROM dish;";
        long dishCount = (long)(dishCommand.ExecuteScalar() ?? 0L);

        return restaurantCount == 0 && dishCount == 0;
    }

    private void EnsureDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS restaurant (
    rest_id   INTEGER PRIMARY KEY,
    rest_name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS dish (
    dish_id   INTEGER PRIMARY KEY,
    rest_id   INTEGER NOT NULL,
    dish_name TEXT NOT NULL,
    price     INTEGER NOT NULL CHECK (price >= 0),
    FOREIGN KEY (rest_id) REFERENCES restaurant(rest_id)
);";
        command.ExecuteNonQuery();
    }
}

/// <summary>
/// Табличный результат SQL-запроса.
/// </summary>
/// <param name="Headers">Названия столбцов.</param>
/// <param name="Rows">Строки результата.</param>
public record QueryResult(string[] Headers, List<string[]> Rows);
