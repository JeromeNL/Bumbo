using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IUserManagerRepository
{
    Task<List<Department>> GetAllDepartments();

    Task<List<Department>> GetAllDepartmentsForUser(ApplicationUser user);

    Task SaveChanges();

    Task<List<Branch>> GetAllBranches();

    Task<List<Department>> GetDepartmentsByIds(List<int> selectedDepartments);

    Task<ApplicationUser> GetUserAdvancedWithAddressById(string id);

    Task UpdateUserDepartments(ApplicationUser user, List<Department> selectedDepartments);

    Task ReactivateUser(string id);
}