using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.Data.DAL;

public class NotificationRepository : INotificationRepository
{
    private readonly BumboDbContext _context;
    private readonly IUserService _userService;

    public NotificationRepository(BumboDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<List<Notification>> GetUnreadNotifications(string userId)
    {
        return await _context.Notifications.AsNoTracking().Where(o => o.EmployeeId.Equals(userId) && !o.IsRead).ToListAsync();
    }

    public async Task CreateNotification(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAsRead(int notificationId)
    {
        var userId = _userService.GetUserId();
        var notification = await _context.Notifications
            .Where(o => o.Id == notificationId && o.EmployeeId == userId)
            .FirstOrDefaultAsync();
        if (notification == null) return;
        notification.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsRead()
    {
        var userId = _userService.GetUserId();
        var notifications = await _context.Notifications
            .Where(o => o.EmployeeId == userId && !o.IsRead)
            .ToListAsync();
        notifications.ForEach(n => n.IsRead = true);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUnreadNotificationByTitle(string title)
    {
        // Get the correct notification
        var notification = await _context.Notifications
            .Where(o => o.Title == title && !o.IsRead)
            .FirstOrDefaultAsync();
        // Remove the notification if it exists
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DateTime> GetLastPlannedDay()
    {
        var user = await _userService.GetUser();
        var shift = await _context.Shifts.AsNoTracking().IncludeOptimized(s => s.Employee)
            .Where(o => o.Employee.BranchId == user.BranchId && o.Start > DateTime.Today)
            .OrderByDescending(o => o.Start)
            .FirstOrDefaultAsync();
        return shift?.Start ?? DateTime.Now.AddDays(-1);
    }

    public async Task<DateTime?> GetNotApprovedClockedHourDay()
    {
        var user = await _userService.GetUser();
        // Get the oldest clocked hour that is not approved
        var clockedHour = await _context.ClockedHours.Include(s => s.Employee)
            .Where(o => o.Employee.BranchId == user.BranchId && o.IsApproved == false)
            .OrderBy(o => o.ClockedIn)
            .FirstOrDefaultAsync();
        return clockedHour?.ClockedIn;
    }

    public async Task<Dictionary<DateTime, double>> GetFutureShiftTotalHoursPerDay(int maxDays)
    {
        var user = await _userService.GetUser();
        var shiftTotalHoursPerDay = await _context.Shifts.Include(s => s.Employee)
            .Where(o => o.Employee.BranchId == user.BranchId && o.Start > DateTime.Today && o.End < DateTime.Today.AddDays(maxDays))
            .ToListAsync();

        var result = shiftTotalHoursPerDay
            .GroupBy(o => o.Start.Date)
            .ToDictionary(o => o.Key, o => o.Sum(s => s.End.Subtract(s.Start).TotalHours));
        return result;
    }

    public async Task<Dictionary<DateTime, double>> GetFuturePrognosisTotalHoursPerDay(int maxDays)
    {
        var user = await _userService.GetUser();
        var prognosisTotalHoursPerDay = await _context.Prognoses
            .Where(o => o.BranchId == user.BranchId && o.Date > DateTime.Today && o.Date < DateTime.Today.AddDays(maxDays))
            .ToListAsync();

        var result = prognosisTotalHoursPerDay
            .GroupBy(o => o.Date).Select(o => new { Date = o.Key, TotalHours = o.Sum(s => decimal.ToDouble(s.ManHoursExpected)) })
            .ToDictionary(o => o.Date, o => o.TotalHours);
        return result;
    }
}