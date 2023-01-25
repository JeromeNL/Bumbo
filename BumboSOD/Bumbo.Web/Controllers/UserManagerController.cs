using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.BranchManager)}")]
public class UserManagerController : Controller
{
    private readonly IUserManagerRepository _repo;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public UserManagerController(IUserService userService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserManagerRepository repo)
    {
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
        _repo = repo;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        List<ApplicationUser> users;

        var isAdmin = User.IsInRole(nameof(Role.Admin));
        if (isAdmin)
        {
            users = await _userManager.Users.ToListAsync();
        }
        else
        {
            var loggedInManager = await _userService.GetUserAdvanced();
            users = await _userManager.Users.Where(u => u.BranchId == loggedInManager.BranchId).ToListAsync();
        }

        // Users with roles are counted as active
        var activeUsers = new List<ApplicationUser>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0) activeUsers.Add(user);
        }

        return View(activeUsers);
    }

    public async Task<IActionResult> UserDetails(string id)
    {
        var userToUpdate = await _repo.GetUserAdvancedWithAddressById(id);

        var currentUser = await _userService.GetUser();

        if (userToUpdate?.BranchId != currentUser.BranchId)
        {
            return NotFound();
        }

        var vm = await FillViewModel(id, false);

        return View(vm);
    }

    public async Task<IActionResult> EditUser(string id)
    {
        var vm = await FillViewModel(id, true);

        var userToUpdate = await _repo.GetUserAdvancedWithAddressById(id);

        var currentUser = await _userService.GetUser();

        if (userToUpdate?.BranchId != currentUser.BranchId)
        {
            return NotFound();
        }

        ViewBag.Departments = new MultiSelectList(vm.AllDepartments, "Name", "Name", vm.SelectedDepartments);

        var branches = await _repo.GetAllBranches();
        ViewBag.Branches = new SelectList(branches, "Id", "Name", vm.BranchId);
        return View(vm);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> EditUser(UserDetailsViewModel userDetailsViewModel)
    {
        if (!ModelState.IsValid)
        {
            userDetailsViewModel.AllDepartments = await _repo.GetAllDepartments();

            return View(userDetailsViewModel);
        }
        try
        {
            await ValidateUser(userDetailsViewModel);
        }
        catch (OperationException e)
        {
            Console.WriteLine(e);
            throw;
        }

        await _repo.SaveChanges();
        return RedirectToAction("UserDetails", new { id = userDetailsViewModel.Id });
    }

    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> MakeUserBranchManager(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == await _userService.GetUser())
        {
            return View("Error", new ErrorViewModel { RequestId = "Je kunt jezelf geen branchmanager maken. Hiermee haal je je eigen permissies namelijk weg." });
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        await _userManager.AddToRoleAsync(user, "BranchManager");
        return RedirectToAction("UserDetails", new { id });
    }

    [Authorize(Policy = "CanDeactivateAccounts")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.GetRolesAsync(user);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DeactivatedUsers()
    {
        List<ApplicationUser> users;

        var isAdmin = User.IsInRole("Admin");
        if (isAdmin)
        {
            users = await _userManager.Users.ToListAsync();
        }
        else
        {
            var loggedInManager = await _userService.GetUserAdvanced();
            users = await _userManager.Users.Where(u => u.BranchId == loggedInManager.BranchId).ToListAsync();
        }

        var deactivatedUsers = new List<ApplicationUser>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0) deactivatedUsers.Add(user);
        }

        return View(deactivatedUsers);
    }

    private async Task ValidateUser(UserDetailsViewModel userDetailsViewModel)
    {
        var user = await _repo.GetUserAdvancedWithAddressById(userDetailsViewModel.Id);

        if (user.FirstName != userDetailsViewModel.FirstName)
        {
            user.FirstName = userDetailsViewModel.FirstName;
        }

        if (user.MiddleName != userDetailsViewModel.MiddleName)
        {
            user.MiddleName = userDetailsViewModel.MiddleName;
        }

        if (user.LastName != userDetailsViewModel.LastName)
        {
            user.LastName = userDetailsViewModel.LastName;
        }

        if (user.BirthDate.Date.CompareTo(userDetailsViewModel.BirthDate.Date) != 0)
        {
            user.BirthDate = userDetailsViewModel.BirthDate;
        }

        if (user.Email != userDetailsViewModel.Email)
        {
            user.Email = userDetailsViewModel.Email;
        }

        if (user.PhoneNumber != userDetailsViewModel.PhoneNumber)
        {
            user.PhoneNumber = userDetailsViewModel.PhoneNumber;
        }

        if (user.PayoutScale != userDetailsViewModel.PayoutScale)
        {
            user.PayoutScale = userDetailsViewModel.PayoutScale;
        }

        if (user.Address.Zipcode != userDetailsViewModel.Zipcode)
        {
            user.Address.Zipcode = userDetailsViewModel.Zipcode;
        }

        if (user.Address.Street != userDetailsViewModel.Street)
        {
            user.Address.Street = userDetailsViewModel.Street;
        }

        if (user.Address.HouseNumber != userDetailsViewModel.HouseNumber)
        {
            user.Address.HouseNumber = userDetailsViewModel.HouseNumber;
        }

        if (user.Address.City != userDetailsViewModel.City)
        {
            user.Address.City = userDetailsViewModel.City;
        }

        if (userDetailsViewModel.BranchId != 0)
        {
            if (user.Branch.Id != userDetailsViewModel.BranchId)
            {
                var isEmployee = await _userManager.IsInRoleAsync(user, nameof(Role.Employee));
                if (!isEmployee)
                {
                    throw new OperationException();
                }

                user.BranchId = userDetailsViewModel.BranchId;
            }
        }

        var userDepartments = await _repo.GetAllDepartmentsForUser(user);

        if (userDetailsViewModel.SelectedDepartments != null)
        {
            var selectedDepartments = await _repo.GetDepartmentsByIds(userDetailsViewModel.SelectedDepartments);
            if (!selectedDepartments.SequenceEqual(userDepartments))
            {
                user.Departments.Clear();
                await _repo.UpdateUserDepartments(user, selectedDepartments);
            }
        }
    }

    private async Task<UserDetailsViewModel> FillViewModel(string id, bool edit)
    {
        var vm = new UserDetailsViewModel();
        var user = await _repo.GetUserAdvancedWithAddressById(id);


        // Fill viewModel with the data of the retrieved user.
        vm.Id = user.Id;
        vm.Email = user.Email;
        vm.PhoneNumber = user.PhoneNumber;
        vm.CurrentRoles = await _userManager.GetRolesAsync(user) as List<string>;
        vm.RegistrationDate = user.RegistrationDate;
        vm.PayoutScale = user.PayoutScale;

        if (edit)
        {
            vm.FirstName = user.FirstName;
            vm.MiddleName = user.MiddleName;
            vm.LastName = user.LastName;
            vm.BirthDate = user.BirthDate.Date;
            vm.Zipcode = user.Address.Zipcode;
            vm.Street = user.Address.Street;
            vm.HouseNumber = user.Address.HouseNumber;
            vm.City = user.Address.City;
            vm.SelectedDepartments = user.Departments.Select(d => d.DepartmentId).ToList();
            vm.AllDepartments = await _repo.GetAllDepartments();
            vm.BranchId = user.Branch.Id;
            vm.AllRoles = await _roleManager.Roles.ToListAsync();
        }
        else
        {
            vm.FullName = user.FullName;
            vm.BirthDate = user.BirthDate.Date;
            vm.Address = user.Address;
            vm.CurrentDepartments = user.Departments.Select(d => d.Department).ToList();
            vm.Branch = user.Branch;
        }

        return vm;
    }

    public async Task<IActionResult> ReactivateUser(string id)
    {
        await _repo.ReactivateUser(id);
        return RedirectToAction("DeactivatedUsers");
    }
}