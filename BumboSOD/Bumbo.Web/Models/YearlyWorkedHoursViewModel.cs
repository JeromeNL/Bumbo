using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class YearlyWorkedHoursViewModel
{
    public ApplicationUser? Employee { get; set; }

    public List<ClockedHoursViewModel> ClockedHoursViewModel = new();
}