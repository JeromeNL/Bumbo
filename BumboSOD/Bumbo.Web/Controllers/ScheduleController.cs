using System.Globalization;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.Utils;
using Bumbo.Prognosis;
using Bumbo.Web.Models;
using Bumbo.Web.Models.Timeline;
using Bumbo.Web.Services.Interfaces;
using Bumbo.WorkingRules.CAORules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.BranchManager))]
public class ScheduleController : Controller
{
    private readonly IPrognosisService _prognosisService;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ITimelineService _timelineService;
    private readonly IUserService _userService;
    private readonly IWorkingRules _workingRules;

    public ScheduleController(IScheduleRepository scheduleRepository, IPrognosisService prognosisService, IUserService userService, IWorkingRules workingRules, ITimelineService timelineService)
    {
        _scheduleRepository = scheduleRepository;
        _workingRules = workingRules;
        _prognosisService = prognosisService;
        _userService = userService;
        _timelineService = timelineService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? dateOfWeek, string? departmentFilter)
    {
        DateTime startDateTime;
        // Default to beginning of week
        startDateTime = dateOfWeek == null ? DateTime.Now.StartOfWeek(DayOfWeek.Monday) : DateTime.Parse(dateOfWeek).StartOfWeek(DayOfWeek.Monday);

        // Timeline boundaries
        var endDateTime = startDateTime.AddDays(7).AddMilliseconds(-1);

        var department = departmentFilter ?? null;
        var employees = await _scheduleRepository.GetAllEmployeesBetween(startDateTime, endDateTime);
        var allShiftConflicts = await GetConflictsPerShift(employees);
        var vm = await GenerateScheduleViewModel(employees, allShiftConflicts, startDateTime, endDateTime, TimelineStep.Day, department);

        return View(vm);
    }

    [HttpPost]
    public IActionResult Index(ScheduleViewModel scheduleViewModel)
    {
        // If the same start and end date are posted
        if (scheduleViewModel.StartDate == scheduleViewModel.EndDate)
        {
            var inputDateTime = DateTime.Parse(scheduleViewModel.StartDate).ToString("yyyy-MM-dd");
            return RedirectToAction("DayInfo", new { inputDateTime });
        }

        // Timeline boundaries
        var startDateTime = DateTime.Parse(scheduleViewModel.StartDate).StartOfWeek(DayOfWeek.Monday);

        return RedirectToAction("Index", new { dateOfWeek = startDateTime.ToString("yyyy-MM-dd") });
    }

    [HttpPost]
    public async Task<IActionResult> SaveChanges(string dateOfWeek, bool returnToWeek, string? newShifts, string? editedShifts, string? deletedShifts)
    {
        var visNewItemList = newShifts != null ? JsonConvert.DeserializeObject<VisItemModel[]?>(newShifts)?.ToList() : null;
        var visEditedItemList = editedShifts != null ? JsonConvert.DeserializeObject<VisItemModel[]?>(editedShifts)?.ToList() : null;
        var visItemShiftsList = deletedShifts != null ? JsonConvert.DeserializeObject<VisItemModel[]?>(deletedShifts)?.ToList() : null;

        var visItemsModified = new VisItemsModified
        {
            NewItems = visNewItemList,
            UpdatedItems = visEditedItemList,
            RemovedItems = visItemShiftsList
        };

        var timelineUpdatedService = _timelineService.GetTimelineItemUpdateService();

        var clockedHourClass = _timelineService.GetTimelineModelService().ClockedHourClass;
        var clockedHourItems = timelineUpdatedService.SortVisItemByClassName(visItemsModified, clockedHourClass);
        await timelineUpdatedService.HandleClockedHoursModifications(clockedHourItems, _scheduleRepository);

        var shiftClass = _timelineService.GetTimelineModelService().ShiftClass;
        var shiftItems = timelineUpdatedService.SortVisItemByClassName(visItemsModified, shiftClass);
        await timelineUpdatedService.HandleShiftModifications(shiftItems, _scheduleRepository);

        return !returnToWeek ? RedirectToAction("DayInfo", new { inputDateTime = DateTime.Parse(dateOfWeek).ToString("yyyy-MM-dd") }) : RedirectToAction(nameof(Index), new { dateOfWeek });
    }

