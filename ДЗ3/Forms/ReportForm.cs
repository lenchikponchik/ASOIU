using Microsoft.EntityFrameworkCore;
using ДЗ3.Data;

namespace ДЗ3.Forms;

/// <summary>
/// Форма отчётов: три секции на LINQ (Include, GroupBy+Count, GroupBy+Average).
/// </summary>
public class ReportForm : Form
{
    private readonly TabControl     _tabs  = new();
    private readonly DataGridView   _dgAll    = new();
    private readonly DataGridView   _dgCount  = new();
    private readonly DataGridView   _dgAvg    = new();
    private readonly Button         _btnClose = new() { Text = "Закрыть" };

    public ReportForm()
    {
        Text          = "Отчёты";
        Size          = new Size(700, 520);
        StartPosition = FormStartPosition.CenterParent;

        // Настройка вкладок
        _tabs.Location = new Point(12, 12);
        _tabs.Size     = new Size(664, 430);

        var pageAll   = new TabPage("Все автомобили");
        var pageCount = new TabPage("Авто по маркам");
        var pageAvg   = new TabPage("Средняя мощность");

        SetupGrid(_dgAll);
        SetupGrid(_dgCount);
        SetupGrid(_dgAvg);

        pageAll.Controls.Add(_dgAll);
        pageCount.Controls.Add(_dgCount);
        pageAvg.Controls.Add(_dgAvg);

        _tabs.TabPages.AddRange([pageAll, pageCount, pageAvg]);

        _btnClose.Location = new Point(580, 454);
        _btnClose.Size     = new Size(96, 30);
        _btnClose.Click   += (_, _) => Close();

        Controls.AddRange([_tabs, _btnClose]);

        LoadAll();
        LoadCount();
        LoadAvg();
    }

    private static void SetupGrid(DataGridView grid)
    {
        grid.Dock                 = DockStyle.Fill;
        grid.ReadOnly             = true;
        grid.AllowUserToAddRows   = false;
        grid.SelectionMode        = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill;
    }

    // §1 — полный список: Include + OrderBy
    private void LoadAll()
    {
        using var db = new AppDbContext();
        _dgAll.DataSource = db.Cars
            .Include(c => c.Brand)
            .OrderBy(c => c.Brand!.Name)
            .ThenBy(c => c.Name)
            .Select(c => new
            {
                Марка    = c.Brand!.Name,
                Модель   = c.Name,
                Мощность = c.Horsepower,
            })
            .ToList();
    }

    // §2 — количество авто по маркам: GroupBy + Count
    private void LoadCount()
    {
        using var db = new AppDbContext();
        _dgCount.DataSource = db.Cars
            .Include(c => c.Brand)
            .GroupBy(c => c.Brand!.Name)
            .Select(g => new
            {
                Марка       = g.Key,
                Количество  = g.Count(),
            })
            .OrderByDescending(x => x.Количество)
            .ThenBy(x => x.Марка)
            .ToList();
    }

    // §3 — средняя мощность: GroupBy + Average + OrderByDescending
    private void LoadAvg()
    {
        using var db = new AppDbContext();
        _dgAvg.DataSource = db.Cars
            .Include(c => c.Brand)
            .GroupBy(c => c.Brand!.Name)
            .Select(g => new
            {
                Марка             = g.Key,
                СредняяМощность   = Math.Round(g.Average(c => (double)c.Horsepower), 1),
            })
            .OrderByDescending(x => x.СредняяМощность)
            .ToList();
    }
}
