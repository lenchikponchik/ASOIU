using ДЗ3.Data;
using ДЗ3.Forms;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

using var db = new AppDbContext();
DbSeeder.Seed(db);

Application.Run(new MainForm());
