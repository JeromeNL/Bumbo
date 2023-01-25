using System.Globalization;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bumbo.Web.Controllers;

[Authorize(Roles = nameof(Role.Admin))]
public class ImportController : Controller
{
    private readonly IImportRepository _repo;

    public ImportController(IImportRepository repo)
    {
        _repo = repo;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var vm = new ImportViewModel();
        var branches = await _repo.GetAllBranches();
        ViewBag.Branches = new SelectList(branches, "Id", "Name", branches.FirstOrDefault());
        return View(vm);
    }

    public async Task<IActionResult> Import(ImportViewModel importViewModel)
    {
        var records = await ReadEmployeeFile(importViewModel.File);

        var branchId = importViewModel.BranchId;

        var employees = new List<ApplicationUser>();

        var hasher = new PasswordHasher<ApplicationUser>();
        const string password = "BumboNederland123!";
        foreach (var employee in records)
        {
            var userName = employee.Voornaam + employee.Tussenvoegsel + employee.Achternaam;
            userName = userName.Replace(" ", "");
            userName = userName.Replace(",", "");
            var user = new ApplicationUser
            {
                Id = employee.Bid,
                FirstName = employee.Voornaam,
                MiddleName = employee.Tussenvoegsel,
                LastName = employee.Achternaam,
                BirthDate = employee.Geboortedatum,
                Email = employee.Email,
                PhoneNumber = employee.Telefoon,
                Address = new Address { Zipcode = employee.Postcode, HouseNumber = employee.Huisnummer, City = "" },
                RegistrationDate = employee.DatumInDienst,
                BranchId = branchId,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                NormalizedEmail = employee.Email.ToUpper()
            };
            hasher.HashPassword(user, password);
            employees.Add(user);
        }

        try
        {
            await _repo.ImportEmployees(employees);
        }
        catch (Exception)
        {
            return View("Error", new ErrorViewModel { RequestId = "Er is iets misgegaan bij het aanmaken van de gebruikers. Probeer het opnieuw." });
        }


        return View("ImportResult");
    }

    private async Task<List<EmployeeImport>> ReadEmployeeFile(IFormFile file)
    {
        var reader = new StreamReader(file.OpenReadStream());
        var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
        await csv.ReadAsync();
        csv.ReadHeader();
        var records = csv.GetRecords<EmployeeImport>().ToList();
        return records;
    }
}