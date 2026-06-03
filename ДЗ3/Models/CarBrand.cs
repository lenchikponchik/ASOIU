using System.ComponentModel.DataAnnotations;

namespace ДЗ3.Models;

/// <summary>Справочник марок автомобилей.</summary>
public class CarBrand
{
    /// <summary>Первичный ключ.</summary>
    public int Id { get; set; }

    /// <summary>Название марки (обязательно, уникально).</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    /// <summary>Автомобили этой марки (навигация «один ко многим»).</summary>
    public ICollection<Car> Cars { get; set; } = [];

    public override string ToString() => Name;
}
