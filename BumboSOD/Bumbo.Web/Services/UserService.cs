using Bumbo.Data;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Bumbo.Web.Services;

public class UserService : IUserService
{
    private readonly BumboDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(BumboDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<ApplicationUser> GetUser()
    {
        return await _context.Users.FirstOrDefaultAsync(o => o.Id == GetUserId())
               ?? throw new InvalidOperationException();
    }

    public async Task<ApplicationUser> GetUserAdvanced()
    {
        return await _context.Users
                   .IncludeOptimized(u => u.Branch)
                   .IncludeOptimized(u => u.Departments)
                   .FirstOrDefaultAsync(u => u.Id == GetUserId())
               ?? throw new InvalidOperationException();
    }

    public string GetUserId()
    {
        return _userManager.GetUserId(_httpContextAccessor.HttpContext!.User);
    }
}