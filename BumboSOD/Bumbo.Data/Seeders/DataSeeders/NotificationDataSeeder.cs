using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class NotificationDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<Notification> GenerateNotifications(List<ApplicationUser> employeeList)
    {
        var manager = employeeList[0];
        var notifications = new List<Notification>();
        var testNotification = new Notification
        {
            Id = ++Id,
            EmployeeId = manager.Id,
            Href = "/schedule",
            Message = "Het rooster van volgende week is nog niet ingepland",
            Title = "Rooster nog niet ingepland",
            IsRead = false,
            HappenedAt = DateTime.Now.AddDays(-1)
        };
        notifications.Add(testNotification);
        return notifications;
    }
}