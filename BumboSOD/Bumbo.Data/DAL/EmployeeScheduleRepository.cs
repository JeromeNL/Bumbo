using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.Data.DAL;

public class EmployeeScheduleRepository : IEmployeeScheduleRepository
{
    private readonly BumboDbContext _context;

    public EmployeeScheduleRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shift>> GetEmployeeShiftsAsync(ApplicationUser? userLoggedIn)
    {
        return await _context.Shifts
            .IncludeOptimized(s => s.Employee)
            .IncludeOptimized(s => s.Department)
            .Where(s => s.EmployeeId.Equals(userLoggedIn!.Id) &&
                        s.IsPublished == true)
            .ToListAsync();
    }

    public List<Shift> OrderEmployeeShiftsAsync(List<Shift> shifts)
    {
        var orderedShifts = shifts
            .OrderBy(s => s.Start.Date)
            .ThenBy(s => s.Start.TimeOfDay)
            .ToList();
        return orderedShifts;
    }

    public async Task<bool> AddShiftToExchangeRequestAsync(int? id, ApplicationUser? userLoggedIn)
    {
        var clickedOnShift = await _context.Shifts.FindAsync(id);
        if (clickedOnShift == null)
        {
            return false;
        }

        var exchangeRequestWithSameId = await _context.ExchangeRequests
            .FirstOrDefaultAsync(er => er.ShiftId.Equals(id));
        if (exchangeRequestWithSameId == null)
        {
            var exchangeRequest = new ExchangeRequest
            {
                Shift = clickedOnShift,
                ShiftId = clickedOnShift.Id,
                OriginalUser = userLoggedIn,
                OriginalUserId = userLoggedIn!.Id,
                NewUser = null,
                NewUserId = null,
                IsApprovedByManager = null
            };
            _context.ExchangeRequests.Add(exchangeRequest);
        }
        else
        {
            exchangeRequestWithSameId.NewUser = null;
            exchangeRequestWithSameId.NewUserId = null;
            exchangeRequestWithSameId.IsApprovedByManager = null;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveShiftFromExchangeRequestAsync(int? id)
    {
        var clickedOnShift = await _context.Shifts.FindAsync(id);
        if (clickedOnShift == null)
        {
            return false;
        }

        var exchangeRequestWithSameId = _context.ExchangeRequests
            .FirstOrDefault(er => er.ShiftId.Equals(id));
        if (exchangeRequestWithSameId == null)
        {
            return false;
        }

        _context.ExchangeRequests.Remove(exchangeRequestWithSameId);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<ExchangeRequest>> GetExchangeRequestsFromUserLoggedInAsync(ApplicationUser? userLoggedIn)
    {
        var exchangeRequestFromUserLoggedIn = _context.ExchangeRequests
            .IncludeOptimized(er => er.Shift)
            .IncludeOptimized(er => er.OriginalUser)
            .IncludeOptimized(er => er.NewUser)
            .Where(er => er.OriginalUserId!.Equals(userLoggedIn!.Id))
            .ToListAsync();
        return exchangeRequestFromUserLoggedIn;
    }
}