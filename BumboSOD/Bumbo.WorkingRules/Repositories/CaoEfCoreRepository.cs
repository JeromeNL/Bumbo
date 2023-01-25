using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.WorkingRules.Repositories;

public class CaoEfCoreRepository : ICaoRepository
{
    private readonly BumboDbContext _context;

    public CaoEfCoreRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<decimal?> GetSpecialSchoolHoursForDay(DateTime time, string employeeId)
    {
        var schoolDay = await _context.SpecialSchoolHours.FirstOrDefaultAsync(e => e.Start.Date == time.Date && e.EmployeeId == employeeId);
        return schoolDay?.Hours;
    }

    public async Task<decimal?> GetDefaultSchoolHoursForDay(DateTime time, string employeeId)
    {
        var schoolDay = await _context.StandardSchoolHours
            .FirstOrDefaultAsync(e => e.DayOfWeek == time.DayOfWeek && e.EmployeeId == employeeId);
        return schoolDay?.Hours;
    }

    public async Task<List<SpecialSchoolHours>> GetSpecialSchoolHoursForPeriod(DateTime startDate, DateTime endDate, int branchId)
    {
        var userSchoolHours = await _context.SpecialSchoolHours
            .Where(e => e.Employee.Branch.Id == branchId && e.Start.Date >= startDate.Date && e.Start.Date <= endDate.Date)
            .ToListAsync();
        return userSchoolHours;
    }

    public async Task<List<StandardSchoolHours>> GetDefaultSchoolHoursForPeriod(int branchId)
    {
        var userSchoolHours = await _context.StandardSchoolHours
            .Where(e => e.Employee.Branch.Id == branchId)
            .ToListAsync();
        return userSchoolHours;
    }

    public async Task<List<Shift>> GetShiftsForDay(DateTime time, string employeeId)
    {
        return await _context.Shifts
            .Where(e => e.Start.Date == time.Date && e.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<List<Shift>> GetAllShiftsWithinPeriod(DateTime startDate, DateTime endDate, int branchId)
    {
        return await _context.Shifts.Where(e => e.Start.Date >= startDate.Date && e.End.Date <= endDate.Date && e.Employee.BranchId == branchId)
            .Include(e => e.Employee)
            .ToListAsync();
    }

    public async Task<SpecialAvailability?> GetSpecialAvailabilityForDay(DateTime time, string employeeId)
    {
        return await _context.SpecialAvailabilities
            .FirstOrDefaultAsync(e => e.Start.Date == time.Date && e.EmployeeId == employeeId);
    }

    public async Task<StandardAvailability?> GetDefaultAvailabilityForDay(DateTime time, string employeeId)
    {
        var userAvailabilities = await _context.StandardAvailabilities
            .Where(e => e.EmployeeId == employeeId)
            .ToListAsync();
        return userAvailabilities.FirstOrDefault(e => e.Start.DayOfWeek == time.DayOfWeek);
    }

    public async Task<List<StandardAvailability>> GetDefaultAvailabilityForPeriod(int branchId)
    {
        var userAvailabilities = await _context.StandardAvailabilities
            .Where(e => e.Employee.Branch.Id == branchId)
            .ToListAsync();
        return userAvailabilities;
    }

    public async Task<List<ClockedHours>> GetAllWorkedHoursWithinPeriod(DateTime startTime, DateTime endTime, Branch branch)
    {
        return await _context.ClockedHours
            .IncludeOptimized(e => e.Employee)
            .Where(e => e.Employee!.Branch == branch && e.ClockedIn.Date >= startTime && e.ClockedOut <= endTime)
            .ToListAsync();
    }

    public async Task<List<SpecialAvailability>> GetSpecialAvailabilityForPeriod(DateTime startDate, DateTime endDate, int branchId)
    {
        var userAvailabilities = await _context.SpecialAvailabilities
            .Where(e => e.Employee.Branch.Id == branchId && e.Start.Date >= startDate.Date && e.End.Date <= endDate.Date)
            .ToListAsync();
        return userAvailabilities;
    }
}