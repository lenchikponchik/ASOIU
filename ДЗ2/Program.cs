using System.Globalization;

const string DbFile = "restaurants.db";
const string RestaurantsCsv = "restaurants.csv";
const string DishesCsv = "menu_items.csv";

var db = new DatabaseManager(DbFile);

if (File.Exists(RestaurantsCsv) && File.Exists(DishesCsv))
{
    if (db.IsDataEmpty())
    {
        db.ImportFromCsv(RestaurantsCsv, DishesCsv);
        Console.WriteLine("CSV-данные загружены в пустую базу.");
    }
    else
    {
        Console.WriteLine("База уже содержит данные. Импорт CSV пропущен.");
    }
}
else
{
    Console.WriteLine("CSV-файлы не найдены. Импорт пропущен.");
}

RunMainMenu(db);

static void RunMainMenu(DatabaseManager db)
{
    while (true)
    {
        Console.WriteLine("=== ДЗ2: Рестораны и блюда ===");
        Console.WriteLine("1. Показать рестораны");
        Console.WriteLine("2. Показать блюда");
        Console.WriteLine("3. Добавить блюдо");
        Console.WriteLine("4. Изменить блюдо");
        Console.WriteLine("5. Удалить блюдо");
        Console.WriteLine("6. Отчеты");
        Console.WriteLine("0. Выход");
        Console.Write("Выберите пункт: ");
        string? choice = Console.ReadLine();
        Console.WriteLine();

        if (choice == "0")
        {
            break;
        }

        try
        {
            switch (choice)
            {
                case "1":
                    PrintRestaurants(db);
                    break;
                case "2":
                    PrintDishes(db);
                    break;
                case "3":
                    AddDish(db);
                    break;
                case "4":
                    UpdateDish(db);
                    break;
                case "5":
                    DeleteDish(db);
                    break;
                case "6":
                    RunReportsMenu(db);
                    break;
                default:
                    Console.WriteLine("Неизвестный пункт меню.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.WriteLine();
    }
}

static void PrintRestaurants(DatabaseManager db)
{
    Console.WriteLine("=== Рестораны ===");
    foreach (Restaurant restaurant in db.GetAllRestaurants())
    {
        Console.WriteLine(restaurant);
    }
}

static void PrintDishes(DatabaseManager db)
{
    Console.WriteLine("=== Блюда ===");
    foreach (MenuDish dish in db.GetAllDishes())
    {
        Console.WriteLine(dish);
    }
}

static void AddDish(DatabaseManager db)
{
    Console.WriteLine("=== Добавление блюда ===");
    PrintRestaurants(db);

    int id = ReadRequiredInt("ID блюда: ");
    int restaurantId = ReadRequiredInt("ID ресторана: ");

    Console.Write("Название блюда: ");
    string name = (Console.ReadLine() ?? "").Trim();

    int price = ReadRequiredInt("Цена (руб): ");

    db.AddDish(new MenuDish(id, restaurantId, name, price));
    Console.WriteLine("Блюдо добавлено.");
}

static void UpdateDish(DatabaseManager db)
{
    Console.WriteLine("=== Изменение блюда ===");
    PrintDishes(db);

    int id = ReadRequiredInt("Введите ID блюда для изменения: ");

    MenuDish? dish = db.GetDishById(id);
    if (dish is null)
    {
        Console.WriteLine("Блюдо не найдено.");
        return;
    }

    PrintRestaurants(db);
    Console.Write($"ID ресторана [{dish.RestaurantId}]: ");
    string? restaurantInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(restaurantInput))
    {
        if (!int.TryParse(restaurantInput, out int newRestaurantId))
        {
            Console.WriteLine("Некорректный ID ресторана.");
            return;
        }

        dish.RestaurantId = newRestaurantId;
    }

    Console.Write($"Название блюда [{dish.Name}]: ");
    string? nameInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nameInput))
    {
        dish.Name = nameInput.Trim();
    }

    Console.Write($"Цена [{dish.Price}]: ");
    string? priceInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(priceInput))
    {
        if (!int.TryParse(priceInput, NumberStyles.Integer, CultureInfo.InvariantCulture, out int newPrice))
        {
            Console.WriteLine("Некорректная цена.");
            return;
        }

        dish.Price = newPrice;
    }

    db.UpdateDish(dish);
    Console.WriteLine("Блюдо обновлено.");
}

