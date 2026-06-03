Console.WriteLine("=== Семинар 6: Рефакторинг и принципы SOLID ===");
Console.WriteLine();

// =============================================================================
// §1. Чистый код
// =============================================================================
Console.WriteLine("--- §1. Чистый код: BonusCalculator ---");

var dirtyCalc = new BonusCalculator_Dirty();
Console.WriteLine($"«Грязный» результат: {dirtyCalc.Calc(3, 100_000, true)}");

var cleanCalc = new BonusCalculator();
Console.WriteLine($"«Чистый» результат:  {cleanCalc.Calculate(3, 100_000, true)}");

Console.WriteLine();

// =============================================================================
// §2. DRY / KISS / YAGNI
// =============================================================================
Console.WriteLine("--- §2. DRY / KISS / YAGNI ---");

var orders = new OrderService();
Console.WriteLine($"Цена для обычного покупателя (1000 руб.): {orders.GetPriceForRegular(1000):F2}");
Console.WriteLine($"Цена для студента:                        {orders.GetPriceForStudent(1000):F2}");
Console.WriteLine($"Цена для сотрудника:                      {orders.GetPriceForEmployee(1000):F2}");

var dateChecker = new DateChecker();
Console.WriteLine($"Сегодня выходной: {dateChecker.IsWeekend(DateTime.Today)}");

Console.WriteLine();

// =============================================================================
// §3–§7. Принципы SOLID
// =============================================================================
Console.WriteLine("--- §3. SRP ---");
var emp  = new Employee("Иванов Иван", 80_000);
var val  = new EmployeeValidator();
var repo = new EmployeeRepository();
var ps   = new PayslipGenerator();
Console.WriteLine($"Валиден: {val.IsValid(emp)}");
repo.Save(emp);
Console.WriteLine(ps.Generate(emp));

Console.WriteLine();
Console.WriteLine("--- §4. OCP: скидки ---");
var calc = new DiscountCalculator(new StudentCustomerDiscount());
Console.WriteLine($"Цена для студента (1000 руб.): {calc.Calculate(1000):F2}");
var calcPremium = new DiscountCalculator(new PremiumCustomerDiscount());
Console.WriteLine($"Цена для Premium (1000 руб.):  {calcPremium.Calculate(1000):F2}");

Console.WriteLine();
Console.WriteLine("--- §5. LSP: фигуры ---");
IShape[] shapes = [new Rectangle(4, 5), new Square(4)];
foreach (var shape in shapes)
    Console.WriteLine($"  {shape.GetType().Name}: площадь = {shape.Area()}");

Console.WriteLine();
Console.WriteLine("--- §6. ISP: рабочие ---");
var human = new HumanWorker();
var robot = new Robot();
var scheduler = new WorkScheduler(human);
scheduler.ScheduleWork();
var robotScheduler = new WorkScheduler(robot);
robotScheduler.ScheduleWork();

Console.WriteLine();
Console.WriteLine("--- §7. DIP: библиотека ---");
var library = new Library(new InMemoryLibraryRepository(), new ConsoleLibraryLogger());
library.AddBook("Мастер и Маргарита");
library.AddBook("Евгений Онегин");
library.PrintBooks();

Console.WriteLine();

// =============================================================================
// §8. Паттерн «Стратегия»
// =============================================================================
Console.WriteLine("--- §8. Паттерн «Стратегия»: штрафы ---");

var fineCalc = new FineCalculator(new RegularFineRule());
fineCalc.ComputeAndPrint("Петров",  bookPrice: 300, daysLate: 5);

fineCalc = new FineCalculator(new StudentFineRule());
fineCalc.ComputeAndPrint("Сидоров", bookPrice: 300, daysLate: 5);

fineCalc = new FineCalculator(new SeniorFineRule());
fineCalc.ComputeAndPrint("Бабушкин", bookPrice: 300, daysLate: 5);

fineCalc = new FineCalculator(new StaffFineRule());
fineCalc.ComputeAndPrint("Библиотекарь", bookPrice: 300, daysLate: 5);

Console.WriteLine();

// =============================================================================
// §9. Практика: рефакторинг LibraryService
// =============================================================================
Console.WriteLine("--- §9. Практика: LibraryService до/после ---");

Console.WriteLine("[ДО рефакторинга]");
var before = new LibraryService_Before();
before.AddLoan("Война и мир", "Иванов", -10); // просрочена
before.AddLoan("Анна Каренина", "Петров", 5);
before.PrintOverdue();
Console.WriteLine($"Штраф Иванова: {before.GetTotalFine("Иванов"):F2} руб.");

Console.WriteLine();
Console.WriteLine("[ПОСЛЕ рефакторинга]");
var loanRepo = new InMemoryLoanRepository();
var service  = new LibraryService(loanRepo, new RegularFineRule(), new ConsoleLibraryLogger());
service.AddLoan("Война и мир", "Иванов", -10);
service.AddLoan("Анна Каренина", "Петров", 5);
service.PrintOverdue();
Console.WriteLine($"Штраф Иванова: {service.GetTotalFine("Иванов"):F2} руб.");

Console.WriteLine();
Console.WriteLine("=== Конец семинара 6 ===");
