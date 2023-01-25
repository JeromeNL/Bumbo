using Bumbo.Web.Models.Timeline;

namespace Bumbo.Web.Services.Interfaces;

public interface ITimelineSortingService
{
    public Dictionary<string, List<int>> GenerateSortings(TimelineViewModel timelineViewModel);
}