using Bumbo.Data.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.Seeders.DataSeeders;

public static class IdentitySeeder
{
    private const string _adminRole = nameof(Role.Admin);
    private const string _branchmanagerRole = nameof(Role.BranchManager);
    private const string _employeeRole = nameof(Role.Employee);

    public static List<IdentityRole> SeedRoles(this ModelBuilder modelBuilder)
    {
        var roles = new List<IdentityRole>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = _adminRole, NormalizedName = _adminRole.ToUpper() },
            new() { Id = Guid.NewGuid().ToString(), Name = _branchmanagerRole, NormalizedName = _branchmanagerRole.ToUpper() },
            new() { Id = Guid.NewGuid().ToString(), Name = _employeeRole, NormalizedName = _employeeRole.ToUpper() }
        };
        modelBuilder.Entity<IdentityRole>().HasData(roles);
        return roles;
    }
}