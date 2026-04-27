# ASOIU
Антонов Леонид ИУ5 - 22Б.

## Состав проекта

- `sem1` - анализ результатов квалификации гоночных болидов.
- `sem2` - разреженные матрицы: COO, LIL, CSR.
- `sem3` - CSV, SQLite и базовые операции реляционной алгебры.
- `sem4` - классы, разреженная матрица, собственный список и стек.
- `sem5` - внедрение зависимостей и DI-контейнер.
- `sem6` - рефакторинг и принципы SOLID/DRY/KISS/YAGNI.
- `ДЗ1` - расстояние Дамерау-Левенштейна.
- `ДЗ2` - консольное приложение SQLite по варианту 22: рестораны и блюда.

## Запуск

Открывать в Rider можно файл `ASOIU.sln`.

Проверка всех проектов:

```bash
dotnet build ASOIU.sln
```

Запуск отдельного задания:

```bash
dotnet run --project sem1/sem1.csproj
dotnet run --project sem2/sem2.csproj
dotnet run --project sem3/sem3.csproj
dotnet run --project sem4/sem4.csproj
dotnet run --project sem5/sem5.csproj
dotnet run --project sem6/sem6.csproj
dotnet run --project ДЗ1/EditDistance.csproj
dotnet run --project ДЗ2/Dz2.csproj
```

Общий отчёт по семинарам находится в `ОТЧЕТ_семинары.md`.
