using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class HistoryViewModel
{
    // Properties for history partial view
    public int Id { get; set; }

    public string ActionString { get; set; } = "default";

    public string StartDate { get; set; } = DateTime.Now.Subtract(TimeSpan.FromDays(7)).ToString("dd-MM-yyyy");

    public string EndDate { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");

    public List<HistoricalData> DateValues { get; set; } = new();

    // Properties for the work standards partial view
    public WorkStandardsViewModel WorkStandardsViewModel { get; set; } = new();
}