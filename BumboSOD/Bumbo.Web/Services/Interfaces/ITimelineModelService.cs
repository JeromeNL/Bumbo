using Bumbo.Data.Models;
using Bumbo.Web.Models.Timeline;
using Bumbo.WorkingRules.CAORules;

namespace Bumbo.Web.Services.Interfaces;

public interface ITimelineModelService
{
    public string ClockedHourClass { get; set; }
    public string ShiftClass { get; set; }
    TimelineViewModel GenerateTimelineModels(IEnumerable<ApplicationUser> employeesSorted, IDictionary<Shift, List<string>> allShiftConflicts, DateTime startDateTime, DateTime endDateTime);
    Task<IDictionary<Shift, List<string>>> GetAllConflictsPerShift(List<ApplicationUser> allEmployees, IWorkingRules workingRules);
}