using System.Globalization;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.Admin))]
public class BranchManagementController : Controller
{
    private readonly IBranchManagementRepository _branchManagementRepository;

    public BranchManagementController(IBranchManagementRepository branchManagementRepository)
    {
        _branchManagementRepository = branchManagementRepository;
    }

    public async Task<IActionResult> Index(BranchManagementViewModel branchManagementViewModel)
    {
        branchManagementViewModel = new BranchManagementViewModel();
        await RetrieveBranchData(branchManagementViewModel, "Empty");
        return View(branchManagementViewModel);
    }

    private async Task<BranchManagementViewModel> RetrieveBranchData(BranchManagementViewModel branchManagementViewModel, string currentPartial)
    {
        //Adding Days of week for view
        var allDays = new List<DayOfWeek>();
        for (var i = 0; i < 7; i++)
        {
            var date = new DateTime(2021, 1, i + 4, new CultureInfo("nl-NL").Calendar).DayOfWeek;
            allDays.Add(date);
        }

        branchManagementViewModel.DaysOfWeek = allDays;
        branchManagementViewModel.AllBranches = await _branchManagementRepository.GetAllBranches();
        branchManagementViewModel.CurrentView = currentPartial;
        return branchManagementViewModel;
    }

    public async Task<IActionResult> GetBranchById(int id, BranchManagementViewModel branchManagementViewModel)
    {
        var selectedBranch = await _branchManagementRepository.GetBranchById(id);
        branchManagementViewModel.SelectedBranch = selectedBranch;
        var currentViewModel = await RetrieveBranchData(branchManagementViewModel, "Read");
        return View("Index", currentViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewBranch(BranchManagementViewModel branchManagementViewModel)
    {
        var newBranch = branchManagementViewModel.AddedBranch;

        //Adding each day of opening hour to the database
        var isValid = CheckIfOpeningHoursAreValid(newBranch);
        if (!isValid.Equals(""))
        {
            return View("Error", new ErrorViewModel { RequestId = isValid });
        }

        await _branchManagementRepository.CreateBranch(newBranch);
        var currentViewModel = await RetrieveBranchData(branchManagementViewModel, "Read");
        return View("Index", currentViewModel);
    }

    private string CheckIfOpeningHoursAreValid(Branch branch)
    {
        for (var i = 0; i < branch.OpeningHours.Count; i++)
        {
            branch.OpeningHours[i].Day = (DayOfWeek)i;
            var openingTime = branch.OpeningHours[i].OpeningTime;
            var closingTime = branch.OpeningHours[i].ClosingTime;

            if (openingTime > closingTime)
            {
                return "Openingstijd kan niet groter zijn dan de sluittijd";
            }

            if (openingTime == closingTime)
            {
                return "Openingstijd kan niet gelijk zijn aan sluittijd, vul het veld goed in";
            }
        }

        return "";
    }

    public async Task<IActionResult> Create(BranchManagementViewModel branchManagementViewModel)
    {
        branchManagementViewModel.AddedBranch = branchManagementViewModel.SelectedBranch;

        var currentViewModel = await RetrieveBranchData(branchManagementViewModel, "Create");
        return View("Index", currentViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditBranch(BranchManagementViewModel branchManagementViewModel)
    {
        var editedBranch = branchManagementViewModel.EditedBranch;

        var isValid = CheckIfOpeningHoursAreValid(editedBranch);
        if (!isValid.Equals(""))
        {
            return View("Error", new ErrorViewModel { RequestId = isValid });
        }

        var changedOpeningHours = editedBranch.OpeningHours;
        var changedAddress = editedBranch.Address;
        await _branchManagementRepository.UpdateBranch(editedBranch, changedOpeningHours, changedAddress);
        var currentViewModel = await RetrieveBranchData(branchManagementViewModel, "Read");
        return View("Index", currentViewModel);
    }

    public async Task<IActionResult> Edit(int id, BranchManagementViewModel branchManagementViewModel)
    {
        branchManagementViewModel.EditedBranch = await _branchManagementRepository.GetBranchById(id);
        var currentViewModel = await RetrieveBranchData(branchManagementViewModel, "Edit");
        return View("Index", currentViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBranch(BranchManagementViewModel branchManagementViewModel)
    {
        var deletedBranch = branchManagementViewModel.EditedBranch;
        await _branchManagementRepository.DeleteBranch(deletedBranch);
        var currentViewModel = await RetrieveBranchData(branchManagementViewModel, "Empty");
        return View("Index", currentViewModel);
    }
}