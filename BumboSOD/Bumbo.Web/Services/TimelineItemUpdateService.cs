using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Web.Models.Timeline;
using Bumbo.Web.Services.Interfaces;

namespace Bumbo.Web.Services;

public class TimelineItemUpdateService : ITimelineItemUpdateService
{
    public async Task HandleClockedHoursModifications(VisItemsModified modifiedItems, IScheduleRepository scheduleRepository)
    {
        // New
        var newClockedList = modifiedItems.NewItems?.Select(ch => new ClockedHours
        {
            EmployeeId = ch.EmployeeId,
            ClockedIn = DateTime.Parse(ch.StartDate),
            ClockedOut = DateTime.Parse(ch.EndDate)
        }).ToList();
        if (newClockedList != null)
            await scheduleRepository.AddClockedHours(newClockedList);

        // Edited
        var editedClockList = modifiedItems.UpdatedItems?.Select(x => new ClockedHours
        {
            Id = x.CustomId!.Value,
            EmployeeId = x.EmployeeId,
            ClockedIn = DateTime.Parse(x.StartDate),
            ClockedOut = DateTime.Parse(x.EndDate)
        }).ToList();
        if (editedClockList != null)
            await scheduleRepository.UpdateClockedHours(editedClockList);

        // Deleted
        var deletedClockList = modifiedItems.RemovedItems?.Select(x => x.CustomId!.Value).ToList();
        if (deletedClockList != null)
            await scheduleRepository.DeleteClockedHours(deletedClockList);
    }

    public async Task HandleShiftModifications(VisItemsModified modifiedItems, IScheduleRepository scheduleRepository)
    {
        // New
        var newShiftsList = modifiedItems.NewItems?.Select(x => new Shift
        {
            EmployeeId = x.EmployeeId!,
            DepartmentId = x.DepartmentId,
            Start = DateTime.Parse(x.StartDate),
            End = DateTime.Parse(x.EndDate),
            IsIll = x.IsIll
        }).ToList();
        if (newShiftsList != null)
            await scheduleRepository.AddEmployeeShifts(newShiftsList);

        // Edited
        var editedShiftsList = modifiedItems.UpdatedItems?.Select(x => new Shift
        {
            Id = x.CustomId!.Value,
            EmployeeId = x.EmployeeId!,
            DepartmentId = x.DepartmentId,
            Start = DateTime.Parse(x.StartDate),
            End = DateTime.Parse(x.EndDate),
            IsIll = x.IsIll
        }).ToList();
        if (editedShiftsList != null)
            await scheduleRepository.UpdateEmployeeShifts(editedShiftsList);

        // Deleted
        var deletedItemIds = modifiedItems.RemovedItems?.Select(x => x.CustomId!.Value).ToList();
        if (deletedItemIds != null)
            await scheduleRepository.DeleteEmployeeShifts(deletedItemIds);
    }

    public VisItemsModified SortVisItemByClassName(VisItemsModified visItemsModified, string className)
    {
        var newItemsByClass = visItemsModified.NewItems?.Where(x => x.ClassName != null && x.ClassName!.Contains(className)).ToList();
        var editedItemsByClass = visItemsModified.UpdatedItems?.Where(x => x.ClassName != null && x.ClassName!.Contains(className)).ToList();
        var deletedItemsByClass = visItemsModified.RemovedItems?.Where(x => x.ClassName != null && x.ClassName!.Contains(className)).ToList();

        return new VisItemsModified
        {
            NewItems = newItemsByClass,
            UpdatedItems = editedItemsByClass,
            RemovedItems = deletedItemsByClass
        };
    }
}