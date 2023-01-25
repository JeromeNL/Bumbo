using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

public class EmployeeManagerController : Controller
{
    private readonly IUserService _userService;

    public EmployeeManagerController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = nameof(Role.Employee))]
    public async Task<IActionResult> EmployeeUserOverview()
    {
        var user = await _userService.GetUser();
        return View(user);
    }
}