using ДЗ3.Forms;

namespace ДЗ3.Forms;

/// <summary>Главное окно с навигационными кнопками.</summary>
public class MainForm : Form
{
    public MainForm()
    {
        Text            = "ДЗ3: Автомобили (EF Core + WinForms, вариант 3)";
        Size            = new Size(400, 260);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;

        var lblTitle = new Label
        {
            Text      = "Антонов Леонид, ИУ5-22Б\nВариант 3: Марки и автомобили",
            Location  = new Point(20, 20),
            Size      = new Size(360, 40),
            TextAlign = ContentAlignment.MiddleCenter,
            Font      = new Font(Font.FontFamily, 10, FontStyle.Regular),
        };

        var btnBrands = new Button
        {
            Text     = "Марки автомобилей",
            Location = new Point(80, 80),
            Size     = new Size(220, 36),
        };
        btnBrands.Click += (_, _) => new BrandsForm().ShowDialog(this);

        var btnCars = new Button
        {
            Text     = "Автомобили",
            Location = new Point(80, 125),
            Size     = new Size(220, 36),
        };
        btnCars.Click += (_, _) => new CarsForm().ShowDialog(this);

        var btnReport = new Button
        {
            Text     = "Отчёты",
            Location = new Point(80, 170),
            Size     = new Size(220, 36),
        };
        btnReport.Click += (_, _) => new ReportForm().ShowDialog(this);

        Controls.AddRange([lblTitle, btnBrands, btnCars, btnReport]);
    }
}
