// =============================================================================
// S — Single Responsibility Principle
// Один класс — одна причина для изменения
// =============================================================================

// ---- ПЛОХО: Employee делает всё сразу ----
public class Employee_Dirty
{
    public string Name     { get; set; } = "";
    public double Salary   { get; set; }

    // Логика валидации
    public bool IsValid() => !string.IsNullOrWhiteSpace(Name) && Salary > 0;

    // Логика сохранения в БД
    public void SaveToDatabase() =>
        Console.WriteLine($"[DB] Сохранён сотрудник: {Name}");

    // Логика формирования документа
    public string GeneratePayslip() =>
        $"Расчётный лист: {Name}, зарплата {Salary:F2} руб.";
}

// ---- ХОРОШО: каждый класс — своя ответственность ----

/// <summary>Данные о сотруднике.</summary>
public class Employee(string name, double salary)
{
    public string Name   => name;
    public double Salary => salary;
}

/// <summary>Проверка корректности данных сотрудника.</summary>
public class EmployeeValidator
{
    public bool IsValid(Employee emp) =>
        !string.IsNullOrWhiteSpace(emp.Name) && emp.Salary > 0;
}

/// <summary>Сохранение сотрудника в хранилище.</summary>
public class EmployeeRepository
{
    public void Save(Employee emp) =>
        Console.WriteLine($"[DB] Сохранён сотрудник: {emp.Name}");
}

/// <summary>Формирование расчётного листа.</summary>
public class PayslipGenerator
{
    public string Generate(Employee emp) =>
        $"Расчётный лист: {emp.Name}, зарплата {emp.Salary:F2} руб.";
}
