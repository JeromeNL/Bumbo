using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class EmployeeAvailabilityViewModel
{
    public List<StandardAvailabilityViewModel> Availabilities { get; set; }

    public double SchoolHours { get; set; }

    public Dictionary<DayOfWeek, double> SchoolHoursForDayOfWeek { get; set; }

    public List<string>? Errors { get; set; }

    public SpecialAvailabilityViewModel SpecialAvailabilityViewModel { get; set; }

    public ApplicationUser UserLoggedIn { get; set; }
}