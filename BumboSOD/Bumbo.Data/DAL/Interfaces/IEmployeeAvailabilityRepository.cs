using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IEmployeeAvailabilityRepository
{
    public Task<IList<StandardAvailability>> GetStandardAvailabilities(string employeeId);

    public Task UpdateStandardAvailability(List<StandardAvailability> standardAvailabilities);
    public Task<bool> RemoveStandardAvailability(DayOfWeek dayOfWeek, string employeeId);

    public Task SetStandardSchoolHours(string employeeId, Dictionary<DayOfWeek, double> standardSchoolHoursPerDayOfWeek);
    public Task<double> GetStandardSchoolHoursForDayOfWeek(string employeeId, DayOfWeek dayOfWeek);

    public Task SetSpecialSchoolHours(string employeeId, SpecialSchoolHours specialSchoolHours);

    public Task<IList<SpecialSchoolHours>> GetSpecialSchoolHours(string employeeId);
    public Task RemoveSpecialSchoolHour(int? id);
    public Task<StandardAvailability> GetStandardAvailabilityForDayOfWeek(string employeeId, DayOfWeek dayOfWeek);

    public Task<IList<SpecialAvailability>> GetSpecialAvailabilities(string employeeId);
    public Task SetSpecialAvailability(string employeeId, SpecialAvailability specialAvailability);
    public Task RemoveSpecialAvailability(int? id);
}