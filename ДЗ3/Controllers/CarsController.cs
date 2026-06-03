using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ДЗ3.Data;
using ДЗ3.Models;

namespace ДЗ3.Controllers;

/// <summary>CRUD для таблицы автомобилей.</summary>
public class CarsController(AppDbContext db) : Controller
{
    public IActionResult Index()
    {
        var cars = db.Cars
            .Include(c => c.Brand)
            .OrderBy(c => c.Brand!.Name)
            .ThenBy(c => c.Name)
            .ToList();
        return View(cars);
    }

    public IActionResult Create()
    {
        LoadBrandsViewBag();
        return View(new Car());
    }

    [HttpPost]
    public IActionResult Create(Car car)
    {
        if (!ModelState.IsValid)
        {
            LoadBrandsViewBag();
            return View(car);
        }
        db.Cars.Add(car);
        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var car = db.Cars.Find(id);
        if (car is null) return NotFound();
        LoadBrandsViewBag();
        return View(car);
    }

    [HttpPost]
    public IActionResult Edit(int id, Car car)
    {
        if (id != car.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            LoadBrandsViewBag();
            return View(car);
        }
        db.Cars.Update(car);
        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var car = db.Cars.Find(id);
        if (car is not null)
        {
            db.Cars.Remove(car);
            db.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    private void LoadBrandsViewBag()
    {
        ViewBag.Brands = new SelectList(
            db.CarBrands.OrderBy(b => b.Name).ToList(),
            "Id", "Name");
    }
}
