using Bumbo.Data.Models;
using Bumbo.Data.Utils;
using Bumbo.Web.Models.Timeline;
using Bumbo.Web.Services.Interfaces;
using Bumbo.WorkingRules.CAORules;

namespace Bumbo.Web.Services;

public class TimelineModelService : ITimelineModelService
{
    private const int _subgroupId = 1;

    public TimelineModelService()
    {
        ShiftClass = "shift-item";
        ClockedHourClass = "clocked-hour-item";
    }

    public string ShiftClass { get; set; }
    public string ClockedHourClass { get; set; }

    public TimelineViewModel GenerateTimelineModels(IEnumerable<ApplicationUser> employeesSorted, IDictionary<Shift, List<string>> allShiftConflicts, DateTime startDateTime, DateTime endDateTime)
    {
        var visGroups = new List<VisGroupModel>();
        var shiftVisItems = new List<VisItemModel>();
        var clockedHoursVisItems = new List<VisItemModel>();
        var availabilityVisItems = new List<VisItemModel>();

        var visGroupId = 1;
        var visItemId = 1;
        foreach (var employee in employeesSorted)
        {
            var departments = employee.Departments?.Select(x => x.Department) ?? new List<Department>();
            var age = DateUtil.CalculateAgeFromBirthDate(employee.BirthDate);
            var visGroup = new VisGroupModel
            {
                Id = visGroupId++,
                Employee = employee,
                Content = employee.FullName + " (" + age + ")",
                AvailableDepartments = departments.Select(x => x.Id).ToList(),
                Order = visGroupId
            };
            visGroups.Add(visGroup);

            // Create shift items

            shiftVisItems.AddRange(GetShiftItemModels(employee.Shifts, allShiftConflicts, visGroupId, visItemId));
            visItemId += shiftVisItems.Count;

            // Create clocked hours items
            clockedHoursVisItems.AddRange(GetClockedHoursItemModels(employee.ClockedHours, visGroupId, visItemId));
            visItemId += clockedHoursVisItems.Count;

            // Create local special availabilities for each employee each day
            var localSpecialAvailabilities = GetSpecialAvailabilitiesFromUser(employee, startDateTime, endDateTime);

            // Create availability items
            availabilityVisItems.AddRange(GetAvailabilityItemModels(localSpecialAvailabilities, visGroupId, visItemId));
            visItemId += availabilityVisItems.Count;
        }

        return new TimelineViewModel
        {
            Groups = visGroups,
            ShiftItems = shiftVisItems,
            ClockedHourItems = clockedHoursVisItems,
            AvailabilityItems = availabilityVisItems
        };
    }

    public async Task<IDictionary<Shift, List<string>>> GetAllConflictsPerShift(List<ApplicationUser> allEmployees, IWorkingRules workingRules)
    {
        var allShifts = new List<Shift>();
        foreach (var item in allEmployees.Select(employee => employee.Shifts!.OrderBy(e => e.Start)))
        {
            allShifts.AddRange(item);
        }

        return await workingRules.CheckAllShiftsForRules(allShifts);
    }

    private List<SpecialAvailability> GetSpecialAvailabilitiesFromUser(ApplicationUser employee, DateTime startDateTime, DateTime endDateTime)
    {
        // Create local special availabilities for each employee each day
        var localSpecialAvailabilities = new List<SpecialAvailability>();
        for (var date = startDateTime; date < endDateTime; date = date.AddDays(1))
        {
            var specialAvailability = new SpecialAvailability
            {
                EmployeeId = employee.Id,
                Start = date.AddHours(0),
                End = date.AddHours(0)
            };
            foreach (var standardAvailability in employee.StandardAvailabilities.Where(standardAvailability => date.DayOfWeek == standardAvailability.DayOfWeek))
            {
                specialAvailability = new SpecialAvailability
                {
                    EmployeeId = employee.Id,
                    Start = date.AddHours(standardAvailability.Start.Hour).AddMinutes(standardAvailability.Start.Minute),
                    End = date.AddHours(standardAvailability.End.Hour).AddMinutes(standardAvailability.End.Minute)
                };
            }

            localSpecialAvailabilities.Add(specialAvailability);
        }

        // Overwrite special availabilities with the ones from the database in applicationuser
        for (var itemId = 0; itemId < localSpecialAvailabilities.Count; itemId++)
        {
            foreach (var specialAvailability in employee.SpecialAvailabilities.Where(specialAvailability => localSpecialAvailabilities[itemId].Start.Date == specialAvailability.Start.Date))
            {
                localSpecialAvailabilities[itemId] = specialAvailability;
            }
        }

        return localSpecialAvailabilities;
    }

