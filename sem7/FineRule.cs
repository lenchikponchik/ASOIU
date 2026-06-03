/// <summary>Контракт правила расчёта штрафа за просрочку книги.</summary>
public interface IFineRule
{
    /// <summary>
    /// Рассчитать штраф.
    /// </summary>
    /// <param name="bookPrice">Стоимость книги (руб.).</param>
    /// <param name="daysLate">Количество дней просрочки (≥ 0).</param>
    decimal Calculate(decimal bookPrice, int daysLate);
}

/// <summary>Обычный читатель — 5 % от стоимости в день.</summary>
public class RegularFineRule : IFineRule
{
    public decimal Calculate(decimal bookPrice, int daysLate) =>
        bookPrice * 0.05m * Math.Max(0, daysLate);
}

/// <summary>Студент — 3 % в день (льготная ставка).</summary>
public class StudentFineRule : IFineRule
{
    public decimal Calculate(decimal bookPrice, int daysLate) =>
        bookPrice * 0.03m * Math.Max(0, daysLate);
}

/// <summary>Пенсионер — 1 % в день.</summary>
public class SeniorFineRule : IFineRule
{
    public decimal Calculate(decimal bookPrice, int daysLate) =>
        bookPrice * 0.01m * Math.Max(0, daysLate);
}

/// <summary>Сотрудник библиотеки — штраф всегда 0.</summary>
public class StaffFineRule : IFineRule
{
    public decimal Calculate(decimal bookPrice, int daysLate) => 0m;
}

/// <summary>
/// Семейный читатель: 5 дней льготного периода,
/// затем 2 % в день, но не более 100 руб.
/// </summary>
public class FamilyFineRule : IFineRule
{
    private const int     GracePeriodDays = 5;
    private const decimal DailyRate       = 0.02m;
    private const decimal MaxFine         = 100m;

    public decimal Calculate(decimal bookPrice, int daysLate)
    {
        int effective = Math.Max(0, daysLate - GracePeriodDays);
        decimal fine  = bookPrice * DailyRate * effective;
        return Math.Min(fine, MaxFine);
    }
}
