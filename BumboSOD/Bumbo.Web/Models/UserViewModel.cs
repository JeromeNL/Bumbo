using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class UserViewModel
{
    public List<Notification> Notifications { get; set; }

    public (ApplicationUser user, string role) UserInfo { get; set; }
}