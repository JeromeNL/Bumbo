using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class ChangedClockedHoursViewModel
{
    public List<ClockedHours> HistoryClockedHours { get; set; }

    public int WeekStartDateTime { get; set; }
}