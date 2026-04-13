# ДЗ2 (вариант 22)

Предметная область:
- Справочник: `restaurant(rest_id, rest_name)`
- Основная таблица: `dish(dish_id, rest_id, dish_name, price)`

Реализовано:
- классы-модели `Restaurant` и `MenuDish` (свойства, конструкторы, `ToString()`, XML-комментарии);
- `DatabaseManager` (создание БД, импорт CSV, CRUD для основной таблицы, выполнение SQL для отчетов);
- консольное меню (просмотр, добавление, редактирование, удаление, отчеты);
- `ReportBuilder` (Fluent Interface: `Query`, `Title`, `Header`, `ColumnWidths`, `Build`, `Print`);
- доп. задание группы `Б`: терминальный метод `SaveToFile(string path)`.

CSV-файлы:
- `restaurants.csv` (4 записи);
- `menu_items.csv` (12 записей).
