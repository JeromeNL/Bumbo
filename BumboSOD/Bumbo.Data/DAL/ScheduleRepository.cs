using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.Data.DAL;

public class ScheduleRepository : IScheduleRepository
{
    private readonly BumboDbContext _context;
    private readonly IUserService _userService;

    public ScheduleRepository(BumboDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<List<ApplicationUser>> GetAllEmployeesBetween(DateTime start, DateTime end)
    {
        var user = await _userService.GetUser();
        var branchId = user.BranchId;
        return await _context.Users.AsNoTracking()
            .Where(o => o.BranchId == branchId)
            .Include(o => o.Shifts!
                .Where(o => o.Start >= start && o.End <= end))
            .Include(o => o.ClockedHours!
                .Where(o => o.ClockedIn >= start && o.ClockedOut <= end))
            .Include(o => o.SpecialAvailabilities!
                .Where(o => o.Start >= start && o.End <= end))
            .Include(o => o.StandardAvailabilities!)
            .Include(o => o.Departments)
            .ThenInclude(o => o.Department)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<List<Department>> GetAllDepartments()
    {
        return await _context.Departments.AsNoTracking().ToListAsync();
    }

    public async Task UpdateClockedHours(List<ClockedHours> clockedHours)
    {
        foreach (var clockedHour in clockedHours)
        {
            var dbClockedHour = await _context.ClockedHours.FindAsync(clockedHour.Id);
            if (dbClockedHour == null) continue;
            dbClockedHour.ClockedIn = clockedHour.ClockedIn;
            dbClockedHour.ClockedOut = clockedHour.ClockedOut;
            dbClockedHour.EmployeeId = clockedHour.EmployeeId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmployeeShifts(List<Shift> shifts)
    {
        foreach (var shift in shifts)
        {
            var dbShift = await _context.Shifts.FindAsync(shift.Id);
            if (dbShift == null) continue;
            dbShift.Start = shift.Start;
            dbShift.End = shift.End;
            dbShift.IsIll = shift.IsIll;
            dbShift.DepartmentId = shift.DepartmentId;
            dbShift.EmployeeId = shift.EmployeeId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddClockedHours(List<ClockedHours> clockedHours)
    {
        await _context.ClockedHours.AddRangeAsync(clockedHours);
        await _context.SaveChangesAsync();
    }

    public async Task AddEmployeeShifts(IEnumerable<Shift> shifts)
    {
        await _context.Shifts.AddRangeAsync(shifts);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClockedHours(List<int> clockedHoursIds)
    {
        await _context.ClockedHours.Where(o => clockedHoursIds.Contains(o.Id)).DeleteAsync();
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEmployeeShifts(List<int> shiftIds)
    {
        await _context.Shifts.Where(o => shiftIds.Contains(o.Id)).DeleteAsync();
        await _context.SaveChangesAsync();
    }

    public async Task<List<Shift>> GetAllShiftsBetween(DateTime startDateTime, DateTime endDateTime)
    {
        var user = await _userService.GetUser();
        var branchId = user.BranchId;

        return await _context.Shifts
            .IncludeOptimized(s => s.Employee)
            .IncludeOptimized(s => s.Department)
            .Where(o => o.Employee.BranchId == branchId && o.Start > startDateTime && o.End < endDateTime)
            .ToListAsync();
    }

    public async Task<List<ClockedHours>> GetChangedClockedHours(DateTime startTime, DateTime endTime)
    {
        var changedClockedHours = await _context.ClockedHours.TemporalContainedIn(DateTime.MinValue, DateTime.Now)
            .OrderByDescending(clockedHours => clockedHours.ClockedIn.Date)
            .Where(ch => ch.ClockedIn > startTime && ch.ClockedOut < endTime)
            .ToListAsync();

        return changedClockedHours;
    }

    /// <summary>
    /// Copies the schedule of the selected week to the next week
    /// </summary>
    /// <param name="shifts">A list of the shifts of the week for whose shifts should be copied.</param>
    /// <param name="daysBetween">Amount of days which should be added to the dates of the given shifts</param>
    public async Task CopySchedule(List<Shift> shifts, int daysBetween)
    {
        foreach (var shift in shifts)
        {
            shift.Id = 0;
            shift.Start = shift.Start.AddDays(daysBetween);
            shift.End = shift.End.AddDays(daysBetween);
            shift.IsPublished = false;
            _context.Add(shift);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<ClockedHours>> GetClockedHoursBetween(DateTime start, DateTime end)
    {
        var user = await _userService.GetUser();
        var branchId = user.BranchId;

        return await _context.ClockedHours
            .Include(s => s.Employee)
            .Where(o => o.Employee.BranchId == branchId && o.ClockedIn > start && o.ClockedOut < end)
            .ToListAsync();
    }

    public async Task ApproveClockedHours(List<ClockedHours> clockedHours)
    {
        foreach (var clockedHour in clockedHours)
        {
            var dbClockedHour = await _context.ClockedHours.Where(x => x.Id == clockedHour.Id).FirstOrDefaultAsync();
            if (dbClockedHour == null) continue;
            dbClockedHour.IsApproved = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task PublishSchedule(List<Shift> shifts)
    {
        foreach (var shift in shifts)
        {
            var dbShift = await _context.Shifts.Where(x => x.Id == shift.Id).FirstOrDefaultAsync();
            if (dbShift == null) continue;
            dbShift.IsPublished = true;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// For given days, returns an overview of the departments with the sum of all it's scheduled man hours
    /// </summary>
    /// <param name="days"></param>
    /// <param name="branchId"></param>
    /// <returns></returns>
    public async Task<IDictionary<DateTime, IDictionary<Department, decimal>>> GetPlannedHoursForEachDepartmentDuringDateRange(IEnumerable<DateTime> days, int branchId)
    {
        var output = new Dictionary<DateTime, IDictionary<Department, decimal>>();

        foreach (var date in days)
        {
            output.Add(date, await GetScheduledHoursForAllDepartments(date, branchId));
        }

        return output;
    }

    /// <summary>
    /// For every hour in the input days, calculate how many employees are at each department
    /// </summary>
    /// <param name="days"></param>
    /// <param name="branchId"></param>
    /// <param name="filterUser">Function to filter users</param>
    /// <returns>
    /// IDictionary with every hour as key, and a dictionary with with each department and it's man hours as value
    /// </returns>
    public async Task<IDictionary<DateTime, double>> EmployeeCountForEveryHour(IEnumerable<DateTime> days, int branchId, Func<ApplicationUser, bool> filterUser)
    {
        // TODO: Make less complex/split up
        // Method works as follows
        // Loop through every hour of every day
        //      loop through every shift for this day
        //          if the current hour falls in this shift,
        //              add one to the amount of employees of that hour

        var listOfDays = days.ToList();
        var firstDay = listOfDays.MinBy(x => x);
        var endDay = listOfDays.MaxBy(x => x).AddDays(1);

        var shifts = await _context.Shifts
            .IncludeOptimized(s => s.Employee)
            .Where(s => s.Start.CompareTo(firstDay) > 0 && s.End.CompareTo(endDay) < 0)
            .Where(s => s.Employee.BranchId == branchId).ToListAsync();

        var output = new Dictionary<DateTime, double>();

        foreach (var day in listOfDays)
        {
            var nextDay = day.AddDays(1).Date;
            var currentHourDay = new DateTime(day.Year, day.Month, day.Day, 4, 0, 0);

            // Loops through all hours in a day
            while (currentHourDay.AddHours(1).Date != nextDay)
            {
                double amountOfEmployees = 0;

                // Find all the shifts that overlap the current hour
                foreach (var shift in shifts.Where(shift => shift.Start.Date == currentHourDay.Date))
                {
                    // Check if the current hour falls in this shift

                    // Skip if the hour does not fall within this shift
                    if (currentHourDay.Hour.CompareTo(shift.Start.Hour) < 0 ||
                        currentHourDay.Hour.CompareTo(shift.End.Hour) >= 0) continue;

                    if (!filterUser(shift.Employee)) continue;

                    // Add one employee to the this hours employee count
                    amountOfEmployees++;
                }

                // Add this day, with it's employee count to the output
                output.Add(currentHourDay, amountOfEmployees);

                // Go to next hour
                currentHourDay = currentHourDay.AddHours(1);
            }
        }

        return output;
    }

    /// <summary>
    /// For given day, sums all shift lengths for each department
    /// </summary>
    /// <param name="inputDate"></param>
    /// <param name="branchId"></param>
    /// <returns>Department with it's total man hours</returns>
    private async Task<IDictionary<Department, decimal>> GetScheduledHoursForAllDepartments(DateTime inputDate, int branchId)
    {
        // Method works as follows:
        // - Get all shifts for input day
        // - Loops over the shift
        // - Calculates how long the shift takes
        // - Adds the hours to the existing hours in the dictionary

        Dictionary<Department, decimal> output = new();

        // get all shifts for this branch
        var shiftsForBranch = await _context.Shifts
            .IncludeOptimized(s => s.Employee)
            .IncludeOptimized(s => s.Department)
            .Where(s => s.Employee.BranchId == branchId && s.Start.Date.Equals(inputDate.Date)).ToListAsync();

        // Foreach shift, sum all planned hours
        foreach (var shift in shiftsForBranch)
        {
            var departmentAlreadyAdded = output.TryGetValue(shift.Department, out var value);

            // Find the shift length
            var shiftLengthInHours = value + (decimal)(shift.End - shift.Start).TotalHours;

            // Add the shift length to the output dictionary
            if (departmentAlreadyAdded)
            {
                output[shift.Department] = shiftLengthInHours;
                continue;
            }

            output.Add(shift.Department, shiftLengthInHours);
        }

        return output;
    }
}