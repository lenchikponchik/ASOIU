/// <summary>
/// Вычисляет и выводит штраф, используя переданную стратегию IFineRule.
/// </summary>
public class FineCalculator(IFineRule rule)
{
    /// <summary>Рассчитать штраф.</summary>
    public decimal Calculate(decimal bookPrice, int daysLate) =>
        rule.Calculate(bookPrice, daysLate);

    /// <summary>Рассчитать и вывести строку с результатом.</summary>
    public decimal ComputeAndPrint(string readerName, decimal bookPrice, int daysLate)
    {
        decimal fine = rule.Calculate(bookPrice, daysLate);
        Console.WriteLine(
            $"  {readerName}: просрочка {daysLate} дн., " +
            $"цена {bookPrice:F0} руб., штраф {fine:F2} руб.");
        return fine;
    }
}
