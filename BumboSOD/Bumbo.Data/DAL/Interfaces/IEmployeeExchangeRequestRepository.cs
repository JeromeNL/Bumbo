using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IEmployeeExchangeRequestRepository
{
    Task<List<ExchangeRequest>> GetEmployeeExchangeRequestsAsync(ApplicationUser? userLoggedIn);

    Task<List<ExchangeRequest>> OrderEmployeeExchangeRequestAsync(IEnumerable<ExchangeRequest> exchangeRequests);

    Task<ExchangeRequest?> GetClickedEmployeeExchangeRequestAsync(int? id);

    Task<bool> SetNewUserAsync(ExchangeRequest? exchangeRequest, ApplicationUser userLoggedIn);

    Task<List<Shift>?> GetShiftsFromUserLoggedInAsync(ApplicationUser? userLoggedIn);

    bool CheckUserLoggedInHasShiftAlreadyAsync(ExchangeRequest? exchangeRequest, IEnumerable<Shift> shiftsFromUserLoggedIn);
}