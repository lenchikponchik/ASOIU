using System.Globalization;
using Microsoft.Data.Sqlite;

/// <summary>
/// Управление базой данных SQLite для марок автомобилей и автомобилей.
/// Инкапсулирует создание таблиц, импорт CSV, CRUD и выполнение SQL-запросов.
/// </summary>
public class DatabaseManager
{
    private readonly string _connectionString;

    /// <summary>
    /// Конструктор. Принимает путь к файлу БД, создает таблицы при необходимости.
    /// </summary>
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        EnsureDatabase();
    }

    /// <summary>
    /// Импортирует данные из CSV-файлов в таблицы car_brand и car.
    /// </summary>
    public void ImportFromCsv(string brandsCsvPath, string carsCsvPath)
    {
        using var connection = CreateOpenConnection();
        using var transaction = connection.BeginTransaction();

        var clearCommand = connection.CreateCommand();
        clearCommand.Transaction = transaction;
        clearCommand.CommandText = "DELETE FROM car; DELETE FROM car_brand;";
        clearCommand.ExecuteNonQuery();

        string[] brandLines = File.ReadAllLines(brandsCsvPath);
        for (int i = 1; i < brandLines.Length; i++)
        {
            string line = brandLines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(';');
            if (parts.Length < 2)
            {
                continue;
            }

            var insertBrand = connection.CreateCommand();
            insertBrand.Transaction = transaction;
            insertBrand.CommandText = @"
INSERT INTO car_brand (brand_id, brand_name)
VALUES (@id, @name);";
            insertBrand.Parameters.AddWithValue("@id", int.Parse(parts[0], CultureInfo.InvariantCulture));
            insertBrand.Parameters.AddWithValue("@name", parts[1]);
            insertBrand.ExecuteNonQuery();
        }

        string[] carLines = File.ReadAllLines(carsCsvPath);
        for (int i = 1; i < carLines.Length; i++)
        {
            string line = carLines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(';');
            if (parts.Length < 4)
            {
                continue;
            }

            var insertCar = connection.CreateCommand();
            insertCar.Transaction = transaction;
            insertCar.CommandText = @"
INSERT INTO car (car_id, brand_id, car_name, horsepower)
VALUES (@id, @brandId, @name, @horsepower);";
            insertCar.Parameters.AddWithValue("@id", int.Parse(parts[0], CultureInfo.InvariantCulture));
            insertCar.Parameters.AddWithValue("@brandId", int.Parse(parts[1], CultureInfo.InvariantCulture));
            insertCar.Parameters.AddWithValue("@name", parts[2]);
            insertCar.Parameters.AddWithValue("@horsepower", int.Parse(parts[3], CultureInfo.InvariantCulture));
            insertCar.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    /// <summary>
    /// Возвращает список всех марок.
    /// </summary>
    public List<CarBrand> GetAllBrands()
    {
        var result = new List<CarBrand>();
        using var connection = CreateOpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT brand_id, brand_name FROM car_brand ORDER BY brand_id;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new CarBrand(reader.GetInt32(0), reader.GetString(1)));
        }

        return result;
    }

    /// <summary>
    /// Возвращает список всех автомобилей.
    /// </summary>
    public List<Car> GetAllCars()
    {
        var result = new List<Car>();
        using var connection = CreateOpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT car_id, brand_id, car_name, horsepower FROM car ORDER BY car_id;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Car(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return result;
    }

    /// <summary>
    /// Возвращает автомобиль по идентификатору или null, если запись не найдена.
    /// </summary>
    public Car? GetCarById(int carId)
    {
        using var connection = CreateOpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT car_id, brand_id, car_name, horsepower
FROM car
WHERE car_id = @id;";
        command.Parameters.AddWithValue("@id", carId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Car(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetInt32(3));
    }

    /// <summary>
    /// Добавляет новый автомобиль.
    /// </summary>
    public void AddCar(Car car)
    {
        using var connection = CreateOpenConnection();

        if (!BrandExists(connection, car.BrandId))
        {
            throw new InvalidOperationException($"Марка с ID={car.BrandId} не найдена.");
        }

        var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO car (car_id, brand_id, car_name, horsepower)
VALUES (@id, @brandId, @name, @horsepower);";
        command.Parameters.AddWithValue("@id", car.Id);
        command.Parameters.AddWithValue("@brandId", car.BrandId);
        command.Parameters.AddWithValue("@name", car.Name);
        command.Parameters.AddWithValue("@horsepower", car.Horsepower);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Обновляет автомобиль по идентификатору.
    /// </summary>
    public bool UpdateCar(Car car)
    {
        using var connection = CreateOpenConnection();

        if (!BrandExists(connection, car.BrandId))
        {
            throw new InvalidOperationException($"Марка с ID={car.BrandId} не найдена.");
        }

        var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE car
SET brand_id = @brandId,
    car_name = @name,
    horsepower = @horsepower
WHERE car_id = @id;";
        command.Parameters.AddWithValue("@id", car.Id);
        command.Parameters.AddWithValue("@brandId", car.BrandId);
        command.Parameters.AddWithValue("@name", car.Name);
        command.Parameters.AddWithValue("@horsepower", car.Horsepower);
        return command.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Удаляет автомобиль по идентификатору.
    /// </summary>
    public bool DeleteCar(int carId)
    {
        using var connection = CreateOpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM car WHERE car_id = @id;";
        command.Parameters.AddWithValue("@id", carId);
        return command.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Выполняет произвольный SQL-запрос для формирования отчетов.
    /// </summary>
    public QueryResult ExecuteQuery(string sql)
    {
        using var connection = CreateOpenConnection();

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
    /// Проверяет, что таблицы car_brand и car пока не содержат данных.
    /// </summary>
    public bool IsDataEmpty()
    {
        using var connection = CreateOpenConnection();

        var brandCommand = connection.CreateCommand();
        brandCommand.CommandText = "SELECT COUNT(*) FROM car_brand;";
        long brandCount = (long)(brandCommand.ExecuteScalar() ?? 0L);

        var carCommand = connection.CreateCommand();
        carCommand.CommandText = "SELECT COUNT(*) FROM car;";
        long carCount = (long)(carCommand.ExecuteScalar() ?? 0L);

        return brandCount == 0 && carCount == 0;
    }

    private void EnsureDatabase()
    {
        using var connection = CreateOpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS car_brand (
    brand_id   INTEGER PRIMARY KEY,
    brand_name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS car (
    car_id      INTEGER PRIMARY KEY,
    brand_id    INTEGER NOT NULL,
    car_name    TEXT NOT NULL,
    horsepower  INTEGER NOT NULL CHECK (horsepower >= 0),
    FOREIGN KEY (brand_id) REFERENCES car_brand(brand_id)
);";
        command.ExecuteNonQuery();
    }

    private SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();

        return connection;
    }

    private static bool BrandExists(SqliteConnection connection, int brandId)
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM car_brand WHERE brand_id = @id LIMIT 1;";
        command.Parameters.AddWithValue("@id", brandId);
        return command.ExecuteScalar() is not null;
    }
}

/// <summary>
/// Табличный результат SQL-запроса.
/// </summary>
/// <param name="Headers">Названия столбцов.</param>
/// <param name="Rows">Строки результата.</param>
public record QueryResult(string[] Headers, List<string[]> Rows);
