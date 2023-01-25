using Bumbo.Data.Models;

namespace Bumbo.WorkingRules.CAORules;

public interface IWorkingFeesCalculator
{
    Task<Dictionary<ApplicationUser, WorkingFees>> GetAllWorkedHoursWithAllowancesForPeriod(DateTime startTime, DateTime endTime, Branch branch);

    WorkingFees GetAllowancesForWorkedShift(ClockedHours workedHours);

    Task<TimeSpan> GetDurationOfSickAllowanceForPeriod(ApplicationUser employee, DateTime startTime, DateTime endTime);

    TimeSpan CalculateNightAllowance(ClockedHours workedHours);

    TimeSpan CalculateSaturday18Till24Allowance(ClockedHours workedHours);

    TimeSpan CalculateSundayAllowance(ClockedHours workedHours);

    TimeSpan CalculateHolidayAllowance(ClockedHours workedHours);

    bool TodayIsHoliday(DateTime date);

    TimeSpan GetShiftDuration(DateTime startShift, DateTime endShift);
}