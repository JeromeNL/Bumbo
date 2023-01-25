using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.BranchManager))]
public class ManagerExchangeRequestController : Controller
{
    private readonly IManagerExchangeRequestRepository _managerExchangeRequestRepository;
    private readonly ManagerExchangeRequestViewModel _managerExchangeRequestViewModel;
    private readonly IUserService _userService;

    public ManagerExchangeRequestController(IManagerExchangeRequestRepository managerExchangeRequestRepository, IUserService userService)
    {
        _userService = userService;
        _managerExchangeRequestViewModel = new ManagerExchangeRequestViewModel();
        _managerExchangeRequestRepository = managerExchangeRequestRepository;
    }

    public async Task<IActionResult> Index()
    {
        var managerLoggedIn = await _userService.GetUserAdvanced();
        _managerExchangeRequestViewModel.ExchangeRequests = await _managerExchangeRequestRepository.GetExchangeRequestsAsync();
        _managerExchangeRequestViewModel.UserLoggedIn = managerLoggedIn;

        return View(_managerExchangeRequestViewModel);
    }

    public async Task<IActionResult> ApprovedOrRejectedExchangeRequestByManager(int? id, bool isApproved)
    {
        if (id == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "Het ruilverzoek kon niet juist worden opgehaald. Probeer het later nogmaals." });
        }

        var exchangeRequest = await _managerExchangeRequestRepository.GetApprovedOrDeclinedExchangeRequestAsync(id);

        if (exchangeRequest == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "Het ruilverzoek kon niet juist worden opgehaald. Probeer het later nogmaals." });
        }

        if (isApproved == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "Er is iets misgegaan bij het goedkeuren of afkeuren. Probeer het later nogmaals." });
        }

        var shiftOfExchangeRequest = await _managerExchangeRequestRepository.GetShiftAsync(exchangeRequest.ShiftId);

        if (shiftOfExchangeRequest == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "De shift van het ruilverzoek kon niet juist worden opgehaald. Probeer het later nogmaals." });
        }

        // The following line sets the exchange request to approved or rejected and returns whether or not it successfully completed doing so.
        var successfullyHandledExchangeRequest = await _managerExchangeRequestRepository.SetApprovedOrDeclinedExchangeRequestAsync(exchangeRequest, exchangeRequest.NewUser, shiftOfExchangeRequest, isApproved);

        if (successfullyHandledExchangeRequest == false)
        {
            return View("Error", new ErrorViewModel { RequestId = "Ruilverzoek kan niet worden goedgekeurd of afgewezen" });
        }

        return RedirectToAction("Index", "ManagerExchangeRequest", new { id = exchangeRequest.Id });
    }
}