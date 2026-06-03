using Microsoft.EntityFrameworkCore;
using ДЗ3.Data;
using ДЗ3.Models;

namespace ДЗ3.Forms;

/// <summary>
/// Форма управления автомобилями (detail CRUD).
/// Марка выбирается из выпадающего списка; мощность ≥ 0.
/// </summary>
public class CarsForm : Form
{
    private readonly DataGridView _grid   = new();
    private readonly ComboBox     _cmbBrand = new();
    private readonly TextBox      _txtName  = new();
    private readonly TextBox      _txtHp    = new();
    private readonly Button       _btnAdd    = new() { Text = "Добавить"  };
    private readonly Button       _btnEdit   = new() { Text = "Изменить"  };
    private readonly Button       _btnDelete = new() { Text = "Удалить"   };
    private readonly Button       _btnClose  = new() { Text = "Закрыть"   };

    public CarsForm()
    {
        Text          = "Автомобили";
        Size          = new Size(680, 500);
        StartPosition = FormStartPosition.CenterParent;

        _grid.Location            = new Point(12, 12);
        _grid.Size                = new Size(644, 320);
        _grid.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect         = false;
        _grid.ReadOnly            = true;
        _grid.AllowUserToAddRows  = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionChanged   += Grid_SelectionChanged;

        // Строка ввода
        int y = 344;
        AddLabel("Марка:",    12, y);      _cmbBrand.Location = new Point(100, y); _cmbBrand.Size = new Size(160, 24); _cmbBrand.DropDownStyle = ComboBoxStyle.DropDownList;
        AddLabel("Модель:",  280, y);      _txtName.Location  = new Point(345, y); _txtName.Size  = new Size(180, 24);
        y += 34;
        AddLabel("Мощность (л.с.):", 12, y); _txtHp.Location = new Point(140, y); _txtHp.Size   = new Size(100, 24);

        _btnAdd.Location    = new Point(12,  420); _btnAdd.Size    = new Size(100, 30);
        _btnEdit.Location   = new Point(120, 420); _btnEdit.Size   = new Size(100, 30);
        _btnDelete.Location = new Point(228, 420); _btnDelete.Size = new Size(100, 30);
        _btnClose.Location  = new Point(544, 420); _btnClose.Size  = new Size(112, 30);

        _btnAdd.Click    += BtnAdd_Click;
        _btnEdit.Click   += BtnEdit_Click;
        _btnDelete.Click += BtnDelete_Click;
        _btnClose.Click  += (_, _) => Close();

        Controls.AddRange([_grid, _cmbBrand, _txtName, _txtHp, _btnAdd, _btnEdit, _btnDelete, _btnClose]);
        LoadBrandsCombo();
        LoadGrid();
    }

    private void AddLabel(string text, int x, int y)
    {
        var lbl = new Label { Text = text, Location = new Point(x, y + 3), AutoSize = true };
        Controls.Add(lbl);
    }

    private void LoadBrandsCombo()
    {
        using var db = new AppDbContext();
        _cmbBrand.DataSource    = db.CarBrands.OrderBy(b => b.Name).ToList();
        _cmbBrand.DisplayMember = "Name";
        _cmbBrand.ValueMember   = "Id";
    }

    private void LoadGrid()
    {
        using var db = new AppDbContext();
        var cars = db.Cars
            .Include(c => c.Brand)
            .OrderBy(c => c.Brand!.Name)
            .ThenBy(c => c.Name)
            .Select(c => new
            {
                c.Id,
                Марка       = c.Brand!.Name,
                Модель      = c.Name,
                Мощность    = c.Horsepower,
            })
            .ToList();

        _grid.DataSource = cars;
        if (_grid.Columns.Count > 0)
            _grid.Columns[0].Visible = false;
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is null) return;
        dynamic row = _grid.CurrentRow.DataBoundItem;
        int id = row.Id;

        using var db = new AppDbContext();
        var car = db.Cars.Include(c => c.Brand).FirstOrDefault(c => c.Id == id);
        if (car is null) return;

        _txtName.Text = car.Name;
        _txtHp.Text   = car.Horsepower.ToString();

        foreach (CarBrand item in _cmbBrand.Items)
        {
            if (item.Id == car.BrandId) { _cmbBrand.SelectedItem = item; break; }
        }
    }

    private int? SelectedId()
    {
        if (_grid.CurrentRow?.DataBoundItem is null) return null;
        dynamic row = _grid.CurrentRow.DataBoundItem;
        return row.Id;
    }

    private bool ReadInputs(out int brandId, out string name, out int hp)
    {
        brandId = 0; name = ""; hp = 0;

        if (_cmbBrand.SelectedItem is not CarBrand brand)
        { Msg("Выберите марку."); return false; }

        name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Msg("Введите название модели."); return false; }

        if (!int.TryParse(_txtHp.Text.Trim(), out hp) || hp < 0)
        { Msg("Мощность — целое число ≥ 0."); return false; }

        brandId = brand.Id;
        return true;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ReadInputs(out int brandId, out string name, out int hp)) return;

        using var db = new AppDbContext();
        db.Cars.Add(new Car { BrandId = brandId, Name = name, Horsepower = hp });
        db.SaveChanges();
        ClearInputs();
        LoadGrid();
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id is null) { Msg("Выберите автомобиль."); return; }
        if (!ReadInputs(out int brandId, out string name, out int hp)) return;

        using var db = new AppDbContext();
        var car = db.Cars.Find(id.Value);
        if (car is null) { Msg("Автомобиль не найден."); return; }

        car.BrandId    = brandId;
        car.Name       = name;
        car.Horsepower = hp;
        db.SaveChanges();
        ClearInputs();
        LoadGrid();
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id is null) { Msg("Выберите автомобиль."); return; }

        using var db = new AppDbContext();
        var car = db.Cars.Include(c => c.Brand).FirstOrDefault(c => c.Id == id.Value);
        if (car is null) return;

        if (MessageBox.Show($"Удалить «{car.Brand?.Name} {car.Name}»?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        db.Cars.Remove(car);
        db.SaveChanges();
        ClearInputs();
        LoadGrid();
    }

    private void ClearInputs() { _txtName.Clear(); _txtHp.Clear(); }

    private void Msg(string text) =>
        MessageBox.Show(text, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
