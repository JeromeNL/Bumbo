using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IImportRepository
{
    Task ImportEmployees(List<ApplicationUser> users);
    Task<List<Branch>> GetAllBranches();
}