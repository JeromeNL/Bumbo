using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [HttpPost]
    public async Task MarkNotificationAsRead(Notification notification)
    {
        await _notificationRepository.MarkAsRead(notification.Id);
    }

    [HttpPost]
    public async Task MarkAllNotificationsAsRead()
    {
        await _notificationRepository.MarkAllAsRead();
    }
}