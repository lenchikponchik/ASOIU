using System.ComponentModel.DataAnnotations;

namespace ДЗ3.Models;

/// <summary>Автомобиль.</summary>
public class Car
{
    /// <summary>Первичный ключ.</summary>
    public int Id { get; set; }

    /// <summary>Внешний ключ на марку.</summary>
    public int BrandId { get; set; }

    /// <summary>Марка (навигационное свойство).</summary>
    public CarBrand? Brand { get; set; }

    /// <summary>Название модели (обязательно).</summary>
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = "";

    /// <summary>Мощность двигателя (л.с.), не меньше 0.</summary>
    [Range(0, int.MaxValue, ErrorMessage = "Мощность не может быть отрицательной.")]
    public int Horsepower { get; set; }

    public override string ToString() => $"{Brand?.Name ?? "?"} {Name} ({Horsepower} л.с.)";
}
