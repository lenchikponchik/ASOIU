// =============================================================================
// O — Open/Closed Principle
// Открыт для расширения, закрыт для изменения
// =============================================================================

// ---- ПЛОХО: добавление нового типа требует редактировать существующий код ----
public class DiscountCalculator_Dirty
{
    public double GetDiscount(string customerType) =>
        customerType switch
        {
            "Regular"  => 0.05,
            "Premium"  => 0.15,
            // Чтобы добавить «Студент» — нужно лезть в этот switch
            _ => 0
        };
}

// ---- ХОРОШО: новые типы добавляются без изменения DiscountCalculator ----

/// <summary>Контракт для расчёта скидки покупателя.</summary>
public interface IDiscountStrategy
{
    /// <summary>Доля скидки (0..1).</summary>
    double GetDiscount();
}

public class RegularCustomerDiscount  : IDiscountStrategy { public double GetDiscount() => 0.05; }
public class PremiumCustomerDiscount  : IDiscountStrategy { public double GetDiscount() => 0.15; }
public class StudentCustomerDiscount  : IDiscountStrategy { public double GetDiscount() => 0.10; }

/// <summary>Калькулятор скидок — не меняется при добавлении новых стратегий.</summary>
public class DiscountCalculator(IDiscountStrategy strategy)
{
    public double Calculate(double price) =>
        price * (1.0 - strategy.GetDiscount());
}

// ---- Пример расширения иерархии без изменения базового кода ----

/// <summary>Базовый элемент библиотечного фонда.</summary>
public abstract class LibraryItem(string title)
{
    public string Title { get; } = title;

    /// <summary>Максимальный срок выдачи в днях.</summary>
    public abstract int MaxLoanDays { get; }
}

public class Book(string title)         : LibraryItem(title) { public override int MaxLoanDays => 14; }
public class Magazine(string title)     : LibraryItem(title) { public override int MaxLoanDays =>  7; }
public class ReferenceBook(string title): LibraryItem(title) { public override int MaxLoanDays =>  1; }
// Добавить «Аудиокнига» — только новый класс, LibraryItem не трогаем.
