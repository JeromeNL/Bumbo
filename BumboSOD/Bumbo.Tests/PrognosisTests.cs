using Bumbo.Prognosis;
using Bumbo.Prognosis.Repositories;
using Xunit;

namespace Bumbo.Tests;

public class PrognosisTests
{
    /// <summary>
    /// When the input day is within five days of a holiday, return the day last year with the same relative offset
    /// </summary>
    [Fact]
    public void CalculateGivenDay_ThreeDaysBeforeChristmas_ReturnsThreeDaysBeforeChristmasLastYear()
    {
        var expected = DateTime.Parse("2021-12-22");
        var input = DateTime.Parse("2022-12-22");

        IPrognosisRepository repo = new PrognosisDummyRepository();
        var branch = repo.BranchWithWorkStandard(1)!;

        var daysFinder = new DaysFinder(repo);

        var output = daysFinder.GetDaysForPrognosisCalculation(branch.Id, input);

        Assert.Equal(output.TodayLastYearRelativeToHoliday, expected);
    }

    /// <summary>
    /// When the input is a on a holiday, it should return the holiday last year
    /// </summary>
    [Fact]
    public void CalculateGivenDay_InputExactlyOnEaster2025_ReturnsEaster2024()
    {
        var expected = DateTime.Parse("2024-03-31");
        var input = DateTime.Parse("2025-04-20");

        IPrognosisRepository repo = new PrognosisDummyRepository();
        var branch = repo.BranchWithWorkStandard(1)!;

        var daysFinder = new DaysFinder(repo);

        var output = daysFinder.GetDaysForPrognosisCalculation(branch.Id, input);

        Assert.NotNull(output);

        Assert.Equal(output.TodayLastYearRelativeToHoliday, expected);
    }

    /// <summary>
    /// When no holiday is within five days, return the exact same date last year
    /// </summary>
    [Fact]
    public void CalculateGivenDay_NoHolidayWithinFiveDays_ReturnsSameDayLastYear()
    {
        var expected = DateTime.Parse("2024-01-01");
        var input = DateTime.Parse("2025-01-01");

        IPrognosisRepository repo = new PrognosisDummyRepository();
        var daysFinder = new DaysFinder(repo);
        var branch = repo.BranchWithWorkStandard(1)!;
        var output = daysFinder.GetDaysForPrognosisCalculation(branch.Id, input);

        Assert.NotNull(output);

        Assert.Equal(output.TodayLastYearRelativeToHoliday, expected);
    }

    /// <summary>
    /// Checks if the calculations for the prognosis compare to the expected manually calculated values
    /// Checks if the calculation gives the expected output on 2022-04-11
    /// NOTE: Makes use of seed data from the database
    /// </summary>
    [Fact]
    public void CalculateManHoursForEntireBranchForGivenDay_returns_CorrectResult()
    {
        var inputDate = DateTime.Parse("2022-11-15");
        IPrognosisRepository repo = new PrognosisDummyRepository();
        var branch = repo.BranchWithWorkStandard(1)!;

        var prognosisCalculator = new PrognosisCalculator(repo);

        var expectedKassa = 28.6m;
        var expectedVers = 8.6m;
        var expectedVakkenVullen = 29.3m;

        var output = prognosisCalculator.CalculateManHoursForEntireBranchForGivenDay(inputDate, branch.Id);

        var departments = repo.GetAllDepartments();

        output.TryGetValue(departments[0], out var kassa);
        output.TryGetValue(departments[1], out var vers);
        output.TryGetValue(departments[2], out var vakkenvullen);

        Assert.NotNull(kassa);
        Assert.NotNull(vers);
        Assert.NotNull(vakkenvullen);

        Assert.Equal(expectedKassa, decimal.Round(kassa.ManHoursExpected, 1));
        Assert.Equal(expectedVers, decimal.Round(vers.ManHoursExpected, 1));
        Assert.Equal(expectedVakkenVullen, decimal.Round(vakkenvullen.ManHoursExpected, 1));
    }

    [Fact]
    public void CalculateManHoursForEntireBranchForGivenDayWithoutOneWeek_returns_CorrectResult()
    {
        var inputDate = DateTime.Parse("2022-08-1");
        IPrognosisRepository repo = new PrognosisDummyRepository();
        var branch = repo.BranchWithWorkStandard(1)!;

        var prognosisCalculator = new PrognosisCalculator(repo);

        var expectedKassa = 28.3m;
        var expectedVers = 8.5m;
        var expectedVakkenVullen = 30.4m;

        var output = prognosisCalculator.CalculateManHoursForEntireBranchForGivenDay(inputDate, branch.Id);

        var departments = repo.GetAllDepartments();

        output.TryGetValue(departments[0], out var kassa);
        output.TryGetValue(departments[1], out var vers);
        output.TryGetValue(departments[2], out var vakkenvullen);

        Assert.NotNull(kassa);
        Assert.NotNull(vers);
        Assert.NotNull(vakkenvullen);

        Assert.Equal(expectedKassa, decimal.Round(kassa.ManHoursExpected, 1));
        Assert.Equal(expectedVers, decimal.Round(vers.ManHoursExpected, 1));
        Assert.Equal(expectedVakkenVullen, decimal.Round(vakkenvullen.ManHoursExpected, 1));
    }
}