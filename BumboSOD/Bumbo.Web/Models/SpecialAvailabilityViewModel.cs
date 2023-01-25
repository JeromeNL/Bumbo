using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class SpecialAvailabilityViewModel
{
    public IList<SpecialAvailability> SpecialAvailabilities { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime DateNow { get; set; }
    public double SchoolHours { get; set; }
    public DateTime Date { get; set; }
    public bool IsWholeDayAvailable { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SpecialAvailability SpecialAvailability { get; set; }
    public StandardAvailability StandardAvailability { get; set; }
    public IList<SpecialSchoolHours> SpecialSchoolHoursList { get; set; }

    public ApplicationUser UserLoggedIn { get; set; }
}