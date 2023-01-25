using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.Utils;
using Bumbo.Web.Models;
using Bumbo.WorkingRules.CAORules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.Employee))]
public class EmployeeExchangeRequestController : Controller
{
    private readonly IEmployeeExchangeRequestRepository _employeeExchangeRequestRepository;
    private readonly EmployeeExchangeRequestViewModel _employeeExchangeRequestViewModel;
    private readonly IWorkingRules _workingRules;
    private readonly IUserService _userService;
    private readonly IScheduleRepository _scheduleRepository;
    private const string _noconflicts = "No Conflicts";

    public EmployeeExchangeRequestController(IUserService userService, IEmployeeExchangeRequestRepository employeeExchangeRequestRepository, IWorkingRules workingRules, IScheduleRepository scheduleRepository)
    {
        _userService = userService;
        _employeeExchangeRequestRepository = employeeExchangeRequestRepository;
        _employeeExchangeRequestViewModel = new EmployeeExchangeRequestViewModel();
        _workingRules = workingRules;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userLoggedIn = await _userService.GetUserAdvanced();
        var allUncheckedShifts = await _employeeExchangeRequestRepository.GetEmployeeExchangeRequestsAsync(userLoggedIn);

        var allExistingShiftsThisWeek = new List<Shift>();
        var allValidRequests = new List<ExchangeRequest>();

        foreach (var request in allUncheckedShifts)
        {
            var currentShift = request.Shift;
            allExistingShiftsThisWeek.Remove(currentShift);

            var newCurrentShift = new Shift()
            {
                Employee = userLoggedIn,
                Start = currentShift.Start,
                End = currentShift.End,
            };

            var MondayOfWeek = DateUtil.StartOfWeek(newCurrentShift.Start, newCurrentShift.Start.DayOfWeek);
            var SundayOfWeek = MondayOfWeek.AddDays(6);
            allExistingShiftsThisWeek = await _scheduleRepository.GetAllShiftsBetween(MondayOfWeek, SundayOfWeek);
            allExistingShiftsThisWeek.Add(newCurrentShift);

           var allChecks =  await _workingRules.CheckAllShiftsForRules(allExistingShiftsThisWeek);

           if (!allChecks.ContainsKey(newCurrentShift))
           {
               allValidRequests.Add(request);
           }

        }

        var unOrderedEmployeeExchangeRequests = allValidRequests;
        _employeeExchangeRequestViewModel.ExchangeRequests = await _employeeExchangeRequestRepository.OrderEmployeeExchangeRequestAsync(unOrderedEmployeeExchangeRequests);
        _employeeExchangeRequestViewModel.UserLoggedIn = userLoggedIn;

        if (_employeeExchangeRequestViewModel.ExchangeRequests == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "Exchange Request is niet gevonden, probeer later om opnieuw te accepteren" });
        }

        return View(_employeeExchangeRequestViewModel);
    }

    public async Task<IActionResult> AcceptRequest(int? id)
    {
        var userLoggedIn = await _userService.GetUserAdvanced();
        if (id == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "Exchange Request id is niet gevonden, probeer later om opnieuw te accepteren" });
        }

        var exchangeRequest = _employeeExchangeRequestRepository.GetClickedEmployeeExchangeRequestAsync(id);

        if (exchangeRequest != null)
        {
            if (await _employeeExchangeRequestRepository.SetNewUserAsync(await exchangeRequest, userLoggedIn) == false)
            {
                return View("Error", new ErrorViewModel { RequestId = "Probleem bij opslaan newUser in Database" });
            }
        }
        else
        {
            return View("Error", new ErrorViewModel { RequestId = "Exchange Request is niet gevonden, probeer later om opnieuw te accepteren" });
        }

        return RedirectToAction("Index", "EmployeeExchangeRequest");
    }
}