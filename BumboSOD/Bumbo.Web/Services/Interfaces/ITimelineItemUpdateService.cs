using Bumbo.Data.DAL.Interfaces;
using Bumbo.Web.Models.Timeline;

namespace Bumbo.Web.Services.Interfaces;

public interface ITimelineItemUpdateService
{
    VisItemsModified SortVisItemByClassName(VisItemsModified visItemsModified, string className);
    Task HandleShiftModifications(VisItemsModified modifiedItems, IScheduleRepository scheduleRepository);
    Task HandleClockedHoursModifications(VisItemsModified modifiedItems, IScheduleRepository scheduleRepository);
}