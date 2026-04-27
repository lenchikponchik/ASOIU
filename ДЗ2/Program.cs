using System.Globalization;

const string DbFile = "cars.db";
const string BrandsCsv = "car_brands.csv";
const string CarsCsv = "cars.csv";

var db = new DatabaseManager(DbFile);

if (File.Exists(BrandsCsv) && File.Exists(CarsCsv))
{
    if (db.IsDataEmpty())
    {
        db.ImportFromCsv(BrandsCsv, CarsCsv);
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
        Console.WriteLine("=== ДЗ2: Автомобильные марки и автомобили ===");
        Console.WriteLine("1. Показать марки");
        Console.WriteLine("2. Показать автомобили");
        Console.WriteLine("3. Добавить автомобиль");
        Console.WriteLine("4. Изменить автомобиль");
        Console.WriteLine("5. Удалить автомобиль");
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
                    PrintBrands(db);
                    break;
                case "2":
                    PrintCars(db);
                    break;
                case "3":
                    AddCar(db);
                    break;
                case "4":
                    UpdateCar(db);
                    break;
                case "5":
                    DeleteCar(db);
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

static void PrintBrands(DatabaseManager db)
{
    Console.WriteLine("=== Автомобильные марки ===");
    foreach (CarBrand brand in db.GetAllBrands())
    {
        Console.WriteLine(brand);
    }
}

static void PrintCars(DatabaseManager db)
{
    Console.WriteLine("=== Автомобили ===");
    foreach (Car car in db.GetAllCars())
    {
        Console.WriteLine(car);
    }
}

static void AddCar(DatabaseManager db)
{
    Console.WriteLine("=== Добавление автомобиля ===");
    PrintBrands(db);

    int id = ReadRequiredPositiveInt("ID автомобиля: ");
    int brandId = ReadRequiredPositiveInt("ID марки: ");
    string name = ReadRequiredText("Название модели: ");
    int horsepower = ReadRequiredNonNegativeInt("Мощность (л.с.): ");

    db.AddCar(new Car(id, brandId, name, horsepower));
    Console.WriteLine("Автомобиль добавлен.");
}

static void UpdateCar(DatabaseManager db)
{
    Console.WriteLine("=== Изменение автомобиля ===");
    PrintCars(db);

    int id = ReadRequiredPositiveInt("Введите ID автомобиля для изменения: ");

    Car? car = db.GetCarById(id);
    if (car is null)
    {
        Console.WriteLine("Автомобиль не найден.");
        return;
    }

    PrintBrands(db);
    Console.Write($"ID марки [{car.BrandId}]: ");
    string? brandInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(brandInput))
    {
        if (!int.TryParse(brandInput, NumberStyles.Integer, CultureInfo.InvariantCulture, out int newBrandId))
        {
            Console.WriteLine("Некорректный ID марки.");
            return;
        }

        if (newBrandId <= 0)
        {
            Console.WriteLine("ID марки должен быть больше нуля.");
            return;
        }

        car.BrandId = newBrandId;
    }

    Console.Write($"Название модели [{car.Name}]: ");
    string? nameInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nameInput))
    {
        car.Name = nameInput.Trim();
    }

    Console.Write($"Мощность [{car.Horsepower}]: ");
    string? horsepowerInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(horsepowerInput))
    {
        if (!int.TryParse(horsepowerInput, NumberStyles.Integer, CultureInfo.InvariantCulture, out int newHorsepower))
        {
            Console.WriteLine("Некорректная мощность.");
            return;
        }

        if (newHorsepower < 0)
        {
            Console.WriteLine("Мощность не может быть отрицательной.");
            return;
        }

        car.Horsepower = newHorsepower;
    }

    bool updated = db.UpdateCar(car);
    Console.WriteLine(updated ? "Автомобиль обновлен." : "Автомобиль не найден.");
}

static void DeleteCar(DatabaseManager db)
{
    Console.WriteLine("=== Удаление автомобиля ===");
    PrintCars(db);

    int id = ReadRequiredPositiveInt("Введите ID автомобиля для удаления: ");
    bool deleted = db.DeleteCar(id);
    Console.WriteLine(deleted ? "Автомобиль удален." : "Автомобиль не найден.");
}

static void RunReportsMenu(DatabaseManager db)
{
    while (true)
    {
        Console.WriteLine("=== Отчеты ===");
        Console.WriteLine("1. Полный список автомобилей с марками");
        Console.WriteLine("2. Количество автомобилей по маркам");
        Console.WriteLine("3. Средняя мощность по маркам");
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
            "1" => BuildReportAllCars(db),
            "2" => BuildReportCountByBrand(db),
            "3" => BuildReportAverageHorsepowerByBrand(db),
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

static ReportBuilder BuildReportAllCars(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT c.car_name, b.brand_name, c.horsepower
FROM car c
JOIN car_brand b ON c.brand_id = b.brand_id
ORDER BY c.car_name;")
        .Title("Список автомобилей по маркам")
        .Header("Автомобиль", "Марка", "Мощность (л.с.)")
        .ColumnWidths(24, 18, 15);
}

static ReportBuilder BuildReportCountByBrand(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT b.brand_name, COUNT(*) AS car_count
FROM car c
JOIN car_brand b ON c.brand_id = b.brand_id
GROUP BY b.brand_name
ORDER BY car_count DESC, b.brand_name;")
        .Title("Количество автомобилей по маркам")
        .Header("Марка", "Количество")
        .ColumnWidths(18, 12);
}

static ReportBuilder BuildReportAverageHorsepowerByBrand(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT b.brand_name, ROUND(AVG(c.horsepower), 2) AS avg_hp
FROM car c
JOIN car_brand b ON c.brand_id = b.brand_id
GROUP BY b.brand_name
ORDER BY avg_hp DESC, b.brand_name;")
        .Title("Средняя мощность по маркам")
        .Header("Марка", "Средняя мощность")
        .ColumnWidths(18, 18);
}

static void SaveReportToFile(DatabaseManager db)
{
    Console.WriteLine("Какой отчет сохранить?");
    Console.WriteLine("1. Полный список автомобилей с марками");
    Console.WriteLine("2. Количество автомобилей по маркам");
    Console.WriteLine("3. Средняя мощность по маркам");
    Console.Write("Номер отчета: ");
    string? reportChoice = Console.ReadLine();

    ReportBuilder? builder = reportChoice switch
    {
        "1" => BuildReportAllCars(db),
        "2" => BuildReportCountByBrand(db),
        "3" => BuildReportAverageHorsepowerByBrand(db),
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

static int ReadRequiredPositiveInt(string prompt)
{
    while (true)
    {
        int value = ReadRequiredInt(prompt);
        if (value > 0)
        {
            return value;
        }

        Console.WriteLine("Ошибка: введите целое число больше нуля.");
    }
}

static int ReadRequiredNonNegativeInt(string prompt)
{
    while (true)
    {
        int value = ReadRequiredInt(prompt);
        if (value >= 0)
        {
            return value;
        }

        Console.WriteLine("Ошибка: введите целое число не меньше нуля.");
    }
}

static string ReadRequiredText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string value = (Console.ReadLine() ?? "").Trim();
        if (value.Length > 0)
        {
            return value;
        }

        Console.WriteLine("Ошибка: строка не должна быть пустой.");
    }
}