    [HttpGet]
    public async Task<IActionResult> DayInfo(string? inputDateTime)
    {
        DateTime startDateTime;
        // Default to today
        startDateTime = inputDateTime == null ? DateTime.Now.Date : DateTime.Parse(inputDateTime);

        var endDateTime = startDateTime.AddDays(1);

        // Get all employees
        var employees = await _scheduleRepository.GetAllEmployeesBetween(startDateTime, endDateTime);

        // Get all shift conflicts
        var allShiftConflicts = await GetConflictsPerShift(employees);

        var vm = await GenerateScheduleViewModel(employees, allShiftConflicts, startDateTime, endDateTime, TimelineStep.Hour, null);

        vm.ConflictsPerShift = allShiftConflicts;
        vm.StartDate = startDateTime.AddHours(4).ToIsoString();
        return View(vm);
    }

    [HttpPost]
    public IActionResult DayInfo(ScheduleViewModel scheduleViewModel)
    {
        var inputDateTime = DateTime.Parse(scheduleViewModel.StartDate).ToString("yyyy-MM-dd");
        return RedirectToAction("DayInfo", new { inputDateTime });
    }

    private async Task<ScheduleViewModel> GenerateScheduleViewModel(List<ApplicationUser> employees, IDictionary<Shift, List<string>> allShiftConflicts, DateTime startDateTime, DateTime endDateTime, TimelineStep timelineStep, string? department)
    {
        var daysWithinRange = DateUtil.EachDay(startDateTime, endDateTime.AddSeconds(-1)).ToList();

        // Get applicable employees
        var employeesSorted = employees.OrderBy(o => o.FirstName);

        // Generate timeline data
        var timelineModels = _timelineService.GetTimelineModelService().GenerateTimelineModels(employeesSorted, allShiftConflicts, startDateTime, endDateTime);

        //Add ChangedClockedHours button if changes are made
        var changedHasBeenMade = ChangedAreMade(await _scheduleRepository.GetChangedClockedHours(startDateTime, endDateTime));

        // Calculate prognosis
        var prognosisDictionary = await _prognosisService.GetExistingOrGenerateFreshPrognosisBetweenTime(daysWithinRange);
        var newPrognosisDictionary = _timelineService.GetTimelinePrognosisService().RewriteDictionary(prognosisDictionary);

        var prognosisList = prognosisDictionary.SelectMany(kvp => kvp.Value.Values).ToList();
        var prognosisIds = prognosisList.Select(pr => pr.Id).ToArray();
        var (prognosisUpToDate, prognosisValidData) = await _prognosisService.CheckIfPrognosesAreValid(prognosisList);

        // Get the existing schedules hours
        var hoursPlannedForRange = await _scheduleRepository.GetPlannedHoursForEachDepartmentDuringDateRange(daysWithinRange, (await _userService.GetUser()).BranchId!.Value);
        var newHoursPlannedForRange = _timelineService.GetTimelinePrognosisService().RewriteDictionary(hoursPlannedForRange);

        var employeeAtEveryHourLowPayout = await _scheduleRepository.EmployeeCountForEveryHour(daysWithinRange, (await _userService.GetUser()).BranchId!.Value, user => user.PayoutScale <= 4);
        var employeeAtEveryHourMediumPayout = await _scheduleRepository.EmployeeCountForEveryHour(daysWithinRange, (await _userService.GetUser()).BranchId!.Value, user => user.PayoutScale is > 4 and < 7);
        var employeeAtEveryHourHighPayout = await _scheduleRepository.EmployeeCountForEveryHour(daysWithinRange, (await _userService.GetUser()).BranchId!.Value, user => user.PayoutScale >= 7);
        var employeeAtEveryHourAll = await _scheduleRepository.EmployeeCountForEveryHour(daysWithinRange, (await _userService.GetUser()).BranchId!.Value, _ => true);
        var employeeAtEveryHourDict = new Dictionary<string, IDictionary<DateTime, double>>
        {
            { "All", employeeAtEveryHourAll },
            { "> 7", employeeAtEveryHourHighPayout },
            { "4 - 7", employeeAtEveryHourMediumPayout },
            { "< 4", employeeAtEveryHourLowPayout }
        };

        // Generate sortings
        var sortings = _timelineService.GetTimelineSortingService().GenerateSortings(timelineModels);

        // Create viewmodel
        var vm = new ScheduleViewModel
        {
            TimelineViewModel = timelineModels,
            StartDate = startDateTime.ToIsoString(),
            EndDate = endDateTime.ToIsoString(),
            Sortings = sortings,
            DepartmentOptions = await _scheduleRepository.GetAllDepartments(),
            DepartmentFilter = department,
            Step = timelineStep,
            EmployeeAtEveryHour = employeeAtEveryHourDict,
            TimelinePrognosis = new TimelinePrognosisViewModel
            {
                PrognosisRangeJson = JsonConvert.SerializeObject(newPrognosisDictionary),
                HoursPlannedRangeJson = JsonConvert.SerializeObject(newHoursPlannedForRange),
                PrognosisToUpdate = prognosisIds,
                PrognosisOutdated = !prognosisUpToDate,
                PrognosisHadInvalidData = !prognosisValidData
            },
            ChangesMade = changedHasBeenMade
        };

        return vm;
    }

