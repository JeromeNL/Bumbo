using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class EmployeeExchangeRequestViewModel
{
    public List<ExchangeRequest>? ExchangeRequests { get; set; }
    public ApplicationUser? UserLoggedIn { get; set; }
}