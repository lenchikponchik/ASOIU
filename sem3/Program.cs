using Microsoft.Data.Sqlite;
using System.Globalization;

const string DbFile = "developers.db";
const string DevCsv = "dev.csv";
const string DepCsv = "dep.csv";
const char Separator = ';';

if (args.Length == 0)
{
    RunSqliteDemo();
}
else
{
    RunCsvPrototype(args);
}

void RunSqliteDemo()
{
    Console.WriteLine("=== ЧАСТЬ 1. РАБОТА С SQLITE ===");

    CreateDatabase(DbFile);
    LoadData(DbFile, DevCsv, DepCsv);
    PrintData(DbFile, "dep");
    PrintData(DbFile, "dev");

    List<string> names = ProjectionSql(DbFile, "dev", "dev_name");
    Console.WriteLine("\n=== Результат Projection(dev, dev_name) ===");
    foreach (string name in names)
    {
        Console.WriteLine(name);
    }

    List<string[]> rows = WhereSql(DbFile, "dev", "dep_id", "2");
    Console.WriteLine("\n=== Результат Where(dev, dep_id, 2) ===");
    foreach (string[] row in rows)
    {
        Console.WriteLine(string.Join(" | ", row));
    }

    var (columns, joinRows) = JoinSql(DbFile, "dev", "dep", "dep_id", "dep_id");
    Console.WriteLine("\n=== Результат Join(dev, dep, dep_id, dep_id) ===");
    Console.WriteLine(string.Join(" | ", columns));
    Console.WriteLine(new string('-', 90));
    foreach (string[] row in joinRows)
    {
        Console.WriteLine(string.Join(" | ", row));
    }

    var (avgColumns, avgRows) = GroupAvgSql(DbFile, "dev", "dep_id", "dev_commits");
    Console.WriteLine("\n=== Результат GroupAvg(dev, dep_id, dev_commits) ===");
    Console.WriteLine(string.Join(" | ", avgColumns));
    Console.WriteLine(new string('-', 40));
    foreach (string[] row in avgRows)
    {
        Console.WriteLine(string.Join(" | ", row));
    }

    Console.WriteLine("\n=== ЧАСТЬ 2. ПРОТОТИП РЕЛЯЦИОННОЙ СУБД НА CSV ===");
    Console.WriteLine("Запуск примеров:");
    Console.WriteLine("dotnet run projection dev_name < dev.csv");
    Console.WriteLine("dotnet run where dep_id 2 < dev.csv");
    Console.WriteLine("dotnet run join dev dep dep_id dep_id");
    Console.WriteLine("dotnet run group_avg dep_id dev_commits < dev.csv");
}

void RunCsvPrototype(string[] programArgs)
{
    string mode = programArgs[0].ToLowerInvariant();

    switch (mode)
    {
        case "projection":
        {
            if (programArgs.Length < 2)
            {
                Console.Error.WriteLine("Использование: program projection <колонка>");
                return;
            }

            CsvTable table = ReadCsv(Console.In, Separator);
            CsvTable result = Projection(table, programArgs[1]);
            WriteCsv(Console.Out, result, Separator);
            break;
        }
        case "where":
        {
            if (programArgs.Length < 3)
            {
                Console.Error.WriteLine("Использование: program where <колонка> <значение>");
                return;
            }

            CsvTable table = ReadCsv(Console.In, Separator);
            CsvTable result = Where(table, programArgs[1], programArgs[2]);
            WriteCsv(Console.Out, result, Separator);
            break;
        }
        case "join":
        {
            if (programArgs.Length < 5)
            {
                Console.Error.WriteLine("Использование: program join <таблица1> <таблица2> <ключ1> <ключ2>");
                return;
            }

            using StreamReader reader1 = File.OpenText(programArgs[1] + ".csv");
            using StreamReader reader2 = File.OpenText(programArgs[2] + ".csv");
            CsvTable left = ReadCsv(reader1, Separator);
            CsvTable right = ReadCsv(reader2, Separator);
            CsvTable result = Join(left, right, programArgs[3], programArgs[4]);
            WriteCsv(Console.Out, result, Separator);
            break;
        }
        case "group_avg":
        {
            if (programArgs.Length < 3)
            {
                Console.Error.WriteLine("Использование: program group_avg <колонка_группировки> <колонка_значений>");
                return;
            }

            CsvTable table = ReadCsv(Console.In, Separator);
            CsvTable result = GroupAvg(table, programArgs[1], programArgs[2]);
            WriteCsv(Console.Out, result, Separator);
            break;
        }
        default:
        {
            Console.Error.WriteLine($"Неизвестный режим: {mode}");
            break;
        }
    }
}

static void CreateDatabase(string dbPath)
{
    if (File.Exists(dbPath))
    {
        File.Delete(dbPath);
    }

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = @"
CREATE TABLE dep (
    dep_id   INTEGER PRIMARY KEY,
    dep_name TEXT NOT NULL
);";
    command.ExecuteNonQuery();

    command.CommandText = @"
CREATE TABLE dev (
    dev_id      INTEGER PRIMARY KEY,
    dep_id      INTEGER NOT NULL,
    dev_name    TEXT NOT NULL,
    dev_commits INTEGER NOT NULL,
    FOREIGN KEY (dep_id) REFERENCES dep(dep_id)
);";
    command.ExecuteNonQuery();

    Console.WriteLine($"[OK] База данных \"{dbPath}\" создана.");
}

