using ДЗ3.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=cars_ef.db"));

var app = builder.Build();

// Создать схему и заполнить начальными данными при старте
using (var scope = app.Services.CreateScope())
    DbSeeder.Seed(scope.ServiceProvider.GetRequiredService<AppDbContext>());

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute("default", "{controller=Brands}/{action=Index}/{id?}");

app.Run();
