using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.Employee))]
public class EmployeeWorkedHoursController : Controller
{
    private readonly IEmployeeWorkedHoursRepository _employeeWorkedHoursRepository;
    private readonly IUserService _userService;
    private readonly YearlyWorkedHoursViewModel _yearlyWorkedHoursViewModel;

    public EmployeeWorkedHoursController(IUserService userService, IEmployeeWorkedHoursRepository employeeWorkedHoursRepository)
    {
        _userService = userService;
        _employeeWorkedHoursRepository = employeeWorkedHoursRepository;
        _yearlyWorkedHoursViewModel = new YearlyWorkedHoursViewModel();
    }

    public async Task<IActionResult> Index()
    {
        var loggedInUser = await _userService.GetUserAdvanced();
        _yearlyWorkedHoursViewModel.Employee = loggedInUser;
        await SetClockedHoursForEmployee(loggedInUser);
        return await Task.FromResult<IActionResult>(View(_yearlyWorkedHoursViewModel));
    }

    private async Task SetClockedHoursForEmployee(ApplicationUser loggedInUser)
    {
        var allYears = await _employeeWorkedHoursRepository.GetWorkedYearsOfEmployee(loggedInUser);

        //For each year
        foreach (var year in allYears)
        {
            //Create for each month a new ClockedHoursViewModel
            var allMonths = await _employeeWorkedHoursRepository.GetWorkedMonthOfYear(loggedInUser, year);

            foreach (var month in allMonths)
            {
                //Gets list by checking months and years, in order to get the correct worked hours
                var workedInMonths = await _employeeWorkedHoursRepository.GetClockedHoursYearlyAndMonthly(loggedInUser, year, month);
                var workedHours = _employeeWorkedHoursRepository.GetHoursTimeSpanOfEmployee(loggedInUser, workedInMonths);

                var totalWorkedHours = Math.Round((decimal)workedHours.Sum(x => x.TotalHours), 2);
                var monthName = new DateTime(year, month, 01).ToString("MMMM");

                //Set new ClockedHoursViewModel
                _yearlyWorkedHoursViewModel.ClockedHoursViewModel.Add(new ClockedHoursViewModel
                {
                    WorkedMonth = monthName,
                    WorkedYear = year,
                    WorkedHours = workedHours,
                    ClockedHoursPerMonth = workedInMonths,
                    TotalMonthlyHours = totalWorkedHours
                });
            }
        }
    }
}