static void DeleteDish(DatabaseManager db)
{
    Console.WriteLine("=== Удаление блюда ===");
    PrintDishes(db);

    int id = ReadRequiredInt("Введите ID блюда для удаления: ");
    db.DeleteDish(id);
    Console.WriteLine("Блюдо удалено (если запись существовала).");
}

static void RunReportsMenu(DatabaseManager db)
{
    while (true)
    {
        Console.WriteLine("=== Отчеты ===");
        Console.WriteLine("1. Полный список блюд с ресторанами");
        Console.WriteLine("2. Количество блюд по ресторанам");
        Console.WriteLine("3. Средняя цена блюд по ресторанам");
        Console.WriteLine("4. Сохранить отчет в файл (группа Б)");
        Console.WriteLine("0. Назад");
        Console.Write("Выберите пункт: ");
        string? choice = Console.ReadLine();
        Console.WriteLine();

        if (choice == "0")
        {
            return;
        }

        ReportBuilder? builder = choice switch
        {
            "1" => BuildReportAllDishes(db),
            "2" => BuildReportCountByRestaurant(db),
            "3" => BuildReportAveragePriceByRestaurant(db),
            _ => null
        };

        if (choice == "4")
        {
            SaveReportToFile(db);
            continue;
        }

        if (builder is null)
        {
            Console.WriteLine("Неизвестный пункт меню отчетов.");
        }
        else
        {
            builder.Print();
        }

        Console.WriteLine();
    }
}

static ReportBuilder BuildReportAllDishes(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT d.dish_name, r.rest_name, d.price
FROM dish d
JOIN restaurant r ON d.rest_id = r.rest_id
ORDER BY d.dish_name;")
        .Title("Список блюд по ресторанам")
        .Header("Блюдо", "Ресторан", "Цена")
        .ColumnWidths(28, 24, 10);
}

static ReportBuilder BuildReportCountByRestaurant(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT r.rest_name, COUNT(*) AS dish_count
FROM dish d
JOIN restaurant r ON d.rest_id = r.rest_id
GROUP BY r.rest_name
ORDER BY dish_count DESC, r.rest_name;")
        .Title("Количество блюд по ресторанам")
        .Header("Ресторан", "Количество")
        .ColumnWidths(24, 12);
}

static ReportBuilder BuildReportAveragePriceByRestaurant(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT r.rest_name, ROUND(AVG(d.price), 2) AS avg_price
FROM dish d
JOIN restaurant r ON d.rest_id = r.rest_id
GROUP BY r.rest_name
ORDER BY avg_price DESC, r.rest_name;")
        .Title("Средняя цена блюд по ресторанам")
        .Header("Ресторан", "Средняя цена")
        .ColumnWidths(24, 14);
}

static void SaveReportToFile(DatabaseManager db)
{
    Console.WriteLine("Какой отчет сохранить?");
    Console.WriteLine("1. Полный список блюд с ресторанами");
    Console.WriteLine("2. Количество блюд по ресторанам");
    Console.WriteLine("3. Средняя цена блюд по ресторанам");
    Console.Write("Номер отчета: ");
    string? reportChoice = Console.ReadLine();

    ReportBuilder? builder = reportChoice switch
    {
        "1" => BuildReportAllDishes(db),
        "2" => BuildReportCountByRestaurant(db),
        "3" => BuildReportAveragePriceByRestaurant(db),
        _ => null
    };

    if (builder is null)
    {
        Console.WriteLine("Некорректный номер отчета.");
        return;
    }

    Console.Write("Путь к файлу (например report.txt): ");
    string? path = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Путь не задан.");
        return;
    }

    builder.SaveToFile(path);
    Console.WriteLine($"Отчет сохранен: {path}");
}

static int ReadRequiredInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            return value;
        }

        Console.WriteLine("Ошибка: введите целое число.");
    }
}
