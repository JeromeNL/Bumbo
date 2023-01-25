using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class EmployeeWorkedHoursRepository : IEmployeeWorkedHoursRepository
{
    private readonly BumboDbContext _context;

    public EmployeeWorkedHoursRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<IList<int>> GetWorkedYearsOfEmployee(ApplicationUser employee)
    {
        return await _context.ClockedHours
            .Where(clockedHours => clockedHours.Employee == employee)
            .Select(x => x.ClockedIn.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .ToListAsync();
    }

    public async Task<IList<int>> GetWorkedMonthOfYear(ApplicationUser employee, int year)
    {
        return await _context.ClockedHours
            .Where(clockedHours => clockedHours.ClockedIn.Year == year
                                   && clockedHours.Employee == employee)
            .Select(clockedHours => clockedHours.ClockedIn.Month)
            .Distinct()
            .OrderByDescending(clockedMonth => clockedMonth)
            .ToListAsync();
    }

    public async Task<IList<ClockedHours>> GetClockedHoursYearlyAndMonthly(ApplicationUser employee, int year, int month)
    {
        return await _context.ClockedHours
            .Where(x => x.ClockedIn.Year == year
                        && x.ClockedIn.Month == month
                        && x.Employee == employee)
            .OrderByDescending(x => x.ClockedIn)
            .ThenByDescending(x => x.ClockedOut)
            .ToListAsync();
    }

    public IList<TimeSpan> GetHoursTimeSpanOfEmployee(ApplicationUser employee, IEnumerable<ClockedHours> clockedHours)
    {
        return clockedHours
            .Where(hours => hours.Employee == employee)
            .Select(clocked => (clocked.ClockedOut - clocked.ClockedIn)!.Value)
            .OrderByDescending(clockedTimeSpan => clockedTimeSpan.Ticks)
            .ToList();
    }
}