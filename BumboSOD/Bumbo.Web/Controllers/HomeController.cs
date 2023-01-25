using System.Diagnostics;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        // See if the user is logged in
        if (!User.Identity.IsAuthenticated)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        if (User.IsInRole(nameof(Role.BranchManager)))
            return RedirectToAction("Index", "Schedule");

        if (User.IsInRole(nameof(Role.Employee)))
            return RedirectToAction("Index", "EmployeeSchedule");

        if (User.IsInRole(nameof(Role.Admin)))
            return View("AdminIndex");

        return View("NotAuthorized");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}