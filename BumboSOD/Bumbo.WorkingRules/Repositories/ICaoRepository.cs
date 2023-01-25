using Bumbo.Data.Models;

namespace Bumbo.WorkingRules.Repositories;

public interface ICaoRepository
{
    Task<decimal?> GetSpecialSchoolHoursForDay(DateTime time, string employeeId);
    Task<decimal?> GetDefaultSchoolHoursForDay(DateTime time, string employeeId);

    Task<List<SpecialSchoolHours>> GetSpecialSchoolHoursForPeriod(DateTime startDate, DateTime endDate, int branchId);
    Task<List<StandardSchoolHours>> GetDefaultSchoolHoursForPeriod(int branchId);

    Task<List<Shift>> GetShiftsForDay(DateTime time, string employeeId);
    Task<List<Shift>> GetAllShiftsWithinPeriod(DateTime startDate, DateTime endDate, int branchId);

    Task<SpecialAvailability?> GetSpecialAvailabilityForDay(DateTime time, string employeeId);
    Task<List<SpecialAvailability>> GetSpecialAvailabilityForPeriod(DateTime startDate, DateTime endDate, int branchId);
    Task<StandardAvailability?> GetDefaultAvailabilityForDay(DateTime time, string employeeId);
    Task<List<StandardAvailability>> GetDefaultAvailabilityForPeriod(int branchId);

    Task<List<ClockedHours>> GetAllWorkedHoursWithinPeriod(DateTime startTime, DateTime endTime, Branch branch);
}