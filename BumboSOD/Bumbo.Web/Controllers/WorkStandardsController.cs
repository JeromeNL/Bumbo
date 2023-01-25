using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.BranchManager))]
public class WorkStandardsController : Controller
{
    private readonly IUserService _userService;
    private readonly IWorkStandardsRepository _workStandardsRepository;
    private readonly WorkStandardsViewModel _workStandardsViewModel;

    public WorkStandardsController(IUserService userService, IWorkStandardsRepository workStandardsRepository)
    {
        _userService = userService;
        _workStandardsRepository = workStandardsRepository;
        _workStandardsViewModel = new WorkStandardsViewModel();
    }

    /// <summary>
    /// Shows a list of all current and previous work standards for a specific branch the current logged in user belongs to.
    /// </summary>
    /// <returns>A view containing all workstandards for a certain branch.</returns>
    // GET: WorkStandards
    public async Task<IActionResult> Index()
    {
        await RetrieveWorkStandardData(_workStandardsViewModel);
        return View(_workStandardsViewModel);
    }

    /// <summary>
    /// Shows the view for creating a new set of workstandards.
    /// </summary>
    /// <returns>A view for creating a new set of workstandards.</returns>
    // GET: WorkStandards/Create
    public async Task<IActionResult> Create()
    {
        await RetrieveWorkStandardData(_workStandardsViewModel);
        return View(_workStandardsViewModel);
    }

    /// <summary>
    /// Posts the entered data in the create view to the database, adding new workstandards for each type of work.
    /// </summary>
    /// <param name="viewModel">ViewModel with the user input for the new Workstandards</param>
    /// <returns>Redirects the user to the workstandardIndex</returns>
    // POST: WorkStandards/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkStandardsViewModel viewModel)
    {
        var user = await _userService.GetUserAdvanced();
        await RetrieveWorkStandardData(_workStandardsViewModel);

        viewModel.SetWorkStandardTypes();

        viewModel.Kassa.Branch = user.Branch;
        viewModel.Uitladen.Branch = user.Branch;
        viewModel.VakkenVullen.Branch = user.Branch;
        viewModel.Vers.Branch = user.Branch;
        viewModel.Spiegelen.Branch = user.Branch;

        List<WorkStandards> workStandards = new()
        {
            viewModel.Kassa,
            viewModel.Uitladen,
            viewModel.VakkenVullen,
            viewModel.Vers,
            viewModel.Spiegelen
        };

        await _workStandardsRepository.AddWorkStandards(workStandards);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Retrieves the current workstandards and the past workstandards for the branch the current user belongs to, adds it to the view model and returns it.
    /// </summary>
    /// <param name="workStandardsViewModel">The view model to be filled with data</param>
    /// <returns>A view model with al necessary data</returns>
    public async Task<WorkStandardsViewModel> RetrieveWorkStandardData(
        WorkStandardsViewModel workStandardsViewModel)
    {
        workStandardsViewModel.Uitladen = await _workStandardsRepository.GetUitladenWorkStandard();
        workStandardsViewModel.VakkenVullen = await _workStandardsRepository.GetVakkenVullenWorkStandard();
        workStandardsViewModel.Kassa = await _workStandardsRepository.GetKassaWorkStandard();
        workStandardsViewModel.Vers = await _workStandardsRepository.GetVersWorkStandard();
        workStandardsViewModel.Spiegelen = await _workStandardsRepository.GetSpiegelenWorkStandard();
        workStandardsViewModel.PastWorkStandards = await _workStandardsRepository.GetPastWorkStandardsDictionary(workStandardsViewModel.Spiegelen);

        return workStandardsViewModel;
    }

    /// <summary>
    /// Action for easy access to the History main page.
    /// </summary>
    /// <returns>Redirects the user to the index page of the history controller.</returns>
    public IActionResult History()
    {
        return RedirectToAction("Index", "History");
    }
}