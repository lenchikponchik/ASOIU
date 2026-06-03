using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ДЗ3.Data;

namespace ДЗ3.Controllers;

/// <summary>Три LINQ-отчёта по автомобилям.</summary>
public class ReportController(AppDbContext db) : Controller
{
    public IActionResult Index()
    {
        // §1 — полный список: Include + OrderBy
        var allCars = db.Cars
            .Include(c => c.Brand)
            .OrderBy(c => c.Brand!.Name)
            .ThenBy(c => c.Name)
            .Select(c => new CarRow(c.Brand!.Name, c.Name, c.Horsepower))
            .ToList();

        // §2 — количество по маркам: GroupBy + Count
        var countByBrand = db.Cars
            .Include(c => c.Brand)
            .GroupBy(c => c.Brand!.Name)
            .Select(g => new CountRow(g.Key, g.Count()))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Brand)
            .ToList();

        // §3 — средняя мощность: GroupBy + Average + OrderByDescending
        var avgByBrand = db.Cars
            .Include(c => c.Brand)
            .GroupBy(c => c.Brand!.Name)
            .Select(g => new AvgRow(g.Key, Math.Round(g.Average(c => (double)c.Horsepower), 1)))
            .OrderByDescending(x => x.AvgHp)
            .ToList();

        ViewBag.AllCars     = allCars;
        ViewBag.CountBrand  = countByBrand;
        ViewBag.AvgBrand    = avgByBrand;
        return View();
    }
}

public record CarRow(string Brand, string Name, int Horsepower);
public record CountRow(string Brand, int Count);
public record AvgRow(string Brand, double AvgHp);
