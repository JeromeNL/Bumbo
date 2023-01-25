using Bumbo.Data;
using Bumbo.Data.Models;

namespace Bumbo.Web.Services.HourRegistration;

public sealed class HourRegistrationService : IHourRegistrationService
{
    private readonly BumboDbContext _context;

    private const double _lockoutTime = 15.0;

    public HourRegistrationService(BumboDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// For the given uuid, decides if the user should be clocked in, or out
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<HourRegistrationResult> Handle(string uuid)
    {
        // Checks the database for the latest ClockedHours entry for given uuid
        // If it doesn't exists, Clock the user in
        // If one exists, but the ClockedOut is _NOT_ set, clock the user out
        // If one exists, but the ClockedOut is set, Clock the user in

        var userExists = _context.Users.Any(u => u.Id == uuid);

        if (!userExists)
        {
            return HourRegistrationResult.Error;
        }

        var currentTime = DateTime.Now;

        var previousClockedHours = _context.ClockedHours.OrderByDescending(ch => ch.ClockedIn).FirstOrDefault(hours => hours.EmployeeId == uuid);

        if (previousClockedHours is null)
        {
            return await ClockIn(uuid);
        }

        // Clocking in and out has a lockout period of 15 seconds.
        // This is to prevent unwanted time registrations
        var dateDiff = currentTime - previousClockedHours.ClockedIn;

        if (dateDiff.TotalSeconds <= _lockoutTime)
        {
            return HourRegistrationResult.ToSoon;
        }

        if (previousClockedHours.ClockedOut is null)
        {
            return await ClockOut(previousClockedHours);
        }

        return await ClockIn(uuid);
    }

    /// <summary>
    /// Creates a new ClockedHour entry in the database
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    private async Task<HourRegistrationResult> ClockIn(string uuid)
    {
        var newEntry = new ClockedHours
        {
            EmployeeId = uuid,
            ClockedIn = DateTime.Now,
            ClockedOut = null
        };

        _context.ClockedHours.Add(newEntry);

        await _context.SaveChangesAsync();

        return HourRegistrationResult.ClockedIn;
    }

    /// <summary>
    /// Sets the end time for given ClockedHour entry
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    private async Task<HourRegistrationResult> ClockOut(ClockedHours entry)
    {
        entry.ClockedOut = DateTime.Now;

        await _context.SaveChangesAsync();

        return HourRegistrationResult.ClockedOut;
    }
}