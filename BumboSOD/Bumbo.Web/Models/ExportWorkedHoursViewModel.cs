using Bumbo.Data.Models;
using Bumbo.WorkingRules.CAORules;

namespace Bumbo.Web.Models;

public class ExportWorkedHoursViewModel
{
    public Dictionary<ApplicationUser, WorkingFees> WorkingFeesForEmployees { get; set; }

    public string BranchName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthString { get; set; }
}