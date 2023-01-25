using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Web.Services.Interfaces;

namespace Bumbo.Web.Services;

public class NotificationService : INotificationService
{
    INotificationRepository _notificationRepository;
    IUserService _userService;

    public NotificationService(IUserService userService, INotificationRepository notificationRepository)
    {
        _userService = userService;
        _notificationRepository = notificationRepository;
    }

    public async Task<List<Notification>> GetUnreadNotifications()
    {
        var user = await _userService.GetUser();
        return await _notificationRepository.GetUnreadNotifications(user.Id);
    }

    public async Task CheckForNewNotifications()
    {
        await CheckForEmptyWeeks();
        await CheckForNotApprovedClockedHours();
        await CheckForLeastPrognosisMatchedDay();
    }

    private async Task CheckForEmptyWeeks()
    {
        var notificationTitle = "Niet ingeplande dag";
        var user = await _userService.GetUser();
        var notifications = await _notificationRepository.GetUnreadNotifications(user.Id);
        var lastPlannedDay = await _notificationRepository.GetLastPlannedDay();
        if (DateTime.Now - lastPlannedDay > TimeSpan.FromDays(30)) return;
        var notificationHref = "/Schedule/DayInfo?inputDateTime=" + lastPlannedDay.AddDays(1).ToString("yyyy-MM-dd");
        if (notifications.Any(n => n.Title == notificationTitle && n.IsRead == false))
        {
            await _notificationRepository.RemoveUnreadNotificationByTitle(notificationTitle);
        }

        var notificationMessage = "Je hebt een niet ingeplande dag in " + ((lastPlannedDay.AddDays(1) - DateTime.Now).Days + " dagen");
        await CreateNotificationInDb(user.Id, notificationTitle, notificationMessage, DateTime.Now, notificationHref);
    }

    private async Task CheckForNotApprovedClockedHours()
    {
        var notificationTitle = "Niet goedgekeurde uren";
        var user = await _userService.GetUser();
        var notifications = await _notificationRepository.GetUnreadNotifications(user.Id);
        var lastPlannedDay = await _notificationRepository.GetNotApprovedClockedHourDay();
        if (lastPlannedDay == null) return;
        var notificationHref = "/Schedule/DayInfo?inputDateTime=" + lastPlannedDay?.ToString("yyyy-MM-dd");
        if (notifications.Any(n => n.Title == notificationTitle && n.IsRead == false))
        {
            await _notificationRepository.RemoveUnreadNotificationByTitle(notificationTitle);
        }

        var notificationMessage = "Je hebt niet goedgekeurde uren van " + ((DateTime.Now - lastPlannedDay)?.Days + " dagen geleden");
        await CreateNotificationInDb(user.Id, notificationTitle, notificationMessage, DateTime.Now, notificationHref);
    }

    private async Task CheckForLeastPrognosisMatchedDay()
    {
        var notificationTitle = "Minst matchende prognose";
        var user = await _userService.GetUser();
        var notifications = await _notificationRepository.GetUnreadNotifications(user.Id);

        // Get the prognosis and the shifts per day
        var maxDays = 4;
        var totalShiftHoursPerDay = await _notificationRepository.GetFutureShiftTotalHoursPerDay(maxDays);
        var totalPrognosisHoursPerDay = await _notificationRepository.GetFuturePrognosisTotalHoursPerDay(maxDays);
        if (totalShiftHoursPerDay.Count == 0 && totalPrognosisHoursPerDay.Count == 0) return;
        // Find out which day has the least matched prognosis
        var leastMatchedDay = totalShiftHoursPerDay.FirstOrDefault().Key;
        var biggestDifference = 0.0;
        for (var dateTime = DateTime.Now.Date; dateTime < DateTime.Now.AddDays(maxDays); dateTime = dateTime.AddDays(1))
        {
            if (totalShiftHoursPerDay.ContainsKey(dateTime) && totalPrognosisHoursPerDay.ContainsKey(dateTime))
            {
                var difference = Math.Abs(totalPrognosisHoursPerDay[dateTime] - totalShiftHoursPerDay[dateTime]);
                if (difference > biggestDifference)
                {
                    leastMatchedDay = dateTime;
                    biggestDifference = difference;
                }
            }
        }

        if (biggestDifference < 1) return;

        var notificationHref = "/Schedule/DayInfo?inputDateTime=" + leastMatchedDay.ToString("yyyy-MM-dd");
        if (notifications.Any(n => n.Title == notificationTitle && n.IsRead == false))
        {
            await _notificationRepository.RemoveUnreadNotificationByTitle(notificationTitle);
        }

        var notificationMessage = "Je rooster komt het minst overeen met de prognose in " + ((leastMatchedDay - DateTime.Now).Days + " dagen" + " (" + Math.Round(biggestDifference, 2) + " uur verschil)");
        await CreateNotificationInDb(user.Id, notificationTitle, notificationMessage, DateTime.Now, notificationHref);
    }

    private async Task CreateNotificationInDb(string employeeId, string title, string message, DateTime happenedAt, string href)
    {
        var notification = new Notification()
        {
            EmployeeId = employeeId,
            Title = title,
            Message = message,
            HappenedAt = happenedAt,
            Href = href
        };
        await _notificationRepository.CreateNotification(notification);
    }
}