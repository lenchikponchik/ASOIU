/// <summary>
/// Блюдо в меню (основная таблица).
/// </summary>
public class MenuDish
{
    /// <summary>
    /// Идентификатор блюда.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Внешний ключ на ресторан.
    /// </summary>
    public int RestaurantId { get; set; }

    /// <summary>
    /// Название блюда.
    /// </summary>
    public string Name { get; set; }

    private int _price;

    /// <summary>
    /// Цена блюда в рублях (не может быть отрицательной).
    /// </summary>
    public int Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Цена блюда не может быть отрицательной.");
            }

            _price = value;
        }
    }

    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    public MenuDish(int id, int restaurantId, string name, int price)
    {
        Id = id;
        RestaurantId = restaurantId;
        Name = name;
        Price = price;
    }

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public MenuDish() : this(0, 0, "", 0)
    {
    }

    public override string ToString() => $"[{Id}] {Name}, ресторан #{RestaurantId}, цена: {Price} руб.";
}
