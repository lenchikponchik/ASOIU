using Xunit;

/// <summary>
/// Тесты для правил расчёта штрафа.
/// Именование: Метод_Сценарий_ОжидаемыйРезультат
/// </summary>
public class FineRuleTests
{
    // ---- RegularFineRule ----

    [Fact]
    public void Calculate_RegularNoDaysLate_ReturnsZero()
    {
        var rule = new RegularFineRule();
        Assert.Equal(0m, rule.Calculate(300m, 0));
    }

    [Fact]
    public void Calculate_RegularNegativeDays_ReturnsZero()
    {
        var rule = new RegularFineRule();
        Assert.Equal(0m, rule.Calculate(300m, -5));
    }

    [Theory]
    [InlineData(300, 1,  15.0)]  // 300 * 0.05 * 1
    [InlineData(300, 5,  75.0)]  // 300 * 0.05 * 5
    [InlineData(200, 10, 100.0)] // 200 * 0.05 * 10
    public void Calculate_RegularVariousDays_ReturnsCorrectFine(
        decimal price, int days, decimal expected)
    {
        var rule = new RegularFineRule();
        Assert.Equal(expected, rule.Calculate(price, days));
    }

    // ---- StudentFineRule ----

    [Fact]
    public void Calculate_StudentNoDaysLate_ReturnsZero()
    {
        var rule = new StudentFineRule();
        Assert.Equal(0m, rule.Calculate(300m, 0));
    }

    [Theory]
    [InlineData(300, 1,   9.0)]  // 300 * 0.03 * 1
    [InlineData(300, 10,  90.0)] // 300 * 0.03 * 10
    public void Calculate_StudentVariousDays_ReturnsCorrectFine(
        decimal price, int days, decimal expected)
    {
        var rule = new StudentFineRule();
        Assert.Equal(expected, rule.Calculate(price, days));
    }

    // ---- SeniorFineRule ----

    [Theory]
    [InlineData(300, 0,  0.0)]
    [InlineData(300, 10, 30.0)] // 300 * 0.01 * 10
    public void Calculate_SeniorVariousDays_ReturnsCorrectFine(
        decimal price, int days, decimal expected)
    {
        var rule = new SeniorFineRule();
        Assert.Equal(expected, rule.Calculate(price, days));
    }

    // ---- StaffFineRule ----

    [Theory]
    [InlineData(300,  0)]
    [InlineData(300, 100)]
    public void Calculate_StaffAnyDays_AlwaysZero(decimal price, int days)
    {
        var rule = new StaffFineRule();
        Assert.Equal(0m, rule.Calculate(price, days));
    }

    // ---- FamilyFineRule ----

    [Fact]
    public void Calculate_FamilyWithinGracePeriod_ReturnsZero()
    {
        var rule = new FamilyFineRule();
        Assert.Equal(0m, rule.Calculate(400m, 5)); // точно на границе
    }

    [Fact]
    public void Calculate_FamilyJustAfterGracePeriod_ReturnsSmallFine()
    {
        var rule = new FamilyFineRule();
        // 6 дней просрочки — 1 день после льготного периода
        // 400 * 0.02 * 1 = 8 руб.
        Assert.Equal(8m, rule.Calculate(400m, 6));
    }

    [Fact]
    public void Calculate_FamilyVeryLate_CappedAt100()
    {
        var rule = new FamilyFineRule();
        // 1000 * 0.02 * 100 = 2000 руб. → ограничено 100 руб.
        Assert.Equal(100m, rule.Calculate(1000m, 200));
    }

    [Fact]
    public void Calculate_FamilyNegativeDays_ReturnsZero()
    {
        var rule = new FamilyFineRule();
        Assert.Equal(0m, rule.Calculate(400m, -3));
    }
}
