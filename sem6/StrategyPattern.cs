// =============================================================================
// Паттерн «Стратегия» на примере правил штрафа за просрочку книги
// =============================================================================

/// <summary>Контракт правила штрафа.</summary>
public interface IFineRule
{
    /// <summary>
    /// Рассчитать штраф.
    /// </summary>
    /// <param name="bookPrice">Стоимость книги (руб.).</param>
    /// <param name="daysLate">Количество дней просрочки.</param>
    decimal Calculate(decimal bookPrice, int daysLate);

    /// <summary>Краткое описание правила.</summary>
    string Description { get; }
}

/// <summary>Обычный читатель — 5 % от стоимости книги за каждый день просрочки.</summary>
public class RegularFineRule : IFineRule
{
    public string Description => "Обычный читатель: 5% в день";

    public decimal Calculate(decimal bookPrice, int daysLate) =>
        bookPrice * 0.05m * Math.Max(0, daysLate);
}

/// <summary>Студент — 3 % в день (льготная ставка).</summary>
public class StudentFineRule : IFineRule
{
    public string Description => "Студент: 3% в день";

    public decimal Calculate(decimal bookPrice, int daysLate) =>
        bookPrice * 0.03m * Math.Max(0, daysLate);
}

/// <summary>Пенсионер — 1 % в день.</summary>
public class SeniorFineRule : IFineRule
{
    public string Description => "Пенсионер: 1% в день";

    public decimal Calculate(decimal bookPrice, int daysLate) =>
        bookPrice * 0.01m * Math.Max(0, daysLate);
}

/// <summary>Сотрудник библиотеки — без штрафа.</summary>
public class StaffFineRule : IFineRule
{
    public string Description => "Сотрудник: без штрафа";

    public decimal Calculate(decimal bookPrice, int daysLate) => 0m;
}

/// <summary>
/// Вычислитель штрафа — принимает любую стратегию IFineRule.
/// Не знает и не должен знать о конкретных правилах.
/// </summary>
public class FineCalculator(IFineRule rule)
{
    /// <summary>Рассчитать штраф и вывести результат.</summary>
    public decimal ComputeAndPrint(string readerName, decimal bookPrice, int daysLate)
    {
        decimal fine = rule.Calculate(bookPrice, daysLate);
        Console.WriteLine(
            $"  {readerName} | {rule.Description} | " +
            $"просрочка {daysLate} дн. | цена {bookPrice:F0} руб. | штраф {fine:F2} руб.");
        return fine;
    }
}
