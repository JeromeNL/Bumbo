using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class ClockedHoursViewModel
{
    public IList<TimeSpan>? WorkedHours { get; set; }
    public int WorkedYear { get; set; }
    public string WorkedMonth { get; set; }
    public IList<ClockedHours> ClockedHoursPerMonth { get; set; }
    public decimal? TotalMonthlyHours { get; set; }
}