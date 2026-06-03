// =============================================================================
// Чистый код: именование, ранний return, константы
// =============================================================================

// ---- ПЛОХО ----

/// <summary>Вычислитель бонуса — «грязная» версия.</summary>
public class BonusCalculator_Dirty
{
    // Магические числа, неговорящие имена, вложенные условия
    public double Calc(int e, double s, bool f)
    {
        double r = 0;
        if (e > 0)
        {
            if (s > 0)
            {
                if (f)
                    r = s * 0.15;
                else
                    r = s * 0.10;
                if (e > 5)
                    r += 500;
            }
        }
        return r;
    }
}

// ---- ХОРОШО ----

/// <summary>Вычислитель бонуса — «чистая» версия.</summary>
public class BonusCalculator
{
    private const double BaseBonusRate       = 0.10;
    private const double FullTimeBonusRate   = 0.15;
    private const double SeniorityBonus      = 500.0;
    private const int    SeniorityThreshold  = 5;

    /// <summary>
    /// Рассчитать бонус сотрудника.
    /// </summary>
    /// <param name="yearsOfService">Лет в компании (≥ 1).</param>
    /// <param name="annualSalary">Годовая зарплата (> 0).</param>
    /// <param name="isFullTime">Признак полной занятости.</param>
    public double Calculate(int yearsOfService, double annualSalary, bool isFullTime)
    {
        if (yearsOfService <= 0 || annualSalary <= 0)
            return 0;

        double rate  = isFullTime ? FullTimeBonusRate : BaseBonusRate;
        double bonus = annualSalary * rate;

        if (yearsOfService > SeniorityThreshold)
            bonus += SeniorityBonus;

        return bonus;
    }
}
