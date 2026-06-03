using Moq;
using Xunit;

/// <summary>
/// Тесты для BonusCalculator.
/// Демонстрирует ручные заглушки (Stub/Dummy) и вариант с Moq.
/// </summary>
public class BonusCalculatorTests
{
    // ---- Ручные заглушки ----

    private class StubRepoFound : IEmployeeRepository
    {
        public Employee? GetById(int id) =>
            new Employee(id, "Тест", 600_000m);
    }

    private class StubRepoNotFound : IEmployeeRepository
    {
        public Employee? GetById(int id) => null;
    }

    private class FixedClock(DateTime date) : IClock
    {
        public DateTime Now => date;
    }

    private class DummyLogger : ILogger
    {
        public void Log(string message) { }
    }

    // ---- Базовая ставка (не новогодний период) ----

    [Fact]
    public void Calculate_BaseRate_ReturnsFivePercent()
    {
        var calc = new BonusCalculator(
            new StubRepoFound(),
            new FixedClock(new DateTime(2025, 6, 1)), // не НГ
            new DummyLogger());

        decimal? bonus = calc.Calculate(1);

        Assert.NotNull(bonus);
        Assert.Equal(30_000m, bonus); // 600_000 * 0.05
    }

    // ---- Новогодний период (1–14 января) ----

    [Theory]
    [InlineData(2025, 1,  1)]
    [InlineData(2025, 1, 14)]
    public void Calculate_NewYearPeriod_ReturnsTenPercent(int y, int m, int d)
    {
        var calc = new BonusCalculator(
            new StubRepoFound(),
            new FixedClock(new DateTime(y, m, d)),
            new DummyLogger());

        decimal? bonus = calc.Calculate(1);

        Assert.NotNull(bonus);
        Assert.Equal(60_000m, bonus); // 600_000 * 0.10
    }

    [Fact]
    public void Calculate_JanFifteenth_NotNewYear()
    {
        var calc = new BonusCalculator(
            new StubRepoFound(),
            new FixedClock(new DateTime(2025, 1, 15)),
            new DummyLogger());

        decimal? bonus = calc.Calculate(1);

        Assert.Equal(30_000m, bonus);
    }

    // ---- Сотрудник не найден ----

    [Fact]
    public void Calculate_EmployeeNotFound_ReturnsNull()
    {
        var calc = new BonusCalculator(
            new StubRepoNotFound(),
            new FixedClock(new DateTime(2025, 6, 1)),
            new DummyLogger());

        Assert.Null(calc.Calculate(999));
    }

    // ---- Moq: проверяем, что логгер вызывается ----

    [Fact]
    public void Calculate_EmployeeFound_LogsOnce()
    {
        var repoMock   = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger>();
        var clockMock  = new Mock<IClock>();

        repoMock.Setup(r => r.GetById(1))
                .Returns(new Employee(1, "Иванов", 500_000m));
        clockMock.Setup(c => c.Now)
                 .Returns(new DateTime(2025, 6, 1));

        var calc = new BonusCalculator(repoMock.Object, clockMock.Object, loggerMock.Object);
        calc.Calculate(1);

        // Убеждаемся, что Log был вызван ровно один раз
        loggerMock.Verify(l => l.Log(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Calculate_EmployeeNotFound_LogsOnce()
    {
        var repoMock   = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger>();
        var clockMock  = new Mock<IClock>();

        repoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Employee?)null);
        clockMock.Setup(c => c.Now).Returns(new DateTime(2025, 6, 1));

        var calc = new BonusCalculator(repoMock.Object, clockMock.Object, loggerMock.Object);
        calc.Calculate(42);

        loggerMock.Verify(l => l.Log(It.IsAny<string>()), Times.Once);
    }
}
