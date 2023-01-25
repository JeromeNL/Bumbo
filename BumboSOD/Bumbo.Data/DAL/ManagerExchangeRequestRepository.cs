using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.Data.DAL;

public class ManagerExchangeRequestRepository : IManagerExchangeRequestRepository
{
    private readonly BumboDbContext _context;

    public ManagerExchangeRequestRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExchangeRequest>> GetExchangeRequestsAsync()
    {
        var exchangeRequests = await _context.ExchangeRequests
            .IncludeOptimized(e => e.Shift)
            .IncludeOptimized(e => e.Shift!.Department)
            .IncludeOptimized(e => e.OriginalUser)
            .IncludeOptimized(e => e.NewUser)
            .Where(e => e.IsApprovedByManager == null &&
                        e.NewUser != null &&
                        e.OriginalUser!.Branch!.Equals(e.NewUser.Branch))
            .OrderBy(e => e.Shift!.Start.Date)
            .ThenBy(e => e.Shift!.Start.TimeOfDay)
            .ThenBy(e => e.OriginalUser!.LastName)
            .ToListAsync();
        return exchangeRequests;
    }

    public async Task<bool> SetApprovedOrDeclinedExchangeRequestAsync(ExchangeRequest exchangeRequest, ApplicationUser newUser, Shift shift, bool isApproved)
    {
        exchangeRequest.IsApprovedByManager = isApproved;
        if (isApproved)
            shift.Employee = newUser;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ExchangeRequest> GetApprovedOrDeclinedExchangeRequestAsync(int? id)
    {
        return await _context.ExchangeRequests
            .IncludeOptimized(e => e.Shift)
            .IncludeOptimized(e => e.Shift.Employee)
            .IncludeOptimized(e => e.NewUser)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Shift> GetShiftAsync(int? id)
    {
        return await _context.Shifts.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}