static void LoadData(string dbPath, string devCsvPath, string depCsvPath)
{
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    using (var transaction = connection.BeginTransaction())
    {
        string[] lines = File.ReadAllLines(depCsvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2)
            {
                continue;
            }

            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO dep (dep_id, dep_name) VALUES (@id, @name);";
            command.Parameters.AddWithValue("@id", int.Parse(parts[0], CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@name", parts[1]);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
        Console.WriteLine($"[OK] Загружено отделов: {lines.Length - 1}");
    }

    using (var transaction = connection.BeginTransaction())
    {
        string[] lines = File.ReadAllLines(devCsvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4)
            {
                continue;
            }

            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
INSERT INTO dev (dev_id, dep_id, dev_name, dev_commits)
VALUES (@devId, @depId, @name, @commits);";
            command.Parameters.AddWithValue("@devId", int.Parse(parts[0], CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@depId", int.Parse(parts[1], CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@name", parts[2]);
            command.Parameters.AddWithValue("@commits", int.Parse(parts[3], CultureInfo.InvariantCulture));
            command.ExecuteNonQuery();
        }

        transaction.Commit();
        Console.WriteLine($"[OK] Загружено разработчиков: {lines.Length - 1}");
    }
}

static void PrintData(string dbPath, string tableName)
{
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var command = connection.CreateCommand();
    string table = QuoteIdentifier(tableName);
    command.CommandText = $"SELECT * FROM {table} ORDER BY 1;";

    using var reader = command.ExecuteReader();
    const int columnWidth = 20;

    Console.WriteLine($"\n========== Таблица {tableName} ==========");
    for (int i = 0; i < reader.FieldCount; i++)
    {
        Console.Write($"{reader.GetName(i),-columnWidth}");
    }

    Console.WriteLine();
    Console.WriteLine(new string('-', columnWidth * reader.FieldCount));

    while (reader.Read())
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            Console.Write($"{reader.GetValue(i),-columnWidth}");
        }

        Console.WriteLine();
    }
}

static List<string> ProjectionSql(string dbPath, string tableName, string columnName)
{
    var result = new List<string>();

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var command = connection.CreateCommand();
    string table = QuoteIdentifier(tableName);
    string column = QuoteIdentifier(columnName);
    command.CommandText = $"SELECT {column} FROM {table} ORDER BY 1;";

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        result.Add(reader.GetValue(0).ToString()!);
    }

    return result;
}

static List<string[]> WhereSql(string dbPath, string tableName, string columnName, string value)
{
    var result = new List<string[]>();

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var command = connection.CreateCommand();
    string table = QuoteIdentifier(tableName);
    string column = QuoteIdentifier(columnName);
    command.CommandText = $"SELECT * FROM {table} WHERE {column} = @value ORDER BY 1;";
    command.Parameters.AddWithValue("@value", value);

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        var row = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            row[i] = reader.GetValue(i).ToString()!;
        }

        result.Add(row);
    }

    return result;
}

static (string[] columns, List<string[]> rows) JoinSql(
    string dbPath,
    string table1,
    string table2,
    string key1,
    string key2)
{
    var rows = new List<string[]>();

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    string table1Safe = QuoteIdentifier(table1);
    string table2Safe = QuoteIdentifier(table2);
    string key1Safe = QuoteIdentifier(key1);
    string key2Safe = QuoteIdentifier(key2);

    var command = connection.CreateCommand();
    command.CommandText = $@"
SELECT *
FROM {table1Safe}
INNER JOIN {table2Safe}
    ON {table1Safe}.{key1Safe} = {table2Safe}.{key2Safe}
ORDER BY 1;";

    using var reader = command.ExecuteReader();

    var columns = new string[reader.FieldCount];
    for (int i = 0; i < reader.FieldCount; i++)
    {
        columns[i] = reader.GetName(i);
    }

    while (reader.Read())
    {
        var row = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            row[i] = reader.GetValue(i).ToString()!;
        }

        rows.Add(row);
    }

    return (columns, rows);
}

static (string[] columns, List<string[]> rows) GroupAvgSql(
    string dbPath,
    string tableName,
    string groupColumn,
    string avgColumn)
{
    var rows = new List<string[]>();

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    string table = QuoteIdentifier(tableName);
    string groupBy = QuoteIdentifier(groupColumn);
    string avgBy = QuoteIdentifier(avgColumn);

    var command = connection.CreateCommand();
    command.CommandText = $@"
SELECT {groupBy}, AVG({avgBy}) AS avg_{avgColumn}
FROM {table}
GROUP BY {groupBy}
ORDER BY 1;";

    using var reader = command.ExecuteReader();

    var columns = new string[reader.FieldCount];
    for (int i = 0; i < reader.FieldCount; i++)
    {
        columns[i] = reader.GetName(i);
    }

    while (reader.Read())
    {
        var row = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            row[i] = reader.GetValue(i).ToString()!;
        }

        rows.Add(row);
    }

    return (columns, rows);
}

