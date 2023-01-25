namespace Bumbo.Web.Models.Timeline;

public class TimelineViewModel
{
    public List<VisGroupModel> Groups { get; set; }
    public List<VisItemModel> ShiftItems { get; set; }
    public List<VisItemModel> ClockedHourItems { get; set; }
    public List<VisItemModel> AvailabilityItems { get; set; }
}