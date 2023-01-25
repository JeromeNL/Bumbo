using Bumbo.Data.Models;
using Bumbo.WorkingRules.CAORules;
using Bumbo.WorkingRules.Repositories;
using Xunit;

namespace Bumbo.Tests;

public class WorkingFeesTests
{
    private readonly ApplicationUser _employee;
    private readonly CaoDummyRepository _repo;

    public WorkingFeesTests()
    {
        _repo = new CaoDummyRepository();
        _employee = _repo.GetFirstUser();
    }

    [Fact]
    public void TodayIsHoliday_ShouldBe_True()
    {
        // Arrange
        var inputDate = DateTime.Parse("2023/04/09");
        var expectedOutput = true;

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.TodayIsHoliday(inputDate);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void TodayIsHoliday_ShouldBe_False()
    {
        // Arrange
        var inputDate = DateTime.Parse("2025/12/24");
        const bool expectedOutput = false;

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.TodayIsHoliday(inputDate);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateHolidayHours_ShouldBe_5()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2021/12/25 10:00:00"),
            ClockedOut = DateTime.Parse("2021/12/25 15:00:00")
        };
        var expectedOutput = TimeSpan.FromHours(5);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateHolidayAllowance(localWorkedHours);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateSundayHours_ShouldBe_10()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2022/11/27 10:00:00"),
            ClockedOut = DateTime.Parse("2022/11/27 20:00:00")
        };
        var expectedOutput = TimeSpan.FromHours(10);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateSundayAllowance(localWorkedHours);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateSundayHours_ShouldBe_0()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2022/11/24 10:00:00"),
            ClockedOut = DateTime.Parse("2022/11/24 15:00:00")
        };
        var expectedOutput = TimeSpan.FromHours(0);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateSundayAllowance(localWorkedHours);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateSaturday18Till24Allowance_ShouldBe_4()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2022/12/03 19:00:00"),
            ClockedOut = DateTime.Parse("2022/12/03 23:00:00")
        };
        var expectedOutput = TimeSpan.FromHours(4);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateSaturday18Till24Allowance(localWorkedHours);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateNightAllowance_ShouldBe_2AndHalf()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2022/12/05 05:00:00"),
            ClockedOut = DateTime.Parse("2022/12/05 22:30:00")
        };
        var expectedOutput = TimeSpan.FromHours(2.5);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateNightAllowance(localWorkedHours);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateBetween20And21Allowance_ShouldBe_halfhour()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2022/12/05 18:00:00"),
            ClockedOut = DateTime.Parse("2022/12/05 20:30:00")
        };
        var expectedOutput = TimeSpan.FromHours(0.5);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateTimeBetween20And21Allowance(localWorkedHours);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateTimeDefault_ShouldBe_6Hours()
    {
        // Arrange
        var localWorkedHours = new ClockedHours
        {
            Id = 1,
            Employee = _employee,
            ClockedIn = DateTime.Parse("2022/11/21 14:00:00"),
            ClockedOut = DateTime.Parse("2022/11/21 22:00:00")
        };
        var expectedOutput = TimeSpan.FromHours(6);

        // Act
        var calculator = new WorkingFeesCalculator(_repo);
        var result = calculator.CalculateTimeDefault(localWorkedHours, TimeSpan.FromHours(2));

        // Assert
        Assert.Equal(expectedOutput, result);
    }
}