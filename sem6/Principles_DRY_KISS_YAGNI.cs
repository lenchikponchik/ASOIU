// =============================================================================
// DRY — Don't Repeat Yourself
// =============================================================================

// ---- ПЛОХО: дублирование скидочной логики ----
public class OrderService_Dirty
{
    public double GetPriceForRegular(double price)   => price * 0.90; // -10%
    public double GetPriceForStudent(double price)   => price * 0.85; // -15%
    public double GetPriceForEmployee(double price)  => price * 0.70; // -30%
    // При изменении формулы нужно менять три места — легко забыть
}

// ---- ХОРОШО: одно место вычисления ----
public class OrderService
{
    public double GetDiscountedPrice(double price, double discountFraction) =>
        price * (1.0 - discountFraction);

    public double GetPriceForRegular(double price)   => GetDiscountedPrice(price, 0.10);
    public double GetPriceForStudent(double price)   => GetDiscountedPrice(price, 0.15);
    public double GetPriceForEmployee(double price)  => GetDiscountedPrice(price, 0.30);
}

// =============================================================================
// KISS — Keep It Simple, Stupid
// =============================================================================

// ---- ПЛОХО: избыточная сложность ----
public class DateChecker_Complex
{
    // LINQ там, где хватает простого условия
    public bool IsWeekend(DateTime date) =>
        new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(date.DayOfWeek);
}

// ---- ХОРОШО: просто и понятно ----
public class DateChecker
{
    public bool IsWeekend(DateTime date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
}

// =============================================================================
// YAGNI — You Ain't Gonna Need It
// =============================================================================

// ---- ПЛОХО: заранее «на всякий случай» ----
public class UserRepository_Overengineered
{
    // Реализован экспорт в XML, PDF, Excel — никто не просил
    public string ExportToXml()   => "<users/>";
    public byte[] ExportToPdf()   => [];
    public byte[] ExportToExcel() => [];

    public List<string> GetAll()  => [];
}

// ---- ХОРОШО: только то, что нужно сейчас ----
public class UserRepository
{
    public List<string> GetAll() => [];
}
