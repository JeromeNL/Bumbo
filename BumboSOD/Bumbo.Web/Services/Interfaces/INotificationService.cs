using Bumbo.Data.Models;

namespace Bumbo.Web.Services.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> GetUnreadNotifications();

    Task CheckForNewNotifications();
}