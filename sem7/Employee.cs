/// <summary>Данные о сотруднике.</summary>
public record Employee(int Id, string Name, decimal AnnualSalary);

/// <summary>Контракт репозитория сотрудников.</summary>
public interface IEmployeeRepository
{
    /// <summary>Найти сотрудника по ID или вернуть null.</summary>
    Employee? GetById(int id);
}
