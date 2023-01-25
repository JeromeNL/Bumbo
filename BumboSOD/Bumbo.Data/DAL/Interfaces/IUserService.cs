using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IUserService
{
    string GetUserId();

    Task<ApplicationUser> GetUser();

    Task<ApplicationUser> GetUserAdvanced();
}