    private async Task<IDictionary<Shift, List<string>>> GetConflictsPerShift(List<ApplicationUser> employees)
    {
        var usersAlphabet = employees.OrderBy(e => e.FirstName).ToList();
        var allShiftConflicts = await _timelineService.GetTimelineModelService().GetAllConflictsPerShift(usersAlphabet, _workingRules);
        return allShiftConflicts;
    }

    [HttpGet]
    public IActionResult CopySchedule(DateTime startDateTime, DateTime endDateTime)
    {
        var desiredWeekStartDateTime = startDateTime.AddDays(7);
        var desiredWeekEndDateTime = endDateTime.AddDays(7);
        var copyScheduleViewModel = new CopyScheduleViewModel
        {
            StartDateTime = startDateTime.AddDays(1).ToString("dd-MM-yyyy"),
            EndDateTime = endDateTime.ToString("dd-MM-yyyy"),
            DesiredWeekStartDateTime = desiredWeekStartDateTime.ToString("dd-MM-yyyy"),
            DesiredWeekEndDateTime = desiredWeekEndDateTime.ToString("dd-MM-yyyy")
        };
        return View(copyScheduleViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CopySchedule(CopyScheduleViewModel copyScheduleViewModel)
    {
        if (ModelState.IsValid)
        {
            DateTime correctStartDateTime;
            DateTime correctEndDateTime;
            try
            {
                correctStartDateTime = DateTime.ParseExact(copyScheduleViewModel.StartDateTime, "yyyy-MM-ddTHH:mm:ss.fffZ", new CultureInfo("nl-NL"));
                correctEndDateTime = DateTime.ParseExact(copyScheduleViewModel.EndDateTime, "yyyy-MM-ddTHH:mm:ss.fffZ", new CultureInfo("nl-NL"));
            }
            catch (FormatException)
            {
                correctStartDateTime = DateUtil.DateTimeFromString(copyScheduleViewModel.StartDateTime);
                correctEndDateTime = DateUtil.DateTimeFromString(copyScheduleViewModel.EndDateTime);
            }

            var correctDesiredWeekStartDateTime = DateUtil.DateTimeFromString(copyScheduleViewModel.DesiredWeekStartDateTime);
            var correctDesiredWeekEndDateTime = DateUtil.DateTimeFromString(copyScheduleViewModel.DesiredWeekEndDateTime);

            var daysBetween = (correctDesiredWeekStartDateTime - correctStartDateTime).Days;

            var selectedWeekShifts = await _scheduleRepository.GetAllShiftsBetween(correctStartDateTime, correctEndDateTime);
            var desiredWeekShifts = await _scheduleRepository.GetAllShiftsBetween(correctDesiredWeekStartDateTime, correctDesiredWeekEndDateTime);

            if (desiredWeekShifts.Any(s => s.IsPublished))
            {
                const string requestId = "Je probeert om een week te kopieren waarvan er al shifts zijn. Dit gaat mis en is helaas niet mogelijk. Probeer een andere week.";
                return View("Error", new ErrorViewModel { RequestId = requestId });
            }

            if (desiredWeekShifts.Any())
            {
                await _scheduleRepository.DeleteEmployeeShifts(desiredWeekShifts.Select(shift => shift.Id).ToList());
            }

            await _scheduleRepository.CopySchedule(selectedWeekShifts, daysBetween);
            return RedirectToAction("Index", new { dateOfWeek = copyScheduleViewModel.DesiredWeekStartDateTime });
        }

        copyScheduleViewModel.StartDateTime = DateTime.Parse(copyScheduleViewModel.StartDateTime).ToString("dd-MM-yyyy");
        copyScheduleViewModel.EndDateTime = DateTime.Parse(copyScheduleViewModel.EndDateTime).ToString("dd-MM-yyyy");

        return View("CopySchedule", copyScheduleViewModel);
    }

    public async Task<IActionResult> ApproveClockedHours(string startDate, string endDate)
    {
        // Get all clocked hours
        var clockedHours = await _scheduleRepository.GetClockedHoursBetween(DateTime.Parse(startDate), DateTime.Parse(endDate));
        // Approve all clocked hours
        await _scheduleRepository.ApproveClockedHours(clockedHours);
        // Redirect to the day or week page
        if (DateTime.Parse(endDate).Date - DateTime.Parse(startDate).Date < TimeSpan.FromDays(2))
        {
            return RedirectToAction("DayInfo", new { inputDateTime = DateTime.Parse(startDate).ToString("yyyy-MM-dd") });
        }

        return RedirectToAction("Index", new { dateOfWeek = startDate });
    }

    public async Task<IActionResult> PublishSchedule(string startDate, string endDate)
    {
        var startDateParsed = DateTime.Parse(startDate);
        var endDateParsed = DateTime.Parse(endDate).AddDays(1);
        var employees = await _scheduleRepository.GetAllEmployeesBetween(startDateParsed, endDateParsed);

        var conflicts = await GetConflictsPerShift(employees);

        var shiftsForRequestedDate = await _scheduleRepository.GetAllShiftsBetween(startDateParsed, endDateParsed);

        var shiftsWithConflicts = conflicts.Where(c => c.Value.Count > 0).Select(c => c.Key).ToList();

        if (shiftsForRequestedDate.Count > 0)
        {
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View();
        }

        return RedirectToAction("PublishSchedulePost", new { startDate, endDate });
    }

    public async Task<IActionResult> ShowChangedClockedHours(string startTime, string endTime)
    {
        var startDateTime = DateTime.Parse(startTime);
        var endDateTime = DateTime.Parse(endTime);
        var employees = await _scheduleRepository.GetAllEmployeesBetween(startDateTime, endDateTime);
        var clockedHours = await _scheduleRepository.GetChangedClockedHours(startDateTime, endDateTime);

        foreach (var hours in clockedHours)
        {
            hours.Employee = employees.FirstOrDefault(e => e.Id == hours.EmployeeId);
        }

        var changedClockedHoursViewModel = new ChangedClockedHoursViewModel
        {
            HistoryClockedHours = clockedHours,
            WeekStartDateTime = DateUtil.GetWeekNumberOfDateTime(startDateTime)
        };

        return View("ChangedClockedHours", changedClockedHoursViewModel);
    }

    private bool ChangedAreMade(List<ClockedHours> changedClockedHoursList)
    {
        return changedClockedHoursList.Count > 0;
    }

    public async Task<IActionResult> PublishSchedulePost(string startDate, string endDate)
    {
        var startDateParsed = DateTime.Parse(startDate);
        var endDateParsed = DateTime.Parse(endDate).AddDays(1);
        var shifts = await _scheduleRepository.GetAllShiftsBetween(startDateParsed, endDateParsed);

        await _scheduleRepository.PublishSchedule(shifts);

        if (DateTime.Parse(endDate).Date - DateTime.Parse(startDate).Date < TimeSpan.FromDays(2))
        {
            return RedirectToAction("DayInfo", new { inputDateTime = DateTime.Parse(startDate).ToString("yyyy-MM-dd") });
        }

        return RedirectToAction("Index", new { dateOfWeek = startDate });
    }

    public async Task<IActionResult> PrintSchedule(string startDate, string endDate)
    {
        var parsedStartDate = DateTime.Parse(startDate);
        var parsedEndDate = DateTime.Parse(endDate).AddSeconds(-1);

        var employees = await _scheduleRepository.GetAllEmployeesBetween(parsedStartDate, parsedEndDate);
        var shifts = await _scheduleRepository.GetAllShiftsBetween(parsedStartDate, parsedEndDate);
        var dictionary = employees.ToDictionary(employee => employee, employee => shifts.Where(shift => shift.EmployeeId == employee.Id).ToList());
        var printScheduleViewModel = new PrintScheduleViewModel
        {
            StartDate = parsedStartDate,
            EndDate = parsedEndDate,
            EmployeeShifts = dictionary
        };

        return new ViewAsPdf("PrintedSchedule", printScheduleViewModel)
        {
            FileName = $"Rooster {printScheduleViewModel.StartDateString} - {printScheduleViewModel.EndDateString}.pdf",
            ContentDisposition = ContentDisposition.Attachment,
            PageOrientation = Orientation.Landscape
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshPrognoses([FromForm] int[] prognosisToUpdate)
    {
        await _prognosisService.OverWritePrognosis(prognosisToUpdate);

        return NoContent();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FineTunePrognosis(string day, Dictionary<int, int> offsets)
    {
        if (offsets.Sum(kv => kv.Value) == 0)
        {
            return RedirectToAction("DayInfo", new { inputDateTime = day.Split("T")[0] });
        }

        var user = await _userService.GetUser();

        if (user.BranchId is null)
        {
            throw new InvalidOperationException("Current user is not bound to branch");
        }

        await _prognosisService.ForceIncreasePrognosis(user.BranchId.Value, DateTime.Parse(day), offsets);

        return RedirectToAction("DayInfo", new { inputDateTime = day.Split("T")[0] });
    }
}