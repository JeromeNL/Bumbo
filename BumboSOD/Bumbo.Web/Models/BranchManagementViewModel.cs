using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class BranchManagementViewModel
{
    public IList<Branch>? AllBranches { get; set; }

    public Branch? AddedBranch { get; set; }
    public Branch? EditedBranch { get; set; }
    public Branch? SelectedBranch { get; set; }
    public string? CurrentView { get; set; }

    public IList<DayOfWeek> DaysOfWeek { get; set; }
    public List<OpeningHours> OpeningHoursList { get; set; }
}