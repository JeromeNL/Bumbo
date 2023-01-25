using Bumbo.Data.Models;
using Bumbo.Web.Models.Timeline;

namespace Bumbo.Web.Models;

public class ScheduleViewModel
{
    public TimelineViewModel TimelineViewModel { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }

    public List<Department> DepartmentOptions { get; set; }

    public string? DepartmentFilter { get; set; }

    public Dictionary<string, List<int>> Sortings { get; set; }

    public TimelineStep Step { get; set; } = TimelineStep.Day;

    public IDictionary<string, IDictionary<DateTime, double>> EmployeeAtEveryHour { get; set; }

    public IDictionary<Shift, List<string>> ConflictsPerShift { get; set; }

    public TimelinePrognosisViewModel TimelinePrognosis { get; set; }

    public bool ChangesMade { get; set; }
}