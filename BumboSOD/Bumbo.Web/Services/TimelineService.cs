using Bumbo.Web.Services.Interfaces;

namespace Bumbo.Web.Services;

public class TimelineService : ITimelineService
{
    private readonly ITimelineItemUpdateService _timelineItemUpdateService;
    private readonly ITimelineModelService _timelineModelService;
    private readonly ITimelinePrognosisService _timelinePrognosisService;
    private readonly ITimelineSortingService _timelineSortingService;

    public TimelineService(ITimelineSortingService timelineSortingService, ITimelineModelService timelineModelService, ITimelinePrognosisService timelinePrognosisService, ITimelineItemUpdateService timelineItemUpdateService)
    {
        _timelineSortingService = timelineSortingService;
        _timelineModelService = timelineModelService;
        _timelinePrognosisService = timelinePrognosisService;
        _timelineItemUpdateService = timelineItemUpdateService;
    }

    public ITimelineModelService GetTimelineModelService()
    {
        return _timelineModelService;
    }

    public ITimelinePrognosisService GetTimelinePrognosisService()
    {
        return _timelinePrognosisService;
    }

    public ITimelineSortingService GetTimelineSortingService()
    {
        return _timelineSortingService;
    }

    public ITimelineItemUpdateService GetTimelineItemUpdateService()
    {
        return _timelineItemUpdateService;
    }
}