using System.Text;
using Bumbo.Data;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.Utils;
using Bumbo.Web.Models;
using Bumbo.WorkingRules.CAORules;
using Bumbo.WorkingRules.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers
{
    [Authorize(Roles = nameof(Role.BranchManager))]
    public class ExportWorkedHoursController : Controller
    {
        private readonly IExportWorkedHoursRepository _exportWorkedHoursRepository;
        private readonly IUserService _userService;
        private readonly WorkingFeesCalculator _workingFeesCalculator;

        public ExportWorkedHoursController(IUserService userService, IExportWorkedHoursRepository exportWorkedHoursRepository, BumboDbContext context)
        {
            _userService = userService;
            _exportWorkedHoursRepository = exportWorkedHoursRepository;
            _workingFeesCalculator = new WorkingFeesCalculator(new CaoEfCoreRepository(context));
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        public async Task<IActionResult> WorkedHoursOverview(ExportWorkedHoursViewModel exportWorkedHoursViewModel)
        {
            return View("WorkedHoursOverview", await FillViewModelWithData(exportWorkedHoursViewModel));
        }

        private async Task<ExportWorkedHoursViewModel> FillViewModelWithData(ExportWorkedHoursViewModel exportWorkedHoursViewModel)
        {
            var workedHoursViewModel = new ExportWorkedHoursViewModel();
            var user = await _userService.GetUser();

            var (firstDayOfMonth, lastDayOfMonth) = DateUtil.GetFirstAndLastDayOfMonth(exportWorkedHoursViewModel.Month, exportWorkedHoursViewModel.Year);

            var workingFeesForEmployees = await _workingFeesCalculator.GetAllWorkedHoursWithAllowancesForPeriod(firstDayOfMonth, lastDayOfMonth, (await _exportWorkedHoursRepository.GetBranch(user.BranchId))!);
            var exportedWorkedHoursBranch = await _exportWorkedHoursRepository.GetBranch(user.BranchId);

            workedHoursViewModel.Month = exportWorkedHoursViewModel.Month;
            workedHoursViewModel.Year = exportWorkedHoursViewModel.Year;
            workedHoursViewModel.MonthString = firstDayOfMonth.ToString("MMMM");
            workedHoursViewModel.StartDate = firstDayOfMonth;
            workedHoursViewModel.EndDate = lastDayOfMonth;
            workedHoursViewModel.BranchName = exportedWorkedHoursBranch.Name!;
            workedHoursViewModel.WorkingFeesForEmployees = workingFeesForEmployees;
            return workedHoursViewModel;
        }

        /// <summary>
        /// Converts a list from the viewmodel to a csv string.
        /// </summary>
        /// <param name="exportWorkedHoursViewModel">Viewmodel where the list to be converted is</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddListToCsv(ExportWorkedHoursViewModel exportWorkedHoursViewModel)
        {
            var user = await _userService.GetUser();

            var workingFeesForEmployees = await _workingFeesCalculator.GetAllWorkedHoursWithAllowancesForPeriod(exportWorkedHoursViewModel.StartDate, exportWorkedHoursViewModel.EndDate, (await _exportWorkedHoursRepository.GetBranch(user.BranchId))!);

            var csvList = new List<ExportClockedHoursCsvViewModel>();
            foreach (var employee in workingFeesForEmployees.Keys)
            {
                var workingFees = workingFeesForEmployees[employee];
                foreach (var allowance in workingFees.WorkedHoursPerAllowance.Keys)
                {
                    csvList.Add(new ExportClockedHoursCsvViewModel
                    {
                        EmployeeUuid = employee.Id,
                        EmployeeName = employee.FullName,
                        TotalHours = Math.Round(workingFees.WorkedHoursPerAllowance[allowance].TotalHours, MidpointRounding.AwayFromZero),
                        Allowance = allowance
                    });
                }
            }

            await ProcessClockedHoursAsRenumerated(exportWorkedHoursViewModel);

            var spaceLessBranchName = exportWorkedHoursViewModel.BranchName.Replace(", ", "-").Replace(" ", "-");

            var csvString = CsvHandler.ListToCsv(csvList);
            return WriteCsvToFolder(csvString, exportWorkedHoursViewModel.MonthString, exportWorkedHoursViewModel.Year, spaceLessBranchName);
        }

        /// <summary>
        ///Downloads the csv file to the user's downloads folder
        /// </summary>
        /// <param name="csv">CsvString</param>
        /// <param name="month">Month for file name in format "MMMM"</param>
        /// <param name="year">Year for file name as an int</param>
        /// <param name="branchName">Branch name for file name without spaces</param>
        /// <returns></returns>
        private FileStreamResult WriteCsvToFolder(string csv, string month, int year, string branchName)
        {
            var fileName = $"ExportedWorkedHours_{branchName}_{month}_{year}_{DateTime.Now:dd-MM-yyyy}.csv";
            var content = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            const string contentType = "text/csv";
            return File(content, contentType, fileName);
        }

        private async Task ProcessClockedHoursAsRenumerated(ExportWorkedHoursViewModel exportWorkedHoursViewModel)
        {
            var user = await _userService.GetUserAdvanced();
            await _exportWorkedHoursRepository.RenumerateClockedHours(user.BranchId, exportWorkedHoursViewModel.StartDate, exportWorkedHoursViewModel.EndDate);
        }
    }
}