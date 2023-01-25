using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IEmployeeWorkedHoursRepository
{
    public Task<IList<int>> GetWorkedYearsOfEmployee(ApplicationUser employee);

    public Task<IList<int>> GetWorkedMonthOfYear(ApplicationUser employee, int year);

    public IList<TimeSpan> GetHoursTimeSpanOfEmployee(ApplicationUser employee, IEnumerable<ClockedHours> clockedHours);

    public Task<IList<ClockedHours>> GetClockedHoursYearlyAndMonthly(ApplicationUser employee, int years, int months);
}