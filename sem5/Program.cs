using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Семинар 5: Внедрение зависимостей (DI) ===");
Console.WriteLine();

// =============================================================================
// §1. Наивная реализация — проблема «Control Freak»
// =============================================================================
Console.WriteLine("--- §1. Наивная реализация (проблема) ---");

var naive = new BookService_ControlFreak();
naive.AddBook("Мастер и Маргарита");
// Чтобы сменить логгер — нужно лезть внутрь класса. Это плохо.

Console.WriteLine();

// =============================================================================
// §2. Внедрение через конструктор
// =============================================================================
Console.WriteLine("--- §2. Внедрение через конструктор ---");

// Передаём ConsoleLogger
var s1 = new BookCatalogService(new ConsoleLogger(), new InMemoryBookStorage(new ConsoleLogger()));
s1.AddBook("Евгений Онегин", "Пушкин");

// Меняем на FileLogger — не трогаем BookCatalogService!
var fileLogger = new FileLogger("sem5_log.txt");
var s2 = new BookCatalogService(fileLogger, new InMemoryBookStorage(fileLogger));
s2.AddBook("Сборник стихотворений", "Пушкин");
Console.WriteLine("(запись ушла в файл sem5_log.txt)");

Console.WriteLine();

// =============================================================================
// §3. Внедрение через свойство
// =============================================================================
Console.WriteLine("--- §3. Внедрение через свойство (необязательная зависимость) ---");

// Демонстрируем через «фиксированный» вариант Property Injection вручную
// (BookCatalogService использует конструктор; здесь покажем концепт напрямую)
var storageWithNull = new InMemoryBookStorage(new NullLogger());
storageWithNull.Save("Тихий Дон", "Шолохов"); // NullLogger — ничего не выводит в лог
Console.WriteLine("(NullLogger молчит — это нормально, он не null)");

Console.WriteLine();

// =============================================================================
// §4. Точка сборки (Pure DI)
// =============================================================================
Console.WriteLine("--- §4. Точка сборки (Pure DI) ---");

ILogger logger    = new ConsoleLogger();
IBookStorage storage = new InMemoryBookStorage(logger);
var catalog = new BookCatalogService(logger, storage);

catalog.AddBook("Война и мир", "Толстой");
catalog.AddBook("Преступление и наказание", "Достоевский");
catalog.RemoveBook("Война и мир");

Console.WriteLine("Текущий каталог:");
catalog.ListBooks();

Console.WriteLine();

// =============================================================================
// §5. DI-контейнер (Microsoft.Extensions.DependencyInjection)
// =============================================================================
Console.WriteLine("--- §5. DI-контейнер ---");

var services = new ServiceCollection();

// Singleton — один экземпляр на всё время жизни контейнера
services.AddSingleton<ILogger, ConsoleLogger>();
// Transient — новый экземпляр при каждом запросе
services.AddTransient<IBookStorage, InMemoryBookStorage>();
// Scoped — один экземпляр на «область» (скоуп)
services.AddScoped<BookCatalogService>();

// ValidateOnBuild проверяет граф зависимостей при старте — ловит ошибки рано
ServiceProvider provider = services.BuildServiceProvider(
    new ServiceProviderOptions { ValidateOnBuild = true });

using IServiceScope scope = provider.CreateScope();
var catalogFromContainer = scope.ServiceProvider.GetRequiredService<BookCatalogService>();
catalogFromContainer.AddBook("Идиот", "Достоевский");
catalogFromContainer.ListBooks();

Console.WriteLine();

// =============================================================================
// §6. Антипаттерны — демонстрация
// =============================================================================
Console.WriteLine("--- §6. Антипаттерны ---");

Console.WriteLine("[Control Freak] плохо:");
var cf = new BookService_ControlFreak();
cf.AddBook("Анна Каренина");

Console.WriteLine("[Control Freak] исправлено — внедряем нужный логгер:");
var cfFixed = new BookService_Fixed_ControlFreak(new ConsoleLogger());
cfFixed.AddBook("Анна Каренина");

Console.WriteLine("[Bastard Injection] плохо — конструктор по умолчанию скрывает зависимость:");
var bi = new BookService_BastardInjection();
bi.AddBook("Обломов");

Console.WriteLine("[Bastard Injection] исправлено:");
var biFixed = new BookService_Fixed_BastardInjection(new ConsoleLogger());
biFixed.AddBook("Обломов");

Console.WriteLine("[Ambient Context] плохо — глобальное состояние:");
LoggerContext.Current = new ConsoleLogger();
var ac = new BookService_AmbientContext();
ac.AddBook("Герой нашего времени");

Console.WriteLine("[Ambient Context] исправлено:");
var acFixed = new BookService_Fixed_AmbientContext(new ConsoleLogger());
acFixed.AddBook("Герой нашего времени");

Console.WriteLine();
Console.WriteLine("=== Конец семинара 5 ===");
