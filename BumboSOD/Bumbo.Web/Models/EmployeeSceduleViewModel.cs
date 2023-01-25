using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class EmployeeSceduleViewModel
{
    public Dictionary<int, List<Shift>>? Shifts { get; set; }
    public ApplicationUser UserLoggedIn { get; set; }
    public List<ExchangeRequest> ExchangeRequests { get; set; }
}