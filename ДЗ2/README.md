# ДЗ2 (вариант 3)

Предметная область:
- Справочник: `car_brand(brand_id, brand_name)`
- Основная таблица: `car(car_id, brand_id, car_name, horsepower)`

Реализовано:
- классы-модели `CarBrand` и `Car` (properties, конструкторы, `ToString()`, XML-комментарии);
- `DatabaseManager` (создание БД, импорт CSV, CRUD для основной таблицы, выполнение SQL для отчетов);
- консольное меню (просмотр, добавление, редактирование, удаление, отчеты);
- `ReportBuilder` (Fluent Interface: `Query`, `Title`, `Header`, `ColumnWidths`, `Build`, `Print`);
- дополнительное задание группы Б: терминальный метод `SaveToFile(string path)`.

CSV-файлы:
- `car_brands.csv` (4 записи);
- `cars.csv` (12 записей).

Дополнительно:
- `diagrams.puml` содержит Chen ER Diagram, IE ER Diagram и Activity Diagram для цепочки `ReportBuilder`.
