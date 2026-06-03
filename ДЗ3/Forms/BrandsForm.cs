using Microsoft.EntityFrameworkCore;
using ДЗ3.Data;
using ДЗ3.Models;

namespace ДЗ3.Forms;

/// <summary>
/// Форма управления марками автомобилей (master CRUD).
/// Удаление запрещено, если к марке привязаны автомобили.
/// </summary>
public class BrandsForm : Form
{
    private readonly DataGridView _grid = new();
    private readonly TextBox      _txtName = new();
    private readonly Button       _btnAdd    = new() { Text = "Добавить"   };
    private readonly Button       _btnEdit   = new() { Text = "Изменить"   };
    private readonly Button       _btnDelete = new() { Text = "Удалить"    };
    private readonly Button       _btnClose  = new() { Text = "Закрыть"    };

    public BrandsForm()
    {
        Text          = "Марки автомобилей";
        Size          = new Size(560, 440);
        StartPosition = FormStartPosition.CenterParent;

        // Таблица
        _grid.Location          = new Point(12, 12);
        _grid.Size              = new Size(520, 300);
        _grid.SelectionMode     = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect       = false;
        _grid.ReadOnly          = true;
        _grid.AllowUserToAddRows = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // Поле ввода
        var lblName = new Label { Text = "Название:", Location = new Point(12, 324), AutoSize = true };
        _txtName.Location = new Point(100, 320);
        _txtName.Size     = new Size(220, 24);

        _btnAdd.Location    = new Point(12,  360); _btnAdd.Size    = new Size(100, 30);
        _btnEdit.Location   = new Point(120, 360); _btnEdit.Size   = new Size(100, 30);
        _btnDelete.Location = new Point(228, 360); _btnDelete.Size = new Size(100, 30);
        _btnClose.Location  = new Point(432, 360); _btnClose.Size  = new Size(100, 30);

        _btnAdd.Click    += BtnAdd_Click;
        _btnEdit.Click   += BtnEdit_Click;
        _btnDelete.Click += BtnDelete_Click;
        _btnClose.Click  += (_, _) => Close();

        Controls.AddRange([_grid, lblName, _txtName, _btnAdd, _btnEdit, _btnDelete, _btnClose]);
        LoadGrid();
    }

    private void LoadGrid()
    {
        using var db = new AppDbContext();
        var brands = db.CarBrands
            .OrderBy(b => b.Name)
            .Select(b => new { b.Id, b.Name, Автомобилей = b.Cars.Count })
            .ToList();

        _grid.DataSource = brands;
        if (_grid.Columns.Count > 0)
            _grid.Columns[0].Visible = false; // скрываем Id
    }

    private int? SelectedId()
    {
        if (_grid.CurrentRow?.DataBoundItem is null) return null;
        dynamic row = _grid.CurrentRow.DataBoundItem;
        return row.Id;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        string name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Msg("Введите название марки."); return; }

        using var db = new AppDbContext();
        if (db.CarBrands.Any(b => b.Name == name))
        { Msg("Марка с таким названием уже существует."); return; }

        db.CarBrands.Add(new CarBrand { Name = name });
        db.SaveChanges();
        _txtName.Clear();
        LoadGrid();
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id is null) { Msg("Выберите марку."); return; }

        string name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Msg("Введите новое название."); return; }

        using var db = new AppDbContext();
        var brand = db.CarBrands.Find(id.Value);
        if (brand is null) { Msg("Марка не найдена."); return; }

        if (db.CarBrands.Any(b => b.Name == name && b.Id != id.Value))
        { Msg("Марка с таким названием уже существует."); return; }

        brand.Name = name;
        db.SaveChanges();
        _txtName.Clear();
        LoadGrid();
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id is null) { Msg("Выберите марку."); return; }

        using var db = new AppDbContext();
        if (db.Cars.Any(c => c.BrandId == id.Value))
        { Msg("Нельзя удалить марку: к ней привязаны автомобили."); return; }

        var brand = db.CarBrands.Find(id.Value);
        if (brand is null) return;

        if (MessageBox.Show($"Удалить марку «{brand.Name}»?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        db.CarBrands.Remove(brand);
        db.SaveChanges();
        LoadGrid();
    }

    private void Msg(string text) =>
        MessageBox.Show(text, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
