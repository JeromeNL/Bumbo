using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class ImportRepository : IImportRepository
{
    private readonly BumboDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ImportRepository(BumboDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task ImportEmployees(List<ApplicationUser> users)
    {
        await _context.Users.AddRangeAsync(users);

        var employeeRole = await _roleManager.FindByNameAsync(nameof(Role.Employee));
        var employeeRoleId = employeeRole.Id;

        foreach (var user in users)
        {
            await _context.UserRoles.AddAsync(new IdentityUserRole<string> { RoleId = employeeRoleId, UserId = user.Id });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Branch>> GetAllBranches()
    {
        return await _context.Branches.AsNoTracking().ToListAsync();
    }
}