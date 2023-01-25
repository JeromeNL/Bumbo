using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class UserManagerRepository : IUserManagerRepository
{
    private readonly BumboDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public UserManagerRepository(BumboDbContext context, IUserService userService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userService = userService;
        _userManager = userManager;
    }

    public async Task<List<Department>> GetAllDepartments()
    {
        return await _context.Departments.AsNoTracking().ToListAsync();
    }

    public async Task<List<Department>> GetAllDepartmentsForUser(ApplicationUser user)
    {
        return await _context.Departments.AsNoTracking().Where(d => d.Employees!.Any(e => e.EmployeeId == user.Id)).ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Branch>> GetAllBranches()
    {
        return await _context.Branches.AsNoTracking().ToListAsync();
    }

    public async Task<List<Department>> GetDepartmentsByIds(List<int> selectedDepartments)
    {
        return await _context.Departments.AsNoTracking().Where(d => selectedDepartments.Contains(d.Id)).ToListAsync();
    }

    public async Task<ApplicationUser> GetUserAdvancedWithAddressById(string id)
    {
        return (await _context.Users
            .Include(u => u.Branch)
            .Include(u => u.Departments)!
            .ThenInclude(u => u.Department)
            .Include(u => u.Address)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id))!;
    }

    public async Task UpdateUserDepartments(ApplicationUser user, List<Department> selectedDepartments)
    {
        var currentDepartments = _context.ApplicationUserDepartments.Where(ud => ud.EmployeeId == user.Id).ToList();
        var currentDepartmentIds = currentDepartments.Select(dep => dep.DepartmentId);

        currentDepartments.RemoveAll(ud => ud.EmployeeId == user.Id && currentDepartmentIds.Contains(ud.DepartmentId));

        _context.ApplicationUserDepartments.RemoveRange(currentDepartments);

        await _context.SaveChangesAsync();

        await _context.ApplicationUserDepartments.AddRangeAsync(selectedDepartments.Select(d => new ApplicationUserDepartment
        {
            DepartmentId = d.Id,
            EmployeeId = user.Id
        }));
    }

    public async Task ReactivateUser(string id)
    {
        var loggedInUser = await _userService.GetUser();
        var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser);

        var selectedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        string roleToBeAdded = null;

        if (loggedInUserRole.Contains(nameof(Role.Admin)))
        {
            roleToBeAdded = nameof(Role.BranchManager);
        }
        else if (loggedInUserRole.Contains(nameof(Role.BranchManager)))
        {
            roleToBeAdded = nameof(Role.Employee);
        }

        await _userManager.AddToRoleAsync(selectedUser, roleToBeAdded);
    }
}