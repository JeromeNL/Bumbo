using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.Data.DAL;

public class EmployeeExchangeRequestRepository : IEmployeeExchangeRequestRepository
{
    private readonly BumboDbContext _context;

    public EmployeeExchangeRequestRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExchangeRequest>> GetEmployeeExchangeRequestsAsync(ApplicationUser? userLoggedIn)
    {
        var suitableExchangeRequestsForLoggedInUser = await _context.ExchangeRequests
            .IncludeOptimized(er => er.Shift)
            .IncludeOptimized(er => er.Shift!.Department)
            .IncludeOptimized(er => er.Shift!.Employee)
            .IncludeOptimized(er => er.OriginalUser)
            .IncludeOptimized(er => er.OriginalUser!.Branch)
            .Where(er =>
                er.Shift!.IsPublished &&
                er.NewUser == null &&
                userLoggedIn!.Departments.Select(x => x.DepartmentId).Contains(er.Shift!.Department.Id) &&
                userLoggedIn.Branch!.Equals(er.OriginalUser!.Branch) &&
                er.Shift.Start > DateTime.Now.AddHours(1))
            .ToListAsync();

        var getShiftsLogged = await GetShiftsFromUserLoggedInAsync(userLoggedIn) ?? new List<Shift>();
        suitableExchangeRequestsForLoggedInUser = suitableExchangeRequestsForLoggedInUser
            .Where(er => CheckUserLoggedInHasShiftAlreadyAsync(er, getShiftsLogged).Equals(false))
            .ToList();

        return suitableExchangeRequestsForLoggedInUser;
    }

    public Task<List<ExchangeRequest>> OrderEmployeeExchangeRequestAsync(IEnumerable<ExchangeRequest> exchangeRequests)
    {
        return Task.FromResult(exchangeRequests
            .OrderBy(er => er.Shift!.Start.Date)
            .ThenBy(er => er.Shift!.Start.TimeOfDay)
            .ThenBy(er => er.OriginalUser!.FirstName)
            .ToList());
    }

    public async Task<ExchangeRequest?> GetClickedEmployeeExchangeRequestAsync(int? id)
    {
        return await _context.ExchangeRequests
            .IncludeOptimized(er => er.Shift)
            .FirstOrDefaultAsync(er => er.Id == id && er.Shift!.Id == er.ShiftId);
    }

    public async Task<bool> SetNewUserAsync(ExchangeRequest? exchangeRequest, ApplicationUser? userLoggedIn)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName.Equals(userLoggedIn!.UserName));
        exchangeRequest!.NewUser = user;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Shift>?> GetShiftsFromUserLoggedInAsync(ApplicationUser? userLoggedIn)
    {
        if (userLoggedIn == null) return null;

        return await _context.Shifts
            .IncludeOptimized(s => s.Employee)
            .Where(s => s.Employee.UserName.Equals(userLoggedIn.UserName))
            .ToListAsync();
    }

    public bool CheckUserLoggedInHasShiftAlreadyAsync(ExchangeRequest? exchangeRequest, IEnumerable<Shift> shiftsFromUserLoggedIn)
    {
        foreach (var shiftFromUserLoggedIn in shiftsFromUserLoggedIn)
        {
            if (shiftFromUserLoggedIn.Start.Date.Equals(exchangeRequest!.Shift!.Start.Date))
            {
                return shiftFromUserLoggedIn.Start.TimeOfDay < exchangeRequest.Shift!.End.TimeOfDay &&
                       (shiftFromUserLoggedIn.Start.TimeOfDay > exchangeRequest.Shift.Start.TimeOfDay ||
                        shiftFromUserLoggedIn.End.TimeOfDay > exchangeRequest.Shift.Start.TimeOfDay) &&
                       shiftFromUserLoggedIn.End.TimeOfDay > exchangeRequest.Shift.Start.TimeOfDay &&
                       (shiftFromUserLoggedIn.End.TimeOfDay < exchangeRequest.Shift.End.TimeOfDay ||
                        shiftFromUserLoggedIn.Start.TimeOfDay < exchangeRequest.Shift.End.TimeOfDay) &&
                       !shiftFromUserLoggedIn.Employee.Equals(exchangeRequest.OriginalUser);
            }
        }

        return false;
    }
}