using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class ManagerExchangeRequestViewModel
{
    public List<ExchangeRequest>? ExchangeRequests { get; set; }
    public ApplicationUser? UserLoggedIn { get; set; }
}