static CsvTable ReadCsv(TextReader reader, char separator)
{
    string? headerLine = reader.ReadLine();
    if (headerLine is null)
    {
        throw new InvalidOperationException("Входной поток пуст.");
    }

    string[] headers = headerLine.Split(separator);
    var rows = new List<CsvRow>();

    string? line;
    while ((line = reader.ReadLine()) is not null)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        rows.Add(new CsvRow(line.Split(separator)));
    }

    return new CsvTable(headers, rows);
}

static void WriteCsv(TextWriter writer, CsvTable table, char separator)
{
    writer.WriteLine(string.Join(separator, table.Headers));

    foreach (CsvRow row in table.Rows)
    {
        writer.WriteLine(string.Join(separator, row.Fields));
    }
}

static int FindColumnIndex(CsvTable table, string columnName)
{
    int index = Array.IndexOf(table.Headers, columnName);
    if (index < 0)
    {
        throw new ArgumentException(
            $"Колонка \"{columnName}\" не найдена. Доступные колонки: {string.Join(", ", table.Headers)}");
    }

    return index;
}

static string QuoteIdentifier(string identifier)
{
    if (string.IsNullOrWhiteSpace(identifier))
    {
        throw new ArgumentException("Идентификатор не может быть пустым.");
    }

    foreach (char c in identifier)
    {
        bool isAllowed = (c >= 'a' && c <= 'z')
                         || (c >= 'A' && c <= 'Z')
                         || (c >= '0' && c <= '9')
                         || c == '_';
        if (!isAllowed)
        {
            throw new ArgumentException($"Недопустимый SQL-идентификатор: {identifier}");
        }
    }

    return $"\"{identifier}\"";
}

static CsvTable Projection(CsvTable table, string columnName)
{
    int columnIndex = FindColumnIndex(table, columnName);
    string[] headers = { columnName };
    var rows = new List<CsvRow>();

    foreach (CsvRow row in table.Rows)
    {
        rows.Add(new CsvRow(new[] { row.Fields[columnIndex] }));
    }

    return new CsvTable(headers, rows);
}

static CsvTable Where(CsvTable table, string columnName, string value)
{
    int columnIndex = FindColumnIndex(table, columnName);
    var rows = new List<CsvRow>();

    foreach (CsvRow row in table.Rows)
    {
        if (row.Fields[columnIndex] == value)
        {
            rows.Add(row);
        }
    }

    return new CsvTable(table.Headers, rows);
}

static CsvTable Join(CsvTable left, CsvTable right, string leftKey, string rightKey)
{
    int leftKeyIndex = FindColumnIndex(left, leftKey);
    int rightKeyIndex = FindColumnIndex(right, rightKey);

    var headers = new string[left.Headers.Length + right.Headers.Length];
    for (int i = 0; i < left.Headers.Length; i++)
    {
        headers[i] = left.Headers[i];
    }

    for (int i = 0; i < right.Headers.Length; i++)
    {
        headers[left.Headers.Length + i] = right.Headers[i];
    }

    var rows = new List<CsvRow>();

    foreach (CsvRow leftRow in left.Rows)
    {
        foreach (CsvRow rightRow in right.Rows)
        {
            if (leftRow.Fields[leftKeyIndex] == rightRow.Fields[rightKeyIndex])
            {
                var fields = new string[leftRow.Fields.Length + rightRow.Fields.Length];

                for (int i = 0; i < leftRow.Fields.Length; i++)
                {
                    fields[i] = leftRow.Fields[i];
                }

                for (int i = 0; i < rightRow.Fields.Length; i++)
                {
                    fields[leftRow.Fields.Length + i] = rightRow.Fields[i];
                }

                rows.Add(new CsvRow(fields));
            }
        }
    }

    return new CsvTable(headers, rows);
}

static double Average(List<double> values)
{
    double sum = 0;
    for (int i = 0; i < values.Count; i++)
    {
        sum += values[i];
    }

    return sum / values.Count;
}

static CsvTable GroupAvg(CsvTable table, string groupColumn, string valueColumn)
{
    int groupIndex = FindColumnIndex(table, groupColumn);
    int valueIndex = FindColumnIndex(table, valueColumn);

    var groups = new Dictionary<string, List<double>>();
    foreach (CsvRow row in table.Rows)
    {
        string key = row.Fields[groupIndex];
        double value = double.Parse(row.Fields[valueIndex], CultureInfo.InvariantCulture);

        if (!groups.ContainsKey(key))
        {
            groups[key] = new List<double>();
        }

        groups[key].Add(value);
    }

    string[] headers = { groupColumn, "avg_" + valueColumn };
    var rows = new List<CsvRow>();

    foreach ((string key, List<double> values) in groups)
    {
        rows.Add(new CsvRow(new[] { key, Average(values).ToString("F2", CultureInfo.InvariantCulture) }));
    }

    return new CsvTable(headers, rows);
}

record CsvRow(string[] Fields);

record CsvTable(string[] Headers, List<CsvRow> Rows);
