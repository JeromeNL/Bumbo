using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetUnreadNotifications(string userId);
    Task CreateNotification(Notification notification);
    Task MarkAsRead(int notificationId);
    Task MarkAllAsRead();
    Task RemoveUnreadNotificationByTitle(string title);
    Task<DateTime> GetLastPlannedDay();
    Task<DateTime?> GetNotApprovedClockedHourDay();
    Task<Dictionary<DateTime, double>> GetFutureShiftTotalHoursPerDay(int maxDays);
    Task<Dictionary<DateTime, double>> GetFuturePrognosisTotalHoursPerDay(int maxDays);
}