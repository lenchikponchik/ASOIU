# ASOIU
Антонов Леонид ИУ5-22Б. Вариант 3: `car_brand` → `car`.

## Состав проекта

| Папка | Содержание |
|---|---|
| `sem1` | Анализ квалификации Гран-при (массивы, пузырьковая сортировка, статистика) |
| `sem2` | Разреженные матрицы: COO, LIL, CSR |
| `sem3` | CSV + SQLite + операции реляционной алгебры |
| `sem4` | Классы-фигуры, дженерик-матрица, собственный список и стек, quicksort |
| `sem5` | Внедрение зависимостей (DI): конструктор / свойство / метод, Pure DI, DI-контейнер, антипаттерны |
| `sem6` | Рефакторинг, чистый код, SOLID/DRY/KISS/YAGNI, паттерн «Стратегия» |
| `sem7` | Тестирование: xUnit, Moq, TDD/BDD; `sem7.Tests/` — модульные тесты |
| `ДЗ1` | Расстояние Дамерау–Левенштейна |
| `ДЗ2` | Консоль SQLite, вариант 3: марки автомобилей и автомобили, Fluent-отчёты |
| `ДЗ3` | EF Core Code First + LINQ + WinForms, вариант 3 |

## Запуск

Открывать в Rider: файл `ASOIU.slnx`.

Сборка всех проектов:
```bash
dotnet build ASOIU.slnx
```

Запуск отдельного задания:
```bash
dotnet run --project sem1/sem1.csproj
dotnet run --project sem2/sem2.csproj
dotnet run --project sem3/sem3.csproj
dotnet run --project sem4/sem4.csproj
dotnet run --project sem5/sem5.csproj
dotnet run --project sem6/sem6.csproj
dotnet run --project sem7/sem7.csproj
dotnet run --project ДЗ1/EditDistance.csproj
dotnet run --project ДЗ2/Dz2.csproj
dotnet run --project ДЗ3/ДЗ3.csproj
```

Модульные тесты:
```bash
dotnet test sem7.Tests/sem7.Tests.csproj
```
