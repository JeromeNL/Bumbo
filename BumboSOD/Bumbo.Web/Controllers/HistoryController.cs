using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.Utils;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.BranchManager))]
public class HistoryController : Controller
{
    private readonly IHistoryRepository _historyRepository;
    private readonly HistoryViewModel _historyViewModel;
    private readonly IUserService _userService;
    private readonly IWorkStandardsRepository _workStandardsRepository;

    public HistoryController(IUserService userService, IHistoryRepository historyRepository, IWorkStandardsRepository workStandardsRepository)
    {
        _userService = userService;
        _historyRepository = historyRepository;
        _workStandardsRepository = workStandardsRepository;
        _historyViewModel = new HistoryViewModel();
    }

    public async Task<IActionResult> Index()
    {
        // Get all dates between the chosen values
        await CreateBaseModelData(_historyViewModel);
        return View(_historyViewModel);
    }

    // GET: WorkStandards
    public async Task<IActionResult> WorkStandardIndex()
    {
        await CreateBaseModelData(_historyViewModel);
        return RedirectToAction("Index", "WorkStandards");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(HistoryViewModel historyViewModelPost, bool submit)
    {
        var startDate = DateUtil.DateTimeFromString(historyViewModelPost.StartDate);
        var endDate = DateUtil.DateTimeFromString(historyViewModelPost.EndDate);

        // If the data is submitted, add it to the db
        if (submit)
        {
            var historicalDataDb = await _historyRepository.GetHistoricalData(startDate, endDate);
            await _historyRepository.UpdateHistoricalDataInDb(historyViewModelPost.DateValues, historicalDataDb);
        }

        // Get all dates between the chosen values
        await CreateBaseModelData(historyViewModelPost);
        return View(historyViewModelPost);
    }

    private async Task CreateBaseModelData(HistoryViewModel historyViewModel)
    {
        await CreateLocalHistoricalDataWithDbData(historyViewModel);
        await RetrieveWorkStandardData(historyViewModel.WorkStandardsViewModel);
    }

    public async Task RetrieveWorkStandardData(WorkStandardsViewModel workStandardsViewModel)
    {
        workStandardsViewModel.Uitladen = await _workStandardsRepository.GetUitladenWorkStandard();
        workStandardsViewModel.VakkenVullen = await _workStandardsRepository.GetVakkenVullenWorkStandard();
        workStandardsViewModel.Kassa = await _workStandardsRepository.GetKassaWorkStandard();
        workStandardsViewModel.Vers = await _workStandardsRepository.GetVersWorkStandard();
        workStandardsViewModel.Spiegelen = await _workStandardsRepository.GetSpiegelenWorkStandard();
    }

    // Create local historical data based on a viewmodel
    public async Task CreateLocalHistoricalDataWithDbData(HistoryViewModel historyViewModel)
    {
        var startDate = DateUtil.DateTimeFromString(historyViewModel.StartDate);
        var endDate = DateUtil.DateTimeFromString(historyViewModel.EndDate);

        historyViewModel.DateValues = await GetLocalHistoricalData(startDate, endDate);
        var databaseData = await _historyRepository.GetHistoricalData(startDate, endDate);
        UpdateListToCorrectDbInfo(historyViewModel.DateValues, databaseData);
    }

    // Create local historical data between the start and end date
    public async Task<List<HistoricalData>> GetLocalHistoricalData(DateTime startDate, DateTime endDate)
    {
        var user = await _userService.GetUserAdvanced();
        var dateValues = new List<HistoricalData>();
        for (var dt = startDate.Date; dt <= endDate.Date; dt = dt.AddDays(1))
        {
            dateValues.Add(new HistoricalData { Date = dt, AmountCustomers = 0, AmountColi = 0, Branch = user.Branch });
        }

        return dateValues;
    }

    // Update all items of the localHistoricalData to get correct values from the DB if there is any data
    public void UpdateListToCorrectDbInfo(List<HistoricalData> localHistoricalData, List<HistoricalData> databaseHistoricalData)
    {
        foreach (var databaseItem in databaseHistoricalData)
        {
            foreach (var localItem in localHistoricalData.Where(localItem => localItem.Date.ToShortDateString() == databaseItem.Date.ToShortDateString()))
            {
                localItem.AmountColi = databaseItem.AmountColi;
                localItem.AmountCustomers = databaseItem.AmountCustomers;
            }
        }
    }
}