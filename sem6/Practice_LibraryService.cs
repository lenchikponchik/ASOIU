// =============================================================================
// Практика §8: рефакторинг LibraryService (SRP / DRY / OCP / DIP)
// =============================================================================

// ---- ДО рефакторинга ----

/// <summary>LibraryService — нарушает SRP, DRY, OCP, DIP.</summary>
public class LibraryService_Before
{
    private readonly List<(string Title, string Reader, DateTime Due)> _loans = [];

    // Нарушение DIP — хардкодим вывод в Console
    // Нарушение SRP — сервис хранит данные, валидирует, считает штраф и логирует
    public void AddLoan(string title, string reader, int days)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Ошибка: название пустое");
            return;
        }
        _loans.Add((title, reader, DateTime.Today.AddDays(days)));
        Console.WriteLine($"Выдано: {title} → {reader}");
    }

    public void PrintOverdue()
    {
        foreach (var loan in _loans)
        {
            int late = (DateTime.Today - loan.Due).Days;
            if (late > 0)
            {
                // Нарушение DRY — формула штрафа продублирована ниже
                decimal fine = 100m * 0.05m * late;
                Console.WriteLine($"{loan.Title} просрочена {late} дн., штраф {fine:F2} руб.");
            }
        }
    }

    public decimal GetTotalFine(string reader)
    {
        decimal total = 0;
        foreach (var loan in _loans.Where(l => l.Reader == reader))
        {
            int late = (DateTime.Today - loan.Due).Days;
            if (late > 0)
                // Нарушение DRY — та же формула третий раз
                total += 100m * 0.05m * late;
        }
        return total;
    }
}

// ---- ПОСЛЕ рефакторинга ----

/// <summary>Запись о выдаче книги.</summary>
public record LoanRecord(string Title, string Reader, DateTime DueDate);

/// <summary>Контракт репозитория выдач.</summary>
public interface ILoanRepository
{
    void Add(LoanRecord loan);
    IEnumerable<LoanRecord> GetAll();
    IEnumerable<LoanRecord> GetByReader(string reader);
}

/// <summary>In-memory репозиторий выдач.</summary>
public class InMemoryLoanRepository : ILoanRepository
{
    private readonly List<LoanRecord> _loans = [];
    public void Add(LoanRecord loan) => _loans.Add(loan);
    public IEnumerable<LoanRecord> GetAll() => _loans;
    public IEnumerable<LoanRecord> GetByReader(string reader) =>
        _loans.Where(l => l.Reader == reader);
}

/// <summary>Валидатор данных выдачи.</summary>
public static class LoanValidator
{
    public static bool IsValid(string title, string reader, int days,
        out string error)
    {
        if (string.IsNullOrWhiteSpace(title))  { error = "Название книги не задано."; return false; }
        if (string.IsNullOrWhiteSpace(reader)) { error = "Имя читателя не задано.";  return false; }
        if (days <= 0)                         { error = "Срок выдачи должен быть > 0."; return false; }
        error = "";
        return true;
    }
}

/// <summary>
/// Рефакторированный сервис: SRP, DRY, OCP, DIP соблюдены.
/// </summary>
public class LibraryService(ILoanRepository repo, IFineRule fineRule, ILibraryLogger log)
{
    private const decimal DefaultBookPrice = 400m; // усреднённая стоимость книги

    public void AddLoan(string title, string reader, int days)
    {
        if (!LoanValidator.IsValid(title, reader, days, out string error))
        {
            log.Log($"Ошибка выдачи: {error}");
            return;
        }

        var loan = new LoanRecord(title, reader, DateTime.Today.AddDays(days));
        repo.Add(loan);
        log.Log($"Выдано: «{title}» → {reader} (срок {days} дн.)");
    }

    public void PrintOverdue()
    {
        log.Log("Просроченные выдачи:");
        foreach (var loan in repo.GetAll())
        {
            int late = (DateTime.Today - loan.DueDate).Days;
            if (late <= 0) continue;

            decimal fine = fineRule.Calculate(DefaultBookPrice, late);
            Console.WriteLine($"  «{loan.Title}» [{loan.Reader}] просрочена {late} дн., штраф {fine:F2} руб.");
        }
    }

    public decimal GetTotalFine(string reader)
    {
        return repo.GetByReader(reader).Sum(loan =>
        {
            int late = (DateTime.Today - loan.DueDate).Days;
            return late > 0 ? fineRule.Calculate(DefaultBookPrice, late) : 0m;
        });
    }
}
