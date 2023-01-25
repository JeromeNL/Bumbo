using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IScheduleRepository
{
    Task<List<ApplicationUser>> GetAllEmployeesBetween(DateTime start, DateTime end);

    Task<List<Department>> GetAllDepartments();

    Task UpdateClockedHours(List<ClockedHours> clockedHours);

    Task UpdateEmployeeShifts(List<Shift> shifts);

    Task AddClockedHours(List<ClockedHours> clockedHours);

    Task AddEmployeeShifts(IEnumerable<Shift> shifts);

    Task DeleteClockedHours(List<int> clockedHours);

    Task DeleteEmployeeShifts(List<int> shiftIds);

    Task<List<Shift>> GetAllShiftsBetween(DateTime startDateTime, DateTime endDateTime);

    Task CopySchedule(List<Shift> shifts, int daysBetween);
    Task<List<ClockedHours>> GetClockedHoursBetween(DateTime start, DateTime end);
    Task ApproveClockedHours(List<ClockedHours> clockedHours);
    Task PublishSchedule(List<Shift> shifts);

    Task<List<ClockedHours>> GetChangedClockedHours(DateTime start, DateTime endTime);

    /// <summary>
    /// For input dates, calculates the total man hours for each department
    /// </summary>
    /// <param name="days"></param>
    /// <param name="branchId"></param>
    /// <returns>Dictionary with date as key and department with it's man hours</returns>
    Task<IDictionary<DateTime, IDictionary<Department, decimal>>> GetPlannedHoursForEachDepartmentDuringDateRange(IEnumerable<DateTime> days, int branchId);

    /// <summary>
    /// For every hour in the input days, calculate how many employees are at each department
    /// </summary>
    /// <param name="days"></param>
    /// <param name="branchId"></param>
    /// <param name="filterUser"></param>
    /// <returns>
    /// IDictionary with every hour as key, and a dictionary with with each department and it's man hours as value
    /// </returns>
    public Task<IDictionary<DateTime, double>> EmployeeCountForEveryHour(IEnumerable<DateTime> days, int branchId, Func<ApplicationUser, bool> filterUser);
}