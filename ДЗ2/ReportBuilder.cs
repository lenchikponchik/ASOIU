using System.Text;

/// <summary>
/// Построитель отчетов в стиле Fluent Interface.
/// </summary>
public class ReportBuilder
{
    private readonly DatabaseManager _db;

    private string _sql = "";
    private string _title = "";
    private string[] _headers = Array.Empty<string>();
    private int[] _widths = Array.Empty<int>();

    /// <summary>
    /// Конструктор принимает менеджер БД.
    /// </summary>
    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }

    /// <summary>
    /// SQL-запрос отчета.
    /// </summary>
    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    /// <summary>
    /// Заголовок отчета.
    /// </summary>
    public ReportBuilder Title(string text)
    {
        _title = text;
        return this;
    }

    /// <summary>
    /// Отображаемые заголовки колонок.
    /// </summary>
    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }

    /// <summary>
    /// Ширина колонок в символах.
    /// </summary>
    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    /// <summary>
    /// Формирует отчет и возвращает его текст.
    /// </summary>
    public string Build()
    {
        if (string.IsNullOrWhiteSpace(_sql))
        {
            throw new InvalidOperationException("Не задан SQL-запрос отчета.");
        }

        QueryResult result = _db.ExecuteQuery(_sql);
        string[] headersToUse = _headers.Length > 0 ? _headers : result.Headers;
        int[] widthsToUse = ResolveWidths(headersToUse, result.Rows);

        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(_title))
        {
            sb.AppendLine($"=== {_title} ===");
        }

        AppendRow(sb, headersToUse, widthsToUse);
        sb.AppendLine(new string('-', GetDividerLength(widthsToUse)));

        foreach (string[] row in result.Rows)
        {
            AppendRow(sb, row, widthsToUse);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Выводит отчет в консоль.
    /// </summary>
    public void Print()
    {
        Console.WriteLine(Build());
    }

    /// <summary>
    /// Сохраняет отчет в текстовый файл.
    /// </summary>
    public void SaveToFile(string path)
    {
        File.WriteAllText(path, Build(), Encoding.UTF8);
    }

    private static void AppendRow(StringBuilder sb, string[] values, int[] widths)
    {
        int columns = Math.Min(values.Length, widths.Length);

        for (int i = 0; i < columns; i++)
        {
            string value = values[i] ?? "";
            if (value.Length > widths[i])
            {
                value = value[..Math.Max(0, widths[i] - 1)] + "…";
            }

            sb.Append($"{value,-1}");
            int padding = Math.Max(0, widths[i] - value.Length);
            sb.Append(new string(' ', padding));
            if (i < columns - 1)
            {
                sb.Append(' ');
            }
        }

        sb.AppendLine();
    }

    private int[] ResolveWidths(string[] headers, List<string[]> rows)
    {
        if (_widths.Length > 0)
        {
            return _widths;
        }

        int columns = headers.Length;
        var widths = new int[columns];

        for (int i = 0; i < columns; i++)
        {
            widths[i] = Math.Max(8, headers[i].Length + 2);
        }

        foreach (string[] row in rows)
        {
            for (int i = 0; i < Math.Min(columns, row.Length); i++)
            {
                int candidate = row[i].Length + 2;
                if (candidate > widths[i])
                {
                    widths[i] = candidate;
                }
            }
        }

        return widths;
    }

    private static int GetDividerLength(int[] widths)
    {
        int total = 0;
        for (int i = 0; i < widths.Length; i++)
        {
            total += widths[i];
        }

        return total + widths.Length - 1;
    }
}
