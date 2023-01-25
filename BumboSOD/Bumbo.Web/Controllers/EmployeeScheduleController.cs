using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.Utils;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.Employee))]
public class EmployeeScheduleController : Controller
{
    private readonly IEmployeeScheduleRepository _employeeScheduleRepository;
    private readonly EmployeeSceduleViewModel _employeeScheduleViewModel;
    private readonly IUserService _userService;

    public EmployeeScheduleController(IUserService userService, IEmployeeScheduleRepository employeeScheduleRepository)
    {
        _employeeScheduleViewModel = new EmployeeSceduleViewModel();
        _employeeScheduleRepository = employeeScheduleRepository;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        _employeeScheduleViewModel.UserLoggedIn = await _userService.GetUser();
        var unOrderedShiftsFromUserLoggedIn = await _employeeScheduleRepository.GetEmployeeShiftsAsync(_employeeScheduleViewModel.UserLoggedIn);
        var ordererShiftsFromUserLoggedIn = _employeeScheduleRepository.OrderEmployeeShiftsAsync(unOrderedShiftsFromUserLoggedIn);
        _employeeScheduleViewModel.ExchangeRequests = await _employeeScheduleRepository.GetExchangeRequestsFromUserLoggedInAsync(_employeeScheduleViewModel.UserLoggedIn);
        _employeeScheduleViewModel.Shifts = CreateWeekDictionary(ordererShiftsFromUserLoggedIn);

        return View(_employeeScheduleViewModel);
    }

    public async Task<IActionResult> CreateRequest(int? id)
    {
        var user = await _userService.GetUser();
        var inserted = await _employeeScheduleRepository.AddShiftToExchangeRequestAsync(id, user);
        if (inserted == false)
        {
            return View("Error", new ErrorViewModel { RequestId = "Verzoek is niet correct ingediend, probeer het later opnieuw" });
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> RemoveRequest(int? id)
    {
        await _userService.GetUser();
        var removed = await _employeeScheduleRepository.RemoveShiftFromExchangeRequestAsync(id);
        if (!removed)
        {
            return View("Error", new ErrorViewModel { RequestId = "Verzoek is niet correct geannuleerd, probeer het later opnieuw" });
        }

        return RedirectToAction("Index");
    }

    private Dictionary<int, List<Shift>> CreateWeekDictionary(List<Shift> shiftsForUserLoggedIn)
    {
        var shiftsPerWeek = new Dictionary<int, List<Shift>>();
        var weekNumberNow = DateUtil.GetWeekNumberOfDateTime(DateTime.Now);
        foreach (var shift in shiftsForUserLoggedIn)
        {
            var weekNumberShift = DateUtil.GetWeekNumberOfDateTime(shift.Start);

            if (shift.Start.Date >= DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek))
            {
                if (!shiftsPerWeek.ContainsKey(weekNumberShift))
                {
                    shiftsPerWeek.Add(weekNumberShift, new List<Shift>());
                }

                shiftsPerWeek[weekNumberShift].Add(shift);
            }
        }

        return shiftsPerWeek;
    }
}