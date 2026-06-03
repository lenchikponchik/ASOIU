Console.WriteLine("=== Семинар 7: Тестирование в .NET ===");
Console.WriteLine("(Тесты запускаются из sem7.Tests; здесь — ручная демонстрация объектов)");
Console.WriteLine();

// Демонстрация FineCalculator + FineRule
Console.WriteLine("--- Штрафы ---");
IFineRule[] rules =
[
    new RegularFineRule(),
    new StudentFineRule(),
    new SeniorFineRule(),
    new StaffFineRule(),
    new FamilyFineRule(),
];

foreach (var rule in rules)
{
    var fc = new FineCalculator(rule);
    fc.ComputeAndPrint(rule.GetType().Name, bookPrice: 400m, daysLate: 8);
}

Console.WriteLine();

// Демонстрация BonusCalculator
Console.WriteLine("--- Премии ---");

// Заглушка-репозиторий (ручной Stub)
var stubRepo = new StubEmployeeRepository();
var bonusCalc = new BonusCalculator(stubRepo, new SystemClock(), new ConsoleLogger());

bonusCalc.Calculate(1);   // Иванов
bonusCalc.Calculate(999); // не найден

Console.WriteLine();
Console.WriteLine("Запустите 'dotnet test sem7.Tests/' для выполнения модульных тестов.");

// ---- Вспомогательные заглушки для ручной демонстрации ----
public class StubEmployeeRepository : IEmployeeRepository
{
    public Employee? GetById(int id) => id switch
    {
        1 => new Employee(1, "Иванов Иван", 600_000),
        2 => new Employee(2, "Петрова Анна", 720_000),
        _ => null
    };
}
