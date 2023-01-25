using Bumbo.Data.Models;
using Bumbo.WorkingRules.Repositories;
using Xunit;

namespace Bumbo.Tests;

public class CaoTests
{
    private ApplicationUser _employee;
    private CaoDummyRepository _repo;

    public CaoTests()
    {
        _repo = new CaoDummyRepository();
        _employee = _repo.GetFirstUser();
    }

    [Fact]
    public void CalculateShiftDurationInMinutes_ShouldBe240()
    {
        // Arrange
        var inputStart = DateTime.Parse("11/22/2022 07:00:00");
        var inputEnd = DateTime.Parse("11/22/2022 11:00:00");
        var expectedOutput = TimeSpan.FromMinutes(240);

        // Act
        var workingRules = new WorkingRules.CAORules.WorkingRules(new CaoDummyRepository());
        var result = workingRules.GetShiftDuration(inputStart, inputEnd);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void CalculateShiftDurationInMinutes_ShouldBe0()
    {
        // Arrange
        var inputStart = DateTime.Parse("11/22/2022 10:00:00");
        var inputEnd = DateTime.Parse("11/22/2022 10:00:00");
        var expectedOutput = TimeSpan.FromMinutes(0);

        // Act
        var workingRules = new WorkingRules.CAORules.WorkingRules(new CaoDummyRepository());
        var result = workingRules.GetShiftDuration(inputStart, inputEnd);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void EmployeeNotAllowedToWorkEvening()
    {
        // Arrange
        var inputEmployee = _repo.GetFirstUser();
        var expected = $"{inputEmployee.FirstName} mag niet na 19.00 werken.";

        Shift shift = new()
        {
            Id = 1,
            Employee = inputEmployee,
            Department = new Department()
            {
                Id = 1,
                Name = "Kassa"
            },
            Start = DateTime.Parse("11/22/2022 18:00:00"),
            End = DateTime.Parse("11/22/2022 21:00:00"),
            IsIll = false,
            IsPublished = true
        };

        // Act
        var workingRules = new WorkingRules.CAORules.WorkingRules(_repo);
        var result = workingRules.CheckMaxWorkTime(shift, inputEmployee);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CheckAmountOfMinutesBreak_ShouldBe_0_30_or_60()
    {
        Shift localshift = new()
        {
            Id = 1,
            Employee = _employee,
            Department = new Department()
            {
                Id = 1,
                Name = "Kassa"
            },
            Start = DateTime.Parse("11/14/2022 11:00:00"),
            End = DateTime.Parse("11/14/2022 19:30:00"),
            IsIll = false,
            IsPublished = true
        };

        // Act
        var workingRules = new WorkingRules.CAORules.WorkingRules(_repo);
        var result = workingRules.GetBreakMinutesForShift(localshift);

        // Assert
        Assert.Equal(60, result);
    }
}