namespace Bumbo.Web.Services.Interfaces;

public interface ITimelineService
{
    ITimelineModelService GetTimelineModelService();
    ITimelinePrognosisService GetTimelinePrognosisService();
    ITimelineSortingService GetTimelineSortingService();
    ITimelineItemUpdateService GetTimelineItemUpdateService();
}