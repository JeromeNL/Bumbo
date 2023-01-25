using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IEmployeeScheduleRepository
{
    Task<List<Shift>> GetEmployeeShiftsAsync(ApplicationUser? userLoggedIn);

    List<Shift> OrderEmployeeShiftsAsync(List<Shift> shifts);

    Task<bool> AddShiftToExchangeRequestAsync(int? id, ApplicationUser? userLoggedIn);

    Task<bool> RemoveShiftFromExchangeRequestAsync(int? id);

    Task<List<ExchangeRequest>> GetExchangeRequestsFromUserLoggedInAsync(ApplicationUser? userLoggedIn);
}