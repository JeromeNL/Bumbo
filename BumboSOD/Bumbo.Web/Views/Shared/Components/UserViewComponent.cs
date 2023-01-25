using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Web.Models;
using Bumbo.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Views.Shared.Components;

public class UserView : ViewComponent
{
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public UserView(IUserService userService, INotificationService notificationService, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userService.GetUser();
        await _notificationService.CheckForNewNotifications();
        var notifications = await _notificationService.GetUnreadNotifications();
        var userRole = await _userManager.GetRolesAsync(user);
        var userRoleName = userRole.FirstOrDefault() ?? "No role";
        var viewModel = new UserViewModel
        {
            Notifications = notifications,
            UserInfo = (user, userRoleName)
        };
        return View(viewModel);
    }
}