using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ДЗ3.Data;
using ДЗ3.Models;

namespace ДЗ3.Controllers;

/// <summary>CRUD для справочника марок автомобилей.</summary>
public class BrandsController(AppDbContext db) : Controller
{
    public IActionResult Index()
    {
        var brands = db.CarBrands
            .OrderBy(b => b.Name)
            .Select(b => new { b.Id, b.Name, CarCount = b.Cars.Count })
            .ToList()
            .Select(x => new BrandRow(x.Id, x.Name, x.CarCount))
            .ToList();
        return View(brands);
    }

    [HttpPost]
    public IActionResult Create(string name)
    {
        name = name?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Название марки не должно быть пустым.";
            return RedirectToAction(nameof(Index));
        }
        if (db.CarBrands.Any(b => b.Name == name))
        {
            TempData["Error"] = $"Марка «{name}» уже существует.";
            return RedirectToAction(nameof(Index));
        }
        db.CarBrands.Add(new CarBrand { Name = name });
        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var brand = db.CarBrands.Find(id);
        if (brand is null) return NotFound();
        return View(brand);
    }

    [HttpPost]
    public IActionResult Edit(int id, string name)
    {
        name = name?.Trim() ?? "";
        var brand = db.CarBrands.Find(id);
        if (brand is null) return NotFound();

        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Название не должно быть пустым.");
            return View(brand);
        }
        if (db.CarBrands.Any(b => b.Name == name && b.Id != id))
        {
            ModelState.AddModelError("name", $"Марка «{name}» уже существует.");
            return View(brand);
        }
        brand.Name = name;
        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (db.Cars.Any(c => c.BrandId == id))
        {
            TempData["Error"] = "Нельзя удалить марку: к ней привязаны автомобили.";
            return RedirectToAction(nameof(Index));
        }
        var brand = db.CarBrands.Find(id);
        if (brand is not null)
        {
            db.CarBrands.Remove(brand);
            db.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}

/// <summary>Строка таблицы марок для отображения.</summary>
public record BrandRow(int Id, string Name, int CarCount);
