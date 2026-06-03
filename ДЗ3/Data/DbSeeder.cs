using ДЗ3.Models;

namespace ДЗ3.Data;

/// <summary>Создаёт схему и заполняет БД начальными данными.</summary>
public static class DbSeeder
{
    /// <summary>Применить EnsureCreated и засеять данные, если таблицы пусты.</summary>
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.CarBrands.Any())
            return; // Данные уже загружены

        var brands = new List<CarBrand>
        {
            new() { Name = "Toyota" },
            new() { Name = "BMW" },
            new() { Name = "Mercedes-Benz" },
            new() { Name = "Lada" },
        };

        db.CarBrands.AddRange(brands);
        db.SaveChanges();

        var toyota = brands[0];
        var bmw    = brands[1];
        var mercs  = brands[2];
        var lada   = brands[3];

        db.Cars.AddRange(
            new Car { BrandId = toyota.Id, Name = "Camry",        Horsepower = 181 },
            new Car { BrandId = toyota.Id, Name = "Corolla",      Horsepower = 122 },
            new Car { BrandId = toyota.Id, Name = "Land Cruiser", Horsepower = 309 },

            new Car { BrandId = bmw.Id, Name = "3 Series",  Horsepower = 184 },
            new Car { BrandId = bmw.Id, Name = "5 Series",  Horsepower = 249 },
            new Car { BrandId = bmw.Id, Name = "X5",        Horsepower = 340 },

            new Car { BrandId = mercs.Id, Name = "C-Class", Horsepower = 194 },
            new Car { BrandId = mercs.Id, Name = "E-Class", Horsepower = 258 },
            new Car { BrandId = mercs.Id, Name = "GLE",     Horsepower = 333 },

            new Car { BrandId = lada.Id, Name = "Vesta",    Horsepower = 106 },
            new Car { BrandId = lada.Id, Name = "Granta",   Horsepower =  90 },
            new Car { BrandId = lada.Id, Name = "Niva 4x4", Horsepower =  83 }
        );

        db.SaveChanges();
    }
}