    private IEnumerable<VisItemModel> GetClockedHoursItemModels(List<ClockedHours> clockedHours, int visGroupId, int visItemId)
    {
        var clockedHoursVisItems = new List<VisItemModel>();
        foreach (var clockedHour in clockedHours)
        {
            var clockedHoursEndDateTime = clockedHour.ClockedOut ?? DateTime.Now;

            var classNameString = ClockedHourClass;
            if (clockedHour.IsApproved == false)
            {
                classNameString += " not-approved-clocked-hour-item";
            }

            // Create clocked hours item model
            var visItem = new VisItemModel
            {
                Id = visItemId++.ToString(),
                CustomId = clockedHour.Id,
                Group = visGroupId - 1,
                StartDate = clockedHour.ClockedIn.ToIsoString(),
                EndDate = clockedHoursEndDateTime.ToIsoString(),
                ClassName = classNameString,
                Subgroup = _subgroupId,
                Title = clockedHour.ClockedIn.ToShortTimeString() + " - " + clockedHoursEndDateTime.ToShortTimeString(),
                Editable = true
            };
            clockedHoursVisItems.Add(visItem);
        }

        return clockedHoursVisItems;
    }

    private IEnumerable<VisItemModel> GetShiftItemModels(List<Shift> shifts, IDictionary<Shift, List<string>> conflictShift, int visGroupId, int visItemId)
    {
        var shiftVisItems = new List<VisItemModel>();
        foreach (var shift in shifts)
        {
            var shiftIsInFuture = !(shift.Start < DateTime.Now);
            var classNameString = ShiftClass;
            if (!shiftIsInFuture)
            {
                classNameString += " past-shift-item";
            }

            if (!shift.IsPublished)
            {
                classNameString += " not-published-shift-item";
            }
            else
            {
                classNameString += " solid-border-item";
            }

            if (conflictShift.ContainsKey(shift))
            {
                classNameString += " conflict-shift-item";
            }

            if (shift.IsIll)
                classNameString += " ill-shift-item";
            // Create shift item model
            var visItem = new VisItemModel
            {
                Id = visItemId++.ToString(),
                CustomId = shift.Id,
                Group = visGroupId - 1,
                StartDate = shift.Start.ToIsoString(),
                EndDate = shift.End.ToIsoString(),
                IsIll = shift.IsIll,
                DepartmentId = shift.DepartmentId,
                Subgroup = _subgroupId,
                ClassName = classNameString,
                Editable = shiftIsInFuture
            };
            shiftVisItems.Add(visItem);
        }

        return shiftVisItems;
    }

    private IEnumerable<VisItemModel> GetAvailabilityItemModels(List<SpecialAvailability> specialAvailabilities, int visGroupId, int visItemId)
    {
        return specialAvailabilities.Select(specialAvailability => new VisItemModel
            {
                Id = visItemId++.ToString(),
                Group = visGroupId - 1,
                StartDate = specialAvailability.Start.ToIsoString(),
                EndDate = specialAvailability.End.ToIsoString(),
                Type = VisItemType.Background,
                ClassName = "availability-item",
                Subgroup = _subgroupId,
                Title = specialAvailability.Start.ToShortTimeString() + " - " + specialAvailability.End.ToShortTimeString(),
                Editable = false
            })
            .ToList();
    }
}