using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IManagerExchangeRequestRepository
{
    Task<List<ExchangeRequest>> GetExchangeRequestsAsync();

    Task<bool> SetApprovedOrDeclinedExchangeRequestAsync(ExchangeRequest exchangeRequest, ApplicationUser newUser, Shift shift, bool isApproved);

    Task<ExchangeRequest> GetApprovedOrDeclinedExchangeRequestAsync(int? id);

    Task<Shift> GetShiftAsync(int? id);
}