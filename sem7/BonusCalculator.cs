/// <summary>
/// Рассчитывает премию сотрудника.
/// Зависит от IEmployeeRepository, IClock и ILogger — всё подменяемо в тестах.
/// </summary>
public class BonusCalculator(
    IEmployeeRepository repository,
    IClock clock,
    ILogger logger)
{
    // В новогодний период (1–14 января) ставка повышенная
    private const decimal NewYearRate = 0.10m;
    private const decimal BaseRate    = 0.05m;

    /// <summary>
    /// Рассчитать премию для сотрудника с заданным ID.
    /// </summary>
    /// <returns>Сумма премии в рублях, или null если сотрудник не найден.</returns>
    public decimal? Calculate(int employeeId)
    {
        Employee? emp = repository.GetById(employeeId);
        if (emp is null)
        {
            logger.Log($"Сотрудник с ID={employeeId} не найден.");
            return null;
        }

        bool isNewYear = IsNewYearPeriod(clock.Now);
        decimal rate   = isNewYear ? NewYearRate : BaseRate;
        decimal bonus  = emp.AnnualSalary * rate;

        logger.Log($"Премия {emp.Name}: {bonus:F2} руб. (ставка {rate:P0}, НГ={isNewYear})");
        return bonus;
    }

    private static bool IsNewYearPeriod(DateTime dt) =>
        dt.Month == 1 && dt.Day is >= 1 and <= 14;
}
